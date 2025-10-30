# Project Overview

This project is a .NET-based e-commerce application that uses a clean architecture approach. It is composed of four projects: `Domain`, `Application`, `Infrastructure`, and `API`.

- **Domain:** Contains the core business logic and entities.
- **Application:** Contains application-specific logic, like use cases and commands.
- **Infrastructure:** Contains external concerns like databases, file systems, etc.
- **API:** Exposes the application through an API.

The project uses Marten as its data persistence mechanism, which provides a document database and event store on top of a PostgreSQL database.

# Building and Running

To build and run the project, you will need to have the .NET SDK and PostgreSQL installed on your machine.

1. **Clone the repository:**

```bash
git clone https://github.com/1nauman/ecommerce-marten-es-demo.git
```

2. **Restore the dependencies:**

```bash
dotnet restore
```

3. **Run the application:**

```bash
dotnet run --project src/API/API.csproj
```

The application will be available at `http://localhost:5101`.

# Development Conventions

The project follows the standard .NET coding conventions. It also uses the following conventions:

- **Clean Architecture:** The project is structured using the Clean Architecture pattern, which separates the concerns of the application into different layers.
- **Domain-Driven Design:** The project uses Domain-Driven Design (DDD) principles to model the business domain.
- **Event Sourcing:** The project uses Event Sourcing to store the state of the application as a sequence of events.
- **CQRS:** The project uses the Command Query Responsibility Segregation (CQRS) pattern to separate the read and write operations of the application.
