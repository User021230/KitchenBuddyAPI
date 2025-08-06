# 👩🏿‍🍳👩🏿‍🍳 KitchenBuddyAPI

-A RESTful API that leverages AI to simulate a professional chef and nutritionist and tailor’s responses based on the provided ingredients

## 🚀 Features

- ✅ CRUD operations for recipes
- ✅ Ingredient and category management
- ✅ RESTful API architecture
- ✅ Entity Framework Core + SQLite (or SQL Server)
- ✅ JWT Authentication 
- ✅ ASP.NET Core Web API
- ✅ Recipe Recommandations

## 📦 Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- C#
- SQLite / SQL Server
- Swagger / Swashbuckle
- LINQ
- DeepSeek AI API
- PostMan (API Documentation and Testing)

# EndPoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/auth/login` | User login with username/email and password |
| `POST` | `/signUp/signUp` | Register new user account |

### Recipe Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/recipes/generate` | Generate recipes from ingredients using AI |
| `GET` | `/api/recipes` | Get all recipes |
| `GET` | `/api/recipes/{id}` | Get recipe by ID |
| `POST` | `/api/recipes` | Create new recipe |
| `PUT` | `/api/recipes/{id}` | Update entire recipe |
| `PATCH` | `/api/recipes/{id}` | Partially update recipe |
| `DELETE` | `/api/recipes/{id}` | Delete recipe |

# 🤖 DeepSeek Integration
The API uses structured prompts to ensure consistent, high-quality recipe generation:

## Prompt Engineering
The system uses carefully crafted prompts that include:

Context about being a professional chef
Instructions for recipe structure and format
Guidelines for nutritional information
Specifications for ingredient quantities and cooking times

## Response Processing

Validates OpenAI responses against expected schema
Handles API rate limits and errors gracefully
Implements retry logic with exponential backoff
Sanitizes and validates generated content

## Token Management

Optimizes prompts to minimize token usage
Implements token counting and budget tracking
Provides fallback responses for token limit errors
