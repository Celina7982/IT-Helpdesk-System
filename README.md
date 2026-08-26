# IT Helpdesk Management System

A full-stack web-based IT Helpdesk Management System built using **ASP.NET Core Web API, React, and Microsoft SQL Server**.

The system provides a structured workflow for managing IT support requests from ticket creation and assignment through technician resolution, Job Card processing, service documentation, audit history, and PDF generation.

> **Project Status:** Active Development  
> The core Ticket and Job Card workflows have been implemented and are currently undergoing further testing, refinement, and production-readiness improvements.

---

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [System Architecture](#system-architecture)
- [Core Features](#core-features)
  - [Authentication and Authorization](#authentication-and-authorization)
  - [Ticket Management](#ticket-management)
  - [Ticket Comments](#ticket-comments)
  - [Job Cards](#job-cards)
  - [Labour Management](#labour-management)
  - [Parts Management](#parts-management)
  - [Job Card Audit History](#job-card-audit-history)
  - [PDF Generation](#pdf-generation)
  - [Dashboard](#dashboard)
  - [User Management](#user-management)
- [System Workflows](#system-workflows)
- [User Roles and Permissions](#user-roles-and-permissions)
- [Business Rules](#business-rules)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Database](#database)
- [Current Project Status](#current-project-status)
- [Known Issues](#known-issues)
- [Future Improvements](#future-improvements)
- [Security Notes](#security-notes)
- [Repository](#repository)

---

# Overview

The IT Helpdesk Management System is designed to centralise and streamline IT support operations within an organisation.

The system provides different interfaces and permissions for:

- **Administrators**
- **Technicians**
- **Clients**

The system manages the support lifecycle from the initial creation of a Ticket through assignment, investigation, escalation or resolution, followed by Job Card processing where required.

The primary service lifecycle is:

```text
Client Creates Ticket
        ↓
Ticket Open
        ↓
Assignment / Technician Claim
        ↓
Technician Investigation
        ↓
Resolve OR Escalate
        ↓
Ticket Resolved
        ↓
Create Job Card
        ↓
Technical Service Work
        ↓
Fault Found / Work Performed
        ↓
Labour / Parts
        ↓
Customer Signature
        ↓
Job Card Completed
        ↓
Audit History
        ↓
PDF Service Documentation




---

# SECTION 3 — System Architecture

```markdown
# System Architecture

The system follows a layered architecture that separates the React frontend, ASP.NET Core API, business logic, data access, and database.

```text
┌──────────────────────────────┐
│        React Frontend        │
│         React + Vite         │
│                              │
│ Pages / Components / Routes  │
└──────────────┬───────────────┘
               │
               │ HTTP / JSON
               │ JWT
               ▼
┌──────────────────────────────┐
│      ASP.NET Core API        │
│         Controllers          │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│        Service Layer         │
│     Business Logic / DTOs    │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│       Repository Layer       │
│     Entity Framework Core    │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│        SQL Server            │
└──────────────────────────────┘


---

# SECTION 3 — System Architecture

```markdown
# System Architecture

The system follows a layered architecture that separates the React frontend, ASP.NET Core API, business logic, data access, and database.

```text
┌──────────────────────────────┐
│        React Frontend        │
│         React + Vite         │
│                              │
│ Pages / Components / Routes  │
└──────────────┬───────────────┘
               │
               │ HTTP / JSON
               │ JWT
               ▼
┌──────────────────────────────┐
│      ASP.NET Core API        │
│         Controllers          │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│        Service Layer         │
│     Business Logic / DTOs    │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│       Repository Layer       │
│     Entity Framework Core    │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│        SQL Server            │
└──────────────────────────────┘





---

# SECTION 5 — Ticket Comments & Job Cards

```markdown
# Ticket Comments

The system provides a dedicated ticket-comment module for communication associated with support requests.

The backend contains a dedicated:

```text
TicketCommentsController



---

# SECTION 6 — Labour, Parts, Audit & PDF

```markdown
# Labour Management

Technicians can record labour performed against a Job Card.

Labour records can include:

- Technician
- Hours Worked
- Work Performed
- Date Worked

Labour actions are also recorded in the Job Card audit history.

---

# Parts Management

The system allows parts used during service work to be recorded against a Job Card.

Parts contain:

- Part Name
- Quantity

The system validates that:

- Part Name is provided.
- Quantity is greater than zero.

Parts can be added and deleted according to the user's permissions.

Part changes are recorded in the Job Card audit history.

---

# Job Card Audit History

Job Card activities are tracked through a dedicated audit system.

The audit functionality records important Job Card lifecycle events and changes.

Tracked activities include:

- Job Card Created
- Job Card Opened
- Job Card Completed
- Status Changed
- Fault Found Updated
- Work Performed Updated
- Completion Notes Updated
- Labour Added
- Labour Updated
- Labour Deleted
- Customer Signature Added
- Part Added
- Part Updated
- Part Deleted
- PDF Generated
- Job Card Deleted

Audit records can contain:

- Job Card ID
- User ID
- Action
- Description
- Previous Value
- New Value
- Date Created

This provides traceability of important changes made to service records.

---

# PDF Generation

Job Cards can be exported as PDF service documents using **QuestPDF**.

The backend contains a dedicated:

```text
JobCardPdfService



---

# SECTION 7 — Dashboard & User Management

```markdown
# Dashboard

The system includes dashboard functionality for providing ticket-related statistics and recent ticket information.

Separate dashboard functionality is available for:

- Administrator
- Technician
- Client

Dashboards provide role-appropriate access to relevant system information.

---

# User Management

User management is available to Administrators.

Implemented functionality includes:

- View all users.
- View individual users.
- Create users.
- Update users.
- Delete users.
- Reset user passwords.
- Retrieve technicians for ticket assignment.
- Manage user roles.
- Manage active/inactive user state.

---


# System Workflows

## Ticket Workflow

```text
1. Client logs into the system
        ↓
2. Client creates a Ticket
        ↓
3. Ticket is created with Open status
        ↓
4. Ticket becomes available to support staff
        ↓
5. Ticket is assigned OR claimed
        ↓
6. Technician investigates the issue
        ↓
7. Issue resolved?
        │
        ├── YES
        │     ↓
        │   Ticket marked Resolved
        │     ↓
        │   Ticket becomes eligible for Job Card creation
        │
        └── NO
              ↓
          Technician escalates Ticket
              ↓
          Administrator reviews escalated Ticket
              ↓
          Further assignment / resolution

          1. Ticket reaches Resolved status
        ↓
2. Administrator or Technician creates Job Card
        ↓
3. System checks whether a Job Card already exists
        ↓
4. Unique Job Number generated
        ↓
5. Job Card created with Open status
        ↓
6. Technician records service information
        ↓
   Fault Found
   Work Performed
   Completion Notes
   Labour
   Parts
        ↓
7. Customer signature captured
        ↓
8. Job Card completed
        ↓
9. Completion date recorded
        ↓
10. Audit history maintained
        ↓
11. Job Card PDF can be generated

┌──────────────┐
│    Ticket    │
│              │
│   Resolved   │
└──────┬───────┘
       │
       │ Create Job Card
       ▼
┌──────────────┐
│  Job Card    │
│              │
│    Open      │
└──────┬───────┘
       │
       ├───────────────┐
       │               │
       ▼               ▼
   Labour           Parts
       │               │
       └───────┬───────┘
               ▼
        Service Details
               │
               ▼
       Customer Signature
               │
               ▼
       Job Card Completed
               │
               ▼
         Audit History
               │
               ▼
          PDF Export


          
---

# SECTION 9 — Roles & Permissions

```markdown
# User Roles and Permissions

## Administrator

Administrators have the highest level of system access.

They can:

- Manage users.
- Create users.
- Update users.
- Delete users.
- Reset passwords.
- Manage user roles.
- Manage active/inactive user state.
- View all tickets.
- Assign tickets.
- Claim tickets.
- Update tickets.
- Resolve tickets.
- Escalate tickets.
- View escalated tickets.
- Manage Job Cards.
- Create Job Cards.
- Update Job Cards.
- Add labour.
- Add parts.
- Delete parts.
- Complete Job Cards.
- Delete Job Cards.
- View Job Card audit history.
- Generate Job Card PDFs.

---

## Technician

Technicians are responsible for investigating and resolving technical support requests.

They can:

- View available tickets.
- View their assigned tickets.
- Claim tickets.
- Investigate tickets.
- Update tickets.
- Escalate tickets.
- Resolve tickets.
- Create Job Cards from resolved tickets.
- View Job Cards.
- Search and filter Job Cards.
- Update Job Cards.
- Add labour.
- Add parts.
- Delete parts where permitted.
- Complete Job Cards.
- View Job Card audit history.
- Generate Job Card PDFs.

---

## Client

Clients primarily interact with the Ticket module.

They can:

- Log in.
- Create support tickets.
- View their own tickets.
- View ticket details.
- Follow ticket status.
- Participate in ticket communication through comments where permitted.

Clients do not have Administrator or Technician-level Job Card management permissions.

---

# Business Rules

The following business rules are enforced by the application:

- **A Job Card can only be created when the related Ticket has `Resolved` status.**

- **A Ticket cannot have multiple Job Cards.**

- **If a Job Card already exists for a Ticket, the existing Job Card is returned instead of creating another one.**

- **New Job Cards are created with `Open` status.**

- **Job Card numbers are generated using the `JC-YYYY-######` format.**

- **Only Administrators and Technicians can access Job Card management endpoints.**

- **Only Administrators can delete Job Cards.**

- **Only Administrators can assign Tickets.**

- **Technicians and Administrators can claim Tickets.**

- **Technicians and Administrators can escalate Tickets.**

- **Technicians and Administrators can resolve Tickets.**

- **Job Card audit history is restricted to Administrators and Technicians.**

- **Completed Job Cards are retained as service records.**

- **Ticket and Job Card operations are protected by role-based authorization.**

---


# Project Structure

```text
IT-Helpdesk-System/
│
├── backend/
│   ├── Assets/
│   │   └── Lbc-Logo.png
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── DashboardController.cs
│   │   ├── JobCardController.cs
│   │   ├── TicketCommentsController.cs
│   │   ├── TicketController.cs
│   │   └── UserController.cs
│   │
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   │
│   ├── DTOs/
│   │
│   ├── Enums/
│   │
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   │
│   ├── Migrations/
│   │
│   ├── Models/
│   │   ├── JobCard.cs
│   │   ├── JobCardAudit.cs
│   │   ├── JobCardLabour.cs
│   │   ├── JobCardPart.cs
│   │   ├── Ticket.cs
│   │   ├── TicketComment.cs
│   │   └── User.cs
│   │
│   ├── Repositories/
│   │
│   ├── Services/
│   │   └── Pdf/
│   │       └── JobCardPdfService.cs
│   │
│   ├── Documentation/
│   │
│   ├── Program.cs
│   └── IThelpdesk.csproj
│
├── frontend/
│   ├── src/
│   │   ├── assets/
│   │   ├── components/
│   │   ├── layouts/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── App.jsx
│   │   ├── App.css
│   │   ├── index.css
│   │   └── main.jsx
│   │
│   ├── package.json
│   └── vite.config.js
│
├── database/
├── docs/
└── README.md



---

# SECTION 12 — Getting Started

```markdown
# Getting Started

## Prerequisites

Install the following software before running the project:

- **.NET 10 SDK**
- **Node.js**
- **npm**
- **Microsoft SQL Server**
- **Git**

---

## 1. Clone the Repository

```bash
git clone https://github.com/Celina7982/IT-Helpdesk-System.git
cd IT-Helpdesk-System




---

# SECTION 13 — API Documentation

```markdown
# API Documentation

The backend exposes REST API endpoints through ASP.NET Core controllers.

Major controllers include:

```text
AuthController
DashboardController
TicketController
TicketCommentsController
JobCardController
UserController


---

# SECTION 14 — Database

```markdown
# Database

The system uses **Microsoft SQL Server** with **Entity Framework Core**.

The application's database context is:

```text
ApplicationDbContext


Ticket
   │
   │ 1 : 0..1
   ▼
Job Card
   │
   ├───────────────┐
   │               │
   ▼               ▼
Labour           Parts
   │
   │
   └───────────────┐
                   ▼
             Audit History


             
---

# SECTION 15 — Current Project Status

```markdown
# Current Project Status

The project is currently in **Active Development**.

The major application modules have been implemented and integrated.

## Implemented

### Authentication

- JWT authentication
- Login
- Role-based authorization
- Password hashing

### Users

- User CRUD
- Password reset
- Role management
- Active/inactive state
- Technician lookup

### Tickets

- Ticket creation
- Ticket retrieval
- Ticket assignment
- Technician claiming
- Ticket escalation
- Ticket resolution
- Ticket updates
- Ticket deletion
- Client ticket views
- Technician ticket views
- Escalated ticket view
- Ticket comments

### Job Cards

- Job Card creation from resolved Tickets
- Duplicate Job Card prevention
- Job Number generation
- Job Card listing
- Search
- Filtering
- Sorting
- Pagination
- Job Card details
- Job Card updates
- Labour entries
- Parts
- Customer signature information
- Job Card completion
- Job Card deletion
- Audit history
- PDF generation

### Frontend

- Login
- Role-based routing
- Administrator dashboard
- Technician dashboard
- Client dashboard
- Administrator layout
- Technician layout
- Client layout
- Ticket pages
- Ticket details
- Job Card list
- Job Card details
- User management
- API service integration

---

# Development Progress

The project has progressed from a basic ticketing application into a more complete IT service-management workflow.

The current lifecycle is:

```text
Ticket
  ↓
Assignment
  ↓
Technician Investigation
  ↓
Resolution / Escalation
  ↓
Resolved Ticket
  ↓
Job Card
  ↓
Labour / Parts / Service Details
  ↓
Customer Sign-off
  ↓
Completion
  ↓
Audit
  ↓
PDF



---

# SECTION 16 — Known Issues

```markdown
# Known Issues

The project remains under active development. The following areas require continued testing and refinement.

## Ticket Workflow

- Further end-to-end testing of ticket status transitions.
- Continued testing of escalated ticket behaviour.
- Verification of ticket list/dashboard refresh behaviour after status changes.
- Further validation of assignment and claiming edge cases.

## Job Cards

- Continued testing of the complete Ticket → Job Card lifecycle.
- Refinement of completed Job Card editing/read-only behaviour.
- Further validation of duplicate Job Card handling.
- Continued testing of Job Card completion.
- Verification of audit coverage across all Job Card lifecycle operations.

## PDF Generation

- Final refinement of Job Card PDF layout.
- Continued testing of PDF table rendering.
- Final verification of branding and company logo placement.

## Frontend

- Continued refinement of frontend error handling.
- UI consistency improvements.
- Additional validation of role-based navigation.
- Improved user feedback for API errors.

## Production Readiness

- Automated testing.
- Security hardening.
- Production configuration.
- Deployment configuration.
- Environment-specific configuration.
- Database deployment strategy.

These items are part of ongoing development and do not represent a lack of the core system architecture.

## Future Improvements / Scheduled Work

The following items replace the previous scheduled-for-later list. They are arranged according to priority so that the most important operational and workflow improvements can be addressed first.

### 🔴 Urgent Priority

#### 1. Improve the Ticket Lifecycle

- Revisit and refine the complete Ticket lifecycle.
- Improve transitions between ticket statuses.
- Ensure ticket behaviour is consistent when tickets are resolved, reopened, escalated, or otherwise changed.
- Ensure the Ticket workflow integrates cleanly with the Job Card workflow.

#### 2. Complete Administrator Permissions

- Review Administrator permissions across the system.
- Ensure Administrators have the required access to tickets, Job Cards, users, statuses, priorities, and system records.
- Ensure administrative actions are correctly protected by role-based authorization.

#### 3. Complete Audit Logging

- Expand audit logging across the system.
- Ensure important Ticket actions are recorded.
- Ensure important Job Card actions are recorded.
- Record user activity where appropriate.
- Provide a reliable history of who performed important actions and when.

#### 4. Ticket Audit History

- Add a dedicated Ticket audit/history view.
- Track important ticket changes such as status, assignment, priority, escalation, resolution, reopening, and other significant actions.

#### 5. User Audit History

- Add a user activity/audit history capability.
- Record significant user actions for accountability and troubleshooting.

#### 6. Priority Levels

- Standardize Ticket priority levels.
- Implement the agreed priority values:
  - **Urgent**
  - **Mid**
  - **Low**
- Ensure priority is consistently displayed, stored, filtered, and used throughout the Ticket workflow.

---

### 🟡 Medium Priority

#### 7. Internal / External Comments

- Introduce a distinction between internal technician/admin comments and external/client-visible comments.
- Ensure internal comments are not exposed to clients.
- Ensure external comments remain visible to the appropriate users.

#### 8. Job Card Email Notifications

- Add email functionality for Job Cards.
- Allow relevant Job Card information to be sent to appropriate recipients.
- Define notification triggers and recipients as part of the workflow.

#### 9. Job Card PDF Printing Enhancement

- Allow a completed Job Card PDF to be printed or generated at any time after the Job Card reaches **Completed** status.
- Ensure the generated document contains the final Job Card information and supporting details.

#### 10. Administrator Job Card Editing

- Allow Administrators to edit Job Cards according to the defined business rules.
- Support controlled editing when a Job Card is **Open**, **In Progress**, or **Completed**, where appropriate.
- Ensure administrative changes are captured by audit logging.

#### 11. Reporting

Develop reporting functionality covering areas such as:

- Technician work reports
- Job Card reports
- Ticket reports
- Ticket and Job Card KPIs
- Technician workload
- Resolution and completion statistics
- Operational performance information

#### 12. Notifications

- Introduce system notifications for important workflow events.
- Notify relevant users when tickets are assigned, escalated, resolved, reopened, or otherwise require attention.
- Extend notifications to relevant Job Card events where appropriate.

#### 13. Technician Workload Dashboard

- Develop a dedicated technician workload dashboard.
- Display workload, assigned tickets, active Job Cards, completed work, and other useful workload indicators.

---

### 🟢 Low Priority

#### 14. Invoice Conversion / Pastel Integration

- Investigate conversion of completed Job Card information into invoice-related data.
- Plan future integration with **Pastel**.
- Define the required data mapping between Job Cards, invoices, customers, labour, parts, and accounting records.

#### 15. Archive Job Cards

- Introduce Job Card archiving functionality.
- Align Job Card archiving with the existing Ticket archive process where appropriate.
- Ensure archived Job Cards remain available for historical and audit purposes without appearing in active operational lists.

#### 16. Documentation Improvements

- Continue improving technical documentation.
- Document important workflows, business rules, API endpoints, database relationships, and development procedures.
- Maintain documentation as new features are introduced.

# Security Notes

The application uses authentication and role-based authorization to restrict access to protected functionality.

However, the project is currently under active development and should not be considered production-ready until the required security review and deployment hardening have been completed.

## Do Not Commit Secrets

Never commit the following to the repository:

- Database passwords
- JWT signing keys
- API secrets
- Production credentials
- Private connection strings
- Environment-specific secrets

Use environment variables, user secrets, or an appropriate secrets-management solution for sensitive configuration.

---

# Development Architecture

The project follows separation of responsibilities:

```text
Controllers
     ↓
Services
     ↓
Repositories
     ↓
Entity Framework Core
     ↓
SQL Server

