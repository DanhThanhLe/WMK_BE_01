#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WMK_BE_RecipesAndPlans_Controller/WMK_BE_RecipesAndPlans_Controller.csproj", "WMK_BE_RecipesAndPlans_Controller/"]
COPY ["WMK_BE_BusinessLogic/WMK_BE_BusinessLogic.csproj", "WMK_BE_BusinessLogic/"]
COPY ["WMK_BE_RecipesAndPlans_DataAccess/WMK_BE_RecipesAndPlans_DataAccess.csproj", "WMK_BE_RecipesAndPlans_DataAccess/"]
RUN dotnet restore "WMK_BE_RecipesAndPlans_Controller/WMK_BE_RecipesAndPlans_Controller.csproj"
COPY . .
WORKDIR "/src/WMK_BE_RecipesAndPlans_Controller"
RUN dotnet build "WMK_BE_RecipesAndPlans_Controller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WMK_BE_RecipesAndPlans_Controller.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WMK_BE_RecipesAndPlans_Controller.dll"]