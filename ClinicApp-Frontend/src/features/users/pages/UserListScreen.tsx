import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { userService } from '../services';
import { useAuth } from '../../../app/providers';
import { Plus, Search, Mail, Shield, Trash2, Edit, Users } from 'lucide-react';
import { toast } from 'sonner';
import { Modal } from '../../../shared/components/Modal';

export const UserListScreen = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [modalConfig, setModalConfig] = useState<{
    isOpen: boolean;
    id?: string;
  }>({ isOpen: false });
  const navigate = useNavigate();

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await userService.getAll();
      if (Array.isArray(data)) {
        setUsers(data);
      } else if (data && Array.isArray(data.Data)) {
        setUsers(data.Data);
      } else {
        setUsers([]);
      }
    } catch (error) {
      console.error('Erro ao carregar usuários:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleDelete = async (id) => {
    setModalConfig({ isOpen: true, id });
  };

  const confirmDelete = async () => {
    const id = modalConfig.id;
    if (!id) return;

    try {
      await userService.delete(id);
      setUsers(users.filter(u => (u.Id || u.id) !== id));
      toast.success('Usuário excluído com sucesso!');
    } catch (error) {
      toast.error('Erro ao excluir usuário: ' + (error.message || 'Erro desconhecido'));
    } finally {
      setModalConfig({ isOpen: false });
    }
  };

  const filteredUsers = users.filter(u => 
    (u.Username || u.username || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (u.Email || u.email || '').toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getRoleName = (role) => {
    switch (role) {
      case 0: return 'Administrador';
      case 1: return 'Médico';
      case 2: return 'Paciente';
      case 3: return 'Agente';
      default: return 'Usuário';
    }
  };

  return (
    <div className="container">
      <div className="glass-card" style={{ minHeight: '80vh' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <Users size={28} color="var(--primary-color)" />
            <h1 style={{ margin: 0, fontSize: '1.5rem' }}>Gestão de Usuários</h1>
          </div>
          <button className="btn-primary" style={{ width: 'auto', padding: '0.5rem 1.5rem' }} onClick={() => navigate('/users/new')}>
            <Plus size={18} style={{ marginRight: '8px' }} /> Novo Usuário
          </button>
        </div>

        <div className="form-group" style={{ position: 'relative' }}>
          <Search style={{ position: 'absolute', left: '12px', top: '12px', color: '#999' }} size={20} />
          <input 
            type="text" 
            placeholder="Buscar por username ou e-mail..." 
            style={{ paddingLeft: '45px' }}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        {loading ? (
          <div style={{ textAlign: 'center', padding: '3rem' }}>Carregando usuários...</div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1.5rem' }}>
            {filteredUsers.length === 0 ? (
              <div style={{ gridColumn: '1/-1', textAlign: 'center', padding: '3rem', color: '#666' }}>
                Nenhum usuário encontrado.
              </div>
            ) : (
              filteredUsers.map(userItem => (
                <div key={userItem.Id || userItem.id} className="appointment-card" style={{ borderLeftColor: '#ff9c6e' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <h2 style={{ margin: 0 }}>{userItem.Username || userItem.username}</h2>
                    <div style={{ display: 'flex', gap: '10px' }}>
                      <button onClick={() => navigate(`/users/edit/${userItem.Id || userItem.id}`)} className="btn-link"><Edit size={18} /></button>
                      <button onClick={() => handleDelete(userItem.Id || userItem.id)} className="btn-link" style={{ color: 'var(--error-color)' }}><Trash2 size={18} /></button>
                    </div>
                  </div>
                  
                  <div style={{ marginTop: '1rem', fontSize: '0.9rem', color: '#555' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Mail size={14} /> {userItem.Email || userItem.email}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Shield size={14} /> Nível: {getRoleName(userItem.Role ?? userItem.role)}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <div className={`status-badge ${(userItem.IsActive ?? userItem.isActive) ? 'status-completed' : 'status-canceled'}`} style={{ padding: '2px 8px', fontSize: '0.7rem' }}>
                        {(userItem.IsActive ?? userItem.isActive) ? 'Ativo' : 'Inativo'}
                      </div>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        )}
      </div>

      <Modal 
        isOpen={modalConfig.isOpen}
        title="Excluir Usuário"
        message="Tem certeza que deseja excluir este usuário? Esta ação não pode ser desfeita."
        type="confirm"
        confirmText="Excluir"
        onClose={() => setModalConfig({ isOpen: false })}
        onConfirm={confirmDelete}
      />
    </div>
  );
};
