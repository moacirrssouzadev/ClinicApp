import api from '../../services/api';

export const userService = {
  async getAll() {
    try {
      const response = await api.get('/Users');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getById(id) {
    try {
      const response = await api.get(`/Users/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async register(userData) {
    try {
      const response = await api.post('/Users/register', userData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async update(id, userData) {
    try {
      const response = await api.put(`/Users/${id}`, userData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async delete(id) {
    try {
      const response = await api.delete(`/Users/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};
