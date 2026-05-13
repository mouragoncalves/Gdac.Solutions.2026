FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/Gdac.Auth.Api/Gdac.Auth.Api.csproj", "Src/Gdac.Auth.Api/"]
COPY ["Src/Gdac.Auth.Application/Gdac.Auth.Application.csproj", "Src/Gdac.Auth.Application/"]
COPY ["Src/Gdac.Auth.Domain/Gdac.Auth.Domain.csproj", "Src/Gdac.Auth.Domain/"]
COPY ["Src/Gdac.Auth.Infrastructure/Gdac.Auth.Infrastructure.csproj", "Src/Gdac.Auth.Infrastructure/"]
COPY ["Src/Gdac.Auth.Shared/Gdac.Auth.Shared.csproj", "Src/Gdac.Auth.Shared/"]

RUN dotnet restore "Src/Gdac.Auth.Api/Gdac.Auth.Api.csproj"

COPY . .
RUN dotnet publish "Src/Gdac.Auth.Api/Gdac.Auth.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN addgroup --system --gid 1001 gdac && \
    adduser --system --uid 1001 --ingroup gdac gdac

COPY --from=build /app/publish .
RUN chown -R gdac:gdac /app

USER gdac
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Gdac.Auth.Api.dll"]
