FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj", "Src/Core/Gdac.Core.Api/"]
COPY ["Src/Core/Gdac.Core.Application/Gdac.Core.Application.csproj", "Src/Core/Gdac.Core.Application/"]
COPY ["Src/Core/Gdac.Core.Domain/Gdac.Core.Domain.csproj", "Src/Core/Gdac.Core.Domain/"]
COPY ["Src/Core/Gdac.Core.Infrastructure/Gdac.Core.Infrastructure.csproj", "Src/Core/Gdac.Core.Infrastructure/"]

RUN dotnet restore "Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj"

COPY . .
RUN dotnet publish "Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj" \
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

ENTRYPOINT ["dotnet", "Gdac.Core.Api.dll"]
