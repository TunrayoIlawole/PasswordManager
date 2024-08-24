## Password Manager API

A .Net Core Web API for securely managing user passwords. The API supports user registration, authentication and CRUD operations for managing password securely.

### Features
- User registration and authentication
- JWT-based authentication
- Password storage with AES encryption
- Crud operations for password management
- Unit testing
- Swagger documentation

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Visual Studio / Visual Studio Code

### Getting Started
#### Installation
1. Clone the repository
```bash
git clone https://github.com/TunrayoIlawole/Passwrd-Manager
cd Passwrd-Manager
```
2. Install dependencies
```bash
dotnet restore
```
3. Configure environment variables
- Create a .env file in the root folder
- Add the following variables:
  ```
  CONNECTION_STRING=YourDatabaseConnectionString
  ENCRYPTION_KEY="YourEncryptionKey"
  JWT_SECRET_KEY=YourJWTSecretKey
  JWT_ISSUER=YourIssuer
  JWT_AUDIENCE=YourAudience
  ```

#### Database Setup
1. Run the migrations to set up the database
```
dotnet ef database update
```
#### Running the application
1. Start the application
```
cd PasswordManager
dotnet run
```
2. Access the Swagger UI at `http://localhost:{port}/swagger`

#### Run tests
1. Run unit tests
```
cd PasswordManager.Tests
dotnet test
```
