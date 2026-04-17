import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { appointmentService } from '../services';
import { patientService } from '../../patients/services';
import { professionalService } from '../../professionals/services';
import { useAuth } from '../../../app/providers';
import { ChevronLeft, CalendarPlus, Clock, User, Calendar, AlertCircle, Search } from 'lucide-react';
import { toast } from 'sonner';

const appointmentSchema = Yup.object().shape({
  patientId: Yup.string()
    .required('Paciente é obrigatório'),
  healthProfessionalId: Yup.string()
    .required('Profissional é obrigatório'),
  appointmentDate: Yup.string()
    .required('Data é obrigatória'),
  startTime: Yup.string()
    .required('Selecione um horário disponível'),
});

export const ScheduleAppointmentScreen = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [patients, setPatients] = useState([]);
  const [professionals, setProfessionals] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);
  const [loadingPatients, setLoadingPatients] = useState(false);
  const [loadingProfessionals, setLoadingProfessionals] = useState(true);
  const [loadingSlots, setLoadingSlots] = useState(false);

  useEffect(() => {
    const fetchProfessionals = async () => {
      try {
        const data = await professionalService.getActive();
        if (Array.isArray(data)) {
          setProfessionals(data);
        } else if (data && Array.isArray(data.Data)) {
          setProfessionals(data.Data);
        }
      } catch (error) {
        console.error('Erro ao carregar profissionais:', error);
      } finally {
        setLoadingProfessionals(false);
      }
    };

    const fetchPatients = async () => {
      if (user?.role !== 2) { // Not a patient, need to list patients
        try {
          setLoadingPatients(true);
          const data = await patientService.getAll();
          setPatients(Array.isArray(data) ? data : (data?.Data || []));
        } catch (error) {
          console.error('Erro ao carregar pacientes:', error);
        } finally {
          setLoadingPatients(false);
        }
      }
    };

    fetchProfessionals();
    fetchPatients();
  }, [user?.role]);

  const handleValuesChange = async (values) => {
    if (values.healthProfessionalId && values.appointmentDate) {
      setLoadingSlots(true);
      try {
        const slots = await appointmentService.getAvailableSlots(values.healthProfessionalId, values.appointmentDate);
        setAvailableSlots(Array.isArray(slots) ? slots : (slots?.Data || []));
      } catch (error) {
        console.error('Erro ao carregar horários:', error);
        setAvailableSlots([]);
      } finally {
        setLoadingSlots(false);
      }
    }
  };

  const handleSchedule = async (values, { setSubmitting, setStatus }) => {
    try {
      const payload = {
        PatientId: values.patientId,
        HealthProfessionalId: values.healthProfessionalId,
        AppointmentDate: values.appointmentDate,
        StartTime: values.startTime.length === 5 ? `${values.startTime}:00` : values.startTime,
      };
      
      await appointmentService.schedule(payload);
      toast.success('Consulta agendada com sucesso!');
      navigate('/');
    } catch (error) {
      const errorMsg = error.response?.data?.Message || error.Message || error.message || 'Falha ao agendar consulta';
      toast.error(errorMsg);
      setStatus(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container">
      <div className="glass-card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <div style={{ display: 'flex', alignItems: 'center', marginBottom: '2rem' }}>
          <button onClick={() => navigate(-1)} className="btn-link" style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
            <ChevronLeft size={20} /> Voltar
          </button>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginLeft: 'auto', marginRight: 'auto' }}>
            <CalendarPlus size={24} color="var(--primary-color)" />
            <h1 style={{ margin: 0, fontSize: '1.5rem' }}>Novo Agendamento</h1>
          </div>
        </div>

        <Formik
          initialValues={{ 
            patientId: user?.role === 2 ? (user.patientId || user.id) : '',
            healthProfessionalId: user?.role === 1 ? (user.healthProfessionalId || user.id) : '',
            appointmentDate: '', 
            startTime: '' 
          }}
          validationSchema={appointmentSchema}
          onSubmit={handleSchedule}
        >
          {({ isSubmitting, status, values, setFieldValue }) => {
            // Monitorar mudanças nos campos para carregar horários
            // eslint-disable-next-line react-hooks/rules-of-hooks
            useEffect(() => {
              handleValuesChange(values);
            }, [values.healthProfessionalId, values.appointmentDate]);

            return (
              <Form>
                {user?.role !== 2 ? (
                  <div className="form-group">
                    <label style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <Search size={16} /> Selecionar Paciente
                    </label>
                    <Field name="patientId" as="select">
                      <option value="">{loadingPatients ? 'Carregando pacientes...' : 'Selecione o paciente...'}</option>
                      {patients.map(p => (
                        <option key={p.Id || p.id} value={p.Id || p.id}>
                          {p.Name || p.name} (CPF: {p.Cpf || p.cpf})
                        </option>
                      ))}
                    </Field>
                    <ErrorMessage name="patientId" component="div" className="error-text" />
                  </div>
                ) : (
                  <Field name="patientId" type="hidden" />
                )}

                <div className="form-group">
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <User size={16} /> Profissional de Saúde
                  </label>
                  <Field name="healthProfessionalId" as="select" onChange={(e) => {
                    setFieldValue('healthProfessionalId', e.target.value);
                    setFieldValue('startTime', '');
                  }} disabled={user?.role === 1}>
                    <option value="">{loadingProfessionals ? 'Carregando...' : 'Selecione um profissional...'}</option>
                    {professionals.map(p => (
                      <option key={p.Id || p.id} value={p.Id || p.id}>
                        {p.Name || p.name} - {p.Specialization || p.specialization}
                      </option>
                    ))}
                  </Field>
                  <ErrorMessage name="healthProfessionalId" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Calendar size={16} /> Data da Consulta
                  </label>
                  <Field name="appointmentDate" type="date" min={new Date().toISOString().split('T')[0]} onChange={(e) => {
                    setFieldValue('appointmentDate', e.target.value);
                    setFieldValue('startTime', '');
                  }} />
                  <ErrorMessage name="appointmentDate" component="div" className="error-text" />
                </div>

                <div className="form-group">
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Clock size={16} /> Horários Disponíveis (Intervalos de 30min)
                  </label>
                  
                  {loadingSlots ? (
                    <div style={{ padding: '1rem', textAlign: 'center', color: 'var(--text-secondary)' }}>Buscando horários...</div>
                  ) : values.healthProfessionalId && values.appointmentDate ? (
                    <div className="slots-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(80px, 1fr))', gap: '8px', marginTop: '10px' }}>
                      {availableSlots.length > 0 ? (
                        availableSlots.map((slot) => {
                          const time = slot.StartTime || slot.startTime || slot;
                          const formattedTime = typeof time === 'string' ? time.substring(0, 5) : time;
                          const isSelected = values.startTime === formattedTime;
                          
                          return (
                            <div 
                              key={formattedTime}
                              onClick={() => setFieldValue('startTime', formattedTime)}
                              style={{
                                padding: '8px',
                                textAlign: 'center',
                                borderRadius: '6px',
                                border: '1px solid',
                                borderColor: isSelected ? 'var(--primary-color)' : '#ddd',
                                backgroundColor: isSelected ? 'var(--primary-color)' : 'transparent',
                                color: isSelected ? 'white' : 'inherit',
                                cursor: 'pointer',
                                fontSize: '0.9rem',
                                transition: 'all 0.2s'
                              }}
                            >
                              {formattedTime}
                            </div>
                          );
                        })
                      ) : (
                        <div style={{ gridColumn: '1/-1', padding: '1rem', textAlign: 'center', backgroundColor: '#fff4f4', borderRadius: '8px', color: '#c53030', display: 'flex', alignItems: 'center', gap: '8px', justifyContent: 'center' }}>
                          <AlertCircle size={18} /> Sem horários disponíveis para esta data.
                        </div>
                      )}
                    </div>
                  ) : (
                    <div style={{ padding: '1rem', textAlign: 'center', border: '1px dashed #ddd', borderRadius: '8px', color: '#999' }}>
                      Selecione um profissional e uma data para ver os horários.
                    </div>
                  )}
                  <Field name="startTime" type="hidden" />
                  <ErrorMessage name="startTime" component="div" className="error-text" />
                </div>

                {status && <div className="error-text" style={{ marginBottom: '1rem', textAlign: 'center' }}>{status}</div>}

                <button type="submit" className="btn-primary" disabled={isSubmitting || !values.startTime} style={{ marginTop: '1rem' }}>
                  {isSubmitting ? 'Confirmando...' : 'Confirmar Agendamento'}
                </button>
              </Form>
            );
          }}
        </Formik>
      </div>
    </div>
  );
};
