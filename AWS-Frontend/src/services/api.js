import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://your-api-gateway-url.amazonaws.com/prod';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Products API
export const productsAPI = {
  // GET /products
  getProducts: (params = {}) => {
    return api.get('/products', { params });
  },
  
  // GET /products/:id
  getProduct: (id) => {
    return api.get(`/products/${id}`);
  },
  
  // GET /products?categoryId=1&search=keyword
  searchProducts: (filters) => {
    return api.get('/products', { params: filters });
  }
};

// Orders API
export const ordersAPI = {
  // GET /orders?mine=true&userId=123
  getMyOrders: (userId) => {
    return api.get('/orders', { 
      params: { mine: 'true', userId } 
    });
  },
  
  // GET /orders (admin)
  getAllOrders: (filters = {}) => {
    return api.get('/orders', { params: filters });
  },
  
  // GET /orders/:id
  getOrder: (id) => {
    return api.get(`/orders/${id}`);
  }
};

// Checkout API
export const checkoutAPI = {
  // POST /checkout
  createOrder: (orderData) => {
    return api.post('/checkout', orderData);
  }
};

// Auth API (simplified for demo)
export const authAPI = {
  login: (credentials) => {
    // In real app, this would call Cognito or your auth service
    return Promise.resolve({
      data: {
        success: true,
        user: {
          id: 1,
          name: 'Demo User',
          email: credentials.email
        },
        token: 'demo-token'
      }
    });
  }
};

export default api;
