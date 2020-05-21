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
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api_server"
                    }
                },
            };
    }
}