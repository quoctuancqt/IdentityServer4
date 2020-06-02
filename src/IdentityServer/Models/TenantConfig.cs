namespace IdentityServer.Models
{
    public class TenantConfig
    {
        public string Domain { get; set; }
        public string SqlServer { get; set; }
        public string SqlUserName { get; set; }
        public string SqlPassword { get; set; }
        public string MongoDbServer { get; set; }
        public string MongoDbUserName { get; set; }
        public string MongoDbPassword { get; set; }

        public string GetTenantDbName(string subDomain)
        {
            var clientId = ParseClientIdBySubDomain(subDomain);
            return clientId.Replace(".", "")
                    .Replace("-", "")
                    .Replace(":", "")
                    .ToLower();
        }

        public string ParseClientIdBySubDomain(string subDomain)
        {
            return string.Format(Domain, subDomain).ToLower();
        }

        public string ParseClientUriBySubDomain(string subDomain, bool isHttps = false)
        {
            if (isHttps) return $"https://{string.Format(Domain, subDomain).ToLower()}";

            return $"http://{string.Format(Domain, subDomain).ToLower()}";
        }
    }
}
