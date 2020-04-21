# Build and publish the app
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy project files and restore as distinct layers
COPY src/Tripello.Server.Web/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/Tripello.Server.Web ./
RUN dotnet publish -c Release -o out

#Build runtime docker image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as runtime-env
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
EXPOSE 443

ENTRYPOINT [ "dotnet", "Tripello.Server.Web.dll" ]