using DocuSign.eSign.Model;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace FIU.ThriveToDocuSign.Core.Services
{
    public interface IDocuSignService
    {
        OAuthToken AuthenticateWithJwt(string docuSignApi,
                                              string clientId,
                                              string impersonatedUserId,
                                              string authServer,
                                              byte[] privateKeyBytes);
        string SendEnvelopeViaEmail(string signerEmail,
                                    string signerName,
                                    string ccEmail,
                                    string ccName,
                                    string accessToken,
                                    string basePath,
                                    string accountId,
                                    string docDocx,
                                    string docPdf,
                                    string envStatus);

        (string, string) SendEnvelopeForEmbeddedSigning(string signerEmail,
                                                        string signerName,
                                                        string signerClientId,
                                                        string accessToken,
                                                        string basePath,
                                                        string accountId,
                                                        string docPdf,
                                                        string returnUrl,
                                                        string pingUrl = null);
        RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null);
        EnvelopeDefinition MakeEnvelopeForEmbeddedSigning(string signerEmail, string signerName, string signerClientId, string docPdf);
        EnvelopeDefinition MakeEnvelopeViaEmail(string signerEmail, string signerName, string ccEmail, string ccName, string docDocx, string docPdf, string envStatus);
        byte[] Document1(string signerEmail, string signerName, string ccEmail, string ccName);
    }
}
