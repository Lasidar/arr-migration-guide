# TV Code Cleanup Plan

## Overview

This document outlines the plan for removing TV-specific code from the Readarr v2 codebase to create a clean, book-focused application.

## Current State

The codebase contains significant TV-specific code including:
- Episode-related classes and services
- TV-specific API endpoints
- TV-specific database migrations
- TV-specific parsers and search criteria
- Mixed domain models supporting both TV and books

## Cleanup Strategy

### Phase 1: API Layer Cleanup
Remove TV-specific API controllers and resources:
- [ ] Remove `/api/v1/episodes/*` endpoints
- [ ] Remove `/api/v1/calendar` (TV-specific)
- [ ] Remove `/api/v1/wanted/missing` (episodes)
- [ ] Remove `/api/v1/wanted/cutoff` (episodes)
- [ ] Remove episode-related resources

### Phase 2: Core Services Cleanup
Remove TV-specific services and repositories:
- [ ] Remove EpisodeService, EpisodeRepository
- [ ] Remove EpisodeMonitoredService
- [ ] Remove RefreshEpisodeService
- [ ] Remove TV-specific download/import services

### Phase 3: Parser Cleanup
Remove TV-specific parsing logic:
- [ ] Remove SingleEpisodeParser
- [ ] Remove MultiEpisodeParser
- [ ] Remove DailyEpisodeParser
- [ ] Remove AnimeEpisodeParser
- [ ] Keep only book-relevant parsing

### Phase 4: Database Cleanup
Handle TV-specific migrations and tables:
- [ ] Create migration to drop TV-specific tables
- [ ] Remove TV-specific migration files
- [ ] Update TableMapping to remove TV references

### Phase 5: Decision Engine Cleanup
Remove TV-specific specifications:
- [ ] Remove episode-specific download specifications
- [ ] Remove episode-specific import specifications
- [ ] Keep only book-relevant specifications

### Phase 6: Notification and Integration Cleanup
Update integrations for book focus:
- [ ] Remove episode-specific webhook payloads
- [ ] Update notification messages for books
- [ ] Remove TV-specific metadata providers

## Implementation Approach

1. **Create Feature Flags**: Temporarily disable TV features
2. **Remove API Layer**: Start with controllers to prevent access
3. **Remove Services**: Work from top-down through service layer
4. **Clean Core**: Remove repositories and domain models
5. **Update Tests**: Remove TV-specific tests
6. **Final Cleanup**: Remove any remaining references

## Risk Mitigation

- Create comprehensive backups before major deletions
- Test each phase thoroughly
- Keep changes in separate commits for easy rollback
- Document any shared code that needs refactoring

## Expected Outcome

- Clean, book-focused codebase
- Reduced maintenance burden
- Clearer architecture
- Better performance (less code to load)
- Easier onboarding for new developers