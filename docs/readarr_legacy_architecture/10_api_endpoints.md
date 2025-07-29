# API Endpoints

This document provides information about Readarr's API endpoints, their purposes, and data formats.

## API Overview

Readarr exposes a comprehensive RESTful API that serves as the interface between the frontend and backend. All API endpoints follow the versioning pattern `/api/v1/[resource]` and use standard HTTP verbs (GET, POST, PUT, DELETE) for operations.

The API is documented using OpenAPI (Swagger) and is available at `/api/v1/openapi.json` when the application is running in debug mode.

## Authentication

API requests require authentication using one of these methods:

1. **API Key**: Include an `X-Api-Key` header with a valid API key
2. **Cookie Authentication**: Using a session cookie from a previous login
3. **Form Authentication**: For the login endpoint

## Common API Patterns

### Resource Endpoints

Most resources follow standard REST conventions:

- `GET /api/v1/[resource]` - Get all resources of a type
- `GET /api/v1/[resource]/{id}` - Get a specific resource
- `POST /api/v1/[resource]` - Create a new resource
- `PUT /api/v1/[resource]/{id}` - Update a resource
- `DELETE /api/v1/[resource]/{id}` - Delete a resource

### Query Parameters

Common query parameters for collection endpoints:

- `page`: Page number for pagination
- `pageSize`: Number of items per page
- `sortKey`: Field to sort by
- `sortDirection`: "ascending" or "descending"
- `filter`: Filter expression

## Core API Resources

### Author Resources

```
/api/v1/author
```

**GET /api/v1/author**  
Returns a list of all authors.

```json
[
  {
    "id": 1,
    "authorName": "J.K. Rowling",
    "sortName": "Rowling, J.K.",
    "status": "continuing",
    "overview": "Biography...",
    "path": "/books/J.K. Rowling",
    "monitored": true,
    "lastInfoSync": "2023-01-15T12:00:00Z",
    "foreignAuthorId": "8282843",
    "titleSlug": "j-k-rowling"
  }
]
```

**GET /api/v1/author/{id}**  
Returns a specific author.

**POST /api/v1/author**  
Adds a new author.

**PUT /api/v1/author/{id}**  
Updates an author.

**DELETE /api/v1/author/{id}**  
Removes an author.

### Book Resources

```
/api/v1/book
```

**GET /api/v1/book**  
Returns a list of all books.

```json
[
  {
    "id": 1,
    "title": "Harry Potter and the Philosopher's Stone",
    "authorId": 1,
    "authorName": "J.K. Rowling",
    "releaseDate": "1997-06-26T00:00:00Z",
    "foreignBookId": "2838844",
    "monitored": true
  }
]
```

**GET /api/v1/book/{id}**  
Returns a specific book.

**POST /api/v1/book**  
Adds a new book.

**PUT /api/v1/book/{id}**  
Updates a book.

**DELETE /api/v1/book/{id}**  
Removes a book.

### BookFile Resources

```
/api/v1/bookfile
```

**GET /api/v1/bookfile**  
Returns a list of all book files.

```json
[
  {
    "id": 1,
    "path": "/books/J.K. Rowling/Harry Potter and the Philosopher's Stone.epub",
    "size": 2048000,
    "dateAdded": "2023-01-15T12:00:00Z",
    "quality": {
      "quality": {
        "id": 3,
        "name": "EPUB"
      }
    },
    "bookId": 1
  }
]
```

### Calendar Resources

```
/api/v1/calendar
```

**GET /api/v1/calendar**  
Returns upcoming book releases.

Query parameters:
- `start`: Start date (ISO8601)
- `end`: End date (ISO8601)
- `includeAuthor`: Include author details (boolean)

```json
[
  {
    "id": 1,
    "title": "The Next Book",
    "authorId": 1,
    "authorName": "J.K. Rowling",
    "releaseDate": "2023-06-26T00:00:00Z"
  }
]
```

### Command Resources

```
/api/v1/command
```

**POST /api/v1/command**  
Executes a command.

Commands include:
- `RefreshAuthor`
- `RefreshBook`
- `BookSearch`
- `AuthorSearch`
- `RssSync`

```json
{
  "name": "RefreshAuthor",
  "authorId": 1
}
```

**GET /api/v1/command**  
Returns a list of active and completed commands.

**GET /api/v1/command/{id}**  
Gets the status of a specific command.

### DownloadClient Resources

```
/api/v1/downloadclient
```

**GET /api/v1/downloadclient**  
Returns a list of configured download clients.

```json
[
  {
    "id": 1,
    "name": "SABnzbd",
    "enable": true,
    "protocol": "usenet",
    "host": "localhost",
    "port": 8080,
    "category": "books"
  }
]
```

**GET /api/v1/downloadclient/schema**  
Returns the schema for available download clients.

### History Resources

```
/api/v1/history
```

**GET /api/v1/history**  
Returns the history of download and import activity.

```json
{
  "page": 1,
  "pageSize": 10,
  "sortDirection": "descending",
  "sortKey": "date",
  "records": [
    {
      "id": 1,
      "bookId": 1,
      "authorId": 1,
      "date": "2023-01-15T12:00:00Z",
      "eventType": "grabbed",
      "data": {}
    }
  ]
}
```

### Indexer Resources

```
/api/v1/indexer
```

**GET /api/v1/indexer**  
Returns a list of configured indexers.

```json
[
  {
    "id": 1,
    "name": "NZBGeek",
    "enable": true,
    "protocol": "usenet",
    "fields": [
      {
        "name": "apiKey",
        "value": "******************"
      }
    ]
  }
]
```

### Profile Resources

```
/api/v1/qualityprofile
```

**GET /api/v1/qualityprofile**  
Returns a list of quality profiles.

