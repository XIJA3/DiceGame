Project Documentation
Overview
This project consists of two main components:

ASP.NET Core Web API (Server): This serves as the backend of the application, designed using Clean Architecture principles. It provides endpoints and functionalities necessary for the application, ensuring a clear separation of concerns and maintainability.

Blazor WebAssembly (Client): This is the frontend of the application, providing a rich user interface to interact with the backend services. It leverages Blazor WebAssembly to run C# code directly in the browser.

The project implements a gaming application with real-time features, user authentication, leaderboard functionality, and more.

Server - ASP.NET Core Web API
Architecture
The server is designed using Clean Architecture, which ensures that the system is organized into distinct layers with well-defined responsibilities. The key components are:

Core Layer: Contains the business logic, entities, and interfaces. It is independent of any external frameworks or technologies.

Application Layer: Implements application-specific logic and uses the interfaces defined in the Core layer. It handles tasks such as processing requests and interacting with the Core layer.

Infrastructure Layer: Provides implementations for interfaces defined in the Core and Application layers. This includes data access, third-party integrations, and other external dependencies.

Presentation Layer: Manages the web API endpoints. It is responsible for handling HTTP requests and responses and delegates work to the Application layer.

Key Services and Components
AuthService: Manages user authentication, session handling, and token management. It interacts with local storage and the server API to handle user sessions and authentication tokens.

BaseHubService: Handles the SignalR Hub connection, providing methods to connect, disconnect, and manage the SignalR hub interactions. It ensures the proper setup of the SignalR connection and maintains its state.

GameHubService: Extends BaseHubService and adds specific methods related to the game's functionality, such as joining and leaving rooms, processing turns, and handling game events. It also manages event registration for various game-related notifications.

LeaderboardServiceClient: Interacts with the leaderboard API to retrieve top players by score. It uses HttpClient to make API calls and handle responses.

Key Endpoints
Login: /api/Authorization/LogIn

Handles user authentication and returns a session token.
GameHub Methods:

JoinRoomAsync(): Joins a game room.
LeaveRoomAsync(long sessionId): Leaves a game room.
RematchAsync(long sessionId): Requests a rematch.
CancelRematchAsync(long sessionId): Cancels a rematch request.
ReadyAsync(): Marks the player as ready.
NotReadyAsync(): Marks the player as not ready.
ProccessTurnAsync(ProccessTurnRequest request): Processes a turn in the game.
Middleware and Dependency Injection
Custom Middleware: Manages HTTP requests and responses, adding necessary headers and handling authentication tokens.

Dependency Injection: Uses ASP.NET Core's DI container to inject services and dependencies into controllers and other components.

Client - Blazor WebAssembly
Features
User Authentication: Manages user login and session handling using AuthService. Ensures that users are authenticated and authorized to access protected resources.

Real-Time Communication: Utilizes SignalR through BaseHubService and GameHubService to provide real-time updates and interactions within the game. Handles events such as room status updates and game results.

Leaderboard Display: Fetches and displays the leaderboard using LeaderboardServiceClient, which retrieves top players' scores from the server.

Components
AuthService: Manages user authentication and session state on the client side. Interacts with the server API to perform login, logout, and session management.

BaseHubService: Provides methods to connect to and interact with the SignalR hub. Manages the connection state and ensures proper setup.

GameHubService: Extends BaseHubService to include game-specific functionalities. Registers event handlers for game-related events and provides methods to interact with the game hub.

LeaderboardServiceClient: Fetches leaderboard data from the server and handles the display of top players in the client application.

Real-Time Features
SignalR Integration: Utilizes SignalR for real-time communication, allowing for seamless interaction and updates within the game. Events are registered and handled using methods in GameHubService.
Setup and Configuration
Server
Configuration: Ensure that ServerBaseUri and other necessary configuration values are set in the appsettings.json file or environment variables.

Database: Configure the database connection and ensure that the database schema is set up according to the application's requirements.

SignalR: Set up SignalR hubs and ensure that they are properly configured to handle client connections.

Client
Configuration: Set the HubBaseURI in the appsettings.json file or environment variables to point to the server's SignalR hub URL.

Dependencies: Ensure that all necessary Blazor and .NET dependencies are included in the project.

Error Handling and Logging
Error Handling: Both server and client components have mechanisms to handle exceptions and errors. HTTP requests handle exceptions related to network issues, and SignalR connections handle errors related to real-time communication.

Logging: The project uses logging to track errors and important events. Ensure that logging is properly configured to capture and store logs for both the server and client components.

Conclusion
This project provides a robust framework for a gaming application with real-time features and user management. By adhering to Clean Architecture principles, the server-side implementation ensures maintainability and scalability. The client-side Blazor WebAssembly application delivers a rich and interactive user experience, leveraging SignalR for real-time updates and communication.
