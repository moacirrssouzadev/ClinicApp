import React from 'react';
import { X, AlertCircle, CheckCircle, Info, HelpCircle } from 'lucide-react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm?: () => void;
  title: string;
  message: string;
  type?: 'info' | 'success' | 'error' | 'confirm';
  confirmText?: string;
  cancelText?: string;
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  type = 'info',
  confirmText = 'OK',
  cancelText = 'Cancelar'
}) => {
  if (!isOpen) return null;

  const getIcon = () => {
    switch (type) {
      case 'success': return <CheckCircle size={48} color="var(--success-color)" />;
      case 'error': return <AlertCircle size={48} color="var(--error-color)" />;
      case 'confirm': return <HelpCircle size={48} color="var(--primary-color)" />;
      default: return <Info size={48} color="var(--primary-color)" />;
    }
  };

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000,
      backdropFilter: 'blur(4px)',
      animation: 'fadeIn 0.2s ease-out'
    }}>
      <div className="glass-card" style={{
        maxWidth: '450px',
        width: '90%',
        padding: '2rem',
        textAlign: 'center',
        position: 'relative',
        boxShadow: '0 20px 40px rgba(0,0,0,0.2)',
        animation: 'slideUp 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)'
      }}>
        <button 
          onClick={onClose}
          style={{
            position: 'absolute',
            top: '1rem',
            right: '1rem',
            background: 'none',
            border: 'none',
            cursor: 'pointer',
            color: '#999'
          }}
        >
          <X size={20} />
        </button>

        <div style={{ marginBottom: '1.5rem' }}>
          {getIcon()}
        </div>

        <h2 style={{ marginBottom: '1rem', fontSize: '1.5rem' }}>{title}</h2>
        <p style={{ color: 'var(--text-secondary)', marginBottom: '2rem' }}>{message}</p>

        <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
          {type === 'confirm' && (
            <button 
              className="btn-link" 
              onClick={onClose}
              style={{ padding: '0.75rem 1.5rem', border: '1px solid #e1e1e1', borderRadius: '8px' }}
            >
              {cancelText}
            </button>
          )}
          <button 
            className="btn-primary" 
            style={{ width: type === 'confirm' ? 'auto' : '100%', minWidth: '120px' }}
            onClick={() => {
              if (onConfirm) onConfirm();
              else onClose();
            }}
          >
            {confirmText}
          </button>
        </div>
      </div>

      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; }
          to { opacity: 1; }
        }
        @keyframes slideUp {
          from { transform: translateY(20px); opacity: 0; }
          to { transform: translateY(0); opacity: 1; }
        }
      `}</style>
    </div>
  );
};
