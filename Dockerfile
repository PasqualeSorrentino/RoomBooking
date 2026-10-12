# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY RoomBooking.slnx ./
COPY RoomBooking.Api/RoomBooking.Api.csproj RoomBooking.Api/
COPY RoomBooking.Domain/RoomBooking.Domain.csproj RoomBooking.Domain/
COPY RoomBooking.Infrastructure/RoomBooking.Infrastructure.csproj RoomBooking.Infrastructure/

RUN dotnet restore RoomBooking.Api/RoomBooking.Api.csproj

COPY . .

RUN dotnet publish RoomBooking.Api/RoomBooking.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RoomBooking.Api.dll"]