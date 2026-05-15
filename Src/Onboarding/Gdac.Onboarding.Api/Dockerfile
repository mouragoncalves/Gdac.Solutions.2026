FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/Onboarding/Gdac.Onboarding.Api/Gdac.Onboarding.Api.csproj", "Src/Onboarding/Gdac.Onboarding.Api/"]
COPY ["Src/Onboarding/Gdac.Onboarding.Application/Gdac.Onboarding.Application.csproj", "Src/Onboarding/Gdac.Onboarding.Application/"]
COPY ["Src/Onboarding/Gdac.Onboarding.Domain/Gdac.Onboarding.Domain.csproj", "Src/Onboarding/Gdac.Onboarding.Domain/"]
COPY ["Src/Onboarding/Gdac.Onboarding.Infrastructure/Gdac.Onboarding.Infrastructure.csproj", "Src/Onboarding/Gdac.Onboarding.Infrastructure/"]

RUN dotnet restore "Src/Onboarding/Gdac.Onboarding.Api/Gdac.Onboarding.Api.csproj"

COPY . .
RUN dotnet publish "Src/Onboarding/Gdac.Onboarding.Api/Gdac.Onboarding.Api.csproj" \
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

ENTRYPOINT ["dotnet", "Gdac.Onboarding.Api.dll"]
