FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/TaskProject.Api/TaskProject.Api.csproj", "src/TaskProject.Api/"]
COPY ["src/TaskProject.Core/TaskProject.Core.csproj", "src/TaskProject.Core/"]
RUN dotnet restore "src/TaskProject.Api/TaskProject.Api.csproj"
COPY src/ src/
WORKDIR "/src/src/TaskProject.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
# Redundant as the base image already sets this
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TaskProject.Api.dll"]
