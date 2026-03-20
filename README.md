# Task Project

## Overview

This is a simple .NET 10, Angular 21 application, with a PostgreSQL database. It can be run 
in containers using Docker with volumes to persist the database data.

The application is a simple task tracker with a simple UI. The user is able to create a new task with a due date, title and description.
They can then drag and drop the task between status columns to update its progress.

## Prerequisites

- [Docker](https://www.docker.com/) (for running via containers)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for local backend development)
- [Node.js](https://nodejs.org/) and [pnpm](https://pnpm.io/) (for local frontend development)

## Running the Application

### Docker (Recommended)

From the root of the project, where the docker-compose is, the application should startup with `docker compose up` starting 3 containers: the backend, frontend, and database.

- Frontend: `http://localhost:4200`
- API: `http://localhost:5130/scalar/v1`

### Local Development

**Backend**

Start the database first, then run the API from the root of the project:
```bash
docker compose up -d db
dotnet run --project src/TaskProject.Api
```

**Frontend**

From the `frontend/` directory:
```bash
pnpm install
pnpm start
```


## Running Tests

Tests can be run from and IDE such as Visual Studio or Rider, or from the command line using `dotnet test`.

## API Endpoints

If running locally in development mode, the API can be seen via Scalar at `http://localhost:5130/scalar/v1`, 
or the raw OpenAPI spec at `http://localhost:5130/openapi/v1.json`.

All endpoints are under `/api/tasks`.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get a task by ID |
| POST | `/api/tasks` | Create a new task |
| PUT | `/api/tasks/{id}` | Update a task |
| PATCH | `/api/tasks/{id}/status` | Update a task's status |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Future Additions

- Setup SignalR so that tasks can be created by calling the API directly and UI can react and remain updated.
- Add detailed logging
- Add Redis to reduce database load
- Add a linter