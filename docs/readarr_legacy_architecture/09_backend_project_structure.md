# Backend Project Structure

This document details the structure of Readarr's .NET solution, explaining the purpose and organization of each project.

## Solution Overview

Readarr is organized as a multi-project .NET solution with clear separation of concerns. The solution structure follows a modular design pattern where functionality is divided into specialized projects.

```
readarr.git/src/
├── Readarr.sln                # Main solution file
├── NzbDrone.Core/             # Core business logic (renamed to Readarr.Core)
├── Readarr.Api.V1/            # API endpoints
├── NzbDrone.Common/           # Shared utilities (renamed to Readarr.Common)
├── Readarr.Http/              # HTTP layer
├── NzbDrone.Host/             # Application host (renamed to Readarr.Host)
├── NzbDrone.SignalR/          # Real-time signaling (renamed to Readarr.SignalR)
├── NzbDrone/                  # Main application (renamed to Readarr)
├── NzbDrone.Console/          # Console runner (renamed to Readarr.Console)
├── NzbDrone.Windows/          # Windows-specific code (renamed to Readarr.Windows)
├── NzbDrone.Mono/             # Mono/.NET Core specific code (renamed to Readarr.Mono)
└── Various test projects      # Unit and integration tests
```

> Note: Many projects retain "NzbDrone" naming in the filesystem but are referenced as "Readarr" in the codebase and solution. This is because Readarr is a fork of Sonarr (formerly NzbDrone).

## Core Projects

### Readarr.Core (NzbDrone.Core)

The heart of the application containing the core business logic.

```
NzbDrone.Core/
├── Books/                # Book and edition management
├── Authors/              # Author management
├── Datastore/           # Database access and ORM
├── MediaFiles/          # Media file handling
├── Indexers/            # Indexer integrations
├── Download/            # Download client integrations
├── Parser/              # Media information parsing
├── Profiles/            # Quality profiles
├── Tags/                # Tagging system
├── Qualities/           # Quality definitions
├── Configuration/       # Application configuration
├── Authentication/      # User authentication
├── Messaging/           # Messaging bus and events
├── Jobs/                # Scheduled tasks
├── Lifecycle/           # Application lifecycle
├── MetadataSource/      # External metadata providers
├── CustomFormats/       # Custom format definitions
├── ImportLists/         # Import list functionality
├── HealthCheck/         # System health checks
└── Validation/          # Validation services
```

### Readarr.Api.V1 (Readarr.Api.V1)

Provides the RESTful API that serves as the interface between the frontend and the core services.

```
Readarr.Api.V1/
├── Author/              # Author API endpoints
├── Books/               # Book API endpoints
├── BookFiles/           # Book file API endpoints
├── Calendar/            # Calendar API endpoints
├── Commands/            # Command API endpoints
├── Config/              # Configuration API endpoints
├── DownloadClient/      # Download client API endpoints
├── Health/              # Health check API endpoints
├── History/             # History API endpoints
├── Indexers/            # Indexer API endpoints
├── Profiles/            # Profile API endpoints
├── Queue/               # Queue API endpoints
├── System/              # System API endpoints
├── Tags/                # Tag API endpoints
└── Wanted/              # Wanted/missing API endpoints
```

### Readarr.Http (Readarr.Http)

Handles HTTP processing including routing, authentication, and content negotiation.

```
Readarr.Http/
├── Authentication/      # Authentication middleware
├── ErrorManagement/     # Error handling
├── Extensions/          # HTTP extensions
├── Frontend/            # Frontend serving
├── Middleware/          # Custom middleware
├── REST/                # RESTful API utilities
└── Validation/          # Request validation
```

### Readarr.Common (NzbDrone.Common)

Shared utilities and infrastructure used throughout the application.

```
NzbDrone.Common/
├── Cache/               # Caching utilities
├── Cloud/               # Cloud service integrations
├── Composition/         # Dependency injection
├── Configuration/       # Configuration handling
├── Disk/                # Filesystem operations
├── EnvironmentInfo/     # Environment detection
├── EnsureThat/          # Precondition validation
├── Exceptions/          # Common exceptions
├── Extensions/          # Extension methods
├── Http/                # HTTP client utilities
├── Instrumentation/     # Logging and metrics
├── Processes/           # Process management
└── Serializer/          # Serialization utilities
```

