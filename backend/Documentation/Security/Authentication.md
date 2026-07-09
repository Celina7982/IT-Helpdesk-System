# Authentication Design

## Purpose

Authentication verifies the identity of users before allowing access to the system.

The application will use JSON Web Tokens (JWT) to authenticate users.

Passwords will never be stored in plain text. Instead, they will be securely hashed using BCrypt before being saved to the database.

---

## Authentication Process

1. User enters email and password.
2. API validates credentials.
3. Password hash is verified.
4. JWT token is generated.
5. Token is returned to the client.
6. Client stores the token securely.
7. Token is included with every API request.
8. API validates the token before processing requests.

---

## Technologies

- ASP.NET Core Identity Components
- JWT Bearer Authentication
- BCrypt Password Hashing

---

## Benefits

- Secure authentication
- Stateless API
- Scalable architecture
- Industry-standard security