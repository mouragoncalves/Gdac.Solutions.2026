FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/Auth/Gdac.Auth.Api/Gdac.Auth.Api.csproj", "Src/Auth/Gdac.Auth.Api/"]
COPY ["Src/Auth/Gdac.Auth.Application/Gdac.Auth.Application.csproj", "Src/Auth/Gdac.Auth.Application/"]
COPY ["Src/Auth/Gdac.Auth.Domain/Gdac.Auth.Domain.csproj", "Src/Auth/Gdac.Auth.Domain/"]
COPY ["Src/Auth/Gdac.Auth.Infrastructure/Gdac.Auth.Infrastructure.csproj", "Src/Auth/Gdac.Auth.Infrastructure/"]
COPY ["Src/Auth/Gdac.Auth.Shared/Gdac.Auth.Shared.csproj", "Src/Auth/Gdac.Auth.Shared/"]

RUN dotnet restore "Src/Auth/Gdac.Auth.Api/Gdac.Auth.Api.csproj"

COPY . .
RUN dotnet publish "Src/Auth/Gdac.Auth.Api/Gdac.Auth.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system --gid 1001 gdac && \
    useradd --system --uid 1001 --gid gdac --no-create-home gdac

COPY --from=build /app/publish .
RUN chown -R gdac:gdac /app

USER gdac
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Gdac.Auth.Api.dll"]