### Readarr.Host (NzbDrone.Host)

Application hosting layer that bootstraps the application.

```
NzbDrone.Host/
├── AccessControl/       # Access control
├── ApplicationServer.cs # Application server
├── Bootstrap.cs         # Application bootstrap
├── RouterConfig.cs      # Routing configuration
└── Startup.cs           # ASP.NET Core startup
```

### Readarr.SignalR (NzbDrone.SignalR)

Real-time communication with the frontend.

```
NzbDrone.SignalR/
├── MessageHub.cs        # SignalR message hub
└── SignalRMiddleware.cs # SignalR middleware
```

## Platform-Specific Projects

### Readarr.Windows (NzbDrone.Windows)

Windows-specific implementation details.

```
NzbDrone.Windows/
├── Disk/               # Windows disk utilities
├── EnvironmentInfo/    # Windows environment info
├── FileSystem/         # Windows filesystem access
├── Installer.cs        # Windows installer
└── Service/            # Windows service integration
```

### Readarr.Mono (NzbDrone.Mono)

Mono/.NET Core implementation for Linux and macOS.

```
NzbDrone.Mono/
├── Disk/               # Linux/Mac disk utilities
├── EnvironmentInfo/    # Linux/Mac environment info
└── FileSystem/         # Linux/Mac filesystem access
```

### Readarr.Console (NzbDrone.Console)

Console application entry point.

```
NzbDrone.Console/
├── ConsoleApp.cs       # Console application
└── Program.cs          # Entry point
```

### Readarr (NzbDrone)

Main application entry point with platform detection.

```
NzbDrone/
└── Program.cs          # Main entry point
```

## Test Projects

### Readarr.Core.Test (NzbDrone.Core.Test)

Unit tests for core business logic.

```
NzbDrone.Core.Test/
├── BookTests/          # Book tests
├── AuthorTests/        # Author tests
├── IndexerTests/       # Indexer tests
├── DownloadClientTests/# Download client tests
└── ParserTests/        # Parser tests
```

### Readarr.Api.Test (NzbDrone.Api.Test)

Tests for the API endpoints.

```
NzbDrone.Api.Test/
├── AuthorTests/        # Author API tests
├── BookTests/          # Book API tests
└── CommandTests/       # Command API tests
```

### Readarr.Integration.Test (NzbDrone.Integration.Test)

End-to-end integration tests.

```
NzbDrone.Integration.Test/
├── ApiTests/           # API integration tests
└── ClientTests/        # Client integration tests
```

### Readarr.Test.Common (NzbDrone.Test.Common)

Common test utilities and fixtures.

```
NzbDrone.Test.Common/
├── AutoMoq/            # AutoMoq testing utilities
├── Builders/           # Test data builders
├── Categories/         # Test categories
└── TestBase.cs         # Base test class
```

## Project Dependencies

The project dependencies follow a clear hierarchical structure:

- **Readarr** depends on the platform-specific projects
- **Platform-specific** projects depend on Readarr.Host
- **Readarr.Host** depends on Readarr.Core, Readarr.Api.V1, Readarr.Http, and Readarr.SignalR
- **Readarr.Api.V1** depends on Readarr.Core
- **Readarr.Http** depends on Readarr.Core
- **Readarr.SignalR** depends on Readarr.Core
- **Most projects** depend on Readarr.Common

This dependency structure enforces a clean separation of concerns, with the core business logic isolated from platform-specific details and user interface concerns.

## Build Infrastructure

Readarr uses a combination of MSBuild properties and targets for building:

```
src/
├── Directory.Build.props       # Common build properties
├── Directory.Build.targets     # Common build targets
└── Directory.Packages.props    # Central package management
```

These files ensure consistent build configuration across all projects in the solution, including:

- Target frameworks (.NET 6.0)
- Common package versions
- Compiler settings
- Output paths
- Debug/release configurations

## Database Schema Management

Database schema migrations are managed in the core project:

```
NzbDrone.Core/Datastore/Migration/
├── Framework/                # Migration framework
└── [version]_[description].cs # Individual migrations
```

Migrations are executed sequentially during application startup to ensure the database schema matches the expected version. 