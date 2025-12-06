# CTTMS - Clinical Trial Ticket Management System

A comprehensive web-based ticket management system built with ASP.NET Core Razor Pages for managing clinical trial-related tickets with role-based access control.

## Overview

CTTMS is a ticket management system designed for clinical trial environments, enabling efficient tracking, assignment, resolution, and review of tickets across different categories (Safety, Data, Technical). The system implements a three-tier workflow with Admin, Resolver, and Reviewer roles.

## Features

### Role-Based Access Control
- **Admin**: Full system access including ticket creation, assignment, editing, and deletion
- **Resolver**: Handles assigned tickets and forwards them to reviewers
- **Reviewer**: Reviews resolved tickets and closes them with comments

### Core Functionality
- User authentication and authorization with cookie-based sessions
- Ticket lifecycle management (Create → Assign → Resolve → Review → Close)
- Priority levels (Low, Medium, High)
- Category classification (Safety, Data, Technical)
- Audit logging for all ticket actions
- Notification system for ticket updates
- Ticket allocation tracking

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: Cookie-based authentication
- **ORM**: Entity Framework Core
- **Session Management**: ASP.NET Core Session

## Database Schema

The system uses 7 main tables:

1. **Users** - User accounts with roles
2. **Tickets** - Main ticket information
3. **Category** - Ticket categories
4. **Priority** - Priority levels
5. **TicketAllocation** - Ticket assignment tracking
6. **AuditLog** - Action history
7. **Notification** - User notifications

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (Express or higher)
- Visual Studio 2022 or VS Code (recommended)

## Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd CTTMSProject/CTTMSProject/Clinical
```

### 2. Configure Database Connection
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "ClinicalContext": "Server=YOUR_SERVER;Database=Clinical;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Create Database
Run the SQL script located at `db/SQLQuery1.sql` to create the database and tables:
```sql
-- Execute the entire script in SQL Server Management Studio or Azure Data Studio
```

### 4. Restore NuGet Packages
```bash
dotnet restore
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## Default User Accounts

The system comes with three default accounts:

| Role     | Email                  | Password      |
|----------|------------------------|---------------|
| Admin    | admin@gmail.com        | admin123      |
| Resolver | resolver@gmail.com     | resolver123   |
| Reviewer | reviewer@gmail.com     | reviewer123   |

## Project Structure

```
Clinical/
├── Data/
│   └── ClinicalContext.cs          # EF Core DbContext
├── Models/
│   ├── User.cs                     # User entity
│   ├── Ticket.cs                   # Ticket entity
│   ├── Category.cs                 # Category entity
│   ├── Priority.cs                 # Priority entity
│   ├── TicketAllocation.cs         # Allocation entity
│   ├── AuditLog.cs                 # Audit log entity
│   └── Notification.cs             # Notification entity
├── Pages/
│   ├── Admin/                      # Admin role pages
│   │   ├── Dashboard.cshtml
│   │   ├── CreateTicket.cshtml
│   │   ├── AssignTicket.cshtml
│   │   ├── EditTicket.cshtml
│   │   ├── DeleteTicket.cshtml
│   │   └── ViewTickets.cshtml
│   ├── Resolver/                   # Resolver role pages
│   │   ├── Dashboard.cshtml
│   │   ├── MyTickets.cshtml
│   │   ├── ResolveTicket.cshtml
│   │   └── AssignToReviewer.cshtml
│   ├── Reviewer/                   # Reviewer role pages
│   │   ├── Dashboard.cshtml
│   │   ├── MyTickets.cshtml
│   │   ├── ReviewTicket.cshtml
│   │   └── CloseTicket.cshtml
│   ├── Shared/                     # Shared layouts
│   │   ├── _Layout.cshtml
│   │   ├── _AdminLayout.cshtml
│   │   ├── _ResolverLayout.cshtml
│   │   └── _ReviewerLayout.cshtml
│   ├── Login.cshtml
│   ├── Register.cshtml
│   └── Index.cshtml
├── Helpers/
│   └── PasswordHelper.cs           # Password utilities
├── wwwroot/                        # Static files (CSS, JS)
├── appsettings.json                # Configuration
└── Program.cs                      # Application entry point
```

## Workflow

1. **Admin** creates a ticket with title, description, category, and priority
2. **Admin** assigns the ticket to a **Resolver**
3. **Resolver** works on the ticket and marks it as resolved
4. **Resolver** assigns the resolved ticket to a **Reviewer**
5. **Reviewer** reviews the ticket and either:
   - Closes it with approval
   - Sends it back for further work
6. All actions are logged in the audit trail

## Key Features by Role

### Admin Dashboard
- Create new tickets
- View all tickets
- Assign tickets to resolvers
- Edit ticket details
- Delete tickets
- View ticket statistics

### Resolver Dashboard
- View assigned tickets
- Update ticket status
- Resolve tickets
- Assign tickets to reviewers
- Add resolution comments

### Reviewer Dashboard
- View assigned tickets for review
- Review resolved tickets
- Close tickets with comments
- Request additional work if needed

## Configuration

### Session Settings
Sessions are configured for 2-hour timeout:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### Authentication
Cookie-based authentication with 2-hour expiration:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });
```

## Development

### Running in Development Mode
```bash
dotnet run --environment Development
```

### Building for Production
```bash
dotnet publish -c Release -o ./publish
```

## NuGet Packages

- Microsoft.EntityFrameworkCore (8.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
- Microsoft.EntityFrameworkCore.Tools (8.0.0)
- Microsoft.Data.SqlClient (6.1.3)
- System.Data.SqlClient (4.9.0)

## Security Considerations

⚠️ **Important**: This is a development/demo application. For production use:
- Implement proper password hashing (currently using plain text)
- Add input validation and sanitization
- Implement CSRF protection
- Use HTTPS only
- Add rate limiting
- Implement proper error handling
- Add logging and monitoring
- Review and update security headers

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists and tables are created
- Verify SQL Server authentication settings

### Authentication Issues
- Clear browser cookies
- Check session configuration
- Verify user credentials in database

## Documentation

Additional documentation can be found in:
- `use case/` - Use case diagrams and ER diagrams
- `db/` - Database scripts

## License

[Add your license information here]

## Contributors

[Add contributor information here]

## Support

For issues and questions, please [create an issue](link-to-issues) in the repository.
