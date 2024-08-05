# LibraryAPI Project

## Introduction
Welcome to the LibraryAPI project! This project is a comprehensive library management system designed to help librarians, employees, and members manage library resources effectively. The project includes features such as book management, member management, loan management, and more. It is built with a focus on scalability, security, and ease of use.

## Project Overview
The LibraryAPI project is a RESTful API built using ASP.NET Core. It provides endpoints for managing library resources such as books, authors, categories, and members. The project also includes features for handling loans, calculating penalties, and managing user roles and authentication. The API is designed to be used by different roles such as librarians, employees, and members, each with specific permissions and capabilities.

## Technologies Used
This project utilizes a variety of technologies to ensure a robust and efficient system. Below is a detailed explanation of the key technologies used:

### ASP.NET Core
ASP.NET Core is a cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications. It is used to build the RESTful API that powers the LibraryAPI project.

### Entity Framework Core
Entity Framework Core (EF Core) is an open-source ORM (Object-Relational Mapper) for .NET. It allows developers to work with a database using .NET objects, eliminating the need for most data-access code.

### Identity Framework
The ASP.NET Core Identity framework is used to manage users, passwords, roles, and claims. It provides a complete, customizable authentication and authorization system. It integrates seamlessly with EF Core to handle the storage and retrieval of user-related data.

### JWT (JSON Web Tokens)
JWT is used for securely transmitting information between parties as a JSON object. It is used for authentication and authorization in the LibraryAPI project.

### Docker
Docker is a platform for developing, shipping, and running applications in containers. The LibraryAPI project is containerized using Docker to ensure consistency across different environments and ease of deployment.

### SQL Server
SQL Server is a relational database management system developed by Microsoft. It is used as the database for the LibraryAPI project to store all the library data.

### Swagger
Swagger is an open-source tool for documenting APIs. It provides a user-friendly interface to explore and test API endpoints. The LibraryAPI project includes Swagger for API documentation and testing.

## Project Structure
The project is organized into several folders and files to maintain a clean and manageable structure. Here is an overview of the project structure:

LibraryAPI/ <br>
├── Auth/ <br>
├── Controllers/ <br>
├── Data/ <br>
├── Models/ <br>
├ ├── Entities <br>
├ ├── DTOs      <br>
├ ├── Enums     <br>
├── Exceptions/ <br>
├── Migrations/ <br>
├── Services/ <br>
├── LibraryAPI/ <br>
├ ├── AuthorImages/ <br>
├ ├── BookImages/ <br>

