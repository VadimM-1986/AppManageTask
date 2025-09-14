FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppManageTasks/AppManageTasks.csproj", "AppManageTasks/"]
COPY ["AppManageTasksTests/AppManageTasksTests.csproj", "AppManageTasksTests/"]
RUN dotnet restore "./AppManageTasks/AppManageTasks.csproj"
COPY . .
WORKDIR "/src/AppManageTasks"
RUN dotnet build "./AppManageTasks.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppManageTasks.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "AppManageTasks.dll"]