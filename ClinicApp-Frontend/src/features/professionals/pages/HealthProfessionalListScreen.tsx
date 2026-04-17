import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { professionalService } from '../services';
import { useAuth } from '../../../app/providers';
import { Plus, Search, Mail, Briefcase, Trash2, Edit, Stethoscope } from 'lucide-react';
import { toast } from 'sonner';
import { Modal } from '../../../shared/components/Modal';

export const HealthProfessionalListScreen = () => {
  const [professionals, setProfessionals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [modalConfig, setModalConfig] = useState<{
    isOpen: boolean;
    id?: string;
  }>({ isOpen: false });
  const navigate = useNavigate();

  const fetchProfessionals = async () => {
    try {
      setLoading(true);
      const data = await professionalService.getAll();
      if (Array.isArray(data)) {
        setProfessionals(data);
      } else if (data && Array.isArray(data.Data)) {
        setProfessionals(data.Data);
      } else {
        setProfessionals([]);
      }
    } catch (error) {
      console.error('Erro ao carregar profissionais:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProfessionals();
  }, []);

  const handleDelete = async (id) => {
    setModalConfig({ isOpen: true, id });
  };

  const confirmDelete = async () => {
    const id = modalConfig.id;
    if (!id) return;

    try {
      await professionalService.delete(id);
      setProfessionals(professionals.filter(p => (p.Id || p.id) !== id));
      toast.success('Profissional excluído com sucesso!');
    } catch (error) {
      toast.error('Erro ao excluir profissional: ' + (error.message || 'Erro desconhecido'));
    } finally {
      setModalConfig({ isOpen: false });
    }
  };

  const filteredProfessionals = professionals.filter(p => 
    (p.Name || p.name || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (p.Specialization || p.specialization || '').toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="container">
      <div className="glass-card" style={{ minHeight: '80vh' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <Stethoscope size={28} color="var(--primary-color)" />
            <h1 style={{ margin: 0, fontSize: '1.5rem' }}>Gestão de Profissionais</h1>
          </div>
          <button className="btn-primary" style={{ width: 'auto', padding: '0.5rem 1.5rem' }} onClick={() => navigate('/professionals/new')}>
            <Plus size={18} style={{ marginRight: '8px' }} /> Novo Profissional
          </button>
        </div>

        <div className="form-group" style={{ position: 'relative' }}>
          <Search style={{ position: 'absolute', left: '12px', top: '12px', color: '#999' }} size={20} />
          <input 
            type="text" 
            placeholder="Buscar por nome ou especialidade..." 
            style={{ paddingLeft: '45px' }}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        {loading ? (
          <div style={{ textAlign: 'center', padding: '3rem' }}>Carregando profissionais...</div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1.5rem' }}>
            {filteredProfessionals.length === 0 ? (
              <div style={{ gridColumn: '1/-1', textAlign: 'center', padding: '3rem', color: '#666' }}>
                Nenhum profissional encontrado.
              </div>
            ) : (
              filteredProfessionals.map(prof => (
                <div key={prof.Id || prof.id} className="appointment-card" style={{ borderLeftColor: 'var(--primary-color)' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <h2 style={{ margin: 0 }}>{prof.Name || prof.name}</h2>
                    <div style={{ display: 'flex', gap: '10px' }}>
                      <button onClick={() => navigate(`/professionals/edit/${prof.Id || prof.id}`)} className="btn-link"><Edit size={18} /></button>
                      <button onClick={() => handleDelete(prof.Id || prof.id)} className="btn-link" style={{ color: 'var(--error-color)' }}><Trash2 size={18} /></button>
                    </div>
                  </div>
                  
                  <div style={{ marginTop: '1rem', fontSize: '0.9rem', color: '#555' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Briefcase size={14} /> Especialidade: {prof.Specialization || prof.specialization}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Stethoscope size={14} /> CRM: {prof.LicenseNumber || prof.licenseNumber}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <div className={`status-badge ${(prof.IsActive ?? prof.isActive) ? 'status-completed' : 'status-canceled'}`} style={{ padding: '2px 8px', fontSize: '0.7rem' }}>
                        {(prof.IsActive ?? prof.isActive) ? 'Ativo' : 'Inativo'}
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
        title="Excluir Profissional"
        message="Tem certeza que deseja excluir este profissional? Esta ação não pode ser desfeita."
        type="confirm"
        confirmText="Excluir"
        onClose={() => setModalConfig({ isOpen: false })}
        onConfirm={confirmDelete}
      />
    </div>
  );
};
