import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { professionalService } from '../services';
import { ChevronLeft, Stethoscope } from 'lucide-react';
import { toast } from 'sonner';

const professionalSchema = Yup.object().shape({
  Name: Yup.string().required('Nome é obrigatório'),
  Email: Yup.string().email('E-mail inválido').required('E-mail é obrigatório'),
  Phone: Yup.string().required('Telefone é obrigatório'),
  Specialization: Yup.string().required('Especialidade é obrigatória'),
  LicenseNumber: Yup.string().required('Registro (CRM/Outros) é obrigatório'),
  IsActive: Yup.boolean()
});

export const HealthProfessionalFormScreen = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [initialValues, setInitialValues] = useState({ 
    Name: '', 
    Email: '', 
    Phone: '', 
    Specialization: '', 
    LicenseNumber: '', 
    IsActive: true 
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      const fetchProf = async () => {
        try {
          setLoading(true);
          const all = await professionalService.getAll();
          const prof = (Array.isArray(all) ? all : all?.Data || []).find(p => (p.Id || p.id) === id);
          if (prof) {
            setInitialValues({
              Name: prof.Name || prof.name || '',
              Email: prof.Email || prof.email || '',
              Phone: prof.Phone || prof.phone || '',
              Specialization: prof.Specialization || prof.specialization || '',
              LicenseNumber: prof.LicenseNumber || prof.licenseNumber || '',
              IsActive: prof.IsActive ?? prof.isActive ?? true
            });
          }
        } catch (error) {
          console.error('Erro ao carregar profissional:', error);
        } finally {
          setLoading(false);
        }
      };
      fetchProf();
    }
  }, [id]);

  const handleSubmit = async (values, { setSubmitting, setStatus }) => {
    try {
      if (id) {
        await professionalService.update(id, values);
      } else {
        await professionalService.create(values);
      }
      toast.success('Profissional salvo com sucesso!');
      navigate('/professionals');
    } catch (error) {
      const errorMsg = error.message || 'Erro ao salvar profissional';
      toast.error(errorMsg);
      setStatus(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div className="container">Carregando...</div>;

  return (
    <div className="container">
      <div className="glass-card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <button onClick={() => navigate(-1)} className="btn-link" style={{ display: 'flex', alignItems: 'center', gap: '5px', marginBottom: '1.5rem' }}>
          <ChevronLeft size={20} /> Voltar
        </button>
        
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '1.5rem' }}>
          <Stethoscope size={24} color="var(--primary-color)" />
          <h1 style={{ margin: 0, fontSize: '1.5rem' }}>{id ? 'Editar Profissional' : 'Novo Profissional'}</h1>
        </div>

        <Formik
          initialValues={initialValues}
          enableReinitialize
          validationSchema={professionalSchema}
          onSubmit={handleSubmit}
        >
          {({ isSubmitting, status }) => (
            <Form>
              <div className="form-group">
                <label>Nome Completo</label>
                <Field name="Name" type="text" placeholder="Nome do Médico/Profissional" />
                <ErrorMessage name="Name" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>E-mail</label>
                <Field name="Email" type="email" placeholder="email@exemplo.com" />
                <ErrorMessage name="Email" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>Telefone</label>
                <Field name="Phone" type="text" placeholder="(00) 00000-0000" />
                <ErrorMessage name="Phone" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>Especialidade</label>
                <Field name="Specialization" type="text" placeholder="Ex: Cardiologia" />
                <ErrorMessage name="Specialization" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>Número de Registro (CRM)</label>
                <Field name="LicenseNumber" type="text" placeholder="Ex: CRM-SP 123456" />
                <ErrorMessage name="LicenseNumber" component="div" className="error-text" />
              </div>

              <div className="form-group" style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Field name="IsActive" type="checkbox" id="IsActive" style={{ width: 'auto' }} />
                <label htmlFor="IsActive" style={{ marginBottom: 0 }}>Profissional Ativo</label>
              </div>

              {status && <div className="error-text" style={{ marginBottom: '1rem' }}>{status}</div>}

              <button type="submit" className="btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Salvando...' : 'Salvar Profissional'}
              </button>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};
