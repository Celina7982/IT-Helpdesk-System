# Database Design

## Lookup Tables

- Roles
- Categories
- Priorities
- Statuses

## Core Tables

- Users
- Tickets
- TicketAssignments
- TicketUpdates
- Attachments
- Notifications
- JobCards
- Invoices
- AuditLogs

## Relationships

One Role
→ Many Users

One User
→ Many Tickets

One Ticket
→ Many Ticket Updates

One Ticket
→ Many Assignments

One Ticket
→ Many Attachments

One Ticket
→ One Job Card

One Job Card
→ One Invoice