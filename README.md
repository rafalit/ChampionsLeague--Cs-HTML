# Champions League Simulator
## Overview
WebApplication1 is an ASP.NET Core web application designed to manage a database of clubs and users. The application integrates with MongoDB to perform CRUD operations for clubs and users. It also includes features for user authentication, session management </br> and knockout stages for competitions.
## Features
* Club Management: Create, read, update, and delete clubs from the database.
* User Management: Register new users and authenticate existing ones.
* Session Management: Maintain user sessions with configurable timeout settings.
* Knockout Stage Management: Handle knockout stage pairs for club competitions.
* API Documentation: Swagger integration for API documentation and testing.
## Technologies Used
* ASP.NET Core: A cross-platform framework for building modern cloud-based internet-connected applications.
* MongoDB: A NoSQL database used for storing club and user data.
* C#: The primary programming language for the application.
* Swagger: Used for generating API documentation.
* Microsoft.Extensions: For dependency injection and configuration management.
## Prerequisites
Before you begin, ensure you have the following installed:
* .NET 5.0 SDK or later
* MongoDB (local or hosted instance)
* Visual Studio 2019 or later or Visual Studio Code
## Getting Started
**1. Clone the Repository**
Clone the repository to your local machine using the following command:
```
git clone https://github.com/yourusername/WebApplication1.git
cd WebApplication1
````
**2. Configure MongoDB Settings**
Update the appsettings.json file with your MongoDB connection details:
```
{
  "ClubsStoreDatabaseSettings": {
    "ConnectionString": "your_mongo_db_connection_string",
    "DatabaseName": "your_database_name",
    "ClubsCoursesCollectionName": "Clubs"
  }
}
```
Replace "your_mongo_db_connection_string" with the connection string for your MongoDB instance, and "your_database_name" with the name of your database.

**3. Build the Application**
Build the application using the .NET CLI:
```
dotnet build
```
**4. Run the Application**
Run the application using the following command:
```
dotnet run
```
The application will start and be accessible at `https://localhost:5001` or `http://localhost:5000` depending on your HTTPS settings.

**5. Access API Documentation**
API documentation is available via Swagger. Open your browser and navigate to `https://localhost:5001/swagger` or `http://localhost:5000/swagger` to view and test the API endpoints.

## Usage
**1. Club Management**
* Create a Club: Use the /api/Clubs/Create endpoint to add a new club to the database.
* Get All Clubs: Use the /api/Clubs/Get endpoint to retrieve all clubs.
* Get a Club by ID: Use the /api/Clubs/Get/{id} endpoint to retrieve a club by its ID.
* Update a Club: Use the /api/Clubs/Update/{id} endpoint to update an existing club's details.
* Delete a Club: Use the /api/Clubs/Delete/{id} endpoint to remove a club from the database.
  
**2. User Management**
* Register a User: Use the /api/Users/Register endpoint to create a new user.
* Login: Use the /api/Users/Login endpoint to authenticate a user.
  
**3. Knockout Stage Management**
* Add Selected Clubs: Use the /Teams/ChooseTeam endpoint to select clubs for the knockout stage.
* Set Knockout Pairs: Use the /Teams/KnockoutStage endpoint to define knockout pairs.

## Session Management
Sessions are configured to expire after 30 minutes of inactivity. This setting can be adjusted in the Startup.cs file within the AddSession configuration:
```
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```
## Video Overview

The following videos provide an overview and demonstration of the application:

- **[Video 1](./vid1.mkv)** Demonstrates the registration and login process..

- **[Video 2](./vid2.mkv)** Shows the team selection process.

- **[Video 3](./vid3.mkv)** Walkthrough of the group stage setup.

- **[Video 4](./vid4.mkv)** Overview of the knockout stage.
