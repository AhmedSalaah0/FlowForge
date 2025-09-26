# FlowForge Project

## Overview
FlowForge is a modern project management application designed to manage tasks, projects, sections, and notifications efficiently. It is built using ASP.NET Core and Entity Framework, featuring a responsive UI for seamless user experience across devices.

## Features
- **User Authentication & Authorization**
  - Secure login and registration system
  - Password reset functionality with email verification
  
- **Project Management**
  - Create and manage multiple projects
  - Invite team members with role-based permissions
  - Track project progress and activity
  
- **Task Management**
  - Drag-and-drop interface for task organization
  - Section-based task categorization
  - Task assignment to project members
  - Task status tracking and priority management
  - Decimal-based ordering system for precise task arrangement
  
- **Real-time Notifications**
  - SignalR-powered real-time notifications
  - Browser notifications for important updates
  - Notification center with read/unread status
  - Project invitation and task assignment notifications

## Project Structure
### Core
Contains the core logic, DTOs, domain entities, enums, and service contracts.
- **Domain Entities**: Project, Task, Section, Notification models
- **Service Contracts**: Interface definitions for all services
- **DTOs**: Data transfer objects for API communication
- **Hubs**: SignalR hub definitions for real-time features

### Infrastructure
Handles database context, migrations, and repositories.
- **Repositories**: Data access implementation
- **Database Context**: Entity Framework configuration
- **Migrations**: Database schema evolution

### UI
Includes controllers, middleware, views, and client-side assets.
- **Controllers**: Request handling and business logic coordination
- **Views**: Razor pages for rendering UI
- **Filters**: Custom action and authorization filters
- **wwwroot**: Client-side JavaScript, CSS, and static assets

## Technologies Used
- **Backend**
  - ASP.NET Core 9
  - Entity Framework Core
  - SignalR for real-time communication
  - Identity Framework for authentication
  - Microsoft SQL Server
  
- **Frontend**
  - Bootstrap 5 for responsive design
  - JavaScript with fetch API
  - SortableJS for drag-and-drop functionality
  
- **DevOps & Tools**
  - Serilog for structured logging
  - Email service integration

## Live Demo
Access the live version of FlowForge at [https://flowforge.runasp.net]

## Unit Tests  
The project includes unit tests for core services using the following frameworks:  

- **xUnit**: Testing framework  
- **Moq**: Mocking framework for isolating components  
- **FluentAssertions**: For more readable assertions  
- **AutoFixture**: For test data generation  

Tests cover key service functionality including:  
- Project service operations  
- Project member management  
- Task and section operations  

### Status  
🧪 Unit Testing: 🚧 In Progress  
![Unit Testing](https://img.shields.io/badge/Unit%20Testing-In%20Progress-yellow?style=flat-square)  



## License
This project is licensed under the MIT License.
