# Authentication and Authorization

This document describes the authentication and authorization mechanisms used in Readarr.

## Authentication Overview

Readarr supports multiple authentication methods to secure access to the application:

1. **Forms Authentication** - Username and password-based login
2. **Basic Authentication** - HTTP Basic Authentication for API access
3. **No Authentication** - Optional mode for private/secure environments

The authentication method is configurable through the application settings.

```mermaid
classDiagram
    class AuthenticationType {
        <<enum>>
        None
        Basic
        Forms
    }
    
    class IAuthenticationService {
        <<interface>>
        +LogUnauthorized(HttpRequest context)
        +Login(HttpRequest request, string username, string password) User
        +Logout(HttpContext context)
    }
    
    class AuthenticationService {
        -IUserService _userService
        -AuthenticationType AUTH_METHOD
        +Login(HttpRequest request, string username, string password) User
        +Logout(HttpContext context)
    }
    
    class User {
        +Guid Identifier
        +string Username
        +string Password
    }
    
    class IUserService {
        <<interface>>
        +Add(string username, string password) User
        +Update(User user) User
        +FindUser(string username, string password) User
    }
    
    class UserService {
        -IUserRepository _repo
        +FindUser(string username, string password) User
    }
    
    IAuthenticationService <|.. AuthenticationService
    AuthenticationService --> AuthenticationType : uses
    AuthenticationService --> IUserService : uses
    IUserService <|.. UserService
    UserService --> User : manages
```

## Authentication Flow

### Forms Authentication Flow

```mermaid
sequenceDiagram
    participant User
    participant AuthenticationController
    participant AuthenticationService
    participant UserService
    participant Database
    
    User->>AuthenticationController: POST /login (username, password)
    AuthenticationController->>AuthenticationService: Login(request, username, password)
    AuthenticationService->>UserService: FindUser(username, password)
    UserService->>Database: Query user by username
    Database-->>UserService: Return user record
    UserService->>UserService: Verify password hash
    UserService-->>AuthenticationService: Return user or null
    AuthenticationService-->>AuthenticationController: Return user or null
    
    alt User authenticated
        AuthenticationController->>AuthenticationController: Create claims principal
        AuthenticationController->>AuthenticationController: Sign in user
        AuthenticationController-->>User: Redirect to home page
    else Authentication failed
        AuthenticationController-->>User: Redirect to login page with error
    end
```

### Basic Authentication Flow

```mermaid
sequenceDiagram
    participant Client
    participant BasicAuthHandler
    participant AuthenticationService
    participant UserService
    
    Client->>BasicAuthHandler: Request with Authorization header
    BasicAuthHandler->>BasicAuthHandler: Extract credentials
    BasicAuthHandler->>AuthenticationService: Login(request, username, password)
    AuthenticationService->>UserService: FindUser(username, password)
    UserService-->>AuthenticationService: Return user or null
    AuthenticationService-->>BasicAuthHandler: Return user or null
    
    alt User authenticated
        BasicAuthHandler->>BasicAuthHandler: Create authentication ticket
        BasicAuthHandler-->>Client: Allow request
    else Authentication failed
        BasicAuthHandler-->>Client: 401 Unauthorized
    end
```

## Password Security

User passwords are stored securely using SHA-256 hashing. The `UserService` handles password verification during login:

```csharp
// Password verification logic
if (user.Password == password.SHA256Hash())
{
    return user;
}
```

## Authorization

Readarr implements a flexible authorization system that can be configured based on the deployment environment.

### Authorization Policies

The application uses policy-based authorization with the following options:

1. **Always Required** - Authentication is always required
2. **Disabled for Local Addresses** - Local network requests bypass authentication
3. **Disabled** - Authentication is disabled completely (not recommended for public deployments)

```mermaid
classDiagram
    class AuthenticationRequiredType {
        <<enum>>
        Enabled
        DisabledForLocalAddresses
        Disabled
    }
    
    class UiAuthorizationPolicyProvider {
        -IConfigFileProvider _config
        +GetPolicyAsync(string policyName) Task~AuthorizationPolicy~
    }
    
    class UiAuthorizationHandler {
        -IConfigFileProvider _configService
        -AuthenticationRequiredType _authenticationRequired
        +HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement) Task
    }
    
    UiAuthorizationPolicyProvider --> AuthenticationRequiredType : uses
    UiAuthorizationHandler --> AuthenticationRequiredType : uses
```

### Local Network Detection

For the "Disabled for Local Addresses" option, Readarr checks if the request comes from a local IP address:

```mermaid
sequenceDiagram
    participant Client
    participant UiAuthorizationHandler
    
    Client->>UiAuthorizationHandler: Request
    
    alt AuthenticationRequired == DisabledForLocalAddresses
        UiAuthorizationHandler->>UiAuthorizationHandler: Check client IP
        
        alt Is local IP address
            UiAuthorizationHandler-->>Client: Allow without authentication
        else Is remote IP address
            UiAuthorizationHandler-->>Client: Require authentication
        end
    else AuthenticationRequired == Enabled
        UiAuthorizationHandler-->>Client: Require authentication
    else AuthenticationRequired == Disabled
        UiAuthorizationHandler-->>Client: Allow without authentication
    end
```

## API Authentication

API endpoints can be accessed using either:

1. API keys for automated tools and integrations
2. Basic authentication for user-based API access
3. Cookie-based authentication for browser sessions

## Security Considerations

1. **Default Configuration**: By default, authentication is enabled for all connections
2. **Local Network Trust**: The option to bypass authentication for local addresses assumes the local network is trusted
3. **HTTPS**: For production deployments, it's recommended to use HTTPS to encrypt credentials during transmission
4. **API Keys**: API keys should be treated as sensitive information and not shared publicly 