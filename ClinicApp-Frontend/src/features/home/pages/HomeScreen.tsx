import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../app/providers';
import { 
  Calendar, 
  Users, 
  UserCircle, 
  Stethoscope, 
  Clock, 
  TrendingUp, 
  PlusCircle,
  ClipboardList,
  Activity
} from 'lucide-react';

export const HomeScreen = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const getGreeting = () => {
    const hour = new Date().getHours();
    if (hour < 12) return 'Bom dia';
    if (hour < 18) return 'Boa tarde';
    return 'Boa noite';
  };

  const stats = [
    { 
      title: 'Consultas Hoje', 
      value: '12', 
      icon: <Calendar size={24} />, 
      color: '#4f46e5',
      roles: [0, 1, 3] 
    },
    { 
      title: 'Novos Pacientes', 
      value: '4', 
      icon: <Users size={24} />, 
      color: '#10b981',
      roles: [0, 3] 
    },
    { 
      title: 'Próxima Consulta', 
      value: '14:30', 
      icon: <Clock size={24} />, 
      color: '#f59e0b',
      roles: [0, 1, 2] 
    },
    { 
      title: 'Consultas Concluídas', 
      value: '85%', 
      icon: <Activity size={24} />, 
      color: '#ec4899',
      roles: [0, 1] 
    }
  ].filter(stat => !stat.roles || stat.roles.includes(user?.role));

  const quickActions = [
    {
      title: 'Agendar Consulta',
      description: 'Marque uma nova consulta no sistema',
      icon: <PlusCircle size={24} />,
      path: '/schedule',
      color: '#4f46e5',
      roles: [0, 1, 2, 3]
    },
    {
      title: 'Ver Pacientes',
      description: 'Gerencie o cadastro de pacientes',
      icon: <UserCircle size={24} />,
      path: '/patients',
      color: '#10b981',
      roles: [0, 3]
    },
    {
      title: 'Minhas Consultas',
      description: 'Veja sua agenda de atendimentos',
      icon: <ClipboardList size={24} />,
      path: '/appointments',
      color: '#f59e0b',
      roles: [0, 1, 2, 3]
    },
    {
      title: 'Profissionais',
      description: 'Lista de profissionais da saúde',
      icon: <Stethoscope size={24} />,
      path: '/professionals',
      color: '#8b5cf6',
      roles: [0]
    }
  ].filter(action => action.roles.includes(user?.role));

  return (
    <div className="container">
      <div style={{ marginBottom: '2rem' }}>
        <h1 style={{ fontSize: '1.875rem', fontWeight: 700, color: '#111827', marginBottom: '0.5rem' }}>
          {getGreeting()}, {user?.fullName?.split(' ')[0] || 'Usuário'}!
        </h1>
        <p style={{ color: '#6b7280', fontSize: '1rem' }}>
          Bem-vindo ao ClinicApp. Aqui está um resumo do que está acontecendo hoje.
        </p>
      </div>

      {/* Stats Grid */}
      <div style={{ 
        display: 'grid', 
        gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', 
        gap: '1.5rem',
        marginBottom: '2.5rem'
      }}>
        {stats.map((stat, index) => (
          <div key={index} className="glass-card" style={{ 
            padding: '1.5rem', 
            display: 'flex', 
            alignItems: 'center', 
            gap: '1rem',
            transition: 'transform 0.2s',
            cursor: 'default'
          }}>
            <div style={{ 
              padding: '0.75rem', 
              borderRadius: '12px', 
              backgroundColor: `${stat.color}15`, 
              color: stat.color 
            }}>
              {stat.icon}
            </div>
            <div>
              <p style={{ margin: 0, fontSize: '0.875rem', color: '#6b7280', fontWeight: 500 }}>{stat.title}</p>
              <p style={{ margin: 0, fontSize: '1.5rem', fontWeight: 700, color: '#111827' }}>{stat.value}</p>
            </div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(350px, 1fr))', gap: '2rem' }}>
        {/* Quick Actions */}
        <div>
          <h2 style={{ fontSize: '1.25rem', fontWeight: 600, color: '#111827', marginBottom: '1.25rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <TrendingUp size={20} color="var(--primary-color)" />
            Ações Rápidas
          </h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {quickActions.map((action, index) => (
              <button
                key={index}
                onClick={() => navigate(action.path)}
                className="glass-card"
                style={{
                  width: '100%',
                  padding: '1.25rem',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '1rem',
                  textAlign: 'left',
                  border: '1px solid rgba(255, 255, 255, 0.3)',
                  cursor: 'pointer',
                  transition: 'all 0.2s'
                }}
                onMouseOver={(e) => {
                  e.currentTarget.style.transform = 'translateY(-2px)';
                  e.currentTarget.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1)';
                }}
                onMouseOut={(e) => {
                  e.currentTarget.style.transform = 'translateY(0)';
                  e.currentTarget.style.boxShadow = 'none';
                }}
              >
                <div style={{ 
                  padding: '0.625rem', 
                  borderRadius: '10px', 
                  backgroundColor: `${action.color}15`, 
                  color: action.color 
                }}>
                  {action.icon}
                </div>
                <div>
                  <h3 style={{ margin: 0, fontSize: '1rem', fontWeight: 600, color: '#111827' }}>{action.title}</h3>
                  <p style={{ margin: 0, fontSize: '0.875rem', color: '#6b7280' }}>{action.description}</p>
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* System Info or Recent Activity Placeholder */}
        <div>
          <h2 style={{ fontSize: '1.25rem', fontWeight: 600, color: '#111827', marginBottom: '1.25rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <Activity size={20} color="var(--primary-color)" />
            Avisos do Sistema
          </h2>
          <div className="glass-card" style={{ padding: '1.5rem', height: 'fit-content' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
              <div style={{ paddingBottom: '1rem', borderBottom: '1px solid #f3f4f6' }}>
                <span style={{ 
                  fontSize: '0.75rem', 
                  fontWeight: 600, 
                  color: '#4f46e5', 
                  backgroundColor: '#4f46e510', 
                  padding: '0.25rem 0.5rem', 
                  borderRadius: '4px',
                  marginBottom: '0.5rem',
                  display: 'inline-block'
                }}>NOVIDADE</span>
                <h3 style={{ margin: '0 0 0.25rem 0', fontSize: '0.9375rem', fontWeight: 600 }}>Atualização do Sistema v1.2</h3>
                <p style={{ margin: 0, fontSize: '0.875rem', color: '#6b7280', lineHeight: 1.5 }}>
                  Novos relatórios de atendimento foram adicionados para administradores.
                </p>
              </div>
              <div>
                <span style={{ 
                  fontSize: '0.75rem', 
                  fontWeight: 600, 
                  color: '#f59e0b', 
                  backgroundColor: '#f59e0b10', 
                  padding: '0.25rem 0.5rem', 
                  borderRadius: '4px',
                  marginBottom: '0.5rem',
                  display: 'inline-block'
                }}>MANUTENÇÃO</span>
                <h3 style={{ margin: '0 0 0.25rem 0', fontSize: '0.9375rem', fontWeight: 600 }}>Backup Semanal</h3>
                <p style={{ margin: 0, fontSize: '0.875rem', color: '#6b7280', lineHeight: 1.5 }}>
                  O sistema passará por backup automático domingo às 02:00.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
