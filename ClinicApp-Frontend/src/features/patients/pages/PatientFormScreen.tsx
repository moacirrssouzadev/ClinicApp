import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { patientService } from '../services';
import { ChevronLeft, User } from 'lucide-react';
import { toast } from 'sonner';

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

export const PatientFormScreen = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [initialValues, setInitialValues] = useState({
    name: '',
    cpf: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    address: ''
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      const fetchPatient = async () => {
        try {
          setLoading(true);
          const patient = await patientService.getById(id);
          if (patient) {
            setInitialValues({
              name: patient.Name || '',
              cpf: patient.Cpf || '',
              email: patient.Email || '',
              phone: patient.Phone || '',
              dateOfBirth: patient.DateOfBirth ? new Date(patient.DateOfBirth).toISOString().split('T')[0] : '',
              address: patient.Address || ''
            });
          }
        } catch (error) {
          console.error('Erro ao carregar paciente:', error);
          toast.error('Erro ao carregar os dados do paciente.');
          navigate('/patients');
        } finally {
          setLoading(false);
        }
      };
      fetchPatient();
    }
  }, [id, navigate]);

  const handleSubmit = async (values, { setSubmitting, setStatus }) => {
    try {
      const payload = {
        Name: values.name,
        Cpf: values.cpf,
        Email: values.email,
        Phone: values.phone,
        DateOfBirth: values.dateOfBirth,
        Address: values.address
      };

      if (id) {
        await patientService.update(id, payload);
        toast.success('Paciente atualizado com sucesso!');
      } else {
        await patientService.register(payload);
        toast.success('Paciente cadastrado com sucesso!');
      }
      navigate('/patients');
    } catch (error) {
      const errorMsg = error.response?.data?.Message || error.Message || error.message || 'Falha ao salvar paciente';
      toast.error(errorMsg);
      setStatus(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div className="container" style={{ textAlign: 'center', marginTop: '5rem' }}><h1>Carregando...</h1></div>;

  return (
    <div className="container">
      <div className="glass-card" style={{ maxWidth: '800px', margin: '0 auto' }}>
        <button onClick={() => navigate(-1)} className="btn-link" style={{ display: 'flex', alignItems: 'center', gap: '5px', marginBottom: '1.5rem' }}>
          <ChevronLeft size={20} /> Voltar
        </button>

        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '1.5rem' }}>
          <User size={24} color="var(--primary-color)" />
          <h1 style={{ margin: 0, fontSize: '1.5rem' }}>{id ? 'Editar Paciente' : 'Novo Paciente'}</h1>
        </div>
        
        <Formik
          initialValues={initialValues}
          enableReinitialize
          validationSchema={patientSchema}
          onSubmit={handleSubmit}
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
                  <Field name="cpf" type="text" placeholder="00000000000" />
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
                  {isSubmitting ? 'Salvando...' : id ? 'Salvar Alterações' : 'Finalizar Cadastro'}
                </button>
              </div>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};
