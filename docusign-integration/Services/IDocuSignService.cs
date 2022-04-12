using docusign_integration.Models;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace docusign_integration.Services
{
    public interface IDocuSignService
    {
        public bool IsDSTokenActive(string dsAuthExpireOn);
        public OAuthToken DSGetToken();
        public string DSCeremony(OAuthToken dsAuthToken, DocuSignModel dsModel);
    }
}
