import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './app/providers';
import { AppRoutes } from './app/routes';
import { Toaster } from 'sonner';
import './styles/index.css';
import './styles/global.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <AppRoutes />
        <Toaster position="top-right" richColors closeButton />
      </Router>
    </AuthProvider>
  );
}

export default App;
