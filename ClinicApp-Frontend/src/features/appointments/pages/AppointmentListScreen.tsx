import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { appointmentService } from '../services';
import { professionalService } from '../../professionals/services';
import { useAuth } from '../../../app/providers';
import { Plus, LogOut, Calendar, Clock, User, ClipboardList, UserCircle, Search, Filter } from 'lucide-react';

export const AppointmentListScreen = () => {
  const [appointments, setAppointments] = useState([]);
  const [filteredAppointments, setFilteredAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [professionals, setProfessionals] = useState([]);
  const [selectedProfessional, setSelectedProfessional] = useState('all');
  const [searchTerm, setSearchTerm] = useState('');
  const navigate = useNavigate();
  const { logout, user } = useAuth();

  const isProfessional = user?.role === 1;
  const isAdminOrAgent = user?.role === 0 || user?.role === 3;

  const fetchProfessionals = async () => {
    if (isAdminOrAgent) {
      try {
        const data = await professionalService.getActive();
        if (Array.isArray(data)) setProfessionals(data);
        else if (data?.Data) setProfessionals(data.Data);
      } catch (error) {
        console.error('Erro ao carregar profissionais:', error);
      }
    }
  };

  const fetchAppointments = async () => {
    if (!user?.id) return;
    
    try {
      setLoading(true);
      let data;
      if (user.role === 1 && user.healthProfessionalId) {
        data = await appointmentService.getByProfessionalId(user.healthProfessionalId);
      } else if (user.role === 2 && user.patientId) {
        data = await appointmentService.getByPatientId(user.patientId);
      } else if (isAdminOrAgent) {
        // Para admin/agente, buscar todos os agendamentos
        data = await appointmentService.getAll();
      }

      let appointmentsList = [];
      if (Array.isArray(data)) {
        appointmentsList = data;
      } else if (data && Array.isArray(data.Data)) {
        appointmentsList = data.Data;
      }
      
      setAppointments(appointmentsList);
      setFilteredAppointments(appointmentsList);
    } catch (error) {
      console.error('Erro ao carregar consultas:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppointments();
    fetchProfessionals();
  }, [user?.id, user?.role]);

  useEffect(() => {
    let result = appointments;

    // Filtro por Profissional (Select)
    if (selectedProfessional !== 'all') {
      result = result.filter(a => (a.HealthProfessionalId || a.healthProfessionalId) === selectedProfessional);
    }

    // Filtro por Nome (Busca)
    if (searchTerm.trim() !== '') {
      const term = searchTerm.toLowerCase();
      result = result.filter(a => 
        (a.HealthProfessionalName || a.healthProfessionalName || '').toLowerCase().includes(term) ||
        (a.PatientName || a.patientName || '').toLowerCase().includes(term) ||
        (a.Specialization || a.specialization || '').toLowerCase().includes(term)
      );
    }

    setFilteredAppointments(result);
  }, [selectedProfessional, searchTerm, appointments]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getStatusClass = (status) => {
    switch (status) {
      case 0: return 'status-scheduled';
      case 1: return 'status-canceled';
      case 2: return 'status-completed';
      default: return 'status-scheduled';
    }
  };

  const getStatusText = (status) => {
    switch (status) {
      case 0: return 'Agendado';
      case 1: return 'Cancelado';
      case 2: return 'Concluído';
      default: return 'Agendado';
    }
  };

  return (
    <div className="container">
      <div className="glass-card" style={{ minHeight: '80vh', padding: '2rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem', borderBottom: '1px solid #eee', paddingBottom: '1rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <ClipboardList size={28} color="var(--primary-color)" />
            <h1 style={{ margin: 0, fontSize: '1.5rem', textAlign: 'left' }}>
              {user?.role === 1 ? 'Minha Agenda de Consultas' : 'Minhas Consultas'}
            </h1>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
            <button onClick={() => navigate('/schedule')} className="btn-primary" style={{ padding: '8px 16px', fontSize: '0.9rem' }}>
              <Plus size={18} style={{ marginRight: '5px' }} /> Nova Consulta
            </button>
            <button onClick={handleLogout} className="btn-link" style={{ display: 'flex', alignItems: 'center', gap: '5px', color: 'var(--error-color)' }}>
              <LogOut size={18} /> Sair
            </button>
          </div>
        </div>

        {isAdminOrAgent && (
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '15px', marginBottom: '2rem', backgroundColor: '#f9f9f9', padding: '1.5rem', borderRadius: '12px' }}>
            <div style={{ flex: '1', minWidth: '250px' }}>
              <label style={{ display: 'block', fontSize: '0.85rem', marginBottom: '5px', color: 'var(--text-secondary)' }}>
                <Search size={14} style={{ marginRight: '5px' }} /> Pesquisar Profissional ou Paciente
              </label>
              <input 
                type="text" 
                placeholder="Nome, especialidade..." 
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #ddd' }}
              />
            </div>
            
            <div style={{ flex: '1', minWidth: '250px' }}>
              <label style={{ display: 'block', fontSize: '0.85rem', marginBottom: '5px', color: 'var(--text-secondary)' }}>
                <Filter size={14} style={{ marginRight: '5px' }} /> Filtrar por Profissional
              </label>
              <select 
                value={selectedProfessional}
                onChange={(e) => setSelectedProfessional(e.target.value)}
                style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #ddd', backgroundColor: 'white' }}
              >
                <option value="all">Todos os Profissionais</option>
                {professionals.map(p => (
                  <option key={p.Id || p.id} value={p.Id || p.id}>
                    {p.FullName || p.fullName} ({p.Specialization || p.specialization})
                  </option>
                ))}
              </select>
            </div>
          </div>
        )}

        {loading ? (
          <div style={{ textAlign: 'center', marginTop: '5rem' }}>
            <div>Carregando agenda...</div>
          </div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1.5rem' }}>
            {filteredAppointments.length === 0 ? (
              <div style={{ gridColumn: '1/-1', textAlign: 'center', marginTop: '3rem', color: 'var(--text-secondary)' }}>
                <Calendar size={48} style={{ opacity: 0.2, marginBottom: '1rem' }} />
                <p>Nenhuma consulta encontrada.</p>
                {!isProfessional && (
                  <button onClick={() => navigate('/schedule')} className="btn-link" style={{ marginTop: '1rem' }}>Agendar minha primeira consulta</button>
                )}
              </div>
            ) : (
              filteredAppointments.map((item) => (
                <div key={item.Id || item.id} className="appointment-card">
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '1rem' }}>
                    <span className={`status-badge ${getStatusClass(item.Status ?? item.status)}`}>
                      {getStatusText(item.Status ?? item.status)}
                    </span>
                    <span style={{ fontSize: '0.8rem', color: '#999' }}>#{ (item.Id || item.id).substring(0, 8) }</span>
                  </div>
                  
                  <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '0.8rem' }}>
                    <Calendar size={16} color="var(--text-secondary)" />
                    <span style={{ fontWeight: 600 }}>
                      {new Date(item.AppointmentDate || item.appointmentDate).toLocaleDateString('pt-BR')}
                    </span>
                  </div>

                  <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '0.8rem' }}>
                    <Clock size={16} color="var(--text-secondary)" />
                    <span>{ (item.StartTime || item.startTime).substring(0, 5) } (30 min)</span>
                  </div>

                  <div style={{ display: 'flex', alignItems: 'center', gap: '10px', borderTop: '1px solid #f0f0f0', paddingTop: '0.8rem', marginTop: '0.5rem' }}>
                    {user?.role === 1 ? (
                      <>
                        <UserCircle size={16} color="var(--primary-color)" />
                        <div>
                          <div style={{ fontSize: '0.8rem', color: '#999' }}>Paciente</div>
                          <div style={{ fontWeight: 500 }}>{item.PatientName || item.patientName || 'Paciente'}</div>
                        </div>
                      </>
                    ) : (
                      <>
                        <User size={16} color="var(--primary-color)" />
                        <div>
                          <div style={{ fontSize: '0.8rem', color: '#999' }}>Profissional</div>
                          <div style={{ fontWeight: 500 }}>{item.HealthProfessionalName || item.healthProfessionalName || 'Médico'}</div>
                          <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{item.Specialization || item.specialization}</div>
                        </div>
                      </>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        )}
      </div>
    </div>
  );
};
