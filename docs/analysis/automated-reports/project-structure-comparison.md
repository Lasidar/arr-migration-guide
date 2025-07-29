# Project Structure Comparison

## Overview
This document compares the project structures of Sonarr, Lidarr, and Readarr to identify how the architecture evolved through the migration process.

## Project Count Analysis

- **Sonarr**: 27 projects
- **Lidarr**: 25 projects
- **Readarr**: 25 projects

## Core Projects Comparison

### API Projects Evolution

**Sonarr:**
- `Sonarr.Api.V3` - Version 3 API
- `Sonarr.Api.V5` - Version 5 API
- `Sonarr.Http` - HTTP infrastructure

**Lidarr:**
- `Lidarr.Api.V1` - Single API version
- `Lidarr.Http` - HTTP infrastructure

**Readarr:**
- `Readarr.Api.V1` - Single API version
- `Readarr.Http` - HTTP infrastructure

### Key Differences:
1. Sonarr maintains two API versions (V3 and V5) for backward compatibility
2. Lidarr and Readarr simplified to a single API version (V1)
3. All projects maintain the NzbDrone namespace for core components

## Namespace Evolution

### Core Namespaces:
- **Sonarr**: `NzbDrone.Core.Tv`
- **Lidarr**: `NzbDrone.Core.Music`
- **Readarr**: `NzbDrone.Core.Books`

### Project Naming Pattern:
All projects follow a consistent pattern where:
- Core projects retain `NzbDrone.*` folder structure
- Project files are renamed to `{AppName}.*`
- Example: `sonarr/src/NzbDrone.Core/Sonarr.Core.csproj`

## Removed Projects

### Sonarr → Lidarr:
- `Sonarr.RuntimePatches` - Runtime patching functionality removed

### Lidarr → Readarr:
- No projects removed

## Project Categories

### 1. Core Business Logic
- `NzbDrone.Core` - Main business logic and domain models
- Contains media-specific subdirectories (Tv/Music/Books)

### 2. API Layer
- Media-specific API projects
- HTTP infrastructure project

### 3. Host/Service Layer
- `NzbDrone.Host` - Application hosting
- `NzbDrone.Console` - Console runner
- `NzbDrone` - Main executable

### 4. Platform-Specific
- `NzbDrone.Mono` - Linux/Mac specific code
- `NzbDrone.Windows` - Windows specific code

### 5. Common/Shared
- `NzbDrone.Common` - Shared utilities
- `NzbDrone.SignalR` - Real-time communication

### 6. Testing Projects
- `*.Test` - Unit tests
- `*.Integration.Test` - Integration tests
- `*.Automation.Test` - UI automation tests
- `NzbDrone.Test.Common` - Shared test utilities
- `NzbDrone.Test.Dummy` - Test dummy application

### 7. Service Helpers
- `ServiceInstall` - Windows service installation
- `ServiceUninstall` - Windows service uninstallation

### 8. Update Infrastructure
- `NzbDrone.Update` - Self-update functionality

## Architecture Insights

1. **Consistent Core Structure**: All three projects maintain the same core architecture
2. **API Simplification**: Migration from Sonarr to Lidarr involved consolidating API versions
3. **Namespace Isolation**: Each project created its own namespace for domain models while keeping shared infrastructure
4. **Test Coverage**: All projects maintain comprehensive test coverage with identical test project structure
5. **Platform Support**: Cross-platform support maintained across all projects
6. **Update Mechanism**: Self-update functionality preserved across all forks