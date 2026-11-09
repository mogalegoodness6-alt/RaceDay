# RaceDay MVC — Part 3 Starter

This is a starter MVC front end for the RaceDay Part 3 requirements. It is designed to consume the existing Part 2 REST API; it does **not** connect directly to SQL Server.

## Before running
1. Open `RaceDayMVC.csproj` in Visual Studio 2022.
2. Update `Api:BaseUrl` in `appsettings.json` to the address where your Part 2 API runs. Keep the trailing slash, e.g. `https://localhost:7001/`.
3. Check the API request/response shapes in Swagger and adjust `Services/RaceDayApiClient.cs` if your API uses different route names or JSON property names.
4. Run the MVC project. Register/login requires the matching API endpoints to be available.

## Run locally
```bash
dotnet restore
dotnet run
```

## Docker
Build:
```bash
docker build -t raceday-mvc .
```
Run (change the API URL to an address reachable from the container):
```bash
docker run --rm -p 8080:8080 -e Api__BaseUrl=http://host.docker.internal:5080/ raceday-mvc
```
The API URL above is an example only. Confirm your API's actual URL and whether HTTP or HTTPS is enabled.

## Important integration notes
- The MVC app uses session to remember the API token and role.
- All event/user/enrolment/result operations go through the Part 2 API.
- Image upload is sent to the API's event-image endpoint. Confirm or change that endpoint to match your API.
- Azure Blob Storage configuration belongs in the API project, not this MVC project.
- This starter cannot guarantee a complete end-to-end run until it is matched against the exact Part 2 API endpoints and DTOs.
