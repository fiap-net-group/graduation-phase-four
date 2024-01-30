# Graduation's Phase Four Project
[![Build Status](https://dev.azure.com/fiap-net-group/FIAP/_apis/build/status%2Fgraduation-phase-four?branchName=main)](https://dev.azure.com/fiap-net-group/FIAP/_build/latest?definitionId=5&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=coverage)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=bugs)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=fiap-net-group_graduation-phase-four&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=fiap-net-group_graduation-phase-four)

Post-graduation second phase project

## The project
The project is about a notification system, with an event-driven architecture used with api and workers

## Build and Test
```bash
# [IMPORTANT]:
# You need to have .NET 6 installed in your computer
# You also need to be at the same folder as the solution file (.sln)

# First, restore the dependencies
dotnet restore

# After, build the application
dotnet build

# If want to execute the application tests
dotnet test
```

## Running the project
Follow these steps to run the project locally.
```bash
# [IMPORTANT]:
# You need to have Docker installed in your computer
	
# Execute all APIs and workers for production
docker-compose -f docker/docker-compose.yml up --build 

# Execute an instance of RabbitMq and all APIs and workers for staging
	docker-compose -f docker/docker-compose-staging.yml up --build

# Execute the instance of RabbitMq for development tests
docker-compose -f docker/docker-compose-develop.yml up --build
```

## Help links
| Link | Description |
|------|-------------|
| [Web-Queue-Worker architecture style](https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/web-queue-worker) | Explains the client-worker relation
| [Worker services in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers?pivots=dotnet-6-0) | Structure for workers
| [Email sending flow](/docs/send-email-flow.png) | The drawing of the email sending flow 

## Wiki
Access our [Wiki](https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/26/Graduation-Phase-Four) for more information!