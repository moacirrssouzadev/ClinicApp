-- ===================================
-- Script de Criação das Tabelas
-- Database: ClinicApp
-- ===================================
CREATE DATABASE ClinicApp;
GO

USE ClinicApp;
GO

-- Tabela de Pacientes
CREATE TABLE Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Cpf NVARCHAR(14) NOT NULL UNIQUE,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    Address NVARCHAR(250) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NULL,
    INDEX IX_Patients_Email (Email),
    INDEX IX_Patients_Cpf (Cpf),
    INDEX IX_Patients_IsActive (IsActive)
);

-- Tabela de Profissionais de Saúde
CREATE TABLE HealthProfessionals (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Cpf NVARCHAR(14) NOT NULL UNIQUE,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    LicenseNumber NVARCHAR(20) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NULL,
    INDEX IX_HealthProfessionals_Email (Email),
    INDEX IX_HealthProfessionals_Cpf (Cpf),
    INDEX IX_HealthProfessionals_LicenseNumber (LicenseNumber),
    INDEX IX_HealthProfessionals_Specialization (Specialization),
    INDEX IX_HealthProfessionals_IsActive (IsActive)
);

-- Tabela de Usuários (Autenticação)
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(512) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Role INT NOT NULL DEFAULT 2,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLoginAt DATETIME NULL,
    RefreshToken NVARCHAR(512) NULL,
    RefreshTokenExpiryTime DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NULL,
    INDEX IX_Users_Username (Username),
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_Role (Role),
    INDEX IX_Users_IsActive (IsActive)
);

-- Tabela de Agendamentos
CREATE TABLE Appointments (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    HealthProfessionalId UNIQUEIDENTIFIER NOT NULL,
    AppointmentDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Appointments_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id),
    CONSTRAINT FK_Appointments_HealthProfessional FOREIGN KEY (HealthProfessionalId) REFERENCES HealthProfessionals(Id),
    INDEX IX_Appointments_PatientId (PatientId),
    INDEX IX_Appointments_HealthProfessionalId (HealthProfessionalId),
    INDEX IX_Appointments_AppointmentDate (AppointmentDate),
    INDEX IX_Appointments_Status (Status)
);

-- ===================================
-- Inserir Dados de Teste
-- ===================================

-- Usuários de Teste
-- Senhas (hashes PBKDF2): Admin123! | Medico123! | Paciente123!
INSERT INTO Users (Id, Username, Email, PasswordHash, FullName, Role, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'moacir', 'moacir@clinicapp.com', 'VgMOj/nPGlBtcsfy38n84tLIDJYNsGZAvq93HOtZL2TkafYJ', 'Moacir Sabino', 0, 1, GETUTCDATE()),
    (NEWID(), 'moacirsouza', 'moacirsouza@clinicapp.com', 'VgMOj/nPGlBtcsfy38n84tLIDJYNsGZAvq93HOtZL2TkafYJ', 'Moacir Souza', 0, 1, GETUTCDATE()),
    (NEWID(), 'ana', 'ana@clinicapp.com', '+/HCmN0bQ9Ln77E6BAUX5mb57gC/tAt1Q6Jh6tp9tL84NM+6', 'Dra. Ana Costa', 1, 1, GETUTCDATE()),
    (NEWID(), 'admin', 'admin@clinicapp.com', 'VgMOj/nPGlBtcsfy38n84tLIDJYNsGZAvq93HOtZL2TkafYJ', 'Administrador', 0, 1, GETUTCDATE()),
    (NEWID(), 'medico', 'medico@clinicapp.com', 'R3nNCl3SqxFMY7PJ57XIEjGuFWB3p+gNpza7R9g6qm1CtacN', 'Dr. Médico', 1, 1, GETUTCDATE()),
    (NEWID(), 'paciente', 'paciente@clinicapp.com', '6s/VosR7Ocmuwj8keQS8esKw96crmwNX8RP9BvIIAw4lKVws', 'João Paciente', 2, 1, GETUTCDATE());

-- Paciente de Teste
INSERT INTO Patients (Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'João Paciente', '000.000.000-00', 'paciente@clinicapp.com', '(11) 90000-0000', '1990-01-01', 'Endereço de Teste, 123', 1, GETUTCDATE()),
    (NEWID(), 'João da Silva', '123.456.789-10', 'joao@email.com', '(11) 98765-4321', '1990-05-15', 'Rua A, 123, São Paulo, SP', 1, GETUTCDATE()),
    (NEWID(), 'Maria Santos', '987.654.321-00', 'maria@email.com', '(11) 99876-5432', '1985-08-22', 'Rua B, 456, São Paulo, SP', 1, GETUTCDATE());

