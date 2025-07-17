FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CFS-1507.Application/CFS-1507.Application.csproj", "CFS-1507.Application/"]
COPY ["CFS-1507.Contract/CFS-1507.Contract.csproj", "CFS-1507.Contract/"]
COPY ["CFS-1507.Controller/CFS-1507.Controller.csproj", "CFS-1507.Controller/"]
COPY ["CFS-1507.Domain/CFS-1507.Domain.csproj", "CFS-1507.Domain/"]
COPY ["CFS-1507.Infrastructure/CFS-1507.Infrastructure.csproj", "CFS-1507.Infrastructure/"]

RUN dotnet restore "CFS-1507.Controller/CFS-1507.Controller.csproj"

COPY . .
WORKDIR "/src/CFS-1507.Controller"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

COPY CFS-1507.Controller/appsettings.json /app/appsettings.json
COPY CFS-1507.Controller/appsettings.Development.json /app/appsettings.Development.json

ENTRYPOINT ["dotnet", "CFS-1507.Controller.dll"]
EXPOSE 5555
