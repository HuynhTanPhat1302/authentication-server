# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
COPY Authentication-Authorization-1.0/*.csproj .
RUN dotnet restore
WORKDIR /src
COPY Authentication-Authorization-1.0 .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Authentication-Authorization-1.0.dll"]