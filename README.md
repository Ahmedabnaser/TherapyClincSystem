# Therapy Clinic System 🏥

A modern healthcare management platform built with .NET 9.0 and Clean Architecture principles.

## 🌟 Features

### Core Functionality
- **Patient Management**
  - Secure registration and authentication (JWT + Identity)
  - Profile management
  - Appointment scheduling workflow

### Appointment System
- Full CRUD operations with validation
- Automatic billing generation
- Doctor/patient specific views

### Medical Records
- Encrypted record storage
- Role-based access (Doctor-only)
- Complete patient history

### Payment Processing
- Stripe integration (v48.2.0)
- Webhook handling for payment confirmation
- Refund processing

### Reporting
- PDF generation (iTextSharp)
- Appointment and financial reports

## 🛠 Technology Stack

### Core Architecture
- **.NET 9.0**
- Onion Architecture
- Repository Pattern (Generic implementation)
- Result Pattern for unified error handling

### Key Packages
| Component           | Technology               |
|---------------------|--------------------------|
| Authentication      | JWT + ASP.NET Identity   |
| Payments           | Stripe.NET (v48.2.0)     |
| PDF Generation     | iTextSharp (v5.5.13.4)   |
| Object Mapping     | AutoMapper (v14.0.0)     |
| Logging           | Serilog (v4.2.0)         |
| API Documentation | Scalar                   |

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL/SQL Server
- Stripe API keys
- SMTP server credentials
- 
 ⚙️ Setup and Configuration
Follow these steps to get the Therapy Clinic System API running locally:

1. 📥 Clone the Repository
```bash
git clone https://github.com/Ahmedabnaser/TherapyClincSystem
cd TherapyClincSystem
```

2. 📦 Restore Dependencies
```bash
dotnet restore
```

3. 🔧 Update Database Connection
Edit the appsettings.json file and replace the connection string under "DefaultConnection" with your local SQL Server connection string.
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TherapyClincDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

4. 🗃️ Apply Migrations
Make sure you have dotnet-ef tools installed:
```bash
dotnet tool install --global dotnet-ef
```
```bash
dotnet ef database update
```

5. ▶️ Run the Application
```bash
dotnet run --project TherapyClincSystem.Api
```
The API should now be accessible at:
```
http://localhost:5255
```

   
