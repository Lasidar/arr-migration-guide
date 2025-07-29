# Master Plan: Re-forking Sonarr to Readarr v2

## Overview
This master plan provides detailed, step-by-step instructions for creating Readarr v2 directly from Sonarr, incorporating all lessons learned from the Lidarr and original Readarr implementations. Each task is broken down into small, actionable steps that a senior developer can execute independently.

## Prerequisites
- Access to Sonarr source code (latest stable branch)
- .NET 6+ development environment
- Node.js 16+ for frontend development
- Git and GitHub account
- Understanding of C# and React/TypeScript

## Reference Documents
- [Migration Patterns Catalog](migration-patterns.md) - For implementation patterns
- [Domain Model Evolution](domain-model-evolution.md) - For entity design
- [Architecture Comparison](architecture-comparison.md) - For architectural decisions
- [Lessons Learned](lessons-learned.md) - For best practices

## Timeline Estimate
- Total Duration: 12-16 weeks
- Team Size: 1-3 senior developers
- Phases can be parallelized after Phase 2

---

## Phase 0: Project Setup and Planning (Week 1)

### Task 0.1: Fork and Initial Setup
**Duration**: 2 hours

1. Fork Sonarr repository to new organization/account
   ```bash
   git clone https://github.com/Sonarr/Sonarr.git readarr-v2
   cd readarr-v2
   git remote rename origin sonarr-upstream
   git remote add origin https://github.com/[your-org]/readarr-v2.git
   ```

2. Create initial branch structure
   ```bash
   git checkout -b develop
   git checkout -b feature/initial-readarr-transformation
   ```

3. Update repository metadata
   - Edit `README.md` to reflect Readarr v2
   - Update `LICENSE` if needed
   - Create `.github/CODEOWNERS` file

### Task 0.2: Development Environment Setup
**Duration**: 2 hours

1. Install required tools
   - Visual Studio 2022 or JetBrains Rider
   - VS Code for frontend work
   - Docker Desktop (optional but recommended)
   - SQLite browser tool

2. Verify build works
   ```bash
   dotnet restore
   dotnet build
   cd frontend && npm install
   npm run build
   ```

3. Set up local development database
   - Copy Sonarr's development database
   - Create backup for reference

### Task 0.3: Create Architecture Decision Records
**Duration**: 1 hour

1. Create `docs/adr` directory
2. Create `001-direct-fork-approach.md`
   ```markdown
   # ADR-001: Direct Fork from Sonarr to Readarr v2
   
   ## Status: Accepted
   
   ## Context
   Forking directly from Sonarr to Readarr v2, skipping Lidarr intermediary.
   
   ## Decision
   - Implement metadata separation pattern from the start
   - Use single API version (v1)
   - Implement lazy loading immediately
   - Design for book series relationships
   
   ## Consequences
   - Cleaner codebase from start
   - Faster development
   - Less technical debt
   ```

### Task 0.4: Set Up Project Management
**Duration**: 1 hour

1. Create GitHub Project board with columns:
   - Backlog
   - In Progress
   - Review
   - Done

2. Create issue templates:
   - Bug report
   - Feature request
   - Task template

3. Create initial milestones:
   - Phase 1: Core Transformation
   - Phase 2: Domain Models
   - Phase 3: Metadata Integration
   - Phase 4: API Layer
   - Phase 5: UI Transformation
   - Phase 6: Testing & Polish

### Task 0.5: Global Search and Replace Preparation
**Duration**: 2 hours

1. Create transformation scripts in `scripts/transform/`:
   
   `scripts/transform/rename-projects.ps1`:
   ```powershell
   # PowerShell script for Windows
   $files = Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse
   foreach ($file in $files) {
       $content = Get-Content $file.FullName
       $content = $content -replace "Sonarr", "Readarr"
       Set-Content -Path $file.FullName -Value $content
   }
   ```

   `scripts/transform/rename-projects.sh`:
   ```bash
   #!/bin/bash
   # Bash script for Linux/Mac
   find src -name "*.csproj" -type f -exec sed -i 's/Sonarr/Readarr/g' {} \;
   ```

2. Create namespace mapping document:
   ```
   Sonarr.Core.Tv -> Readarr.Core.Books
   Sonarr.Api.V3 -> Readarr.Api.V1
   Sonarr.Http -> Readarr.Http
   ```

3. Document files that need manual review:
   - Configuration files
   - Database migrations
   - API endpoints
   - UI components

### Task 0.6: Remove Unnecessary Components
**Duration**: 3 hours

1. Remove Sonarr.Api.V5 project entirely
   ```bash
   rm -rf src/Sonarr.Api.V5
   ```

2. Remove from solution file
   ```bash
   dotnet sln remove src/Sonarr.Api.V5/Sonarr.Api.V5.csproj
   ```

3. Update any references in other projects

4. Remove Sonarr.RuntimePatches if present
   ```bash
   rm -rf src/Sonarr.RuntimePatches
   ```

### Task 0.7: Initial Git Commit
**Duration**: 30 minutes

1. Stage all changes
   ```bash
   git add -A
   ```

2. Create detailed commit message
   ```
   Initial Readarr v2 transformation setup
   
   - Forked from Sonarr [commit-hash]
   - Removed unnecessary projects (Api.V5, RuntimePatches)
   - Added transformation scripts
   - Created ADR structure
   - Set up project management
   ```

3. Push to repository
   ```bash
   git commit -m "Initial Readarr v2 transformation setup"
   git push origin feature/initial-readarr-transformation
   ```

## Checkpoint 0
- [ ] Repository forked and accessible
- [ ] Development environment builds successfully
- [ ] Unnecessary projects removed
- [ ] Project structure documented
- [ ] Team can run application locally

---