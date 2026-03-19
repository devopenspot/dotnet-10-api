FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GameStore.Api/GameStore.Api.csproj", "GameStore.Api/"]
RUN dotnet restore "GameStore.Api/GameStore.Api.csproj"

COPY GameStore.Api/ GameStore.Api/
WORKDIR /src/GameStore.Api
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM build AS test
WORKDIR /src
COPY ["GameStore.Api.test/GameStore.Api.test.csproj", "GameStore.Api.test/"]
RUN dotnet restore "GameStore.Api.test/GameStore.Api.test.csproj"
COPY GameStore.Api.test/ GameStore.Api.test/
RUN dotnet test "GameStore.Api.test/GameStore.Api.test.csproj" --no-build

FROM build AS publish
WORKDIR /src/GameStore.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GameStore.Api.dll"]
