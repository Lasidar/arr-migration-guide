# Frontend Folder Structure

This document provides an overview of the frontend directory structure of Readarr and explains the purpose of the major folders and files.

## Root Frontend Structure

```
/frontend
├── build/               # Webpack build configuration
├── src/                 # Source code
├── typings/             # TypeScript type definitions
├── .eslintignore        # ESLint ignore patterns
├── .eslintrc.js         # ESLint configuration
├── .prettierignore      # Prettier ignore patterns
├── .prettierrc.json     # Prettier configuration
├── .stylelintrc         # StyleLint configuration
├── .tern-project        # Tern JS completion configuration
├── babel.config.js      # Babel configuration
├── jsconfig.json        # JavaScript project configuration
├── postcss.config.js    # PostCSS configuration
├── tsconfig.json        # TypeScript configuration
```

## Frontend Source Code Structure (`/frontend/src`)

```
/frontend/src
├── Activity/            # Activity monitoring components
├── AddAuthor/           # Author addition screens
├── App/                 # Core application components
├── Author/              # Author management components
├── Book/                # Book management components
├── BookFile/            # Book file management components
├── Bookshelf/           # Bookshelf view components
├── Calendar/            # Calendar view components
├── Commands/            # Command handling
├── Components/          # Reusable UI components
├── Content/             # Static content and assets
├── Diag/                # Diagnostics components
├── FirstRun/            # First run setup wizard
├── Helpers/             # Helper utilities
├── InteractiveImport/   # Interactive import components
├── InteractiveSearch/   # Interactive search components
├── Organize/            # Organization components
├── Retag/               # Tag management components
├── Search/              # Search functionality
├── Settings/            # Application settings components
├── Shared/              # Shared components and utilities
├── Store/               # Redux store configuration
├── Styles/              # Global styles and CSS variables
├── System/              # System-related components
├── UnmappedFiles/       # Unmapped files management
├── Utilities/           # Utility functions
├── Wanted/              # Wanted/missing books components
├── bootstrap.tsx        # Application bootstrap
├── index.css            # Root CSS file
├── index.css.d.ts       # TypeScript definitions for CSS
├── index.ejs            # EJS template for HTML
├── index.ts             # Main entry point
├── login.html           # Login page
├── oauth.html           # OAuth authentication page
├── polyfills.js         # JavaScript polyfills
└── preload.js           # Preload script
```

## Major Folders Description

### Core Application Structure

**`/App`**
- Core application components including the main layout, navigation, and application initialization
- Contains the app root component that wraps the entire application

**`/Components`**
- Reusable UI components used throughout the application
- Examples include buttons, form elements, modal dialogs, tables, and other common interface elements

**`/Store`**
- Redux store configuration and setup
- Contains store creation, middleware configuration, and root reducer setup

**`/Styles`**
- Global CSS styles and variables
- Theme definitions and shared styling utilities

### Feature Modules

**`/Author`**
- Components for displaying and managing authors
- Author detail views, edit pages, and related functionality

**`/Book`**
- Book management components
- Book detail views, edit functionality, metadata management

**`/Bookshelf`**
- Main bookshelf view components
- Grid/list displays of books and organization tools

**`/Search`**
- Search functionality for finding books and authors
- Search results display and filtering components

**`/Settings`**
- Application settings and configuration components
- Various settings pages for different aspects of the application

### Utility and Helper Directories

**`/Helpers`**
- Helper functions and utilities
- Common operations used across different parts of the application

**`/Utilities`**
- General utility functions for tasks like string formatting, date handling, etc.
- Shared logic that doesn't fit into specific feature modules

**`/Shared`**
- Components and utilities shared across multiple feature modules
- Common patterns and reusable feature-specific elements

### Special Views

**`/Calendar`**
- Calendar view showing upcoming releases
- Timeline display of book releases

**`/Activity`**
- Activity monitoring and history components
- Displays download history, recent activity, and queue

**`/FirstRun`**
- First-run setup wizard components
- Initial configuration screens for new installations

## Entry Point Files

**`index.ts`**
- The main entry point for the application
- Initializes the React application and mounts it to the DOM

**`bootstrap.tsx`**
- Bootstraps the React application
- Sets up providers and initial configuration

**`index.ejs`**
- HTML template used by webpack to generate the index.html file
- Contains the root element where the React application mounts 