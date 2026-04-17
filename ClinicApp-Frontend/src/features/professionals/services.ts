import api from '../../services/api';

export const professionalService = {
  async getAll() {
    try {
      const response = await api.get('/HealthProfessionals');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getActive() {
    try {
      const response = await api.get('/HealthProfessionals/active');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async create(professionalData) {
    try {
      const response = await api.post('/HealthProfessionals', professionalData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async update(id, professionalData) {
    try {
      const response = await api.put(`/HealthProfessionals/${id}`, professionalData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async delete(id) {
    try {
      const response = await api.delete(`/HealthProfessionals/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};
