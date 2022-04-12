using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using docusign_integration.Helper;
using docusign_integration.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace docusign_integration.Services
{
    public class DocuSignService : IDocuSignService
    {
        private readonly IConfiguration _configuration;

        [AllowNull]
        protected static ApiClient dsApiClient { get; private set; }

        public DocuSignService(IConfiguration configuration)
        {
            _configuration = configuration;

            dsApiClient ??= new ApiClient();
        }

        public bool IsDSTokenActive(string dsAuthExpireOn)
        {
            bool ret = false;

            if (!string.IsNullOrEmpty(dsAuthExpireOn))
            {
                DateTime expireOn = DateTime.UtcNow;
                expireOn = DateTime.ParseExact(dsAuthExpireOn, AppConstant.DateFormat, CultureInfo.InvariantCulture);

                if (DateTime.UtcNow < expireOn)
                {
                    ret = true;
                }
            }

            return ret;
        }

        public OAuthToken DSGetToken()
        {
            var scopes = new List<string>
                {
                    Scope_SIGNATURE,
                    Scope_IMPERSONATION
                };

            return dsApiClient.RequestJWTUserToken(
                _configuration[CfgDocuSign.ClientId],
                _configuration[CfgDocuSign.ImpersonatedUserId],
                Demo_OAuth_BasePath,
                FileHelper.ReadFileContent(FileHelper.PrepareFullPrivateKeyFilePath(_configuration[CfgDocuSign.PrivateKeyFile])),
                1,
                scopes);
        }

        public string DSCeremony(OAuthToken dsAuthToken, DocuSignModel dsModel)
        {
            EnvelopeDefinition envelope = DSCreateEnvelope(dsAuthToken, dsModel);

            var test = _configuration[CfgDocuSign.APIAccountId];

            var apiClient = new ApiClient(_configuration[CfgDocuSign.BasePath] + "/restapi");
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + dsAuthToken.access_token);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(_configuration[CfgDocuSign.APIAccountId], envelope);

            RecipientViewRequest viewRequest = new RecipientViewRequest
            {
                ReturnUrl = _configuration[CfgDocuSign.RedirectUrl],
                AuthenticationMethod = "none",
                Email = dsModel.UserEmail,
                UserName = dsModel.UserName,
                ClientUserId = _configuration[CfgDocuSign.ClientId]
            };

            ViewUrl dsResult = envelopesApi.CreateRecipientView(_configuration[CfgDocuSign.APIAccountId], results.EnvelopeId, viewRequest);

            return dsResult.Url;
        }

        private EnvelopeDefinition DSCreateEnvelope(OAuthToken dsAuthToken, DocuSignModel dsModel)
        {
            var uniqueId = Guid.NewGuid();

            var fileName = $"{dsModel.UserEmail}_{uniqueId.ToString().Replace("-", "")}";

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = $"AdjiDemoDS - {fileName}"
            };

            string docBase64 = string.Empty;

            using (var ms = new MemoryStream())
            {
                dsModel.UploadFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                docBase64 = Convert.ToBase64String(fileBytes);
            }

            Document doc1 = new Document
            {
                DocumentBase64 = docBase64,
                Name = fileName,
                FileExtension = FileHelper.GetExtension(dsModel.UploadFile.FileName),
                DocumentId = "1"
            };
            envelopeDefinition.Documents = new List<Document> { doc1 };

            Signer signer1 = new Signer
            {
                Email = dsModel.UserEmail,
                Name = dsModel.UserName,
                ClientUserId = _configuration[CfgDocuSign.ClientId],
                RecipientId = uniqueId.ToString()
            };

            SignHere signHere1 = new SignHere
            {
                AnchorString = dsModel.KeyString,
                AnchorUnits = "pixels",
                AnchorXOffset = "10",
                AnchorYOffset = "40"
            };

            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 }
            };
            signer1.Tabs = signer1Tabs;

            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };
            envelopeDefinition.Recipients = recipients;

            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }
    }
}
