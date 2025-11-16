    # User Management System – Full Stack Application

This repository contains a complete **User Management System**, built as part of a full-stack development assignment.

The solution includes:

- **ASP.NET Core 9 Web API** (`net9.0`)
- **Angular 20** frontend
- **SQL Server 2019+** database
- **Postman collection** for all API endpoints
- Clean architecture, logging, audit trail, and localization

---

## 1. Project Structure

```text
user-management-task/
│── api/                 # ASP.NET Core Web API (.NET 9)
│── ui/                  # Angular 20 frontend
│── postman/             # Postman collection (JSON)
│── README.md
│── .gitignore
```

---

## 2. Prerequisites

### Backend

- .NET **9** SDK  
- SQL Server **2019 or later**  
- Visual Studio / VS Code (optional, for development)

### Frontend

- Node.js **18+**  
- Angular CLI **20+** (you already have 20.3.x)

---

## 3. Database Setup

1. Open a terminal and go to the API project:

   ```bash
   cd api
   ```

2. Apply EF Core migrations to create the database and seed initial data:

   ```bash
   dotnet ef database update
   ```

   This will:

   - Create the database (if it doesn’t exist)
   - Seed **15 users** using the same secure hashing routine as the app:
     - 5 × Admin users: `admin1` … `admin5`
     - 5 × normal users: `user1` … `user5`
     - 5 × read-only users: `readonly1` … `readonly5`
   - All of them share the same initial password:

     ```text
     123456789!
     ```

---

## 4. Run the Backend (API)

From the `api` folder:

```bash
dotnet run
```

The API will start on:

```text
http://localhost:5177
```

Swagger UI (for exploring the API):

```text
http://localhost:5177/swagger
```

The Angular app uses this base URL for all API calls:

```text
http://localhost:5177/api
```

Versioned endpoints are under:

```text
/ api / v1 / ...
```

---

## 5. Run the Frontend (Angular)

1. Open a new terminal and go to the UI project:

   ```bash
   cd ui
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the Angular dev server:

   ```bash
   ng serve --open
   ```

   The app will open in your browser at:

   ```text
   http://localhost:4200
   ```

---

## 6. Default Login Credentials (Seeded Users)

The seeding logic creates users with this pattern:

- **Admins**  
  - `admin1`, `admin2`, `admin3`, `admin4`, `admin5`
- **Standard Users**  
  - `user1`, `user2`, `user3`, `user4`, `user5`
- **Read-only Users**  
  - `readonly1`, `readonly2`, `readonly3`, `readonly4`, `readonly5`

All of them have the same initial password:

```text
123456789!
```

Example accounts you can use for testing:

### 👑 Admin example

```text
username: admin1
password: 123456789!
```

### 👤 Standard User example

```text
username: user1
password: 123456789!
```

### 👁 Read-only User example

```text
username: readonly1
password: 123456789!
```

> Roles are enforced in both the API and the UI. Only **Admin** users can create, edit, and delete other users.

---

## 7. Main API Endpoints (v1)

Base path:

```text
/ api / v1
```

### Auth

- `POST /auth/login`  
  Log in with username & password, returns JWT.

### Users

- `GET    /users`           – list users (paged, optional search & sort)
- `POST   /users`           – create a user (**Admin only**)
- `GET    /users/{id}`      – get user by id
- `PUT    /users/{id}`      – update a user (**Admin only**)
- `DELETE /users/{id}`      – soft delete a user (**Admin only**)
- `POST   /users/dt`        – DataTable-style server-side listing

### Profile

- `GET /users/profile` – get current logged-in user profile
- `PUT /users/profile` – update current logged-in user profile

### Localization

- `GET /localization/en` – English labels
- `GET /localization/ar` – Arabic labels

---

## 8. Postman Collection

A ready-to-use Postman collection is included at:

```text
postman/UserManagement.postman_collection.json
```

The collection contains requests for:

- Login  
- Users CRUD  
- Profile endpoints  
- DataTable server-side endpoint  
- Localization (EN / AR)

Import this file into Postman to quickly test the API.

---

## 9. Key Features Implemented

- Secure authentication with **JWT**
- Role-based authorization:
  - `Admin`
  - `User`
  - `ReadOnlyUser`
- Password hashing & salting (**PBKDF2 with SHA-256**), used in both runtime and seeding
- **Soft delete** for users (flag instead of physical removal)
- **Audit logging**:
  - Who did it
  - When
  - From which IP
  - Old vs. new values (stored as JSON)
- **Localization** support (English & Arabic) on API + Angular side
- **Structured JSON error responses** (no HTML)
- **Daily log files** for exceptions (`log-YYYY-MM-DD.txt` in a `logs` folder)

---

## 10. Quick Start Summary

1. Install prerequisites (**.NET 9**, **SQL Server**, **Node.js**, **Angular CLI 20**)
2. From `/api`:
   - `dotnet ef database update`
   - `dotnet run`
3. From `/ui`:
   - `npm install`
   - `ng serve --open`
4. Open `http://localhost:4200` and log in, for example, as:

   ```text
   admin1 / 123456789!
   ```

The project is now ready to use and demo as a complete full-stack User Management solution.
