# ClinicApp - Sistema de Gestão de Clínicas Médicas

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![React 18](https://img.shields.io/badge/React-18.2-61DAFB)](https://react.dev/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED)](https://www.docker.com/)
[![CI/CD](https://github.com/Moacir/ClinicApp/actions/workflows/clinicapp-ci.yml/badge.svg)](https://github.com/Moacir/ClinicApp/actions)

## 📋 Visão Geral

O **ClinicApp** é uma plataforma robusta e moderna para o gerenciamento de clínicas médicas, focada em simplificar o fluxo de agendamentos entre pacientes e profissionais de saúde. Desenvolvido com uma arquitetura de alta performance e escalabilidade, o projeto demonstra a aplicação de padrões avançados de engenharia de software tanto no Backend quanto no Frontend.

## 🚀 Funcionalidades Principais

- **Autenticação Avançada**:
  - Login seguro com JWT (JSON Web Tokens).
  - **Refresh Token**: Sistema de renovação de sessão automático e persistente.
  - Controle de Acesso Baseado em Perfil (RBAC): Admin, Médico, Paciente e Agente.
- **Gestão de Agendamentos**:
  - Validação inteligente de horários (Seg-Sex, 08h-18h).
  - Prevenção de conflitos de agenda em tempo real.
  - Regra de negócio: limite de uma consulta por dia por paciente/profissional.
- **Painéis Especializados**:
  - **Administrador**: Visão global de todos os agendamentos da clínica e gestão de usuários.
  - **Médico**: Agenda personalizada com lista de pacientes e horários.
  - **Paciente**: Histórico de consultas e agendamento simplificado.
- **Interface Moderna**:
  - Design elegante com **Glassmorphism**.
  - Componentes de UI customizados (Modais, Toasts, Badges de Status).
  - Feedback visual em tempo real para ações do usuário.

## 🏗️ Arquitetura e Decisões Técnicas

O projeto foi construído seguindo padrões de engenharia de software para garantir manutenibilidade e escalabilidade.

### Backend (`/backend`)
Baseado em **Domain-Driven Design (DDD)** e **Clean Architecture**:
- **ASP.NET Core 8**: Escolhido por ser o framework mais moderno e performático do ecossistema .NET.
- **Dapper (Micro-ORM)**: Decidimos pelo Dapper em vez do Entity Framework para obter o máximo de performance em queries SQL e total controle sobre o esquema do banco de dados, o que é crucial em sistemas de alta concorrência como agendamentos.
- **Result Pattern**: Implementado para evitar o uso excessivo de exceções para controle de fluxo, tornando o código mais limpo e previsível.
- **SOLID & Clean Code**: Interfaces bem definidas para desacoplamento (ex: `IUserRepository`, `IJwtTokenService`) facilitando a troca de implementações e a criação de testes.
- **Testes**: Foco em testes de domínio e serviços (`xUnit`, `Moq`) para garantir a integridade das regras de agendamento.

### Frontend (`/frontend`)
Organizado por **Features/Domínios**:
- **React 18 + Vite**: Proporciona um ambiente de desenvolvimento extremamente rápido e uma SPA performática para o usuário final.
- **TypeScript**: Adotado para trazer segurança de tipos em toda a aplicação, reduzindo drasticamente erros de runtime.
- **Arquitetura de Features**: Cada funcionalidade (Appointments, Auth, Patients) tem seu próprio diretório com componentes, serviços e tipos, evitando o "código espaguete" e facilitando o trabalho em equipe.
- **Intercepção de API**: Uso de interceptores Axios para gerenciar automaticamente a inclusão do Token JWT e a lógica de **Refresh Token** de forma transparente.

### Infraestrutura e DevOps
- **Docker**: Todo o ambiente (App + DB) é containerizado, garantindo que o projeto rode exatamente da mesma forma em qualquer máquina ("Write once, run anywhere").
- **GitHub Actions**: Pipeline automatizado que valida cada Pull Request, garantindo que código que não compila ou que quebra testes nunca chegue à branch principal.

## 🛠️ Instruções de Execução Local

### Opção 1: Via Docker (Recomendado)
Esta opção sobe todo o ecossistema (Frontend, Backend e SQL Server) automaticamente.
1. Certifique-se de ter o **Docker Desktop** instalado.
2. Na raiz do projeto, execute:
   ```bash
   docker-compose up --build
   ```
3. Acesse o sistema em: [http://localhost:3000](http://localhost:3000)
4. A documentação da API (Swagger) estará em: [http://localhost:5002/swagger](http://localhost:5002/swagger)

### Opção 2: Execução Nativa (Desenvolvimento)
Para rodar os serviços separadamente:
1. **Banco de Dados**: 
   - Tenha uma instância SQL Server ativa.
   - Execute o script de inicialização: [init.sql](file:///c:/Users/Moacir/Documents/Projetos/ClinicApp/backend/database/init.sql)
2. **Backend**:
   - Ajuste a string de conexão no `appsettings.json`.
   ```bash
   cd backend
   dotnet run --project src/ClinicApp.Api/ClinicApp.Api.csproj
   ```
3. **Frontend**:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

## 🔐 Usuários de Teste (Credenciais)

Para facilitar a avaliação, o banco de dados já vem populado com os seguintes perfis:

| Perfil | Usuário/E-mail | Senha | Acesso |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin@clinicapp.com` | `Admin123!` | Visão total + Gestão de Usuários |
| **Médico** | `medico@clinicapp.com` | `Medico123!` | Agenda do Profissional |
| **Paciente** | `paciente@clinicapp.com` | `Paciente123!` | Agendamento e Histórico |
| **Agente** | `agente@clinicapp.com` | `Agente123!` | Gestão de Agendamentos |

---
*Este projeto faz parte de uma avaliação técnica para Desenvolvedor Pleno/Sênior.*
Desenvolvido por **Moacir Souza**.