```json
[
  {
    "id": 1,
    "name": "eBook",
    "upgradeAllowed": true,
    "cutoff": {
      "id": 3,
      "name": "EPUB"
    },
    "items": [
      {
        "quality": {
          "id": 2,
          "name": "PDF"
        },
        "allowed": true
      },
      {
        "quality": {
          "id": 3,
          "name": "EPUB"
        },
        "allowed": true
      }
    ]
  }
]
```

### Queue Resources

```
/api/v1/queue
```

**GET /api/v1/queue**  
Returns the current download queue.

```json
{
  "page": 1,
  "pageSize": 10,
  "sortDirection": "ascending",
  "sortKey": "timeleft",
  "records": [
    {
      "id": 1,
      "bookId": 1,
      "authorId": 1,
      "title": "Harry Potter and the Philosopher's Stone",
      "size": 2048000,
      "sizeleft": 1024000,
      "status": "downloading",
      "trackedDownloadStatus": "ok",
      "estimatedCompletionTime": "2023-01-15T12:30:00Z",
      "protocol": "usenet"
    }
  ]
}
```

### System Resources

#### Status

```
/api/v1/system/status
```

**GET /api/v1/system/status**  
Returns application status information.

```json
{
  "version": "0.1.1.652",
  "buildTime": "2023-01-15T08:30:00Z",
  "isDebug": false,
  "isProduction": true,
  "isAdmin": true,
  "isUserInteractive": true,
  "startupPath": "/app/readarr",
  "appData": "/config",
  "osName": "ubuntu",
  "osVersion": "20.04",
  "isNetCore": true,
  "isMono": false,
  "isLinux": true,
  "isOsx": false,
  "isWindows": false,
  "mode": "console",
  "branch": "develop",
  "authentication": "forms",
  "sqliteVersion": "3.31.1",
  "migrationVersion": 169,
  "urlBase": "",
  "runtimeVersion": "6.0.14"
}
```

#### Backup

```
/api/v1/system/backup
```

**GET /api/v1/system/backup**  
Returns a list of available backups.

```json
[
  {
    "id": 1,
    "name": "readarr_backup_2023-01-15_12-00-00.zip",
    "path": "/app/readarr/Backups/readarr_backup_2023-01-15_12-00-00.zip",
    "type": "scheduled",
    "time": "2023-01-15T12:00:00Z"
  }
]
```

**POST /api/v1/system/backup**  
Creates a new backup.

#### Tasks

```
/api/v1/system/task
```

**GET /api/v1/system/task**  
Returns a list of scheduled tasks.

```json
[
  {
    "id": 1,
    "name": "Check Health",
    "interval": 43200,
    "lastExecution": "2023-01-15T06:00:00Z",
    "lastDuration": "00:00:05",
    "nextExecution": "2023-01-15T18:00:00Z"
  }
]
```

### Tag Resources

```
/api/v1/tag
```

**GET /api/v1/tag**  
Returns a list of all tags.

```json
[
  {
    "id": 1,
    "label": "fiction"
  },
  {
    "id": 2,
    "label": "non-fiction"
  }
]
```

### Wanted Resources

#### Missing

```
/api/v1/wanted/missing
```

**GET /api/v1/wanted/missing**  
Returns a list of missing/wanted books.

```json
{
  "page": 1,
  "pageSize": 10,
  "sortDirection": "descending",
  "sortKey": "releaseDate",
  "records": [
    {
      "id": 1,
      "bookId": 1,
      "authorId": 1,
      "title": "Harry Potter and the Philosopher's Stone",
      "releaseDate": "1997-06-26T00:00:00Z"
    }
  ]
}
```

#### Cutoff Unmet

```
/api/v1/wanted/cutoff
```

**GET /api/v1/wanted/cutoff**  
Returns books that have been downloaded but don't meet the quality cutoff.

```json
{
  "page": 1,
  "pageSize": 10,
  "sortDirection": "descending",
  "sortKey": "releaseDate",
  "records": [
    {
      "id": 1,
      "bookId": 1,
      "authorId": 1,
      "title": "Harry Potter and the Philosopher's Stone",
      "quality": {
        "quality": {
          "id": 2,
          "name": "PDF"
        }
      },
      "cutoffQuality": {
        "id": 3,
        "name": "EPUB"
      }
    }
  ]
}
```

## API Implementation Details

API controllers extend core controller classes:

1. **RestController**: Base controller for RESTful endpoints
2. **ResourceController**: For CRUD operations on resources
3. **ProviderControllerBase**: For provider-type resources (indexers, download clients)

```csharp
public class AuthorController : RestController<AuthorResource>
{
    // Implementation details
}
```

Controllers use attributes for:
- Route definition
- Authentication requirements
- CORS policy
- Response types

```csharp
[V1ApiController]
[Route("author")]
public class AuthorController : RestController<AuthorResource>
{
    [HttpGet]
    public List<AuthorResource> GetAll() { ... }
    
    [HttpGet("{id:int}")]
    public AuthorResource Get(int id) { ... }
    
    // Other endpoints
}
```

## Error Handling

API endpoints return standard HTTP status codes:

- 200 OK: Successful operation
- 201 Created: Resource created
- 400 Bad Request: Invalid input
- 401 Unauthorized: Authentication failure
- 403 Forbidden: Insufficient permissions
- 404 Not Found: Resource not found
- 500 Internal Server Error: Server-side error

Error responses include a standardized structure:

```json
{
  "message": "Error message",
  "description": "Detailed error description",
  "errorCode": 1000
}
```

## API Clients

The API is designed to be consumed by:

1. The React frontend (primary client)
2. Third-party automation tools
3. API clients in various programming languages
4. Command line tools 