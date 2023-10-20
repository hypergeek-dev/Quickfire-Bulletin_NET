# Quickfire Bulletin

## Description
Quickfire Bulletin is a .NET-based web application that fetches news articles from various sources and archives them for easy access. The application uses the NewsData API to fetch the latest headlines and archives them under different categories.

## Features
- Fetch latest news articles from NewsData API
- Archive old articles for historical reference
- User authentication for personalized experience
- Cloudinary integration for media storage

## Prerequisites
- .NET SDK
- SQL Server
- Cloudinary Account

## Installation
1. Clone the repository:
    ```
    git clone https://github.com/yourusername/Quickfire-Bulletin.git
    ```
2. Navigate to the project directory:
    ```
    cd Quickfire-Bulletin
    ```
3. Install the required packages:
    ```
    dotnet restore
    ```
4. Update the `appsettings.json` file with your database and Cloudinary credentials.

## Usage
1. To run the application, execute:
    ```
    dotnet run
    ```
2. Open your web browser and navigate to `http://localhost:5000`.

## API Documentation
The application uses the NewsData API for fetching news articles. For more information, visit [NewsData API Documentation](https://newsdata.io/docs/getting-started).

## Contributing
If you'd like to contribute, please fork the repository and make changes as you'd like. Pull requests are warmly welcome.

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments
- NewsData API for providing the news data.
- Cloudinary for media storage solutions.

