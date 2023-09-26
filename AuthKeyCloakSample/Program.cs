using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;

var builder = WebApplication.CreateBuilder(args);

var authenticationOptions = builder
	.Configuration
	.GetSection(KeycloakAuthenticationOptions.Section)
	.Get<KeycloakAuthenticationOptions>();

var authorizationOptions = builder
	.Configuration
	.GetSection(KeycloakProtectionClientOptions.Section)
	.Get<KeycloakProtectionClientOptions>();

var adminClientOptions = builder
	.Configuration
	.GetSection(KeycloakAdminClientOptions.Section)
	.Get<KeycloakAdminClientOptions>();

builder.Services.AddAuthorization();

builder.Services.AddKeycloakAuthentication(
	authenticationOptions ?? throw new ArgumentNullException(nameof(authenticationOptions)));

builder.Services.AddKeycloakAuthorization(
	authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions)));

builder.Services.AddKeycloakAdminHttpClient(
	adminClientOptions ?? throw new ArgumentNullException(nameof(adminClientOptions)));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.Run();