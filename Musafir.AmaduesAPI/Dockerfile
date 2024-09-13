# download asp.net runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
WORKDIR /src
COPY Musafir.AmaduesAPI.csproj ./
RUN dotnet restore "Musafir.AmaduesAPI.csproj" 
COPY . .

# Add the environment variable to set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Include appsettings.Development.json in the image
COPY appsettings.Development.json /app/appsettings.Development.json

RUN dotnet build "Musafir.AmaduesAPI.csproj" -c Debug -o /app/build

#publish the project 
FROM build AS publish
RUN dotnet publish "Musafir.AmaduesAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false

#use base image as final 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Musafir.AmaduesAPI.dll"]

