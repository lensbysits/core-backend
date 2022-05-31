#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["libs/framework/Services/Masterdata/Services.Mastedata.Web/Services.Mastedata.Web.csproj", "libs/framework/Services/Masterdata/Services.Mastedata.Web/"]
RUN dotnet restore "libs/framework/Services/Masterdata/Services.Mastedata.Web/Services.Mastedata.Web.csproj"
COPY . .
WORKDIR "/src/libs/framework/Services/Masterdata/Services.Mastedata.Web"
RUN dotnet build "Services.Mastedata.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services.Mastedata.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lens.Services.Mastedata.Web.dll"]