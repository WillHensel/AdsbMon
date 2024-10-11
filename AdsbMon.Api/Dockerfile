# Need the SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# This is the working directory INSIDE the image
WORKDIR /source

# Copy the solution
COPY ./*.sln .
# Copy the API and Core projects
COPY ./AdsbMon.Api ./AdsbMon.Api
COPY ./AdsbMon.Core ./AdsbMon.Core

# Restore
RUN dotnet restore AdsbMon.Api

WORKDIR /source/AdsbMon.Api
RUN dotnet publish --no-restore -o /app 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
EXPOSE 8080
WORKDIR /app
COPY --link --from=build /app .
ENTRYPOINT ["./AdsbMon.Api"]
