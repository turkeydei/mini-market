const mysql = require('mysql2/promise');

// Database connection
const dbConfig = {
    host: process.env.RDS_HOST,
    user: process.env.RDS_USER,
    password: process.env.RDS_PASSWORD,
    database: process.env.RDS_DATABASE,
    ssl: { rejectUnauthorized: false }
};

exports.handler = async (event) => {
    console.log('Orders Lambda triggered:', JSON.stringify(event, null, 2));
    
    try {
        const connection = await mysql.createConnection(dbConfig);
        
        // Get query parameters
        const { mine, userId, status } = event.queryStringParameters || {};
        
        let query = `
            SELECT hd.MaHD, hd.NgayDat, hd.DiaChiGiao, hd.PhiVanChuyen, hd.GhiChu, hd.SoDienThoai,
                   u.HoTen as CustomerName, u.Email as CustomerEmail,
                   tt.TenTrangThai, tt.MaTrangThai as StatusId,
                   COUNT(ct.MaCT) as ItemCount,
                   SUM(ct.DonGia * ct.SoLuong) as SubTotal
            FROM HoaDon hd
            LEFT JOIN User u ON hd.MaUser = u.MaUser
            LEFT JOIN TrangThai tt ON hd.MaTrangThai = tt.MaTrangThai
            LEFT JOIN ChiTietHD ct ON hd.MaHD = ct.MaHD
            WHERE 1=1
        `;
        
        const params = [];
        
        // Add filters
        if (mine === 'true' && userId) {
            query += ' AND hd.MaUser = ?';
            params.push(userId);
        }
        
        if (status) {
            query += ' AND hd.MaTrangThai = ?';
            params.push(status);
        }
        
        query += ' GROUP BY hd.MaHD ORDER BY hd.NgayDat DESC';
        
        const [rows] = await connection.execute(query, params);
        
        // Calculate final totals
        const orders = rows.map(order => ({
            ...order,
            TotalAmount: (order.SubTotal || 0) + (order.PhiVanChuyen || 0),
            ItemCount: order.ItemCount || 0
        }));
        
        await connection.end();
        
        return {
            statusCode: 200,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Headers': 'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token',
                'Access-Control-Allow-Methods': 'GET,POST,PUT,DELETE,OPTIONS'
            },
            body: JSON.stringify({
                success: true,
                data: orders
            })
        };
        
    } catch (error) {
        console.error('Error:', error);
        return {
            statusCode: 500,
            headers: {
                'Content-Type': 'application/json',
                'Access-Control-Allow-Origin': '*'
            },
            body: JSON.stringify({
                success: false,
                error: 'Internal server error'
            })
        };
    }
};
