FROM mcr.microsoft.com/dotnet/sdk:8.0

COPY . /app
WORKDIR /app

RUN apt-get update && apt-get install -y tzdata

RUN dotnet restore ProjetoTelegram.sln
RUN dotnet publish ProjetoTelegram.sln -o out

WORKDIR ./out

ENTRYPOINT ["dotnet", "ProjetoTelegram.dll"]