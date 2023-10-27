# Quickfire Bulletin .NET Web Application

## Table of Contents
- [Description](#description)
- [Technologies Used](#technologies-used)
- [Core Components](#core-components)
- [Features](#features)
- [User Stories](#user-stories)
- [Installation & Setup](#installation--setup)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

## Description
Quickfire Bulletin is a .NET-based web application that appears to be focused on news aggregation and management. It allows users to interact with news articles, add comments, and perform administrative tasks like seeding the database and deleting articles.

## Technologies Used
- .NET Core
- SQL Server
- Entity Framework
- ASP.NET Identity for authentication
- Possibly Stanford CoreNLP for natural language processing

## Core Components
1. **Controllers**
    - `AdminController`: Handles administrative tasks like seeding the database and deleting all articles.
    - `HomeController`: Manages the main functionalities like displaying articles, adding, editing, and deleting comments.

2. **Models**
    - `Comment`: Represents a comment made on a news article.
    - `Like`: Represents a like on a comment.
    - `NewsApiResponse`: Represents the response from an external news API.
    - `NewsArticle`: Represents a news article.

3. **Services**
    - `MyAppSettings`: Previously used for storing API keys, now uses a plugin for API key storage.
    - `NewsService`: Handles the business logic for news articles and comments.

## Features
1. **News Aggregation**: Fetches news articles possibly from an external API.
2. **Commenting**: Users can add, edit, and delete comments on news articles.
3. **Administration**: Admin users can seed the database with news articles and delete all articles.
4. **User Authentication**: Uses ASP.NET Identity for user authentication and role management.

## User Stories
1. **As a user, I want to view the latest news articles so that I can stay updated.**
2. **As a user, I want to comment on articles to share my opinions.**
3. **As an admin, I want to seed the database to populate it with fresh articles.**
4. **As an admin, I want to delete all articles to refresh the content.**


## Installation & Setup
1. Clone the repository.
2. Navigate to the project directory and restore the required packages.
3. Update `appsettings.json` with your SQL Server and other credentials.
4. Run the application.

## Usage
1. Run the application.
2. Navigate to the home page to view the list of articles.
3. Log in to add or edit comments.
4. Admin users can seed the database and delete all articles.

## Contributing
Feel free to fork the repository and submit pull requests.

## License
The project appears to be open-source, but the license is not specified.

## Acknowledgments
- The application might be using an external news API for fetching news articles.
- Stanford CoreNLP may be used for natural language processing tasks.
