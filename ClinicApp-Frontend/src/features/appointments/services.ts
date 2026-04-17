import api from '../../services/api';

export const appointmentService = {
  async schedule(appointmentData) {
    try {
      const response = await api.post('/Appointments', appointmentData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getAll() {
    try {
      const response = await api.get('/Appointments');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getByPatientId(patientId) {
    try {
      const response = await api.get(`/Appointments/patient/${patientId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getByProfessionalId(professionalId) {
    try {
      const response = await api.get(`/Appointments/professional/${professionalId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  async getAvailableSlots(healthProfessionalId, date) {
    try {
      const response = await api.get(`/Appointments/professional/${healthProfessionalId}/available-slots`, {
        params: { date }
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};
