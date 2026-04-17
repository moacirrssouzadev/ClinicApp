import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { authService } from '../services';
import { useAuth } from '../../../app/providers';
import { Modal } from '../../../shared/components/Modal';

const loginSchema = Yup.object().shape({
  username: Yup.string()
    .required('Usuário é obrigatório'),
  password: Yup.string()
    .min(6, 'A senha deve ter pelo menos 6 caracteres')
    .required('Senha é obrigatória'),
});

export const LoginScreen = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [errorModal, setErrorModal] = useState({ isOpen: false, message: '' });

  const handleLogin = async (values, { setSubmitting, setStatus }) => {
    try {
      const response = await authService.login(values.username, values.password);
      login(response);
      navigate('/');
    } catch (error) {
      console.error('Login Error:', error);
      const errorMsg = error.response?.data?.Message || error.Message || error.message || (typeof error === 'string' ? error : 'Falha na autenticação. Verifique suas credenciais.');
      setErrorModal({ isOpen: true, message: errorMsg });
      setStatus(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '80vh' }}>
      <div className="glass-card" style={{ maxWidth: '450px', width: '100%' }}>
        <h1>ClinicApp</h1>
        <p style={{ textAlign: 'center', color: 'var(--text-secondary)', marginBottom: '2rem' }}>
          Acesse sua conta para gerenciar suas consultas
        </p>
        
        <Formik
          initialValues={{ username: '', password: '' }}
          validationSchema={loginSchema}
          onSubmit={handleLogin}
        >
          {({ isSubmitting, status }) => (
            <Form>
              <div className="form-group">
                <label htmlFor="username">Usuário</label>
                <Field name="username" type="text" placeholder="Seu nome de usuário" />
                <ErrorMessage name="username" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label htmlFor="password">Senha de Acesso</label>
                <Field name="password" type="password" placeholder="••••••••" />
                <ErrorMessage name="password" component="div" className="error-text" />
              </div>

              {status && <div className="error-text" style={{ marginBottom: '1rem', textAlign: 'center' }}>{status}</div>}

              <button type="submit" className="btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Autenticando...' : 'Entrar no Sistema'}
              </button>

              <div style={{ marginTop: '1.5rem', textAlign: 'center', fontSize: '0.9rem' }}>
                Novo por aqui? <Link to="/register" className="btn-link">Criar conta de paciente</Link>
              </div>
            </Form>
          )}
        </Formik>
      </div>

      <Modal 
        isOpen={errorModal.isOpen}
        title="Erro de Login"
        message={errorModal.message}
        type="error"
        confirmText="Tentar Novamente"
        onClose={() => setErrorModal({ isOpen: false, message: '' })}
      />
    </div>
  );
};
