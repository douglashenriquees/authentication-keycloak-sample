## Running Container Keycloak

* docker network create -d bridge auth-sample
* docker run -p 9090:8080 --network=auth-sample --name keycloak-local -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:latest start-dev
* configure to increase token expiration time:
  * realm master
  * realm settings
  * access token lifespan: 30 minutes

## Creating the Project

* dotnet new gitignore
* dotnet new web -o AuthKeyCloakSample --no-https true
* cd AuthSample and execute the debug tool for create the assets
* dotnet add package IdentityModel.AspNetCore
* dotnet add package Keycloak.AuthServices.Authentication
* dotnet add package Keycloak.AuthServices.Authorization
* dotnet add package Refit.HttpClientFactory
* dotnet add package Swashbuckle.AspNetCore
