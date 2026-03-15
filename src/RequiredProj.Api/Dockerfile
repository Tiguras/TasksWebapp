FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/RequiredProj.Api/RequiredProj.Api.csproj", "src/RequiredProj.Api/"]
COPY ["src/RequiredProj.Core/RequiredProj.Core.csproj", "src/RequiredProj.Core/"]
RUN dotnet restore "src/RequiredProj.Api/RequiredProj.Api.csproj"
COPY src/ src/
WORKDIR "/src/src/RequiredProj.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
# Redundant as the base image already sets this
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "RequiredProj.Api.dll"]
