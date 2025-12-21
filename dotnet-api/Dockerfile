# Multi-stage Dockerfile for a .NET 10 Web API
# Builds with the SDK image and runs with the smaller ASP.NET runtime image.
# Place this file at /home/andreas/Code/locking.springboot.postgres/dotnet-api/Dockerfile

ARG DOTNET_VERSION=10.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# copy everything and restore (simple, works for single-project repo)
COPY . .
RUN dotnet restore

# publish optimized for container
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
# copy published output from build stage
COPY --from=build /app/publish .

EXPOSE 80

# run the first DLL found in the publish folder (works when project produces a single dll)
ENTRYPOINT ["dotnet", "dotnet-api.dll"]