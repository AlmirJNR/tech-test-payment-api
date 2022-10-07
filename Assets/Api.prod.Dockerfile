FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Build

COPY *.sln ./
COPY ./Src ./Src

RUN dotnet restore
RUN dotnet publish -c Release -o Outdir

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /Api

COPY --from=build /Build/Outdir .

ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]