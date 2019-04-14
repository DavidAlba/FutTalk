FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Delivery.API/Delivery.API.csproj", "Delivery.API/"]
RUN dotnet restore "Delivery.API/Delivery.API.csproj"
COPY . .
WORKDIR "/src/Delivery.API"
RUN dotnet build "Delivery.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Delivery.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Delivery.API.dll"]