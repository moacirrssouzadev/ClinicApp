import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { patientService } from '../services';
import { toast } from 'sonner';
import { Modal } from '../../../shared/components/Modal';

const patientSchema = Yup.object().shape({
  name: Yup.string()
    .min(3, 'Nome muito curto')
    .required('Nome é obrigatório'),
  cpf: Yup.string()
    .matches(/^\d{11}$/, 'CPF deve ter 11 dígitos numéricos')
    .required('CPF é obrigatório'),
  email: Yup.string()
    .email('E-mail inválido')
    .required('E-mail é obrigatório'),
  phone: Yup.string()
    .required('Telefone é obrigatório'),
  dateOfBirth: Yup.string()
    .required('Data de nascimento é obrigatória'),
  address: Yup.string()
    .required('Endereço é obrigatório'),
});

export const RegisterPatientScreen = () => {
  const navigate = useNavigate();
  const [errorModal, setErrorModal] = useState({ isOpen: false, message: '' });

  const handleRegister = async (values, { setSubmitting, setStatus }) => {
    try {
      const payload = {
        Name: values.name,
        Cpf: values.cpf,
        Email: values.email,
        Phone: values.phone,
        DateOfBirth: values.dateOfBirth,
        Address: values.address
      };
      await patientService.register(payload);
      toast.success('Paciente cadastrado com sucesso!');
      navigate('/login');
    } catch (error) {
      const errorMsg = error.response?.data?.Message || error.Message || error.message || 'Falha ao cadastrar paciente';
      setErrorModal({ isOpen: true, message: errorMsg });
      setStatus(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container">
      <div className="glass-card" style={{ maxWidth: '800px', margin: '0 auto' }}>
        <h1>Cadastro de Paciente</h1>
        <p style={{ textAlign: 'center', color: 'var(--text-secondary)', marginBottom: '2rem' }}>
          Preencha as informações abaixo para criar sua ficha médica
        </p>
        
        <Formik
          initialValues={{ name: '', cpf: '', email: '', phone: '', dateOfBirth: '', address: '' }}
          validationSchema={patientSchema}
          onSubmit={handleRegister}
        >
          {({ isSubmitting, status }) => (
            <Form>
              <div className="grid-2">
                <div className="form-group">
                  <label>Nome Completo</label>
                  <Field name="name" type="text" placeholder="Nome do Paciente" />
                  <ErrorMessage name="name" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label>CPF (apenas números)</label>
                  <Field name="cpf" type="text" placeholder="000.000.000-00" />
                  <ErrorMessage name="cpf" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label>E-mail Pessoal</label>
                  <Field name="email" type="email" placeholder="seu@email.com" />
                  <ErrorMessage name="email" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label>Telefone / WhatsApp</label>
                  <Field name="phone" type="text" placeholder="(00) 00000-0000" />
                  <ErrorMessage name="phone" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label>Data de Nascimento</label>
                  <Field name="dateOfBirth" type="date" />
                  <ErrorMessage name="dateOfBirth" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label>Endereço Residencial</label>
                  <Field name="address" type="text" placeholder="Rua, Número, Bairro" />
                  <ErrorMessage name="address" component="div" className="error-text" />
                </div>
              </div>

              {status && <div className="error-text" style={{ marginBottom: '1rem', textAlign: 'center' }}>{status}</div>}

              <div style={{ marginTop: '1rem' }}>
                <button type="submit" className="btn-primary" disabled={isSubmitting}>
                  {isSubmitting ? 'Processando Cadastro...' : 'Finalizar Cadastro'}
                </button>
              </div>

              <div style={{ marginTop: '1.5rem', textAlign: 'center', fontSize: '0.9rem' }}>
                Já possui uma ficha? <Link to="/login" className="btn-link">Voltar ao Login</Link>
              </div>
            </Form>
          )}
        </Formik>
      </div>

      <Modal 
        isOpen={errorModal.isOpen}
        title="Erro no Cadastro"
        message={errorModal.message}
        type="error"
        confirmText="Entendido"
        onClose={() => setErrorModal({ isOpen: false, message: '' })}
      />
    </div>
  );
};
