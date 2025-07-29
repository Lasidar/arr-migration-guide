# Readarr Architecture Documentation

This directory contains comprehensive documentation about the architecture of Readarr, an open-source book collection manager.

## Document Structure

### Phase 1: High-Level Architecture Overview

1. [System Context Diagram](01_system_context_diagram.md) - High-level view of Readarr and its interactions with external systems and users
2. [Big Picture Narrative](02_big_picture_narrative.md) - Overview of Readarr's purpose, functionality, and technology choices
3. [Container Diagram](03_container_diagram.md) - Breakdown of Readarr's major components and their interactions

### Phase 2: Frontend Documentation

4. [Frontend Folder Structure](04_frontend_folder_structure.md) - Organization of the React/Redux frontend codebase
5. [Component Hierarchy](05_component_hierarchy.md) - Hierarchy and documentation of React components
6. [State Management](06_state_management.md) - Redux state management patterns and data flow
7. [API Interaction](07_api_interaction.md) - How the frontend communicates with the backend API

### Phase 3: Backend Documentation

8. [Backend Microservice Overview](08_backend_microservice_overview.md) - Overview of Readarr's backend services
9. [Backend Project Structure](09_backend_project_structure.md) - Organization of the .NET Core backend codebase
10. [API Endpoints](10_api_endpoints.md) - Documentation of available API endpoints
11. [Data Models and Database](11_data_models_database.md) - Entity relationships and database schema
12. [Business Logic](12_business_logic.md) - Key workflows and business logic implementation
13. [Inter-Service Communication](13_inter_service_communication.md) - How services communicate with each other

### Phase 4: Cross-Cutting Concerns

14. [Authentication and Authorization](14_authentication_authorization.md) - Security model and user authentication flows
15. [Logging and Monitoring](15_logging_monitoring.md) - Logging strategy and application monitoring
16. [Deployment and Infrastructure](16_deployment_infrastructure.md) - Deployment options and CI/CD pipeline

## How to Use This Documentation

- **New Developers**: Start with the High-Level Architecture Overview to understand the big picture, then dive into specific areas of interest.
- **Frontend Developers**: Focus on documents 4-7 to understand the frontend architecture.
- **Backend Developers**: Focus on documents 8-13 to understand the backend architecture.
- **DevOps Engineers**: Documents 15-16 will be most relevant for deployment and infrastructure concerns.

## Diagrams

Most documents include Mermaid diagrams to visualize architectural concepts. These diagrams are rendered automatically in GitHub and other Markdown viewers that support Mermaid.

## Contributing

This documentation is maintained alongside the codebase. If you find inaccuracies or want to improve the documentation, please submit a pull request following the same process as code contributions.
