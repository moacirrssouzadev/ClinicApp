import React from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../app/providers';
import { 
  LayoutDashboard, 
  Users, 
  Stethoscope, 
  Calendar, 
  UserCircle, 
  LogOut,
  ChevronRight,
  Menu,
  X,
  PlusCircle
} from 'lucide-react';

export const Layout = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [isSidebarOpen, setSidebarOpen] = React.useState(true);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const menuItems = [
    { 
      title: 'Início', 
      path: '/', 
      icon: <LayoutDashboard size={20} />, 
      roles: [0, 1, 2, 3] 
    },
    { 
      title: 'Minhas Consultas', 
      path: '/appointments', 
      icon: <Calendar size={20} />, 
      roles: [0, 1, 2, 3] 
    },
    { 
      title: 'Agendar Consulta', 
      path: '/schedule', 
      icon: <PlusCircle size={20} />, 
      roles: [0, 1, 2, 3] 
    },
    { 
      title: 'Pacientes', 
      path: '/patients', 
      icon: <UserCircle size={20} />, 
      roles: [0, 3] 
    },
    { 
      title: 'Profissionais', 
      path: '/professionals', 
      icon: <Stethoscope size={20} />, 
      roles: [0] 
    },
    { 
      title: 'Usuários', 
      path: '/users', 
      icon: <Users size={20} />, 
      roles: [0] 
    },
  ];

  const filteredMenu = menuItems.filter(item => item.roles.includes(user?.role));

  const getRoleLabel = (role) => {
    switch (role) {
      case 0: return 'Administrador';
      case 1: return 'Profissional de Saúde';
      case 2: return 'Paciente';
      case 3: return 'Atendimento';
      default: return 'Usuário';
    }
  };

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--bg-gradient)' }}>
      {/* Sidebar */}
      <aside style={{
        width: isSidebarOpen ? '260px' : '80px',
        background: 'rgba(255, 255, 255, 0.9)',
        backdropFilter: 'blur(10px)',
        borderRight: '1px solid rgba(0,0,0,0.05)',
        transition: 'all 0.3s ease',
        display: 'flex',
        flexDirection: 'column',
        padding: '1.5rem 0'
      }}>
        <div style={{ padding: '0 1.5rem', marginBottom: '2rem', display: 'flex', alignItems: 'center', justifyContent: isSidebarOpen ? 'space-between' : 'center' }}>
          {isSidebarOpen && <h1 style={{ margin: 0, fontSize: '1.2rem', color: 'var(--primary-color)' }}>ClinicApp</h1>}
          <button onClick={() => setSidebarOpen(!isSidebarOpen)} className="btn-link" style={{ padding: 0 }}>
            {isSidebarOpen ? <X size={20} /> : <Menu size={20} />}
          </button>
        </div>

        <nav style={{ flex: 1 }}>
          {filteredMenu.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              style={{
                display: 'flex',
                alignItems: 'center',
                padding: '0.75rem 1.5rem',
                textDecoration: 'none',
                color: location.pathname === item.path ? 'var(--primary-color)' : 'var(--text-secondary)',
                background: location.pathname === item.path ? 'rgba(0, 112, 243, 0.08)' : 'transparent',
                borderLeft: location.pathname === item.path ? '4px solid var(--primary-color)' : '4px solid transparent',
                transition: 'all 0.2s ease',
                gap: '12px',
                fontSize: '0.95rem',
                fontWeight: location.pathname === item.path ? '600' : '500'
              }}
            >
              {item.icon}
              {isSidebarOpen && <span>{item.title}</span>}
              {isSidebarOpen && location.pathname === item.path && <ChevronRight size={16} style={{ marginLeft: 'auto' }} />}
            </Link>
          ))}
        </nav>

        <div style={{ padding: '1rem', borderTop: '1px solid #eee' }}>
          {isSidebarOpen && (
            <div style={{ padding: '0 0.5rem', marginBottom: '1rem' }}>
              <p style={{ margin: 0, fontSize: '0.9rem', fontWeight: 600, color: 'var(--primary-color)' }}>{user?.name}</p>
              <p style={{ margin: 0, fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{getRoleLabel(user?.role)}</p>
            </div>
          )}
          <button 
            onClick={handleLogout} 
            className="btn-link" 
            style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '12px', 
              color: 'var(--error-color)',
              padding: '0.5rem',
              width: '100%',
              justifyContent: isSidebarOpen ? 'flex-start' : 'center'
            }}
          >
            <LogOut size={20} />
            {isSidebarOpen && <span>Sair</span>}
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main style={{ flex: 1, padding: '2rem', overflowY: 'auto' }}>
        {children}
      </main>
    </div>
  );
};
