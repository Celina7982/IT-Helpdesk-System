# IT Helpdesk Management System

A full-stack IT Helpdesk Management System for managing support tickets, technician assignments, job cards, service records, notifications, and audit history.

## Overview

The IT Helpdesk Management System is a web application for managing the lifecycle of IT support requests. Clients can submit and track tickets, technicians can manage assigned work and record service activities, and administrators can manage users, tickets, job cards, and system activity.

The application is built with a React frontend, ASP.NET Core Web API backend, and Microsoft SQL Server database. The backend follows a layered architecture using controllers, services, repositories, DTOs, and Entity Framework Core.

---

## Features

### Authentication & Authorization

* JWT-based authentication
* Role-based authorization
* Administrator, Technician, and Client roles
* Protected API endpoints
* BCrypt password hashing

### Ticket Management

* Create support tickets
* View and manage tickets
* Assign technicians
* Track ticket status
* Resolve tickets
* Client-specific ticket views
* Ticket-to-Job Card workflow

### Job Cards

Job Cards provide a service record for resolved tickets.

* Create Job Cards from resolved tickets
* Automatic Job Card numbering
* Technician assignment
* Update Job Card details
* Record labour
* Record parts used
* Complete Job Cards
* View Job Card audit history
* Generate Job Card PDFs
* Administrator Job Card management

### Notifications

* User-specific notifications
* Unread notification count
* Mark notifications as read
* Delete notifications
* Notification timestamps
* Frontend notification polling

### Audit Logging

The system records significant Job Card actions to provide a history of changes and activities.

Audit information can be viewed through the Job Card audit endpoint.

### Dashboard

Dashboard statistics provide an overview of ticket activity, including:

* Open tickets
* In-progress tickets
* Resolved tickets

---

## User Roles

| Role              | Access                                                                                        |
| ----------------- | --------------------------------------------------------------------------------------------- |
| **Administrator** | System management, users, tickets, Job Cards, audit information and administrative operations |
| **Technician**    | Assigned tickets, ticket updates, Job Cards, labour and parts                                 |
| **Client**        | Create tickets and view their submitted tickets and status                                    |

---

## Ticket & Job Card Workflow

```text
Client
  │
  ▼
Create Ticket
  │
  ▼
Ticket Assigned
  │
  ▼
Technician Works on Ticket
  │
  ▼
Ticket Resolved
  │
  ▼
Create Job Card
  │
  ├── Labour
  ├── Parts
  └── Service Details
  │
  ▼
Complete Job Card
  │
  ├── Audit History
  └── PDF
```

A Job Card can only be created from a ticket that has reached the required resolved state.

---

## Technology Stack

| Layer            | Technology            |
| ---------------- | --------------------- |
| Frontend         | React, Vite           |
| UI               | Bootstrap 5           |
| HTTP Client      | Axios                 |
| Routing          | React Router          |
| Backend          | ASP.NET Core Web API  |
| Language         | C#                    |
| ORM              | Entity Framework Core |
| Database         | Microsoft SQL Server  |
| Authentication   | JWT                   |
| Password Hashing | BCrypt                |
| API Testing      | Postman               |
| Version Control  | Git / GitHub          |

---

## Architecture

The backend uses a layered architecture to separate API handling, business logic, and data access.

```text
React Frontend
      │
      │ REST API
      ▼
ASP.NET Core Controllers
      │
      ▼
Service Layer
      │
      ▼
Repository Layer
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
```

### Backend Structure

```text
backend/
└── IThelpdesk/
    ├── Controllers/
    ├── DTOs/
    ├── Enums/
    ├── Interfaces/
    │   ├── Repositories/
    │   └── Services/
    ├── Models/
    ├── Repositories/
    ├── Services/
    └── ...
```

**Controllers** handle HTTP requests and responses.

**Services** contain application and business logic.

**Repositories** handle database operations.

**DTOs** define the data exchanged between the API and frontend.

**Models** represent the application's domain entities.

**Enums** provide controlled values for roles, statuses, and audit actions.

---

## Project Structure

