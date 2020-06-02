// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api_oauth_server", "Api Oauth Server"),
                new ApiResource("api_server", "Api Server")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "03c3d277-3f1f-456d-af6e-75f816094767",
                    ClientName = "OAuth Server",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    ClientSecrets = { new Secret("13bc95f9-f336-47d4-bc0a-debec4f2adbd".Sha256()) },
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = {
                        "http://localhost:5000/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = {
                        "http://localhost:5000/swagger/index.html"
                    },
                    AllowedScopes =
                    {
                        "api_oauth_server"
                    },
                    Description = "13bc95f9-f336-47d4-bc0a-debec4f2adbd"
                },
                new Client
                {
                    ClientId = "0b791e06-0d97-4fc4-8682-65e18b5d3a8f",
                    ClientName = "Tenant 1",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("2e9c95d0-19e9-4ee6-9c8b-b65824f64179".Sha256()) },
                    RequireConsent = false,
                    RedirectUris = {
                        "http://localhost:5052/signin-oidc",
                        "http://localhost:5000/swagger/oauth2-redirect.html",
                    },
                    PostLogoutRedirectUris = {
                        "http://localhost:5052/signout-callback-oidc",
                        "http://localhost:5000/swagger/index.html",
                    },
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "api_server"
                    },
                    Description = "0b791e06-0d97-4fc4-8682-65e18b5d3a8f"
                },
                new Client
                {
                    ClientId = "435c3b72-b310-495a-8ad6-8de3995492da",
                    ClientName = "Tenant 2",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("089e122c-6f55-4eaa-a2aa-1b7718a2a8de".Sha256()) },
                    RequireConsent = false,
                     RedirectUris = {
                        "http://localhost:5053/signin-oidc",
                        "http://localhost:5000/swagger/oauth2-redirect.html",
                    },
                    PostLogoutRedirectUris = {
                        "http://localhost:5053/signout-callback-oidc",
                        "http://localhost:5000/swagger/index.html",
                    },
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "api_server"
                    },
                    Description = "435c3b72-b310-495a-8ad6-8de3995492da"
                },
            };
    }
}