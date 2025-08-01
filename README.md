# KitchenBuddyAPI
This API allows to generate, store and edit Recipes

## 🚀 Features

- ✅ CRUD operations for recipes
- ✅ Ingredient and category management
- ✅ RESTful API architecture
- ✅ Entity Framework Core + SQLite (or SQL Server)
- ✅ JWT Authentication 
- ✅ ASP.NET Core Web API

## 📦 Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- C#
- SQLite / SQL Server
- Swagger / Swashbuckle
- LINQ
- OpenAI API

# EndPoints

# 🤖 OpenAI Integration
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
