FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DevTalles.Ecommerce.WebAPI.csproj", "./"]
RUN dotnet restore "./DevTalles.Ecommerce.WebAPI.csproj"

COPY . .
RUN dotnet publish "./DevTalles.Ecommerce.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DevTalles.Ecommerce.WebAPI.dll"]

