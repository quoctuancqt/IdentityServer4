using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer.Services.Idsrv4
{
    public class RedirectUriValidator : StrictRedirectUriValidator, IRedirectUriValidator
    {

        public override Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        public override Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }
    }
}
