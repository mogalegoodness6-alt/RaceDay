# RaceDay Database Project

## Project Overview

The RaceDay system is a database project designed to manage race events, users, participants and related race information.

This repository contains the SQL scripts and testing scripts developed for the RaceDay system.

## Contents

- RaceDay SQL database script
- RaceDay testing script

## Requirements

To run the SQL scripts, you will need:

- Microsoft SQL Server
- SQL Server Management Studio (SSMS)

## Setup Instructions

1. Open SQL Server Management Studio.
2. Connect to your SQL Server instance.
3. Open the RaceDay SQL database script.
4. Execute the script to create the RaceDay database and its tables.
5. Open the RaceDay testing script.
6. Execute the testing queries to verify that the database is working correctly.

## Testing

The testing SQL script contains queries used to verify the functionality and correctness of the RaceDay database.



## CI/CD

GitHub Actions is used to automatically build and/or validate the project.

A screenshot showing the successful green CI/CD build is included in this repository.

## Author

## Database Structure

The RaceDay database consists of the following main entities:

- Users – Stores user account and role information.
- Events – Stores information about race events.
- Routes – Stores race route details.
- Weather – Stores weather information associated with events.
- Categories – Stores race categories.
- EventCategory – Links events to their categories.
- Enrollments – Stores participant registrations for events.
- Results – Stores participant race results.

The database uses primary keys and foreign keys to maintain data integrity and establish relationships between related entities.
## Testing Process

The RaceDay testing script is used to verify that the database tables, relationships, sample data and queries are functioning correctly.

The testing process includes:

- Retrieving records from the database.
- Testing relationships between related tables.
- Using JOIN queries to retrieve related information.
- Using ORDER BY to organise query results.
- Checking that inserted sample data can be retrieved correctly.
- Verifying that the database structure operates as expected.

The testing script should be executed after the database script has been successfully completed.

## Database Requirements

The RaceDay database was developed using Microsoft SQL Server and can be managed using SQL Server Management Studio (SSMS).

### Required Software

- Microsoft SQL Server
- SQL Server Management Studio (SSMS)

### Recommended Environment

The SQL scripts should be executed in a SQL Server environment that supports:

- T-SQL
- IDENTITY columns
- Primary and foreign key constraints
- CHECK constraints
- DEFAULT constraints

  ## Database Relationships

The RaceDay database uses primary keys and foreign keys to connect related entities.

Key relationships include:

- Users → Events: Organisers can create and manage race events.
- Events → Routes: Each event can have an associated race route.
- Events → Weather: Weather information is associated with race events.
- Events → Categories: Events can be associated with one or more race categories through the EventCategory table.
- Users → Enrollments: Participants can register for race events.
- Events → Enrollments: An event can have multiple participant enrollments.
- Enrollments → Results: Participant results are linked to their event enrollment.

These relationships help maintain referential integrity and reduce duplication within the database.

## Database Features

The RaceDay database includes:

- Primary keys for uniquely identifying records.
- Foreign keys for linking related tables.
- Identity columns for automatically generated IDs.
- Unique constraints for preventing duplicate email addresses.
- CHECK constraints for validating permitted values.
- Default constraints for providing automatic values where required.
- Sample data for testing database functionality.

  ## SQL Execution Order

For successful execution, the RaceDay SQL scripts should be run in the appropriate order.

1. Execute the database/table creation script.
2. Confirm that the tables and constraints are created successfully.
3. Execute the data insertion statements.
4. Execute the testing script.
5. Review the returned query results to confirm that the database is functioning correctly.

   ## Project Structure

The repository contains the following main project files:

- SQL database script – Contains the RaceDay database tables, constraints and sample data.
- SQL testing script – Contains queries used to test and verify the database.
- README.md – Provides project information and setup instructions.
- .github/workflows/ci.yml – Contains the GitHub Actions CI workflow used to validate the repository.

  ## Security Considerations

Security considerations for the RaceDay database include:

- User passwords are represented using PasswordHash rather than storing plain-text passwords.
- User roles are restricted using database validation rules.
- Primary and foreign keys help protect relationships between records.
- Unique constraints help prevent duplicate user email addresses.
- Input validation should be applied by the application before data is stored in the database.

  ## Data Integrity

The RaceDay database uses several mechanisms to maintain data integrity:

