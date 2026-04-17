import api from '../../services/api';

export const authService = {
  async login(username, password) {
    try {
      const response = await api.post('/Users/login', { Username: username, Password: password });
      const { Token, RefreshToken } = response.data;
      
      if (Token) {
        localStorage.setItem('@ClinicApp:token', Token);
      }

      if (RefreshToken) {
        localStorage.setItem('@ClinicApp:refreshToken', RefreshToken);
      }
      
      return response.data;
    } catch (error) {
      if (error.response?.data) {
        throw error.response.data;
      }
      throw new Error(error.message || 'Falha na comunicação com o servidor');
    }
  },

  async logout() {
    localStorage.removeItem('@ClinicApp:token');
    localStorage.removeItem('@ClinicApp:refreshToken');
  },

  async isAuthenticated() {
    const token = localStorage.getItem('@ClinicApp:token');
    return !!token;
  }
};
