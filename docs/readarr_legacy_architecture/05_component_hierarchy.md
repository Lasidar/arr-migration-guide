# Component Hierarchy

This document outlines the hierarchy of the main React components in Readarr's frontend application, detailing the structure and relationships between components.

## Application Root Structure

```mermaid
graph TD
    App["App (App.js)"]
    DocumentTitle["DocumentTitle"]
    Provider["Redux Provider"]
    Router["ConnectedRouter"]
    ApplyTheme["ApplyTheme"]
    Page["PageConnector"]
    AppRoutes["AppRoutes"]
    
    App --> DocumentTitle
    DocumentTitle --> Provider
    Provider --> Router
    Router --> ApplyTheme
    ApplyTheme --> Page
    Page --> AppRoutes
```

### App Root Components

**`App`**
- Props: 
  - `store`: Redux store (required)
  - `history`: Browser history object (required)
- Purpose: Root component that sets up the Redux provider, router, and theme

**`DocumentTitle`**
- Props: 
  - `title`: Window title string (Readarr instance name)
- Purpose: Sets the document title for the browser window

**`ApplyTheme`**
- Purpose: Applies the selected UI theme to the application

**`PageConnector`**
- Purpose: Connects page-level state and actions to the component tree

**`AppRoutes`**
- Props:
  - `app`: Reference to the App component
- Purpose: Defines all application routes and their corresponding components

## Primary Navigation Structure

```mermaid
graph TD
    AppRoutes["AppRoutes"]
    
    subgraph "Main Routes"
        AuthorIndex["AuthorIndexConnector (/)"]
        BookshelfConnector["/shelf"]
        BookIndexConnector["/books"]
        AuthorDetails["/author/:titleSlug"]
        BookDetails["/book/:titleSlug"]
        Search["/add/search"]
        UnmappedFiles["/unmapped"]
    end
    
    subgraph "Feature Routes"
        Calendar["/calendar"]
        Activity["Activity Routes"]
        Wanted["Wanted Routes"]
        Settings["Settings Routes"]
        System["System Routes"]
    end
    
    AppRoutes --> AuthorIndex
    AppRoutes --> BookshelfConnector
    AppRoutes --> BookIndexConnector
    AppRoutes --> AuthorDetails
    AppRoutes --> BookDetails
    AppRoutes --> Search
    AppRoutes --> UnmappedFiles
    AppRoutes --> Calendar
    AppRoutes --> Activity
    AppRoutes --> Wanted
    AppRoutes --> Settings
    AppRoutes --> System
    
    subgraph "Activity Routes"
        History["/activity/history"]
        Queue["/activity/queue"]
        Blocklist["/activity/blocklist"]
    end
    
    subgraph "Wanted Routes"
        Missing["/wanted/missing"]
        CutoffUnmet["/wanted/cutoffunmet"]
    end
    
    subgraph "Settings Routes"
        SettingsIndex["/settings"]
        MediaManagement["/settings/mediamanagement"]
        Profiles["/settings/profiles"]
        Quality["/settings/quality"]
        CustomFormats["/settings/customformats"]
        Indexers["/settings/indexers"]
        DownloadClients["/settings/downloadclients"]
        ImportLists["/settings/importlists"]
        Connect["/settings/connect"]
        Metadata["/settings/metadata"]
        Tags["/settings/tags"]
        General["/settings/general"]
        UI["/settings/ui"]
        Development["/settings/development"]
    end
    
    subgraph "System Routes"
        Status["/system/status"]
        Tasks["/system/tasks"]
        Backup["/system/backup"]
        Updates["/system/updates"]
        Events["/system/events"]
        LogFiles["/system/logs/files"]
        LogsTable["/system/logs/table"]
    end
```

## Major Component Descriptions

### Author Components

**`AuthorIndexConnector`**
- Props:
  - Various filtering and display options
- State:
  - List of authors
  - Selected filters
  - Sort options
- Purpose: Displays the main author list with filtering and sorting capabilities

**`AuthorDetailsPageConnector`**
- Props:
  - `titleSlug`: Author identifier from URL
- State:
  - Author details
  - Author's books
  - Author's history
- Purpose: Shows detailed information for a specific author and their books

### Book Components

**`BookIndexConnector`**
- Props:
  - Filtering and display options
- State:
  - List of books
  - Filter state
  - Sort options
- Purpose: Displays the complete book list with filtering and sorting options

**`BookDetailsPageConnector`**
- Props:
  - `titleSlug`: Book identifier from URL
- State:
  - Book details
  - Files
  - History
  - Available releases
- Purpose: Shows detailed information for a specific book

### Bookshelf Component

**`BookshelfConnector`**
- Props:
  - View options
- State:
  - Bookshelf data
  - View configuration
- Purpose: Provides a visual bookshelf-style view of the book collection

### Activity Components

**`HistoryConnector`**
- Purpose: Shows history of all download and import activities

**`QueueConnector`**
- Purpose: Displays current download queue with status and controls

**`BlocklistConnector`**
- Purpose: Shows blocked releases with reason for blocking

### Settings Components

**`Settings`**
- Purpose: Main settings navigation page

**`MediaManagementConnector`**
- Purpose: Configure media management settings like naming, file handling

**`ProfilesConnector`**
- Purpose: Manage quality and metadata profiles

**`QualityConnector`**
- Purpose: Configure quality definitions

**`IndexerSettingsConnector`**
- Purpose: Manage indexers for searching and RSS feeds

**`DownloadClientSettingsConnector`**
- Purpose: Configure download clients like SABnzbd, QBittorrent, etc.

## Reusable Components

### UI Components

**`Form`** components
- Input fields, checkboxes, dropdowns, etc.
- Form validation and submission handling

**`Table`** components
- Data display with sorting, pagination
- Row selection and actions

**`Modal`** components
- Dialog windows for confirmations, forms, and information
- Portal-based rendering for correct stacking context

**`Page`** components
- Page layouts, headers, and toolbars
- Loading and error states

### Feature Components

**`Filter`** components
- Input components for filtering data
- Filter presets and custom filter management

**`Link`** components
- Enhanced links with proper routing integration
- External and internal navigation handling

**`Loading`** components
- Loading indicators and spinners
- Placeholder content while data loads

**`Error`** components
- Error display and handling
- Retry mechanisms 