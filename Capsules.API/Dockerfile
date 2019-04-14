FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["../Capsules.API/Capsules.API.csproj", "../Capsules.API/"]
RUN dotnet restore "../Capsules.API/Capsules.API.csproj"
COPY . .
WORKDIR "/src/../Capsules.API"
RUN dotnet build "Capsules.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Capsules.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Capsules.API.dll"]