import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './providers';
import { LoginScreen } from '../features/auth/pages/LoginScreen';
import { RegisterPatientScreen } from '../features/patients/pages/RegisterPatientScreen';
import { AppointmentListScreen } from '../features/appointments/pages/AppointmentListScreen';
import { ScheduleAppointmentScreen } from '../features/appointments/pages/ScheduleAppointmentScreen';
import { PatientListScreen } from '../features/patients/pages/PatientListScreen';
import { PatientFormScreen } from '../features/patients/pages/PatientFormScreen';
import { HealthProfessionalListScreen } from '../features/professionals/pages/HealthProfessionalListScreen';
import { UserListScreen } from '../features/users/pages/UserListScreen';
import { HealthProfessionalFormScreen } from '../features/professionals/pages/HealthProfessionalFormScreen';
import { UserFormScreen } from '../features/users/pages/UserFormScreen';
import { HomeScreen } from '../features/home/pages/HomeScreen';
import { Layout } from '../shared/components/Layout';

const PrivateRoute = ({ children, roles }) => {
  const { isAuthenticated, user, loading } = useAuth();
  
  if (loading) return <div style={{ textAlign: 'center', marginTop: '5rem' }}><h1>Carregando...</h1></div>;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  
  if (roles && !roles.includes(user?.role)) {
    return <Navigate to="/" replace />;
  }
  
  return <Layout>{children}</Layout>;
};

const PublicRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();
  
  if (loading) return <div style={{ textAlign: 'center', marginTop: '5rem' }}><h1>Carregando...</h1></div>;
  return !isAuthenticated ? children : <Navigate to="/" replace />;
};

export const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={
        <PublicRoute>
          <LoginScreen />
        </PublicRoute>
      } />
      <Route path="/register" element={
        <PublicRoute>
          <RegisterPatientScreen />
        </PublicRoute>
      } />
      
      <Route path="/" element={
        <PrivateRoute roles={[0, 1, 2, 3]}>
          <HomeScreen />
        </PrivateRoute>
      } />

      <Route path="/appointments" element={
        <PrivateRoute roles={[0, 1, 2, 3]}>
          <AppointmentListScreen />
        </PrivateRoute>
      } />
      
      <Route path="/schedule" element={
        <PrivateRoute roles={[0, 1, 2, 3]}>
          <ScheduleAppointmentScreen />
        </PrivateRoute>
      } />

      <Route path="/patients" element={
        <PrivateRoute roles={[0, 3]}>
          <PatientListScreen />
        </PrivateRoute>
      } />

      <Route path="/patients/new" element={
        <PrivateRoute roles={[0, 3]}>
          <PatientFormScreen />
        </PrivateRoute>
      } />

      <Route path="/patients/edit/:id" element={
        <PrivateRoute roles={[0, 3]}>
          <PatientFormScreen />
        </PrivateRoute>
      } />

      <Route path="/professionals" element={
        <PrivateRoute roles={[0]}>
          <HealthProfessionalListScreen />
        </PrivateRoute>
      } />

      <Route path="/professionals/new" element={
        <PrivateRoute roles={[0]}>
          <HealthProfessionalFormScreen />
        </PrivateRoute>
      } />

      <Route path="/professionals/edit/:id" element={
        <PrivateRoute roles={[0]}>
          <HealthProfessionalFormScreen />
        </PrivateRoute>
      } />

      <Route path="/users" element={
        <PrivateRoute roles={[0]}>
          <UserListScreen />
        </PrivateRoute>
      } />

      <Route path="/users/new" element={
        <PrivateRoute roles={[0]}>
          <UserFormScreen />
        </PrivateRoute>
      } />

      <Route path="/users/edit/:id" element={
        <PrivateRoute roles={[0]}>
          <UserFormScreen />
        </PrivateRoute>
      } />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};