### Key Folders and Files
- **Auth/**: Contains authentication-related files.
- **Controllers/**: Contains the API controllers that handle HTTP requests.
- **DTOs/**: Contains Data Transfer Objects used for data encapsulation.
- **Data/**: Contains the database context and migration files.
- **Entities/**: Contains the entity models representing the database schema.
- **Exceptions/**: Contains custom exception classes.
- **Services/**: Contains the service classes implementing the business logic.
- **docker/**: Contains Docker configuration files.

## Model Classes
The model classes represent the entities in the database. Here are some key model classes used in the project:

### ApplicationUser
Represents a user in the system with properties like Id, Name, Email, and more.

## Endpoints
The LibraryAPI provides various endpoints for managing library resources. Here is a table of the key endpoints and their purposes:

### Department Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/departments              | Retrieves all departments                            |
| POST        | /api/departments              | Creates a new department                             |
| GET         | /api/departments/{id}         | Retrieves a specific department by ID                |
| PUT         | /api/departments/{id}         | Updates a department's details                       |
| DELETE      | /api/departments/{id}         | Deletes a department                                 |

### Employee Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/employees                | Retrieves all employees                              |
| POST        | /api/employees                | Creates a new employee                               |
| GET         | /api/employees/{id}           | Retrieves a specific employee by ID                  |
| PUT         | /api/employees/{id}           | Updates an employee's details                        |
| PATCH       | /api/employees/{id}/status/active | Sets an employee's status to active                 |
| PATCH       | /api/employees/{id}/status/quit | Sets an employee's status to quit                   |
| PATCH       | /api/employees/{id}/password  | Updates an employee's password                       |
| POST        | /api/employees/{id}/address   | Adds an address for an employee                      |

### Location Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/locations                | Retrieves all locations                              |
| POST        | /api/locations                | Creates a new location                               |
| GET         | /api/locations/{id}           | Retrieves a specific location by ID                  |
| PUT         | /api/locations/{id}           | Updates a location's details                         |
| PATCH       | /api/locations/{id}/status/active | Sets a location's status to active                  |
| PATCH       | /api/locations/{id}/status/inactive | Sets a location's status to inactive                |
| GET         | /api/locations/section/{sectionCode} | Retrieves books by section code                     |
| GET         | /api/locations/aisle/{aisleCode} | Retrieves books by aisle code                       |
| GET         | /api/locations/shelf/{shelfNumber} | Retrieves books by shelf number                     |

### Language Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/languages                | Retrieves all languages                              |
| POST        | /api/languages                | Creates a new language                               |
| GET         | /api/languages/{id}           | Retrieves a specific language by ID                  |
| PUT         | /api/languages/{id}           | Updates a language's details                         |
| PATCH       | /api/languages/{id}/status/active | Sets a language's status to active                  |
| PATCH       | /api/languages/{id}/status/inactive | Sets a language's status to inactive                |

### Nationality Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/nationalities            | Retrieves all nationalities                          |
| POST        | /api/nationalities            | Creates a new nationality                            |
| GET         | /api/nationalities/{id}       | Retrieves a specific nationality by ID               |
| PUT         | /api/nationalities/{id}       | Updates a nationality's details                      |
| DELETE      | /api/nationalities/{id}       | Deletes a nationality                                |
| GET         | /api/nationalities/{id}/authors | Retrieves authors by nationality ID                 |

### Category Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/categories               | Retrieves all categories                             |
| POST        | /api/categories               | Creates a new category                               |
| GET         | /api/categories/{id}          | Retrieves a specific category by ID                  |
| PUT         | /api/categories/{id}          | Updates a category's details                         |
| PATCH       | /api/categories/{id}/status/active | Sets a category's status to active                  |
| PATCH       | /api/categories/{id}/status/inactive | Sets a category's status to inactive                |
| GET         | /api/categories/{id}/books    | Retrieves books by category ID                       |

### SubCategory Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/subcategories            | Retrieves all subcategories                          |
| POST        | /api/subcategories            | Creates a new subcategory                            |
| GET         | /api/subcategories/{id}       | Retrieves a specific subcategory by ID               |
| PUT         | /api/subcategories/{id}       | Updates a subcategory's details                      |
| PATCH       | /api/subcategories/{id}/status/active | Sets a subcategory's status to active              |
| PATCH       | /api/subcategories/{id}/status/inactive | Sets a subcategory's status to inactive            |
| GET         | /api/subcategories/{id}/books | Retrieves books by subcategory ID                    |

### Author Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/authors                  | Retrieves all authors                                |
| POST        | /api/authors                  | Creates a new author                                 |
| GET         | /api/authors/{id}             | Retrieves a specific author by ID                    |
| PUT         | /api/authors/{id}             | Updates an author's details                          |
| PATCH       | /api/authors/{id}/status/active | Sets an author's status to active                  |
| PATCH       | /api/authors/{id}/status/inactive | Sets an author's status to inactive                |
| PATCH       | /api/authors/{id}/status/banned | Sets an author's status to banned                   |
| PATCH       | /api/authors/{id}/image       | Updates an author's image                            |
| GET         | /api/authors/{id}/image       | Retrieves an author's image                          |

### Book Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/books                    | Retrieves all books                                  |
| POST        | /api/books                    | Creates a new book                                   |
| GET         | /api/books/{id}               | Retrieves a specific book by ID                      |
| PUT         | /api/books/{id}               | Updates a book's details                             |
| PATCH       | /api/books/{id}/status/active | Sets a book's status to active                       |
| PATCH       | /api/books/{id}/status/inactive | Sets a book's status to inactive                    |
| PATCH       | /api/books/{id}/status/banned | Sets a book's status to banned                       |
| PATCH       | /api/books/{id}/image         | Updates a book's image                               |
| GET         | /api/books/{id}/image         | Retrieves a book's image                             |
| PUT         | /api/books/{id}/rating        | Updates a book's rating                              |
| PUT         | /api/books/{id}/copies        | Updates the number of copies of a book               |

### Loan Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/loans/employee           | Retrieves loans by the currently logged-in employee  |
| GET         | /api/loans/member             | Retrieves loans by the currently logged-in member    |
| GET         | /api/loans/{id}               | Retrieves a specific loan by ID                      |
| POST        | /api/loans                    | Creates a new loan                                   |
| PUT         | /api/loans/{id}               | Updates an existing loan's status                    |
| PATCH       | /api/loans/{id}/return        | Marks a loan as returned                             |

### Member Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/members                  | Retrieves all members                                |
| POST        | /api/members                  | Creates a new member                                 |
| GET         | /api/members/{id}             | Retrieves a specific member by ID                    |
| PUT         | /api/members/{id}             | Updates a member's details                           |
| PATCH       | /api/members/remove           | Removes a member by setting their status to removed  |
| PATCH       | /api/members/{id}/status/blocked | Sets a member's status to blocked                   |
| PATCH       | /api/members/password         | Updates a member's password                          |
| POST        | /api/members/address          | Adds an address for a member                         |

### Penalty Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/penalties/member         | Retrieves all penalties for the currently logged-in member |
| GET         | /api/penalties/{id}           | Retrieves a specific penalty by ID                   |

### Publisher Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/publishers               | Retrieves all publishers                             |
| POST        | /api/publishers               | Creates a new publisher                              |
| GET         | /api/publishers/{id}          | Retrieves a specific publisher by ID                 |
| PUT         | /api/publishers/{id}          | Updates a publisher's details                        |
| PATCH       | /api/publishers/{id}/status/active | Sets a publisher's status to active                 |
| PATCH       | /api/publishers/{id}/status/inactive | Sets a publisher's status to inactive               |
| GET         | /api/publishers/{id}/books    | Retrieves books published by a specific publisher    |
| GET         | /api/publishers/{id}/address  | Retrieves a publisher's address                      |
| PUT         | /api/publishers/{id}/address  | Adds or updates a publisher's address                |

### Wanted Book Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| GET         | /api/wantedbooks              | Retrieves all wanted books                           |
| POST        | /api/wantedbooks              | Creates a new wanted book                            |
| GET         | /api/wantedbooks/{id}         | Retrieves a specific wanted book by ID               |
| DELETE      | /api/wantedbooks/{id}         | Deletes a wanted book                                |

## Authentication Endpoints
| HTTP Method | Endpoint                      | Purpose                                              |
|-------------|-------------------------------|------------------------------------------------------|
| POST        | /api/authentication/login     | Logs in a user and returns a JWT token               |
| POST        | /api/authentication/logout    | Logs out the currently logged-in user                |
| GET         | /api/authentication/externallogin | Initiates an external login                      |
| GET         | /api/authentication/externallogincallback | Handles the external login callback      |

## Testing the Project
To test the project, follow these steps:

### Create a file with name docker-compose.yml in a folder

[docker-compose.yml](https://github.com/MervanMunis/LibraryAPI-Dockerized/blob/master/docker-compose.yml)

```yml
version: '3.4'

services:
  libraryapi:
    depends_on:
      - sqlserver
    image: mervanmunis/libraryapi:latest
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__LibraryAPIContext=Server=sqlserver;Database=LibraryAPI;User=sa;Password=Master123!;
      - Jwt__Key=b9T9j54dI4E9kR2lA1V3cG7xN0Q2zY8uJ5R7mS6hP4L8vX9qK2
      - Jwt__Issuer=LibraryAPI
      - Jwt__Audience=LibraryAPIUsers
    networks:
      - library-network
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      SA_PASSWORD: "Master123!"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    networks:
      - library-network

networks:
  library-network:
    driver: bridge
```

### Run the Docker Compose file from the file directory

```sh
docker-compose up
```

### Access the Swagger UI

Open your browser and navigate to 
```sh 
http://localhost:5000/swagger
```

### Authentication and Authorization

1. Login as Admin:
   * Endpoint: POST /api/authentication/login
   * JSON Body:
```json
{
    "email": "admin@admin.com",
    "password": "Admin1234!"
}
```
  * Copy the token from the response.
    
2. Authorize:
  * Click on the "Authorize" button in Swagger.
  * Paste the token and click "Authorize".

3. Create a Department:
  * Endpoint: POST /api/Departments
  * JSON Body:
```json
{
    "name": "Information Department"
}
```

4. Create an Employee:
  * Endpoint: POST /api/Employees
  * Employee Shift Values: Night, Morning
  * Employee Title Values: Librarian, BranchManager, DepartmentHead, MovableRecodeOfficer, MovableControlOfficer, HeadOfLibrary
  * Employee Status: Working, Quit
  * Gender Values: Male, Female
  * JSON Body:
```json
{
    "idNumber": "123456789",
    "name": "John",
    "lastName": "Doe",
    "userName": "johndoe",
    "email": "johndoe@example.com",
    "phoneNumber": "1234567890",
    "gender": "Male",
    "birthDate": "1990-01-01",
    "salary": 50000,
    "employeeShift": "Morning",
    "employeeTitle": "Librarian",
    "departmentId": 1,
    "password": "Password123!",
    "confirmPassword": "Password123!",
}
```

5. Login as Employee:
  * Repeat the login and authorization steps using the employee's credentials.

6. Create a Location:
  * Endpoint: POST /api/Locations
  * JSON Body:
```json
{
    "sectionCode": "A1",
    "aisleCode": "B2",
    "shelfNumber": "3"
}
```

7. Create a Nationality:
  * Endpoint: POST /api/Nationalities
  * JSON Body:
```json
{
    "name": "American",
    "nationalityCode": "US"
}
```

8. Create a Language:
  * Endpoint: POST /api/Languages
  * JSON Body:
```json
{
    "name": "English",
    "nationalityId": 1
}
```

9. Create a Category:
  * Endpoint: POST /api/Categories
  * JSON Body:
```json
{
    "name": "Literature"
}
```

10. Create a SubCategory:
  * Endpoint: POST /api/SubCategories
  * JSON Body:
```json
{
    "name": "Fiction",
    "categoryId": 1
}
```

11. Create an Author:
  * Endpoint: POST /api/Authors
  * JSON Body:
```json
{
    "fullName": "George Orwell",
    "biography": "Author of 1984 and Animal Farm",
    "birthYear": 1903,
    "deathYear": 1950,
    "languageId": 1
}
```

12. Create a Book:
  * Endpoint: POST /api/Books
  * JSON Body:
```json
{
    "title": "1984",
    "isbn": "1234567890123",
    "publicationYear": 1949,
    "locationId": 1,
    "publisherId": 1,
    "authorIds": [1],
    "languageId": [1],
    "subCategoryIds": [1]
}
```
13. Logout:
  * Endpoint: POST /api/authentication/logout

14. Create a Member Account:
  * Endpoint: POST /api/Members
  * EducationalDegree Values: None, HighSchool, Associate, Bachelor, Master, Doctorate, PostDoctorate
  * MemberStatus Values: BlockedAccount, RemovedAccount, ActiveAccount 
  * JSON Body:
```json
{
    "idNumber": "987654321",
    "name": "Jane",
    "lastName": "Doe",
    "userName": "janedoe",
    "email": "janedoe@example.com",
    "phoneNumber": "0987654321",
    "gender": "Female",
    "birthDate": "1992-02-02",
    "educationalDegree": "Bachelor's"
    "password": "Password123!",
    "confirmPassword": "Password123!",
}
```
15. Login as Member:
  * Repeat the login and authorization steps using the member's credentials.

16. Borrow a Book:
  * Endpoint: POST /api/Loans
  * JSON Body:
```json
{
    "howManyDays": 14,
    "bookCopyId": 1,
    "bookId": 1,
    "memberIdNumber": "987654321"
}
```

17. Rate a Book:
  * Endpoint: POST /api/Books/{id}/ratings
  * JSON Body:
```json
{
    "rating": 4.2
}
```

18. Return a Book:
  * Endpoint: PATCH /api/Loans/{id}/return

Not: 
  * PenaltyType Values: None, BookTenDays, BookTwoMonths, BookOneYear, BookLimitless, LibraryTenDays, LibraryTwoMonths, LibraryOneYear, LibraryLimitless

## Conclusion

The LibraryAPI project provides a robust and scalable solution for managing library resources. With its comprehensive set of features, modern technology stack, and detailed documentation, it is well-suited for deployment in real-world library environments. The use of Docker ensures easy deployment and consistency across different environments.

## Acknowledgements
This project was developed as part of the backend program at [Softito Yazılım - Bilişim Akademisi](https://softito.com.tr/index.php). Special thanks to the instructors and peers who provided valuable feedback and support throughout the development process.
