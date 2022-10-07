FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Build

COPY *.sln ./
COPY ./Src ./Src

RUN dotnet restore
RUN dotnet test