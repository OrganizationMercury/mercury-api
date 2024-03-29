﻿FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY /src .
RUN dotnet restore "Api/Api.csproj"
RUN dotnet publish "Api/Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]
