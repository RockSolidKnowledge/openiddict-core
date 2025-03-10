﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace OpenIddict.Validation;

/// <summary>
/// Provides various settings needed to configure the OpenIddict validation handler.
/// </summary>
public sealed class OpenIddictValidationOptions
{
    /// <summary>
    /// Gets the list of encryption credentials used by the OpenIddict validation services.
    /// Note: the encryption credentials are not used to protect/unprotect tokens issued
    /// by ASP.NET Core Data Protection, that uses its own key ring, configured separately.
    /// </summary>
    /// <remarks>
    /// Note: OpenIddict automatically sorts the credentials based on the following algorithm:
    /// <list type="bullet">
    ///   <item><description>Symmetric keys are always preferred when they can be used for the operation (e.g token encryption).</description></item>
    ///   <item><description>X.509 keys are always preferred to non-X.509 asymmetric keys.</description></item>
    ///   <item><description>X.509 keys with the furthest expiration date are preferred.</description></item>
    ///   <item><description>X.509 keys whose backing certificate is not yet valid are never preferred.</description></item>
    /// </list>
    /// </remarks>
    public List<EncryptingCredentials> EncryptionCredentials { get; } = [];

    /// <summary>
    /// Gets the list of signing credentials used by the OpenIddict validation services.
    /// Multiple credentials can be added to support key rollover, but if X.509 keys
    /// are used, at least one of them must have a valid creation/expiration date.
    /// Note: the signing credentials are not used to protect/unprotect tokens issued
    /// by ASP.NET Core Data Protection, that uses its own key ring, configured separately.
    /// </summary>
    /// <remarks>
    /// Note: OpenIddict automatically sorts the credentials based on the following algorithm:
    /// <list type="bullet">
    ///   <item><description>Symmetric keys are always preferred when they can be used for the operation (e.g token signing).</description></item>
    ///   <item><description>X.509 keys are always preferred to non-X.509 asymmetric keys.</description></item>
    ///   <item><description>X.509 keys with the furthest expiration date are preferred.</description></item>
    ///   <item><description>X.509 keys whose backing certificate is not yet valid are never preferred.</description></item>
    /// </list>
    /// </remarks>
    public List<SigningCredentials> SigningCredentials { get; } = [];

    /// <summary>
    /// Gets or sets the period of time client assertions remain valid after being issued. The default value is 5 minutes.
    /// While not recommended, this property can be set to <see langword="null"/> to issue client assertions that never expire.
    /// </summary>
    public TimeSpan? ClientAssertionLifetime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the JWT handler used to protect and unprotect tokens.
    /// </summary>
    public JsonWebTokenHandler JsonWebTokenHandler { get; set; } = new()
    {
        SetDefaultTimesOnTokenCreation = false
    };

    /// <summary>
    /// Gets the list of the handlers responsible for processing the OpenIddict validation operations.
    /// Note: the list is automatically sorted based on the order assigned to each handler descriptor.
    /// As such, it MUST NOT be mutated after options initialization to preserve the exact order.
    /// </summary>
    public List<OpenIddictValidationHandlerDescriptor> Handlers { get; } = new(OpenIddictValidationHandlers.DefaultHandlers);

    /// <summary>
    /// Gets or sets the type of validation used by the OpenIddict validation services.
    /// By default, local validation is always used.
    /// </summary>
    public OpenIddictValidationType ValidationType { get; set; } = OpenIddictValidationType.Direct;

