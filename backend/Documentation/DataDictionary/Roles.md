# Roles Table

## Purpose

Stores the roles available in the system.

---

## Columns

| Column | Type | Description |
|---------|------|-------------|
| RoleId | int | Primary Key |
| Name | nvarchar(50) | Name of the role |
| Description | nvarchar(250) | Description of the role |
| IsActive | bit | Indicates whether the role is active |
| CreatedDate | datetime2 | Date the role was created |

---

## Relationships

One Role can be assigned to many Users.