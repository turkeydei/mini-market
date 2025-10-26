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
    console.log('Products Lambda triggered:', JSON.stringify(event, null, 2));
    
    try {
        const connection = await mysql.createConnection(dbConfig);
        
        // Get query parameters
        const { categoryId, search, page = 1, limit = 10 } = event.queryStringParameters || {};
        
        let query = `
            SELECT h.MaHH, h.TenHH, h.DonGia, h.Hinh, h.GiamGia, h.SoLanXem, h.MoTa, h.NgaySX,
                   l.TenLoai, l.MaLoai
            FROM HangHoa h
            LEFT JOIN Loai l ON h.MaLoai = l.MaLoai
            WHERE 1=1
        `;
        
        const params = [];
        
        // Add filters
        if (categoryId) {
            query += ' AND h.MaLoai = ?';
            params.push(categoryId);
        }
        
        if (search) {
            query += ' AND (h.TenHH LIKE ? OR h.MoTa LIKE ?)';
            params.push(`%${search}%`, `%${search}%`);
        }
        
        // Add pagination
        const offset = (page - 1) * limit;
        query += ' ORDER BY h.SoLanXem DESC LIMIT ? OFFSET ?';
        params.push(parseInt(limit), offset);
        
        const [rows] = await connection.execute(query, params);
        
        // Calculate final price with discount
        const products = rows.map(product => ({
            ...product,
            GiaCuoi: product.GiamGia > 0 
                ? product.DonGia * (1 - product.GiamGia / 100)
                : product.DonGia
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
                data: products,
                pagination: {
                    page: parseInt(page),
                    limit: parseInt(limit),
                    total: products.length
                }
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
