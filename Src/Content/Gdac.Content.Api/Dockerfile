FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/Content/Gdac.Content.Api/Gdac.Content.Api.csproj", "Src/Content/Gdac.Content.Api/"]
COPY ["Src/Content/Gdac.Content.Application/Gdac.Content.Application.csproj", "Src/Content/Gdac.Content.Application/"]
COPY ["Src/Content/Gdac.Content.Domain/Gdac.Content.Domain.csproj", "Src/Content/Gdac.Content.Domain/"]
COPY ["Src/Content/Gdac.Content.Infrastructure/Gdac.Content.Infrastructure.csproj", "Src/Content/Gdac.Content.Infrastructure/"]

RUN dotnet restore "Src/Content/Gdac.Content.Api/Gdac.Content.Api.csproj"

COPY . .
RUN dotnet publish "Src/Content/Gdac.Content.Api/Gdac.Content.Api.csproj" \
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

ENTRYPOINT ["dotnet", "Gdac.Content.Api.dll"]
