# Build and publish the app
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /Tripello.Server.Web

# Copy project files and restore as distinct layers
COPY src/Tripello.Server.Web/*.csproj ./
RUN dotnet restore ./

# Copy everything else and build
COPY src/Tripello.Server.Web ./
RUN dotnet publish -c Release -o out ./

WORKDIR /Tripello.Tool
COPY tool/Tripello.Tool/*.fsproj ./
RUN dotnet restore ./

COPY tool/Tripello.Tool ./
RUN dotnet publish -c Release -o out ./

#Build runtime docker image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as runtime-env

WORKDIR /tool
COPY --from=build-env /Tripello.Tool/out .

WORKDIR /app
COPY --from=build-env /Tripello.Server.Web/out .

EXPOSE 80
EXPOSE 443

ENTRYPOINT [ "dotnet", "Tripello.Server.Web.dll" ]