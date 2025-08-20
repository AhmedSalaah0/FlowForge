# FlowForge Project

## Overview
FlowForge is a backend project designed to manage tasks, projects, sections, and notifications efficiently. It is built using ASP.NET Core and Entity Framework.

## Features
- User authentication and authorization.
- Project and task management.
- Section-based task organization.
- Notifications for project members.
- Admin functionalities for task migration.

## Project Structure
### Core
Contains the core logic, DTOs, domain entities, enums, and service contracts.
### Infrastructure
Handles database context, migrations, and repositories.
### UI
Includes controllers, middleware, and views for user interaction.

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server
- Serilog for logging

## How to Run
1. Clone the repository.
2. Set up the database connection string in `appsettings.json`.
3. Run the migrations to set up the database schema.
4. Start the application using `dotnet run`.

## License
This project is licensed under the MIT License.