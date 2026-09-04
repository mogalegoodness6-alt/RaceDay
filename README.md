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

RaceDay Project – 2026