-- Profissional de Teste
INSERT INTO HealthProfessionals (Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'Dr. Carlos Oliveira', '111.222.333-44', 'carlos@clinicapp.com', '(11) 91234-5678', 'Cardiologia', 'CRM12345', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Ana Costa', '555.666.777-88', 'ana@clinicapp.com', '(11) 92345-6789', 'Dermatologia', 'CRM54321', 1, GETUTCDATE()),
    (NEWID(), 'Dr. Pedro Silva', '999.888.777-66', 'pedro@clinicapp.com', '(11) 93456-7890', 'Pediatria', 'CRM99999', 1, GETUTCDATE());

-- ===================================
-- Criar Índices de Performance 
-- ===================================

-- Índice composto para buscas frequentes de agendamentos
CREATE INDEX IX_Appointments_HealthProfessional_Date ON Appointments(HealthProfessionalId, AppointmentDate, Status);
CREATE INDEX IX_Appointments_Patient_HealthProfessional_Date ON Appointments(PatientId, HealthProfessionalId, AppointmentDate, Status);
-- Created by GitHub Copilot in SSMS - review carefully before executing

-- ===================================
-- Limpar dados existentes (opcional)
-- ===================================
-- DELETE FROM Appointments;
-- DELETE FROM Patients;
-- DELETE FROM HealthProfessionals;

-- ===================================
-- Inserir 10 Pacientes
-- ===================================
INSERT INTO Patients (Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'João da Silva', '123.456.789-10', 'joao@email.com', '(11) 98765-4321', '1990-05-15', 'Rua A, 123, São Paulo, SP', 1, GETUTCDATE()),
    (NEWID(), 'Maria Santos', '987.654.321-00', 'maria@email.com', '(11) 99876-5432', '1985-08-22', 'Rua B, 456, São Paulo, SP', 1, GETUTCDATE()),
    (NEWID(), 'Carlos Mendes', '456.789.123-45', 'carlos.mendes@email.com', '(21) 98765-1234', '1992-03-10', 'Av. Paulista, 1000, São Paulo, SP', 1, GETUTCDATE()),
    (NEWID(), 'Ana Ferreira', '789.123.456-78', 'ana.ferreira@email.com', '(21) 99876-2345', '1988-07-18', 'Rua C, 789, Rio de Janeiro, RJ', 1, GETUTCDATE()),
    (NEWID(), 'Pedro Oliveira', '321.654.987-32', 'pedro.oliveira@email.com', '(31) 98765-5678', '1995-11-25', 'Rua D, 321, Belo Horizonte, MG', 1, GETUTCDATE()),
    (NEWID(), 'Lucia Costa', '654.987.321-65', 'lucia.costa@email.com', '(85) 99876-6789', '1991-01-30', 'Rua E, 654, Fortaleza, CE', 1, GETUTCDATE()),
    (NEWID(), 'Roberto Alves', '987.321.654-98', 'roberto.alves@email.com', '(47) 98765-7890', '1987-09-12', 'Rua F, 987, Blumenau, SC', 1, GETUTCDATE()),
    (NEWID(), 'Fernanda Dias', '321.987.654-32', 'fernanda.dias@email.com', '(61) 99876-8901', '1993-06-05', 'Rua G, 147, Brasília, DF', 1, GETUTCDATE()),
    (NEWID(), 'Gustavo Rodrigues', '654.321.987-65', 'gustavo.rodrigues@email.com', '(71) 98765-9012', '1989-04-20', 'Rua H, 258, Salvador, BA', 1, GETUTCDATE()),
    (NEWID(), 'Beatriz Lima', '123.987.654-12', 'beatriz.lima@email.com', '(81) 99876-9123', '1994-12-08', 'Rua I, 369, Recife, PE', 1, GETUTCDATE());

-- ===================================
-- Inserir 10 Profissionais de Saúde
-- ===================================
INSERT INTO HealthProfessionals (Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'Dr. Carlos Oliveira', '111.222.333-44', 'carlos@clinicapp.com', '(11) 91234-5678', 'Cardiologia', 'CRM12345', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Ana Costa', '555.666.777-88', 'ana@clinicapp.com', '(11) 92345-6789', 'Dermatologia', 'CRM54321', 1, GETUTCDATE()),
    (NEWID(), 'Dr. Pedro Silva', '999.888.777-66', 'pedro@clinicapp.com', '(11) 93456-7890', 'Pediatria', 'CRM99999', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Juliana Pereira', '222.333.444-55', 'juliana@clinicapp.com', '(21) 91234-5679', 'Oftalmologia', 'CRM23456', 1, GETUTCDATE()),
    (NEWID(), 'Dr. Fernando Gomes', '333.444.555-66', 'fernando@clinicapp.com', '(31) 92345-6780', 'Ortopedia', 'CRM34567', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Mariana Rocha', '444.555.666-77', 'mariana@clinicapp.com', '(85) 93456-7891', 'Ginecologia', 'CRM45678', 1, GETUTCDATE()),
    (NEWID(), 'Dr. Luciano Martins', '666.777.888-99', 'luciano@clinicapp.com', '(47) 91234-5680', 'Neurologia', 'CRM56789', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Patricia Neves', '777.888.999-00', 'patricia@clinicapp.com', '(61) 92345-6781', 'Psiquiatria', 'CRM67890', 1, GETUTCDATE()),
    (NEWID(), 'Dr. Bruno Santos', '888.999.000-11', 'bruno@clinicapp.com', '(71) 93456-7892', 'Clínica Geral', 'CRM78901', 1, GETUTCDATE()),
    (NEWID(), 'Dra. Cristina Almeida', '000.111.222-33', 'cristina@clinicapp.com', '(81) 91234-5681', 'Endocrinologia', 'CRM89012', 1, GETUTCDATE());

-- ===================================
-- Inserir 10 Agendamentos
-- ===================================
-- Nota: Os IDs abaixo precisam corresponder aos IDs gerados das tabelas Patients e HealthProfessionals
-- Você precisará atualizar os UUIDs com os valores reais gerados

-- Exemplo com placeholder - ajuste com IDs reais:
INSERT INTO Appointments (Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt)
VALUES 
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Cardiologia' ORDER BY NEWID()), CAST(GETDATE() AS DATE), '08:00', '08:30', 1, 'Consulta de rotina', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Dermatologia' ORDER BY NEWID()), CAST(GETDATE() + 1 AS DATE), '09:00', '09:30', 1, 'Avaliação dermatológica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Pediatria' ORDER BY NEWID()), CAST(GETDATE() + 2 AS DATE), '10:00', '10:30', 1, 'Consulta pediátrica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Oftalmologia' ORDER BY NEWID()), CAST(GETDATE() + 3 AS DATE), '14:00', '14:30', 1, 'Exame oftalmológico', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Ortopedia' ORDER BY NEWID()), CAST(GETDATE() + 4 AS DATE), '15:00', '15:30', 1, 'Avaliação ortopédica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Ginecologia' ORDER BY NEWID()), CAST(GETDATE() + 5 AS DATE), '11:00', '11:30', 1, 'Consulta ginecológica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Neurologia' ORDER BY NEWID()), CAST(GETDATE() + 6 AS DATE), '16:00', '16:30', 1, 'Avaliação neurológica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Psiquiatria' ORDER BY NEWID()), CAST(GETDATE() + 7 AS DATE), '13:00', '13:30', 1, 'Consulta psiquiátrica', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Clínica Geral' ORDER BY NEWID()), CAST(GETDATE() + 8 AS DATE), '08:30', '09:00', 1, 'Consulta de clínica geral', GETUTCDATE()),
    (NEWID(), (SELECT TOP 1 Id FROM Patients ORDER BY NEWID()), (SELECT TOP 1 Id FROM HealthProfessionals WHERE Specialization = 'Endocrinologia' ORDER BY NEWID()), CAST(GETDATE() + 9 AS DATE), '10:30', '11:00', 1, 'Avaliação endocrinológica', GETUTCDATE());

-- 1. Administradores (Admin123!)
UPDATE Users 
SET PasswordHash = 'VgMOj/nPGlBtcsfy38n84tLIDJYNsGZAvq93HOtZL2TkafYJ' 
WHERE Email IN ('admin@clinicapp.com', 'moacir@clinicapp.com');

-- 2. Médicos (Medico123!)
UPDATE Users 
SET PasswordHash = 'R3nNCl3SqxFMY7PJ57XIEjGuFWB3p+gNpza7R9g6qm1CtacN' 
WHERE Email = 'medico@clinicapp.com';

-- 3. Pacientes (Paciente123!)
UPDATE Users 
SET PasswordHash = '6s/VosR7Ocmuwj8keQS8esKw96crmwNX8RP9BvIIAw4lKVws' 
WHERE Email = 'paciente@clinicapp.com';