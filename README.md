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

RaceDay Project – 2026
