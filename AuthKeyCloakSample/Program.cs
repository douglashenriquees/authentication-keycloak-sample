using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo()
	{
		Version = "v1",
		Title = "Auth With Keycloak Sample - API"
	});

	options.AddSecurityDefinition(
	"oauth",
	new OpenApiSecurityScheme
	{
		Flows = new OpenApiOAuthFlows
		{
			ClientCredentials = new OpenApiOAuthFlow
			{
				Scopes = new Dictionary<string, string>
				{
					["api"] = "api scope description"
				},
				TokenUrl = new Uri("http://localhost:9090/realms/realm-sample/protocol/openid-connect/token"),
			},
		},
		In = ParameterLocation.Header,
		Name = HeaderNames.Authorization,
		Type = SecuritySchemeType.OAuth2
	}
);
	options.AddSecurityRequirement(
		new OpenApiSecurityRequirement
		{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
								{ Type = ReferenceType.SecurityScheme, Id = "oauth" },
						},
						new[] { "api" }
					}
		}
	);
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.RoutePrefix = string.Empty;
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1.0");
});

app.MapGet("/hello-world", () => "Hello World!");

app.MapGet("/hello-world-auth", () => "Hello World!").RequireAuthorization();

app.Run();