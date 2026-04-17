import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { userService } from '../services';
import { patientService } from '../../patients/services';
import { professionalService } from '../../professionals/services';
import { ChevronLeft, Users, Search } from 'lucide-react';
import { toast } from 'sonner';

const userSchema = Yup.object().shape({
  Username: Yup.string().required('Username é obrigatório'),
  FullName: Yup.string().required('Nome Completo é obrigatório'),
  Email: Yup.string().email('E-mail inválido').required('E-mail é obrigatório'),
  Password: Yup.string().min(6, 'Senha deve ter pelo menos 6 caracteres'),
  Role: Yup.number().required('Role é obrigatória'),
  IsActive: Yup.boolean()
});

export const UserFormScreen = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [initialValues, setInitialValues] = useState({ Username: '', FullName: '', Email: '', Password: '', Role: 2, IsActive: true });
  const [loading, setLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(true);
  const [patients, setPatients] = useState([]);
  const [professionals, setProfessionals] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoadingLists(true);
        const [patientsRes, professionalsRes] = await Promise.all([
          patientService.getAll(),
          professionalService.getAll()
        ]);
        
        // Função auxiliar para extrair a lista independente da estrutura (Array ou Result<T>)
        const extractList = (res) => {
          if (Array.isArray(res)) return res;
          if (res && Array.isArray(res.Data)) return res.Data;
          if (res && Array.isArray(res.data)) return res.data;
          if (res && res.Data && Array.isArray(res.Data)) return res.Data;
          return [];
        };

        setPatients(extractList(patientsRes));
        setProfessionals(extractList(professionalsRes));
      } catch (error) {
        console.error('Erro ao buscar dados auxiliares:', error);
        toast.error('Erro ao carregar lista de pacientes/profissionais');
      } finally {
        setLoadingLists(false);
      }
    };
    fetchData();

    if (id) {
      const fetchUser = async () => {
        try {
          setLoading(true);
          const user = await userService.getById(id);
          const data = user?.Data || user;
          if (data) {
            setInitialValues({
              Username: data.Username || data.username || '',
              FullName: data.FullName || data.fullName || '',
              Email: data.Email || data.email || '',
              Password: '',
              Role: data.Role ?? data.role ?? 2,
              IsActive: data.IsActive ?? data.isActive ?? true
            });
          }
        } catch (error) {
          console.error('Erro ao carregar usuário:', error);
        } finally {
          setLoading(false);
        }
      };
      fetchUser();
    }
  }, [id]);

  const handleSubmit = async (values, { setSubmitting, setStatus }) => {
    try {
      if (id) {
        await userService.update(id, values);
      } else {
        await userService.register(values);
      }
      toast.success('Usuário salvo com sucesso!');
      navigate('/users');
    } catch (error) {
      const errorMsg = error.message || 'Erro ao salvar usuário';
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
          <Users size={24} color="var(--primary-color)" />
          <h1 style={{ margin: 0, fontSize: '1.5rem' }}>{id ? 'Editar Usuário' : 'Novo Usuário'}</h1>
        </div>

        <Formik
          initialValues={initialValues}
          enableReinitialize
          validationSchema={userSchema}
          onSubmit={handleSubmit}
        >
          {({ isSubmitting, status, values, setFieldValue }) => (
            <Form>
              <div className="form-group">
                <label>Nível de Acesso (Role)</label>
                <Field name="Role" as="select" onChange={(e) => {
                  const role = parseInt(e.target.value);
                  setFieldValue('Role', role);
                }}>
                  <option value={0}>Administrador</option>
                  <option value={1}>Médico / Profissional</option>
                  <option value={2}>Paciente</option>
                  <option value={3}>Agente / Recepcionista</option>
                </Field>
                <ErrorMessage name="Role" component="div" className="error-text" />
              </div>

              {(values.Role === 1 || values.Role === 2) && (
                <div className="form-group" style={{ padding: '1rem', backgroundColor: '#f0f7ff', borderRadius: '8px', marginBottom: '1.5rem' }}>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px', color: '#0056b3' }}>
                    <Search size={16} /> Vincular a {values.Role === 1 ? 'um Profissional' : 'um Paciente'}
                  </label>
                  <select 
                    key={`select-link-${values.Role}`}
                    style={{ marginTop: '0.5rem' }}
                    onChange={(e) => {
                      const selectedId = e.target.value;
                      if (!selectedId) return;
                      
                      if (values.Role === 1) {
                        const prof = professionals.find(p => (p.Id || p.id) === selectedId);
                        if (prof) {
                          setFieldValue('FullName', prof.Name || prof.name);
                          setFieldValue('Email', prof.Email || prof.email);
                        }
                      } else {
                        const patient = patients.find(p => (p.Id || p.id) === selectedId);
                        if (patient) {
                          setFieldValue('FullName', patient.Name || patient.name);
                          setFieldValue('Email', patient.Email || patient.email);
                        }
                      }
                    }}
                  >
                    <option value="">
                      {loadingLists ? 'Carregando lista...' : '-- Selecione para preencher dados --'}
                    </option>
                    {!loadingLists && (values.Role === 1 ? (
                      professionals.map(p => (
                        <option key={p.Id || p.id} value={p.Id || p.id}>{p.Name || p.name} ({p.Specialization || p.specialization})</option>
                      ))
                    ) : (
                      patients.map(p => (
                        <option key={p.Id || p.id} value={p.Id || p.id}>{p.Name || p.name} (CPF: {p.Cpf || p.cpf})</option>
                      ))
                    ))}
                  </select>
                  <p style={{ fontSize: '0.8rem', color: '#666', marginTop: '0.5rem' }}>
                    * Ao selecionar, o nome e e-mail serão preenchidos automaticamente.
                  </p>
                </div>
              )}

              <div className="form-group">
                <label>Nome Completo</label>
                <Field name="FullName" type="text" placeholder="Nome completo do usuário" />
                <ErrorMessage name="FullName" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>Username (Login)</label>
                <Field name="Username" type="text" placeholder="Ex: joao.silva" />
                <ErrorMessage name="Username" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>E-mail (usado para vincular o perfil)</label>
                <Field name="Email" type="email" placeholder="email@exemplo.com" />
                <ErrorMessage name="Email" component="div" className="error-text" />
              </div>

              <div className="form-group">
                <label>Senha {id && '(deixe em branco para não alterar)'}</label>
                <Field name="Password" type="password" placeholder="••••••••" />
                <ErrorMessage name="Password" component="div" className="error-text" />
              </div>

              <div className="form-group" style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Field name="IsActive" type="checkbox" id="IsActive" style={{ width: 'auto' }} />
                <label htmlFor="IsActive" style={{ marginBottom: 0 }}>Usuário Ativo</label>
              </div>

              {status && <div className="error-text" style={{ marginBottom: '1rem' }}>{status}</div>}

              <button type="submit" className="btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Salvando...' : 'Salvar Usuário'}
              </button>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};
