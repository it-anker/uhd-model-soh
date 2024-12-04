# OGC Process API for MARS Simulations

This ASP.NET Core application provides an OGC-compliant API to execute MARS simulations synchronously and asynchronously, and to retrieve the results. The application uses **Redis** to store jobs, processes, and results. Configuration data (e.g., Redis connection details) is defined in the `appsettings.json` file.

## Features

- **Synchronous and asynchronous** execution of MARS simulations.  
- Storage of jobs and results in **Redis**.  
- Provision of **Swagger documentation** at the local endpoint: `http://localhost:7002/swagger`.

## Prerequisites

- Installed **Docker** and **Docker Compose**.  
- A **Redis** container or running instance.  
- **.NET 9** SDK (for local development).

## Running the Application

The application can be started either via a Docker container or directly locally.

## Docker Compose

By using the `docker-compose.yml` file, all required services are created and the service is build, if necessary:

```bash
docker compose up
```

To start the service not in the Docker but in the host, the services are still required:

```bash
dotnet run --environment Development --project SOH.Process.Server/SOH.Process.Server.csproj
```

After starting the redis databse, you can run the service directly:

```bash
dotnet run --environment Development --project SOH.Proces.
```

### Docker Build

Create the single Docker image for the service, use the following command:

```bash
docker build -t mars-ogc-api -f ./SOH.Process.Server/Dockerfile .
```

### Starting the Docker Container

Start the application with:

```bash
docker run -p 7002:7002 --env-file .env mars-ogc-api
```

> Note: Adjust the environment variables in the `.env` file or the `docker run` command as needed.

### Accessing Swagger

After starting the application, you can access the API documentation at the Swagger endpoint:

```
https://localhost:7002/swagger
```

## Configuration

Redis connection details and other environment variables are managed in the `appsettings.json` file. Example:

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Endpoints

The API provides the following main endpoints according to the specification ([OGC API Processes Specification](https://docs.ogc.org/is/18-062r2/18-062r2.html#toc0)):

- **/**: Retrieve the landing page.
- **/processes**: Manage and list available processes. Execute processes.
- **/jobs**: Create, query status, and retrieve results of asynchronous jobs.
- **/conformances**: Retrieve the server's conformance declarations.

## Development

Run the application locally using the .NET CLI:

```bash
dotnet run --urls=http://localhost:7002
```

For testing, the system uses `testcontainers` and ad-hoc start an own redis or other required services in background.

```bash
dotnet test
```