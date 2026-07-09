# Password Policy

To improve security, passwords must comply with the following rules:

- Minimum length: 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

Passwords are never stored in plain text.

Passwords are hashed using BCrypt before being saved to the database.