# Login Sequence

The following sequence describes the login process.

1. User enters email and password.
2. React application sends a POST request to `/api/auth/login`.
3. ASP.NET Core API validates the credentials.
4. Password hash is verified using BCrypt.
5. JWT token is generated.
6. Token is returned to the client.
7. Client stores the token.
8. Token is sent in the Authorization header with every protected request.
9. API validates the JWT before executing the requested action.