import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { patientService } from '../services';
import { useAuth } from '../../../app/providers';
import { Plus, Search, User, Mail, Phone, Calendar, Trash2, Edit } from 'lucide-react';
import { toast } from 'sonner';
import { Modal } from '../../../shared/components/Modal';

export const PatientListScreen = () => {
  const [patients, setPatients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [modalConfig, setModalConfig] = useState<{
    isOpen: boolean;
    id?: string;
  }>({ isOpen: false });
  const navigate = useNavigate();

  const fetchPatients = async () => {
    try {
      setLoading(true);
      const data = await patientService.getAll();
      if (Array.isArray(data)) {
        setPatients(data);
      } else if (data && Array.isArray(data.Data)) {
        setPatients(data.Data);
      } else {
        setPatients([]);
      }
    } catch (error) {
      console.error('Erro ao carregar pacientes:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPatients();
  }, []);

  const handleDelete = async (id) => {
    setModalConfig({ isOpen: true, id });
  };

  const confirmDelete = async () => {
    const id = modalConfig.id;
    if (!id) return;

    try {
      await patientService.delete(id);
      setPatients(patients.filter(p => p.Id !== id));
      toast.success('Paciente excluído com sucesso!');
    } catch (error) {
      toast.error('Erro ao excluir paciente: ' + (error.message || 'Erro desconhecido'));
    } finally {
      setModalConfig({ isOpen: false });
    }
  };

  const filteredPatients = patients.filter(p => 
    p.Name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.Cpf.includes(searchTerm)
  );

  return (
    <div className="container">
      <div className="glass-card" style={{ minHeight: '80vh' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
          <h1>Gestão de Pacientes</h1>
          <button className="btn-primary" style={{ width: 'auto', padding: '0.5rem 1.5rem' }} onClick={() => navigate('/patients/new')}>
            <Plus size={18} style={{ marginRight: '8px' }} /> Novo Paciente
          </button>
        </div>

        <div className="form-group" style={{ position: 'relative' }}>
          <Search style={{ position: 'absolute', left: '12px', top: '12px', color: '#999' }} size={20} />
          <input 
            type="text" 
            placeholder="Buscar por nome ou CPF..." 
            style={{ paddingLeft: '45px' }}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        {loading ? (
          <div style={{ textAlign: 'center', padding: '3rem' }}>Carregando pacientes...</div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1.5rem' }}>
            {filteredPatients.length === 0 ? (
              <div style={{ gridColumn: '1/-1', textAlign: 'center', padding: '3rem', color: '#666' }}>
                Nenhum paciente encontrado.
              </div>
            ) : (
              filteredPatients.map(patient => (
                <div key={patient.Id} className="appointment-card" style={{ borderLeftColor: '#52c41a' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <h2 style={{ margin: 0 }}>{patient.Name}</h2>
                    <div style={{ display: 'flex', gap: '10px' }}>
                      <button onClick={() => navigate(`/patients/edit/${patient.Id}`)} className="btn-link"><Edit size={18} /></button>
                      <button onClick={() => handleDelete(patient.Id)} className="btn-link" style={{ color: 'var(--error-color)' }}><Trash2 size={18} /></button>
                    </div>
                  </div>
                  
                  <div style={{ marginTop: '1rem', fontSize: '0.9rem', color: '#555' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <User size={14} /> CPF: {patient.Cpf}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Mail size={14} /> {patient.Email}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '0.5rem' }}>
                      <Phone size={14} /> {patient.Phone}
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <Calendar size={14} /> Nasc: {new Date(patient.DateOfBirth).toLocaleDateString()}
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
        title="Excluir Paciente"
        message="Tem certeza que deseja excluir este paciente? Esta ação não pode ser desfeita."
        type="confirm"
        confirmText="Excluir"
        onClose={() => setModalConfig({ isOpen: false })}
        onConfirm={confirmDelete}
      />
    </div>
  );
};
