## POTTENCIAL SEGURADORA TECHNICAL TEST
OBS: The test instructions are located [HERE](./TESTINSTRUCTIONS.md)

## PROJECT TECHNOLOGIES:
- ASP.NET Core 6
- XUnit
- OpenApi/Swagger
- PostgreSQL 14
- Docker
- Docker Compose

## BUILD & RUN
To run this api you are going to first build the database,
then either run it in development or production mode.

- Database:
  - **You Need** to have docker installed
  - RUN `docker compose up database --build`

DISCLAIMER: you can skip the database part
if you pretend to run the api in production mode

- Developer mode:
  - Open your favourite IDE and run it
  - OR
  - RUN `dotnet restore && dotnet run`

- Production mode (with docker compose):
  - **You Need** to have docker installed
  - RUN `docker compose up --build`

Ports that are going to be exposed by the container by default are:
- 5432 -> PostgreSQL
- 5182 -> Protocol: Http, Api: ECommerce

## DOCUMENTATION
Swagger is available at the root of `http://localhost:5182`, but
only when the project is executed in `Development mode`