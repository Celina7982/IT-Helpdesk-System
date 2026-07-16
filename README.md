# IT Helpdesk System

## Overview

The IT Helpdesk System is a web-based application developed to help organisations manage IT support requests efficiently.

Clients can submit support tickets, while technicians can view, update and resolve assigned tickets through a dedicated dashboard.

---

## Features

### Client

- Login
- Dashboard
- Create Ticket
- View My Tickets
- View Ticket Details

### Technician

- Login
- Dashboard
- View Assigned Tickets
- Update Ticket Status

### Backend

- ASP.NET Core Web API
- Repository Pattern
- Service Layer
- Entity Framework Core

---

## Technologies

### Frontend

- React
- React Router
- Bootstrap
- Axios
- Vite

### Backend

- ASP.NET Core
- Entity Framework Core
- SQL Server
- C#

---

## Project Structure

```
IT-Helpdesk-System
│
├── backend
│   ├── Controllers
│   ├── Data
│   ├── Interfaces
│   ├── Models
│   ├── Repositories
│   ├── Services
│   └── Program.cs
│
└── frontend
    ├── src
    │   ├── components
    │   ├── pages
    │   └── assets
    └── public
```

---

## Current Progress

✅ React frontend completed

✅ Client Dashboard

✅ Technician Dashboard

✅ Ticket pages

✅ Backend Clean Architecture

✅ Repository Pattern

✅ Service Layer

✅ Controllers

⏳ SQL Server database integration in progress

---

## Running the Project

### Backend

```bash
cd backend
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

---

## Team Workflow

- Feature branches are used for development.
- Changes are submitted through Pull Requests to the `develop` branch.
- The `main` branch contains stable releases.

---

## Contributors

- Celina Chetty
- Chaterin

---

## Future Work

- SQL Server integration
- Entity Framework migrations
- JWT Authentication
- Dashboard statistics
- Notifications
- Reporting
- Deployment
