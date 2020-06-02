namespace IdentityServer.Models
{
    public class ClientConfiguration
    {
        public ClientConfiguration() { }

        public ClientConfiguration(string clientId,
            string key,
            string value)
        {
            ClientId = clientId;
            Key = key;
            Value = value;
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
