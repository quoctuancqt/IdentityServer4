namespace IdentityServer.Dtos
{
    public class ClientDto
    {
        public ClientDto()
        {
            ClientParser = new ClientParserValueDto();
        }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string CurrentSecret { get; set; }

        public string[] AllowedGrantTypes { get; set; }

        public string[] ClientSecrets { get; set; }

        public string[] RedirectUris { get; set; }

        public string[] PostLogoutRedirectUris { get; set; }

        public string[] AllowedScopes { get; set; }

        public bool AllowOfflineAccess { get; set; }

        public bool RequirePkce { get; set; }
        public ClientParserValueDto ClientParser { get; set; }
    }

    public class ClientParserValueDto
    {
        public string ClientSecretsValue { get; set; }

        public string RedirectUrisValue { get; set; }

        public string PostLogoutRedirectUrisValue { get; set; }

        public string AllowedScopesValue { get; set; }

    }
}