    /// <summary>
    /// Gets or sets the client identifier sent to the authorization server when using remote validation.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret sent to the authorization server when using remote validation.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether a database call is made
    /// to validate the authorization entry associated with the received tokens.
    /// Note: enabling this option may have an impact on performance and
    /// can only be used with an OpenIddict-based authorization server.
    /// </summary>
    public bool EnableAuthorizationEntryValidation { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether a database call is made
    /// to validate the token entry associated with the received tokens.
    /// Note: enabling this option may have an impact on performance but
    /// is required when the OpenIddict server emits reference tokens.
    /// </summary>
    public bool EnableTokenEntryValidation { get; set; }

    /// <summary>
    /// Gets or sets the issuer that will be attached to the <see cref="Claim"/>
    /// instances created by the OpenIddict validation stack.
    /// </summary>
    /// <remarks>
    /// Note: if this property is not explicitly set, the
    /// issuer URI is automatically used as a fallback value.
    /// </remarks>
    public string? ClaimsIssuer { get; set; }

    /// <summary>
    /// Gets or sets the absolute URI of the OAuth 2.0/OpenID Connect server.
    /// </summary>
    public Uri? Issuer { get; set; }

    /// <summary>
    /// Gets or sets the URI of the configuration endpoint exposed by the server.
    /// When the URI is relative, <see cref="Issuer"/> must be set and absolute.
    /// </summary>
    public Uri? ConfigurationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the OAuth 2.0/OpenID Connect static server configuration, if applicable.
    /// </summary>
    public OpenIddictConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the configuration manager used to retrieve
    /// and cache the OAuth 2.0/OpenID Connect server configuration.
    /// </summary>
    public IConfigurationManager<OpenIddictConfiguration> ConfigurationManager { get; set; } = default!;

    /// <summary>
    /// Gets the intended audiences of this resource server.
    /// Setting this property is recommended when the authorization
    /// server issues access tokens for multiple distinct resource servers.
    /// </summary>
    public HashSet<string> Audiences { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the OAuth 2.0 client authentication methods enabled for this application.
    /// </summary>
    public HashSet<string> ClientAuthenticationMethods { get; } = new(StringComparer.Ordinal)
    {
        // Note: client_secret_basic is deliberately not added here as it requires
        // a dedicated event handler (typically provided by the HTTP integration)
        // to attach the client credentials to the standard Authorization header.
        //
        // The System.Net.Http integration supports the client_secret_basic,
        // self_signed_tls_client_auth and tls_client_auth authentication
        // methods and automatically add them to this list at runtime.
        OpenIddictConstants.ClientAuthenticationMethods.ClientSecretPost,
        OpenIddictConstants.ClientAuthenticationMethods.PrivateKeyJwt
    };

    /// <summary>
    /// Gets the token validation parameters used by the OpenIddict validation services.
    /// </summary>
    public TokenValidationParameters TokenValidationParameters { get; } = new()
    {
        AuthenticationType = TokenValidationParameters.DefaultAuthenticationType,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = Claims.Name,
        RoleClaimType = Claims.Role,
        // In previous versions of OpenIddict (1.x and 2.x), all the JWT tokens (access and identity tokens)
        // were issued with the generic "typ": "JWT" header. To prevent confused deputy and token substitution
        // attacks, a special "token_usage" claim was added to the JWT payload to convey the actual token type.
        // This validator overrides the default logic used by IdentityModel to resolve the type from this claim.
        TypeValidator = static (type, token, parameters) =>
        {
            // If available, try to resolve the actual type from the "token_usage" claim.
            if (((JsonWebToken) token).TryGetPayloadValue(Claims.TokenUsage, out string usage))
            {
                type = usage switch
                {
                    TokenTypeHints.AccessToken => JsonWebTokenTypes.AccessToken,
                    TokenTypeHints.IdToken     => JsonWebTokenTypes.Jwt,

                    _ => throw new NotSupportedException(SR.GetResourceString(SR.ID0269))
                };
            }

            // At this point, throw an exception if the type cannot be resolved from the "typ" header
            // (provided via the type delegate parameter) or inferred from the token_usage claim.
            if (string.IsNullOrEmpty(type))
            {
                throw new SecurityTokenInvalidTypeException(SR.GetResourceString(SR.ID0270));
            }

            // Note: unlike IdentityModel, this custom validator deliberately uses case-insensitive comparisons.
            if (parameters.ValidTypes is not null && parameters.ValidTypes.Any() &&
               !parameters.ValidTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                throw new SecurityTokenInvalidTypeException(SR.GetResourceString(SR.ID0271))
                {
                    InvalidType = type
                };
            }

            return type;
        },
        // Note: audience and lifetime are manually validated by OpenIddict itself.
        ValidateAudience = false,
        ValidateLifetime = false
    };

#if SUPPORTS_TIME_PROVIDER
    /// <summary>
    /// Gets or sets the time provider.
    /// </summary>
    public TimeProvider? TimeProvider { get; set; }
#endif
}
