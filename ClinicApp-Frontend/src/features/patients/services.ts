import api from '../../services/api';

export const patientService = {
  async register(patientData) {
    try {
      const response = await api.post('/Patients', patientData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getById(id) {
    try {
      const response = await api.get(`/Patients/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getByCpf(cpf) {
    try {
      const response = await api.get(`/Patients/cpf/${cpf}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getAll() {
    try {
      const response = await api.get('/Patients');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async update(id, patientData) {
    try {
      const response = await api.put(`/Patients/${id}`, patientData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async delete(id) {
    try {
      const response = await api.delete(`/Patients/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};
