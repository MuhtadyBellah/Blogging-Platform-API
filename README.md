📘 Blogging Platform API
A simple RESTful API for managing blog posts, demonstrating core CRUD functionality with clean architecture and best practices.

🎯 Goals
This project is designed to help you:

Understand RESTful APIs, conventions, and best practices.

Learn how to implement CRUD operations using HTTP methods.

Master status codes and error handling in APIs.

Practice working with a relational database using Entity Framework Core.

Build a fully functional API with filtering capabilities.

Learn advanced backend patterns: IUnitOfWork, Specification Pattern, and Redis Caching.

Use tools like Ngrok to expose local APIs to the internet for testing.

✅ Features
Create, Read, Update, Delete blog posts

Search blog posts by title, content, or category

Associate tags with blog posts

Returns structured DTOs, consistent responses

Built using ASP.NET Core Web API

🧱 Technologies Used
ASP.NET Core Web API

Entity Framework Core

PostgreSQL

AutoMapper

Repository + Specification Pattern

IUnitOfWork Pattern

Redis Cache (via StackExchange.Redis)

Ngrok (for public tunneling)

🔧 Requirements
.NET 8.0 or higher

PostgreSQL

Visual Studio / VS Code

🌐 Using Ngrok for Public API Access
Ngrok allows you to securely expose your local development server (e.g., https://localhost:5001) to the internet via a public URL.

🔧 Setup Ngrok
Download Ngrok
https://ngrok.com/download

Install & Authenticate (first time only)

bash
Copy code
ngrok config add-authtoken YOUR_NGROK_AUTH_TOKEN
Start Ngrok for HTTPS port (e.g., 5001)

bash
Copy code
ngrok http https://localhost:5001
Copy the public URL (e.g., https://abc123.ngrok.io)
You can now:

Share it with mobile apps or teammates

Test webhooks or remote services

Access Swagger at https://abc123.ngrok.io/swagger

📦 API Endpoints
🔹 Create Blog Post
POST /posts

json
Copy code
{
  "title": "My First Blog Post",
  "content": "This is the content of my first blog post.",
  "category": "Technology",
  "tags": ["Tech", "Programming"]
}
✅ Response: 201 Created

🔹 Update Blog Post
PUT /posts/{id}

json
Copy code
{
  "title": "My Updated Blog Post",
  "content": "Updated content here.",
  "category": "Technology",
  "tags": ["Tech", "Programming"]
}
✅ Response: 200 OK
❌ 404 Not Found if post doesn't exist

🔹 Delete Blog Post
DELETE /posts/{id}

✅ Response: 204 No Content
❌ 404 Not Found if post doesn't exist

🔹 Get Blog Post By ID
GET /posts/{id}

✅ Response: 200 OK

json
Copy code
{
  "id": 1,
  "title": "My First Blog Post",
  "content": "This is the content of my first blog post.",
  "category": "Technology",
  "tags": ["Tech", "Programming"],
  "createdAt": "2021-09-01T12:00:00Z",
  "updatedAt": "2021-09-01T12:00:00Z"
}
🔹 Get All Blog Posts
GET /posts

✅ Response: 200 OK

json
Copy code
[
  {
    "id": 1,
    "title": "My First Blog Post",
    "content": "This is the content of my first blog post.",
    "category": "Technology",
    "tags": ["Tech", "Programming"],
    "createdAt": "2021-09-01T12:00:00Z",
    "updatedAt": "2021-09-01T12:00:00Z"
  },
  ...
]

🔹 Filter by Search Term
GET /posts?term=tech

Performs a case-insensitive search on:

title

content

category

tag name

✅ Response: Filtered list of matching posts

🧪 Validation & Error Handling
400 Bad Request for validation errors

404 Not Found for missing blog posts

500 Internal Server Error for unhandled exceptions (with middleware)

🚀 Getting Started
bash
Copy code
git clone https://github.com/yourusername/Blogging-Platform-API.git
cd Blogging-Platform-API
Configure settings in appsettings.json:

json
Copy code
"ConnectionStrings": {
  "DefaultConnection": "your-db-connection"
},
"Redis": {
  "ConnectionString": "localhost"
}
Run EF migrations:

bash
Copy code
dotnet ef database update
Run the API:

bash
Copy code
dotnet run
Expose using Ngrok:

bash
Copy code
ngrok http https://localhost:5001

Test endpoints using Postman or Swagger (/swagger)
