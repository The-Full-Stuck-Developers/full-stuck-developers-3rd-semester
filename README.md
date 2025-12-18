# DeadPigeons - Weekly Lottery Game Platform

A full-stack web application for managing a weekly lottery game system where users can place bets on number combinations, with separate admin and player interfaces. The platform supports multi-week betting, transaction management, and game history tracking.

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Routes & Authorization](#routes--authorization)
- [Environment & Configuration](#environment--configuration)
- [Linting & Code Quality](#linting--code-quality)
- [Current State](#current-state)
- [Setup Instructions](#setup-instructions)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Deployment](#deployment)

## Project Overview

DeadPigeons is a weekly lottery game platform built for Jerne IF, where players select 5-8 numbers from a pool of 16 numbers. Games run weekly with betting deadlines on Saturdays at 17:00, and winning numbers are drawn on Sundays.

### Key Features

- **User Management**: Registration, authentication, and profile management
- **Betting System**: 
  - Place bets with 5-8 numbers (pricing: 20, 40, 80, 160 DKK respectively)
  - Multi-week betting (1-20 weeks) with automatic bet creation for future games
  - Bet series tracking for multi-week bets
- **Transaction Management**: 
  - Balance deposits via MobilePay
  - Automatic transaction creation for purchases and refunds
  - Transaction history tracking
- **Game Management** (Admin):
  - Create and manage weekly games
  - Set winning numbers
  - Track revenue and winning bets
  - Support for physical players
- **Player Dashboard**:
  - View active boards
  - Game history with winning board highlighting
  - Balance management
  - Bet deadline tracking

## Architecture

### Technology Stack

**Backend:**
- **.NET 9.0** - ASP.NET Core Web API
- **PostgreSQL** - Database (via Npgsql)
- **Entity Framework Core** - ORM with migrations
- **JWT Bearer Authentication** - Token-based authentication
- **NSwag** - OpenAPI/Swagger documentation and TypeScript client generation
- **Sieve** - Query filtering, sorting, and pagination

**Frontend:**
- **React 19** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **React Router** - Client-side routing
- **Tailwind CSS** - Utility-first CSS framework
- **react-i18next** - Internationalization (English/Danish)
- **Jotai** - State management
- **Axios** - HTTP client
- **react-hot-toast** - Toast notifications

### Project Structure

```
full-stuck-developers-3rd-semester/
├── client/                 # React frontend application
│   ├── src/
│   │   ├── components/    # React components
│   │   │   ├── admin/      # Admin dashboard components
│   │   │   └── sections/  # Public and user sections
│   │   ├── core/         # API clients and utilities
│   │   ├── locales/      # i18n translation files
│   │   └── hooks/        # Custom React hooks
│   ├── package.json
│   └── vite.config.ts
├── server/                # .NET backend application
│   ├── api/              # Main API project
│   │   ├── Controllers/  # API endpoints
│   │   ├── Services/     # Business logic
│   │   ├── Models/       # DTOs and request/response models
│   │   ├── Security/     # Authorization handlers
│   │   └── Program.cs    # Application entry point
│   ├── dataccess/        # Data access layer
│   │   ├── Context/      # DbContext configuration
│   │   ├── Entities/     # Domain entities
│   │   ├── Migrations/   # EF Core migrations
│   │   └── Repositories/ # Repository pattern implementation
│   └── tests/            # Unit and integration tests
└── README.md
```

## Routes & Authorization

### Authentication

The application uses **JWT (JSON Web Tokens)** for authentication:
- **Token Generation**: Tokens are created using HMAC-SHA512 algorithm
- **Token Expiration**: 7 days
- **Token Storage**: Tokens are stored in browser localStorage (client-side)
- **Token Validation**: Server validates tokens on each authenticated request

### Authorization Policies

The application implements role-based access control with the following policies:

1. **`AllowAnonymous`** - Public access, no authentication required
2. **`[Authorize]`** - Requires authenticated user (any logged-in user)
3. **`[Authorize(Policy = "IsAdmin")]`** - Requires authenticated admin user

**IsAdmin Policy Implementation:**
- Custom `IsAdmin` authorization requirement
- `AdminAuthorizationHandler` checks user's `IsAdmin` property
- Fast path: Checks JWT claim `is_admin` first
- Fallback: Database lookup if claim not present
- Applied via `[Authorize(Policy = "IsAdmin")]` attribute

### Complete Route Listing

#### Public Routes (AllowAnonymous)

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/` | API status check |
| `GET` | `/api/Health/App/Up` | Application health check |
| `GET` | `/api/Health/Database/Up` | Database health check |
| `POST` | `/api/auth/Login` | User login |
| `POST` | `/api/auth/Register` | User registration |
| `POST` | `/api/auth/ForgotPassword` | Request password reset email |
| `POST` | `/api/auth/ResetPassword` | Reset password with token |

#### Authenticated Routes (Any Logged-in User)

| Method | Route | Description | Notes |
|--------|-------|-------------|-------|
| `POST` | `/api/auth/Logout` | User logout | Currently not implemented |
| `GET` | `/api/auth/UserInfo` | Get current user info | Returns user's own info |
| `POST` | `/api/Bets` | Place a bet | Users can only place bets for themselves |
| `GET` | `/api/Bets/player/active` | Get user's active boards | Returns only user's own bets |
| `GET` | `/api/Bets/player/history` | Get user's bet history | Returns only user's own bets |
| `DELETE` | `/api/Bets/{id}` | Delete a bet | Users can only delete their own bets |
| `PUT` | `/api/Bets/{id}` | Update a bet | Users can only update their own bets |
| `GET` | `/api/Games/player/current` | Get current game for player | Public game information |
| `POST` | `/api/Transactions` | Create transaction | Typically deposits |
| `GET` | `/api/Transactions/User/{userId}` | Get transactions by user | Access depends on userId matching current user or admin status |
| `GET` | `/api/Transactions/GetUserBalance` | Get user balance | Requires userId parameter |
| `GET` | `/api/Transactions/GetUserDepositTotal` | Get total deposits | Requires userId parameter |
| `GET` | `/api/Transactions/GetUserPurchaseTotal` | Get total purchases | Requires userId parameter |

#### Admin-Only Routes (IsAdmin Policy)

**User Management:**
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/Users` | Get all users (paginated, filtered) |
| `GET` | `/api/Users/{id}` | Get user by ID |
| `POST` | `/api/Users` | Create new user |
| `PATCH` | `/api/Users/{id}` | Update user |
| `DELETE` | `/api/Users/{id}` | Delete user |
| `POST` | `/api/Users/{id}/RenewMembership` | Renew user membership |
| `PATCH` | `/api/Users/{id}/Activate` | Activate user account |
| `PATCH` | `/api/Users/{id}/Dectivate` | Deactivate user account |
| `GET` | `/api/Users/GetPlayerCount` | Get total player count |

**Game Management:**
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/Games/GetAllUpcomingGames` | Get all upcoming games (paginated) |
| `GET` | `/api/Games/GetAllPastGames` | Get all past games (paginated) |
| `GET` | `/api/Games/{id}` | Get game by ID |
| `GET` | `/api/Games/GetCurrentGame` | Get current game |
| `PATCH` | `/api/Games/{id}/UpdateWinningNumbers` | Set winning numbers for game |
| `PATCH` | `/api/Games/{id}/DrawWinners` | Draw winners for game |
| `PATCH` | `/api/Games/{id}/UpdateInPersonData` | Update in-person player data |

**Transaction Management:**
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/Transactions` | Get all transactions (paginated, filtered) |
| `GET` | `/api/Transactions/{id}` | Get transaction by ID |
| `PATCH` | `/api/Transactions/{id}` | Update transaction status |
| `DELETE` | `/api/Transactions/{id}` | Delete transaction |
| `PATCH` | `/api/Transactions/{id}/approve` | Approve transaction |
| `PATCH` | `/api/Transactions/{id}/reject` | Reject transaction |
| `GET` | `/api/Transactions/GetPendingTransactionsCount` | Get count of pending transactions |

### Security Features

1. **Password Hashing**: Uses ASP.NET Core Identity `PasswordHasher<User>` with secure hashing
2. **CORS Configuration**: Restricted to specific origins:
   - `http://localhost:5173` (dev)
   - `http://localhost:5174` (dev alt)
   - `https://deadpigeons.vercel.app` (production)
3. **Input Validation**: Model validation on all DTOs
4. **SQL Injection Protection**: Entity Framework Core parameterized queries
5. **XSS Protection**: React's built-in XSS protection, proper data sanitization
6. **Error Handling**: Global exception handler prevents sensitive information leakage

### Data Access Control

- **User Isolation**: Users can only view/modify their own bets (enforced in controllers via user ID checks)
- **Admin Override**: Admins can view all data via admin-only endpoints
- **Soft Deletes**: Bets are soft-deleted (marked with `DeletedAt`) when games finish, preserving history
- **Transaction Access**: Users can query their own transactions via `/api/Transactions/User/{userId}` endpoint

## Environment & Configuration

### Backend Configuration

Configuration is managed through `appsettings.json` (not in repository) and environment variables. Required settings:

**AppOptions** (in `appsettings.json`):
```json
{
  "AppOptions": {
    "DefaultConnection": "Host=localhost;Database=deadpigeons;Username=postgres;Password=your_password",
    "JwtSecret": "base64_encoded_secret_key_minimum_64_bytes",
    "FrontendUrl": "http://localhost:5173",
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "your_email@example.com",
    "SmtpPassword": "your_password",
    "SmtpFromEmail": "noreply@example.com"
  }
}
```

**Key Configuration Points:**
- `DefaultConnection`: PostgreSQL connection string
- `JwtSecret`: Base64-encoded secret key (minimum 64 bytes) for JWT signing
- SMTP settings: Optional, for email functionality

**Environment-Specific:**
- Development: Auto-migrations and seeding if database is empty
- Production: Manual migrations required

### Frontend Configuration

**Environment Variables:**
- `VITE_API_BASE_URL`: API base URL (defaults to `http://localhost:5000`)

**Base URL Configuration:**
- Configured in `client/src/core/baseUrl.ts`
- Automatically detects environment

### Database Setup

**Initial Setup:**
```bash
# Run migrations and seed database
dotnet run -- setup <default_admin_password>
```

**Manual Migration:**
```bash
cd server/api
dotnet ef migrations add <MigrationName> --project ../dataccess
dotnet ef database update --project ../dataccess
```

### Default Login Credentials

- **Admin**: `admin@example.com` / `hashed_password_here`
- **User**: `user@example.com` / `hashed_password_here`
- **Player**: `player@example.com` / `hashed_password_here`

## Linting & Code Quality

### Frontend Linting

**ESLint Configuration:**
- **Config File**: `client/eslint.config.ts`
- **Plugins Used**:
  - `typescript-eslint` - TypeScript-specific rules
  - `react` - React best practices
  - `react-hooks` - React Hooks rules
  - `unused-imports` - Detects and removes unused imports
  - `prettier` - Code formatting integration

**Key Rules:**
- React 17+ JSX transform (no need to import React)
- Unused imports are errors
- Unused variables are warnings (except those prefixed with `_`)
- React Hooks exhaustive deps warning
- TypeScript strict mode enabled

**Prettier Configuration:**
- **Config File**: `client/.prettierrc`
- Auto-formatting on save (if configured in IDE)
- Format script: `npm run format`

**Linting Commands:**
```bash
cd client
npm run lint          # Check for linting errors
npm run format        # Auto-format code
```

### Backend Code Quality

**C# Best Practices:**
- Nullable reference types enabled
- Async/await pattern throughout
- Repository pattern for data access
- Dependency injection for all services
- DTOs for all API communication
- Entity Framework Core migrations for schema changes

**Code Organization:**
- Controllers handle HTTP concerns only
- Business logic in Services
- Data access in Repositories
- Clear separation of concerns

### Code Cleanup

The codebase has been cleaned of redundant comments. Only essential, non-obvious comments remain. All obvious comments (e.g., "// Create transaction") have been removed for cleaner code.

## Current State

### ✅ What Works

1. **Authentication & Authorization**
   - User registration and login
   - JWT token generation and validation
   - Role-based access control (Admin/User)
   - Password reset functionality

2. **Betting System**
   - Place bets with 5-8 numbers
   - Multi-week betting (1-20 weeks)
   - Automatic bet creation for future games
   - Bet series tracking and display
   - Price calculation and balance validation
   - Bet deletion with refunds

3. **Game Management**
   - Weekly game creation (ISO week-based)
   - Bet deadline enforcement (Saturday 17:00)
   - Winning number drawing
   - Revenue calculation
   - Physical player support

4. **Transaction Management**
   - Balance deposits
   - Automatic transaction creation for purchases
   - Refund transactions on bet deletion
   - Transaction history with pagination

5. **User Interface**
   - Responsive design (mobile and desktop)
   - Internationalization (English/Danish)
   - Admin dashboard with statistics
   - Player dashboard with active boards
   - Game history with winning board highlighting
   - Real-time balance updates

6. **Data Management**
   - Pagination for boards and history
   - Soft deletion for bets (preserves history)
   - Game history filtering
   - Active boards filtering (only open games)

### 🐛 Known Issues & Limitations

1. **Multi-Week Betting Edge Case**
   - If current week's deadline has passed, multi-week bets should start from the next week
   - Currently being addressed in `GetOrCreateGamesForWeeksAsync`

2. **Bet Deadline Display**
   - Deadline is shown in UTC, may need timezone conversion for better UX

3. **Email Functionality**
   - SMTP configuration is optional, email features may not work if not configured

4. **Transaction Refunds**
   - When deleting bets, refunds are created but original transaction amount is preserved for audit trail
   - This is intentional but may cause confusion in transaction history

5. **Game History Pagination**
   - Pagination works correctly, but series information display could be improved

### 🔄 Recent Changes

- Removed bet editing functionality (as per user requirements)
- Cleaned up redundant comments throughout codebase
- Fixed bet deadline to Saturday 17:00 (not 18:00)
- Improved multi-week bet creation logic
- Enhanced winning board visual design in game history

## Setup Instructions

### Prerequisites

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **PostgreSQL 14+** - [Download](https://www.postgresql.org/download/)
- **Git** - [Download](https://git-scm.com/)

### Backend Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd full-stuck-developers-3rd-semester
   ```

2. **Configure database:**
   - Create a PostgreSQL database
   - Update `server/api/appsettings.json` with connection string

3. **Generate JWT Secret:**
   ```bash
   # Generate a base64-encoded secret (64+ bytes)
   openssl rand -base64 64
   ```
   Add to `appsettings.json` under `AppOptions:JwtSecret`

4. **Run setup:**
   ```bash
   cd server/api
   dotnet run -- setup <admin_password>
   ```
   This will:
   - Run database migrations
   - Seed initial data (admin user, sample games)

5. **Run the API:**
   ```bash
   dotnet run
   ```
   API will be available at `http://localhost:5000`

### Frontend Setup

1. **Install dependencies:**
   ```bash
   cd client
   npm install
   ```

2. **Configure API URL** (if different from default):
   - Update `client/src/core/baseUrl.ts`

3. **Run development server:**
   ```bash
   npm run dev
   ```
   Frontend will be available at `http://localhost:5173`

### Development Workflow

1. **Backend Changes:**
   - Make code changes
   - Create migration if schema changed: `dotnet ef migrations add <Name> --project ../dataccess`
   - Apply migration: `dotnet ef database update --project ../dataccess`
   - Run: `dotnet run`

2. **Frontend Changes:**
   - Make code changes
   - Hot reload is automatic in dev mode
   - Lint: `npm run lint`
   - Format: `npm run format`

3. **API Client Regeneration:**
   - Backend automatically generates TypeScript client on startup
   - Output: `client/src/core/generated-client.ts`
   - Regenerate manually by restarting backend

## API Documentation

### OpenAPI/Swagger

Interactive API documentation is available at:
- **Swagger UI**: `http://localhost:5000/swagger`
- **Scalar UI**: Available via Scalar integration
- **OpenAPI JSON**: `http://localhost:5000/openapi/v1.json`

### Key Endpoints

**Authentication:**
- `POST /api/Auth/register` - User registration
- `POST /api/Auth/login` - User login
- `POST /api/Auth/reset-password` - Password reset

**Bets (Player):**
- `POST /api/Bets` - Place a bet
- `GET /api/Bets/player/active` - Get active boards (paginated)
- `GET /api/Bets/player/history` - Get game history (paginated)
- `DELETE /api/Bets/{id}` - Delete a bet (with refund)

**Games:**
- `GET /api/Games/player/current` - Get current game (player)
- `GET /api/Games/upcoming` - Get upcoming games (admin)
- `GET /api/Games/past` - Get past games (admin)
- `PUT /api/Games/{id}/winning-numbers` - Set winning numbers (admin)
- `POST /api/Games/{id}/draw` - Draw winners (admin)

**Transactions:**
- `GET /api/Transactions/user/balance` - Get user balance
- `GET /api/Transactions/user/deposit-total` - Get total deposits
- `POST /api/Transactions/deposit` - Create deposit transaction

**Users (Admin only):**
- `GET /api/Users` - Get all users (paginated, filtered)
- `GET /api/Users/{id}` - Get user by ID
- `POST /api/Users` - Create user
- `PATCH /api/Users/{id}` - Update user
- `DELETE /api/Users/{id}` - Delete user

### Authentication

All endpoints except `/api/Auth/*` and `/` require authentication. Include JWT token in Authorization header:

```
Authorization: Bearer <token>
```

## Testing

### Backend Tests

**Test Framework:** xUnit

**Run Tests:**
```bash
cd server/tests
dotnet test
```

**Test Coverage:**
- Game service tests
- Transaction service tests
- Integration tests with Testcontainers (PostgreSQL)

### Frontend Tests

**Test Framework:** Vitest

**Run Tests:**
```bash
cd client
npm test              # Watch mode
npm run test:run      # Single run
npm run test:ui       # UI mode
```

**Test Configuration:**
- `vitest.config.ts` - Test configuration
- Uses `@testing-library/react` for component testing
- Uses `happy-dom` for DOM simulation

## Deployment

### Backend Deployment

**Requirements:**
- .NET 9.0 runtime
- PostgreSQL database
- Environment variables or `appsettings.json` configured

**Build:**
```bash
cd server/api
dotnet publish -c Release -o ./publish
```

**Environment Variables:**
- `ASPNETCORE_ENVIRONMENT` - Set to `Production`
- Database connection string
- JWT secret

### Frontend Deployment

**Build:**
```bash
cd client
npm run build
```

**Output:** `client/dist/` directory contains static files

**Deployment Options:**
- **Vercel**: Configured via `vercel.json` (if present)
- **Static Hosting**: Any static file server (Netlify, AWS S3, etc.)

**Environment Variables:**
- `VITE_API_BASE_URL` - API base URL for production

### CI/CD

GitHub Actions workflows:
- `.github/workflows/format.yml` - Code formatting check
- `.github/workflows/ci-tests.yml` - Continuous integration tests

## Additional Notes

### Database Migrations

Migrations are managed via Entity Framework Core:
- Location: `server/dataccess/Migrations/`
- Create: `dotnet ef migrations add <Name> --project ../dataccess`
- Apply: `dotnet ef database update --project ../dataccess`
- Rollback: Remove migration files and update database

### Internationalization

Translation files located in:
- `client/src/locales/en/` - English translations
- `client/src/locales/dk/` - Danish translations

Namespaces:
- `common` - Common UI elements
- `player` - Player dashboard
- `game` - Game-related
- `user` - User management
- `validation` - Form validation messages

### Code Generation

TypeScript API client is auto-generated from OpenAPI spec:
- Generated on backend startup
- Output: `client/src/core/generated-client.ts`
- Do not manually edit this file

---

**Last Updated:** December 2025  
**Maintainers:** Full Stack Developers Team
