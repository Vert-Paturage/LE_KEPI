# Étape 1 : Utiliser l'image SDK .NET pour la phase de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier uniquement le fichier .csproj du projet API
COPY src/Middleware.API/Middleware.API.csproj ./Middleware.API/

# Restaurer les dépendances pour le projet API
RUN dotnet restore ./Middleware.API/Middleware.API.csproj

# Copier le reste des fichiers du projet API
COPY src/Middleware.API/. ./Middleware.API/

# Publier le projet API en mode Release
WORKDIR /src/Middleware.API
RUN dotnet publish -c Release -o /app/out

# Étape 2 : Utiliser l'image Runtime pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copier les fichiers publiés depuis l'étape de build
COPY --from=build /app/out .

# Exposer le port sur lequel l'application écoute
EXPOSE 8001

# Démarrer l'application
ENTRYPOINT ["dotnet", "Middleware.API.dll"]
