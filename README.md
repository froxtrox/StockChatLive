# StockChatLive

StockChatLive is a real-time chat application for stock traders. It leverages ASP.NET Core SignalR for real-time web functionality, enabling users to communicate instantly. The application also integrates with external stock listing services to provide live stock prices within chat rooms.

## Features

- Real-time chat functionality using SignalR.
- Integration with stock listing services for live stock prices.
- Customizable chat rooms for different stock market interests.
- User authentication and authorization for secure access.

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Node.js (for SignalR client-side library management)
- Visual Studio 2026 or later, or VS Code

### Setup

1. Clone the repository to your local machine.
2. Navigate to the project directory and restore the .NET packages using `dotnet restore`
3. **Configure application settings:**
   ```bash
   # Copy the template files to create your local configuration
   cp StockChatLive/appsettings.template.json StockChatLive/appsettings.json
   cp StockChatLive/appsettings.Development.template.json StockChatLive/appsettings.Development.json
   ```
   Edit the `appsettings.json` file to customize your local settings (ports, logging levels, etc.). Your changes will not be tracked by git.
4. Trust the development HTTPS certificate (first-time setup):
   ```bash
   dotnet dev-certs https --trust
   ```
   This command generates and installs a self-signed SSL certificate for local HTTPS development, preventing browser security warnings.
5. Build the project using `dotnet run`

### Running the Application

To run the application, use the following command:
Open a web browser and navigate to http://localhost:5000 to view the application.

### Client-Side Development

Client-side scripts can be found in the wwwroot/js directory. SignalR functionality is demonstrated in the signalr directory, including how to establish connections and handle events.

## Configuration

This project uses a template-based configuration pattern. Configuration files follow this structure:

- `appsettings.template.json` - **Committed to git** - Contains default settings and serves as documentation
- `appsettings.json` - **Gitignored** - Your local configuration (created from template)
- `appsettings.Development.template.json` - **Committed to git** - Development environment defaults
- `appsettings.Development.json` - **Gitignored** - Your local development settings

Your local changes to `appsettings.json` and `appsettings.Development.json` will never be tracked by git, allowing you to customize settings without affecting version control.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request with your improvements.

## SignalR Message Size Limits

SignalR message buffers are limited to 64 KB to protect against large message
attacks and excessive memory usage.

These limits can be adjusted in `Program.cs` if needed.
