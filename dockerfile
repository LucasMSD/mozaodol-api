FROM mcr.microsoft.com/dotnet/sdk:8.0

COPY . /app
WORKDIR /app

RUN apt-get update && apt-get install -y tzdata

RUN dotnet restore Mozaodol.sln
RUN dotnet publish Mozaodol.sln -o out

WORKDIR ./out

ENTRYPOINT ["dotnet", "Mozaodol.Api.dll"]