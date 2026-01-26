# LibraryManagementAPI
LibraryManagementAPI is a RESTful API for managing a library system (users, books, issues, fines, requests). The project follows a layered design with a class library for domain logic and an ASP.NET Core Web API for HTTP surface and integration.

## Topics Covered:
* **JWT Authentication**
* **Role-based authorization**
  * **Admin(Librarian)** & **Member** Roles
* **Login & Register endpoints**
* **Secure password hashing with ASP.NET Core Identity**
* **CRUD endpoints**
* **Entity Framework Core Relationships**
  * **One-to-One: Book ? Publisher**
  * **One-to-Many: User ? BookIssues**
  * **Many-to-Many: Books ? Categories**

## Technologies used: <br/>
* **ASP.NET Core Web API** <br/>
* **Microsoft SQL Server** <br/>
* **Entity Framework Core** <br/>
* **JWT Authentication**<br/>
* **ASP.NET Core Identity**
