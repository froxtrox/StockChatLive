# StockChatLive

StockChatLive is a real-time chat application for stock traders. It leverages ASP.NET Core SignalR for real-time web functionality, enabling users to communicate instantly. The application also integrates with external stock listing services to provide live stock prices within chat rooms.

## Features

- Real-time chat functionality using SignalR.
- Integration with stock listing services for live stock prices.
- Customizable chat rooms for different stock market interests.
- User authentication and authorization for secure access.

## Getting Started

### Prerequisites

- .NET 6.0 SDK
- Node.js (for SignalR client-side library management)
- Visual Studio 2019 or later, or VS Code

### Setup

1. Clone the repository to your local machine.
2. Navigate to the project directory and restore the .NET packages using `dotnet restore`
3. Trust the development HTTPS certificate (first-time setup):
   ```bash
   dotnet dev-certs https --trust
   ```
   This command generates and installs a self-signed SSL certificate for local HTTPS development, preventing browser security warnings.
4. Build the project using `dotnet run`

### Running the Application

To run the application, use the following command:
Open a web browser and navigate to http://localhost:5000 to view the application.

### Client-Side Development

Client-side scripts can be found in the wwwroot/js directory. SignalR functionality is demonstrated in the signalr directory, including how to establish connections and handle events.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request with your improvements.
