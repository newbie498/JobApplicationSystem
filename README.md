# Job Application System

A modern web API for managing job postings, applications, and candidate profiles. Built with .NET 7 following Clean Architecture principles.

## Features

- **Authentication & Authorization**
  - Company and Candidate registration
  - JWT-based authentication
  - Role-based access control

- **Companies**
  - Company profile management
  - Job posting creation and management
  - Application review and status updates

- **Candidates**
  - Profile management
  - Resume/CV upload
  - Job application submission
  - Application tracking

- **Job Posts**
  - Detailed job descriptions
  - Salary range specification
  - Location and job type
  - Application status tracking

## Technology Stack

- **.NET 7**
- **Entity Framework Core**
- **AutoMapper**
- **FluentValidation**
- **JWT Authentication**
- **SQL Server** (can be configured to use other databases)

## Project Structure

The solution follows Clean Architecture principles with the following layers:

- **Core** (`JobApplicationSystem.Core`)
  - Entities
  - Interfaces
  - DTOs
  - Domain logic

- **Application** (`JobApplicationSystem.Application`)
  - Services
  - Validation
  - Mapping profiles

- **Infrastructure** (`JobApplicationSystem.Infrastructure`)
  - Database context
  - Repositories
  - External service implementations

- **API** (`JobApplicationSystem.Api`)
  - Controllers
  - Middleware
  - Configuration

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/sql-server) (or another compatible database)
- [Visual Studio](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/) (optional)

## Getting Started

1. **Clone the Repository**
   ```powershell
   git clone https://github.com/newbie498/JobApplicationSystem.git
   cd JobApplicationSystem
   ```

2. **Set Up the Database**
   - Update the connection string in `JobApplicationSystem.Api/appsettings.json`
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JobApplicationSystem;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Install Dependencies**
   ```powershell
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```powershell
   cd JobApplicationSystem.Api
   dotnet ef database update
   ```

5. **Run the Application**
   ```powershell
   dotnet run
   ```

   The API will be available at `https://localhost:7176` and `http://localhost:5172`

## Running Tests

```powershell
cd JobApplicationSystem.Tests
dotnet test
```

## API Documentation

Once the application is running, you can access the Swagger documentation at:
- `https://localhost:7176`
- `http://localhost:5172`

## Main API Endpoints

### Authentication
- POST `/api/auth/register/company` - Register a new company
- POST `/api/auth/register/candidate` - Register a new candidate
- POST `/api/auth/login` - Login for both companies and candidates

### Companies
- GET `/api/companies/{id}` - Get company profile
- PUT `/api/companies/{id}` - Update company profile
- DELETE `/api/companies/{id}` - Delete company

### Candidates
- GET `/api/candidates/{id}` - Get candidate profile
- PUT `/api/candidates/{id}` - Update candidate profile
- DELETE `/api/candidates/{id}` - Delete candidate

### Job Posts
- GET `/api/jobs` - List all job posts
- GET `/api/jobs/{id}` - Get job post details
- POST `/api/jobs` - Create new job post
- PUT `/api/jobs/{id}` - Update job post
- DELETE `/api/jobs/{id}` - Delete job post

### Job Applications
- POST `/api/applications` - Submit job application
- GET `/api/applications/{id}` - Get application details
- PUT `/api/applications/{id}/status` - Update application status
- GET `/api/jobs/{id}/applications` - Get applications for a job
- GET `/api/candidates/{id}/applications` - Get candidate's applications

## Development

### Adding New Features
1. Add entities to the Core layer
2. Create corresponding DTOs
3. Define interfaces in Core
4. Implement services in Application
5. Add repository implementations in Infrastructure
6. Create API endpoints in Controllers

### Database Migrations
```powershell
cd JobApplicationSystem.Api
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. 