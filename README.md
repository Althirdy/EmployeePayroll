# Employee Payroll API

A simple payroll system built using ASP.NET Core Web API that allows managing employees and computing their take-home pay based on defined business rules.

---

## Screenshots

### Create Employee

![Create Employee Screen](./docs/CreateEmpoloyee.png)

### Employee List and Take-home Pay

![Employee List and Take-home Pay Screen](./docs/EmployeeList.png)

---

## Features

- Employee CRUD operations
- Auto-generated Employee Number:
  - First 3 letters of last name (padded if needed)
  - 5-digit zero-padded random number
  - Date of birth (`ddMMMyyyy` format)
- Take-home pay computation:
  - Double pay on working days
  - Additional pay on employee birthday
- Simple frontend interface using HTML, jQuery, and AJAX
- Dockerized SQL Server database
- Swagger UI for API testing

---

## Tech Stack

### Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server (Docker)

### Frontend

- HTML
- jQuery (AJAX)
- Css
- Javascript

### Tools

- DBeaver (Database management)
- Docker

---

## Design Decisions

- **Service Layer**: Introduced `EmployeeService` with `IEmployeeService` interface to separate business logic from controllers.
- **DTO Pattern**: Used request and response DTOs to not expose the Employee model and have more control for In/Out of Data.
- **Helpers**:
  - `EmployeeNumberGenerator` helps to generate a EmployeeNumber with the require format
  - `WorkingDayHelper` helps the compute take-home pay to check if the day is a working day
- **EmployeeNumber is Immutable**:
  - Once created, it does not change even if employee data is updated.
- **Case-insensitive Working Days**:
  - Ensures robustness when handling user input.

---

## Docker Setup (SQL Server)

Run SQL Server using Docker:

```bash
docker compose up -d 
```
## notes
Docker was used  for the SQL Server setup to make the project easier to run. This also avoided
manually install and configure SQL Server locally, allowing more focus on the development of the application 
and business logic.

---

## Sample API Usage

### Create Employee

```json
{
  "lastName": "DELA CRUZ",
  "firstName": "JUAN",
  "middleName": "SANTOS",
  "dateOfBirth": "1994-05-17",
  "dailyRate": 2000,
  "workingDays": "MWF"
}
```

```json
{
  "dailyRate": 2500,
  "workingDays": "TTHS"
}
```

### Compute Take-home Pay

```json
{
  "startDate": "2011-05-16",
  "endDate": "2011-05-20"
}
```

---

## Business Rules

- Employees only earn on their assigned working days (`MWF` or `TTHS`)
- Working day pay = `dailyRate × 2`
- Birthday pay = `+ dailyRate`
- If birthday falls on a working day, both apply

---

## Frontend

A simple UI is included using HTML + jQuery:

- Create employee
- View employees
- Update employee
- Delete employee
- Compute take-home pay

---

## Notes

This project was developed as part of a developer assessment, focusing on:

- Clean architecture
- Correct business logic implementation
- Practical and maintainable design
