FROM postgres:alpine

COPY ./InitDatabase.sql /docker-entrypoint-initdb.d
