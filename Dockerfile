#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.sln .
COPY JamOrder.API/JamOrder.API.csproj JamOrder.API/

RUN dotnet restore
COPY . .

# publish=
FROM build AS publish
WORKDIR /src/JamOrder.API
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
RUN curl -SL "https://github.com/rdvojmoc/DinkToPdf/raw/v1.0.8/v0.12.4/64%20bit/libwkhtmltox.so" --output /app/libwkhtmltox.so

RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev

# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet JamOrder.API.dll
