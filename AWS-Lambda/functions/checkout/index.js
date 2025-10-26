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
    console.log('Checkout Lambda triggered:', JSON.stringify(event, null, 2));
    
    try {
        const body = JSON.parse(event.body);
        const { userId, items, shippingAddress, phone, notes } = body;
        
        // Validate input
        if (!userId || !items || !Array.isArray(items) || items.length === 0) {
            return {
                statusCode: 400,
                headers: {
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin': '*'
                },
                body: JSON.stringify({
                    success: false,
                    error: 'Invalid input data'
                })
            };
        }
        
        const connection = await mysql.createConnection(dbConfig);
        await connection.beginTransaction();
        
        try {
            // Calculate totals
            let totalAmount = 0;
            const orderItems = [];
            
            for (const item of items) {
                const [productRows] = await connection.execute(
                    'SELECT DonGia, GiamGia FROM HangHoa WHERE MaHH = ?',
                    [item.productId]
                );
                
                if (productRows.length === 0) {
                    throw new Error(`Product ${item.productId} not found`);
                }
                
                const product = productRows[0];
                const finalPrice = product.GiamGia > 0 
                    ? product.DonGia * (1 - product.GiamGia / 100)
                    : product.DonGia;
                
                const itemTotal = finalPrice * item.quantity;
                totalAmount += itemTotal;
                
                orderItems.push({
                    productId: item.productId,
                    quantity: item.quantity,
                    unitPrice: finalPrice,
                    total: itemTotal
                });
            }
            
            // Add shipping fee (simple business logic)
            const shippingFee = totalAmount > 1000000 ? 0 : 50000; // Free shipping over 1M VND
            const finalTotal = totalAmount + shippingFee;
            
            // Create HoaDon
            const [orderResult] = await connection.execute(
                `INSERT INTO HoaDon (MaUser, NgayDat, DiaChiGiao, PhiVanChuyen, MaTrangThai, GhiChu, SoDienThoai) 
                 VALUES (?, NOW(), ?, ?, 1, ?, ?)`,
                [userId, shippingAddress, shippingFee, notes || '', phone]
            );
            
            const orderId = orderResult.insertId;
            
            // Create ChiTietHD for each item
            for (const item of orderItems) {
                await connection.execute(
                    `INSERT INTO ChiTietHD (MaHD, MaHH, DonGia, SoLuong, GiamGia) 
                     VALUES (?, ?, ?, ?, ?)`,
                    [orderId, item.productId, item.unitPrice, item.quantity, 0]
                );
            }
            
            await connection.commit();
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
                    data: {
                        orderId: orderId,
                        status: 'Pending',
                        totalAmount: finalTotal,
                        shippingFee: shippingFee,
                        items: orderItems
                    }
                })
            };
            
        } catch (error) {
            await connection.rollback();
            await connection.end();
            throw error;
        }
        
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
                error: error.message || 'Internal server error'
            })
        };
    }
};
