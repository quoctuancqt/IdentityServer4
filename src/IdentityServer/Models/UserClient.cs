namespace IdentityServer.Models
{
    public class UserClient
    {
        public UserClient() { }

        public UserClient(string userId, string clientId)
        {
            UserId = userId;
            ClientId = clientId;
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ClientId { get; set; }
    }
}