```text
IT-Helpdesk-System/
│
├── backend/
│   └── IThelpdesk/
│
├── frontend/
│
├── database/
│
├── docs/
│
└── README.md
```

---

# Installation

## Prerequisites

The following are required to run the project locally:

* .NET SDK
* Node.js
* npm
* Microsoft SQL Server
* Git

---

## Clone the Repository

```bash
git clone https://github.com/Celina7982/IT-Helpdesk-System.git
cd IT-Helpdesk-System
```

---

## Backend Setup

Navigate to the backend project:

```bash
cd backend/IThelpdesk
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

The API will start using the HTTP/HTTPS ports configured by the project.

---

## Database Setup

The backend uses SQL Server with Entity Framework Core.

Configure the database connection in the application's configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ITHelpdesk;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER` with the SQL Server instance used on the development machine.

If migrations are included in the project, apply them using:

```bash
dotnet ef database update
```

---

## Frontend Setup

Open a new terminal and navigate to the frontend:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

Vite will display the local development URL in the terminal.

---

# Configuration

## API URL

The frontend communicates with the backend through Axios.

The API client is configured with the backend API URL, for example:

```javascript
baseURL: "http://localhost:5057/api"
```

Update the URL if the backend is running on a different port or environment.

## JWT Authentication

After a successful login, the JWT is stored by the frontend and included in authenticated API requests.

Requests use the following format:

```http
Authorization: Bearer <JWT_TOKEN>
```

The backend uses JWT claims to identify the authenticated user and determine their role.

---

# API Overview

The main API areas are:

| Module         | Endpoint                             | Description                           |
| -------------- | ------------------------------------ | ------------------------------------- |
| Authentication | `/api/Auth/login`                    | User authentication                   |
| Tickets        | `/api/Ticket`                        | Ticket management                     |
| Client Tickets | `/api/Ticket/mytickets`              | Retrieve the current client's tickets |
| Job Cards      | `/api/JobCard`                       | Job Card management                   |
| Labour         | `/api/JobCard/{id}/labour`           | Manage labour entries                 |
| Parts          | `/api/JobCard/{id}/parts`            | Manage Job Card parts                 |
| Job Card PDF   | `/api/JobCard/{id}/pdf`              | Generate Job Card PDF                 |
| Notifications  | `/api/Notifications`                 | Retrieve user notifications           |
| Mark as Read   | `/api/Notifications/markasread/{id}` | Update notification status            |
| Dashboard      | `/api/Dashboard/stats`               | Retrieve dashboard statistics         |
| Audit          | `/api/JobCard/{id}/audit`            | Retrieve Job Card audit history       |

---

# Example API Request

An authenticated request to retrieve notifications:

```http
GET /api/notifications
Authorization: Bearer <JWT_TOKEN>
```

The API uses the authenticated user's claims to retrieve notifications belonging to that user.

---

# Frontend API Integration

The frontend uses Axios service modules to communicate with the backend.

The API client is responsible for:

* Base API configuration
* JWT token handling
* Authorization headers
* HTTP requests
* API error handling

Feature-specific services handle operations for areas such as tickets, dashboards, and notifications.

---

# Security

The application implements:

* JWT authentication
* Role-based authorization
* BCrypt password hashing
* Protected API endpoints
* User identity from JWT claims
* Server-side authorization checks

Production credentials, connection strings, JWT secrets, and other sensitive configuration values should not be committed to the repository.

---

# Current Status

### Completed

* JWT authentication
* Role-based authorization
* Administrator, Technician and Client roles
* Ticket creation and management
* Technician assignment
* Ticket status workflow
* Client ticket dashboard
* Job Card creation from resolved tickets
* Job Card management
* Labour entries
* Parts management
* Job Card completion
* Job Card PDF generation
* Audit logging
* Notifications
* Mark-as-read functionality
* Notification deletion
* Dashboard statistics
* Frontend/API integration
* SQL Server integration

### Future Development

* Advanced reporting
* Quotations
* Invoicing
* Expanded dashboard analytics
* Email notifications
* Additional service management functionality

###  Author
Celina Chetty
GitHub:
https://github.com/Celina7982
Repository:
https://github.com/Celina7982/IT-Helpdesk-System

