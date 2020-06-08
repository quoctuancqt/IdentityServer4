using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OAuthClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "ed5d49ee-30bf-4df5-b528-4e6ecc0de23f",
                ClientSecret = "8abc8220-8b8e-4ea9-8881-eeec1ff5f6c1",
                Scope = "api_server"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            Console.ReadLine();
        }
    }
}
