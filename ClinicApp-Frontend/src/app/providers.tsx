import React, { createContext, useState, useContext, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext({});

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const decodeToken = (token) => {
    if (!token) return null;
    try {
      const decoded = jwtDecode(token);
      
      // Mapeamento de roles numéricas para compatibilidade
      const roleClaim = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;
      let role = roleClaim;
      
      // Se for string, tentar converter para número ou mapear nomes
      if (typeof roleClaim === 'string') {
        const parsed = parseInt(roleClaim, 10);
        if (!isNaN(parsed)) {
          role = parsed;
        } else {
          if (roleClaim === 'Admin') role = 0;
          else if (roleClaim === 'Medico') role = 1;
          else if (roleClaim === 'Paciente') role = 2;
          else if (roleClaim === 'Agente') role = 3;
        }
      }
      
      return {
        id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded.sub || decoded.id,
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || decoded.email,
        name: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded.unique_name || decoded.name || 'Usuário',
        role: role ?? 2,
        token
      };
    } catch (error) {
      console.error('Erro ao decodificar token:', error);
      return null;
    }
  };

  useEffect(() => {
    const initializeAuth = async () => {
      try {
        const token = localStorage.getItem('@ClinicApp:token');
        if (token) {
          const userData = decodeToken(token);
          if (userData) {
            const userDetails = localStorage.getItem('@ClinicApp:user_details');
            if (userDetails) {
              const { patientId, healthProfessionalId } = JSON.parse(userDetails);
              setUser({ ...userData, patientId, healthProfessionalId });
            } else {
              setUser(userData);
            }
          } else {
            localStorage.removeItem('@ClinicApp:token');
            localStorage.removeItem('@ClinicApp:refreshToken');
          }
        }
      } catch (error) {
        console.error('Erro ao inicializar autenticação:', error);
      } finally {
        setLoading(false);
      }
    };
    initializeAuth();
  }, []);

  const login = (authResponse) => {
    if (!authResponse || !authResponse.Token) {
      console.error('Resposta de login inválida:', authResponse);
      return;
    }
    const userData = decodeToken(authResponse.Token);
    if (userData) {
      const fullUserData = {
        ...userData,
        patientId: authResponse.PatientId,
        healthProfessionalId: authResponse.HealthProfessionalId
      };
      setUser(fullUserData);
      localStorage.setItem('@ClinicApp:user_details', JSON.stringify({
        patientId: authResponse.PatientId,
        healthProfessionalId: authResponse.HealthProfessionalId
      }));
    }
  };

  const logout = () => {
    localStorage.removeItem('@ClinicApp:token');
    localStorage.removeItem('@ClinicApp:refreshToken');
    localStorage.removeItem('@ClinicApp:user_details');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, login, logout, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