- Primary keys ensure that each record has a unique identifier.
- Foreign keys prevent invalid references between related tables.
- NOT NULL constraints ensure that required information is provided.
- UNIQUE constraints prevent duplicate values where uniqueness is required.
- CHECK constraints restrict values to valid options.
- DEFAULT constraints provide appropriate values when no value is supplied.

  ## Sample Data

Sample records are included in the RaceDay database to demonstrate how the system can store and retrieve information.

The sample data covers areas such as:

- Users and user roles.
- Race events.
- Race categories.
- Event-category relationships.
- Race routes.
- Weather information.
- Participant enrollments.
- Race results.

This sample data supports testing of the database relationships and SQL queries.

## Testing Outcomes

The RaceDay testing process is designed to confirm that:

- Database records can be retrieved successfully.
- Related records can be retrieved using JOIN queries.
- Results can be sorted using ORDER BY.
- Data relationships are correctly maintained.
- Sample records are available for testing.

The successful GitHub Actions run also confirms that the repository's CI validation workflow completes successfully.

## Future Improvements

Future improvements to the RaceDay system could include:

- Developing an application interface for users and organisers.
- Adding authentication and authorisation functionality.
- Providing dashboards for race organisers.
- Adding more comprehensive automated database testing.
- Connecting the database to a web or mobile application.
- Expanding reporting functionality for race results and event statistics.

  ## Submission Checklist

Before submission, the RaceDay repository should contain:

- RaceDay SQL database script.
- RaceDay testing script.
- README documentation.
- GitHub Actions CI workflow.
- A successful green CI/CD build.
- YouTube demonstration link.
- All required project files committed to GitHub.

The repository is maintained as the central location for the RaceDay project source files and documentation.

RaceDay Project – 2026

RaceDay — PROG POE Project
Part 2: RESTful API Development
Project Overview
RaceDay is an event management system designed to support running, walking and cycling events. Part 2 extends the original project by developing an ASP.NET Core Web API using C# and SQL Server.

The API provides endpoints for interacting with event-related information and is designed to support secure access and data management.

Technologies Used
C#
ASP.NET Core Web API
Microsoft SQL Server
Entity Framework Core
Swagger / OpenAPI
GitHub and GitHub Actions
Visual Studio
Database
The API connects to the RaceDayDB database hosted on SQL Server.

The database includes tables for users, events, categories, event categories, enrollments, routes, weather and results.

Entity Framework Core is used to map application models to database tables.

Note: The database configuration must use the appropriate SQL Server instance and connection string for the environment in which the project is running.

API Features
The project includes API controllers for the following areas:

User registration and authentication
User information
Event management
Categories
Event categories
Participant enrollments
Routes
Weather information
Race results
Available operations depend on the endpoints implemented in the project.

Authentication and Authorisation
The API includes registration and login functionality.

Role-based access is intended to distinguish between Organisers and Participants. Access to protected endpoints depends on the user’s authentication status and assigned role.

Swagger Documentation
Swagger provides an interactive interface for viewing and testing the API endpoints.

To test the API:

Run the RaceDayAPI project in Visual Studio.
Open the Swagger interface provided by the application.
Select an endpoint.
Click Try it out.
Enter the required information, if applicable.
Click Execute to view the HTTP status code and response.
Some endpoints require authentication. Use the authentication process configured in the application before testing protected operations.

Running the Project
Install a compatible .NET SDK and Visual Studio.
Ensure that Microsoft SQL Server is available.
Open the RaceDayAPI solution in Visual Studio.
Check the database connection string in the application configuration.
Ensure the database and required tables are available.
Restore the NuGet packages.
Build and run the project.
Open the Swagger interface to explore the available API endpoints.
Important: Update the connection string to match your own SQL Server instance. Do not publish database passwords or other secrets in this repository.

Testing
API endpoints can be tested using Swagger. Tests should cover successful requests, invalid input, authentication requirements and role-based access where applicable.

Automated unit tests should be run using the .NET test command when a test project is configured:

dotnet test

Continuous Integration
GitHub Actions can be used to automate the restoration, building and testing of the .NET project whenever changes are pushed to the repository.

The workflow configuration is stored in the .github/workflows directory.

A successful workflow run should be verified in the repository’s Actions tab.

Project Repository
GitHub: https://github.com/mogalegoodness6-alt/RaceDay

Project Status
This repository contains the RaceDay project files and documentation. The implementation status of individual features should be verified against the current source code and successful test results.
