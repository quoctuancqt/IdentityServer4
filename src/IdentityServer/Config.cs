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
                new ApiResource("api1", "My API #1")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
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
                        "api1"
                    }
                },
                new Client
                    {
                        ClientId = "product1.mci.com",
                        ClientName = "Prduct 1 MasterCard",
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        RequireConsent = false,

                        RedirectUris = {"http://product1.mci.com:6001/signin-oidc"},
                        PostLogoutRedirectUris = {"http://product1.mci.com:6001"},

                        ClientSecrets =
                        {
                          new Secret("product1".Sha256())
                        },

                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile
                        }
                    },
                    new Client
                    {
                        ClientId = "product1.amex.com",
                        ClientName = "Prduct 1 Amex",
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        RequireConsent = false,

                        RedirectUris = {"http://product1.amex.com:7001/signin-oidc"},
                        PostLogoutRedirectUris = {"http://product1.amex.com:7001"},

                        ClientSecrets =
                        {
                          new Secret("product1".Sha256())
                        },

                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile
                        }
                    }
            };
    }
}