
using System.Text;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using static DocuSign.eSign.Client.Auth.OAuth;

namespace FIU.ThriveToDocuSign.Core.Services
{
    public class DocuSignService : IDocuSignService
    {
        private readonly DocuSignClient _docuSignClient;
        private readonly EnvelopesApi _envelopesApi;
        private readonly string _authServer;

        public DocuSignService(string authServer,
                               DocuSignClient docuSignClient,
                               EnvelopesApi envelopesApi)
        {
            _authServer = authServer;
            _docuSignClient = docuSignClient == null ? new DocuSignClient(authServer) : docuSignClient;
            _envelopesApi = envelopesApi == null ? new EnvelopesApi(docuSignClient) : envelopesApi;

            _docuSignClient.SetOAuthBasePath(authServer);
        }

        public OAuthToken AuthenticateWithJwt(string docuSignApi,
                                              string clientId,
                                              string impersonatedUserId,
                                              string authServer,
                                              byte[] privateKeyBytes)
        {
            var docuSignApiType = Enum.Parse<DocuSignApiType>(docuSignApi);

            var scopes = GetRightScope(docuSignApiType);

            var tokenExpiryInHours = 1;

            var authToken = _docuSignClient.RequestJWTUserToken(
                clientId,
                impersonatedUserId,
                authServer,
                privateKeyBytes,
                tokenExpiryInHours,
                scopes);

          //  _docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + authToken.access_token);

            return authToken;
        }
        private List<string> GetRightScope(DocuSignApiType apiType)
        {
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            if (apiType == DocuSignApiType.Rooms)
            {
                scopes.AddRange(new List<string>
                {
                    "dtr.rooms.read",
                    "dtr.rooms.write",
                    "dtr.documents.read",
                    "dtr.documents.write",
                    "dtr.profile.read",
                    "dtr.profile.write",
                    "dtr.company.read",
                    "dtr.company.write",
                    "room_forms",
                });
            }

            if (apiType == DocuSignApiType.Click)
            {
                scopes.AddRange(new List<string>
                            {
                                "click.manage",
                                "click.send",
                            });
            }

            if (apiType == DocuSignApiType.Monitor)
            {
                scopes.AddRange(new List<string>
                            {
                                "signature",
                                "impersonation",
                            });
            }

            if (apiType == DocuSignApiType.Admin)
            {
                scopes.AddRange(new List<string>
                            {
                                "user_read",
                                "user_write",
                                "account_read",
                                "organization_read",
                                "group_read",
                                "permission_read",
                                "identity_provider_read",
                                "domain_read",
                                "user_data_redact",
                                "asset_group_account_read",
                                "asset_group_account_clone_write",
                                "asset_group_account_clone_read",
                            });
            }

            return scopes;
        }


        /// <summary>
        /// Creates an envelope that would include two documents and add a signer and cc recipients to be notified via email
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="docDocx">String of bytes representing the Word document (docx)</param>
        /// <param name="envStatus">Status to set the envelope to</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public string SendEnvelopeViaEmail(string signerEmail, string signerName, string ccEmail, string ccName, string accessToken, string basePath, string accountId, string docDocx, string docPdf, string envStatus)
        {
            //ds-snippet-start:eSign2Step3
            EnvelopeDefinition env = MakeEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, docDocx, docPdf, envStatus);

            // _docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopeSummary results = _envelopesApi.CreateEnvelope(accountId, env);
          
            return results.EnvelopeId;
            //ds-snippet-end:eSign2Step
        }

        /// <summary>
        /// Creates a new envelope, adds a single document and a signle recipient (signer) and generates a url that is used for embedded signing.
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="signerClientId">A unique ID for the embedded signing session for this signer</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="returnUrl">URL user will be redirected to after they sign</param>
        /// <param name="pingUrl">URL that DocuSign will be able to ping to incdicate signing session is active</param>
        /// <returns>The envelopeId (GUID) of the resulting Envelope and the URL for the embedded signing</returns>
        public (string, string) SendEnvelopeForEmbeddedSigning(
            string signerEmail,
            string signerName,
            string signerClientId,
            string accessToken,
            string basePath,
            string accountId,
            string docPdf,
            string returnUrl,
            string pingUrl = null)
        {
            //ds-snippet-start:eSign1Step3
            EnvelopeDefinition envelope = MakeEnvelopeForEmbeddedSigning(signerEmail, signerName, signerClientId, docPdf);

            // _docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopeSummary results = _envelopesApi.CreateEnvelope(accountId, envelope);

            string envelopeId = results.EnvelopeId;
            //ds-snippet-end:eSign1Step3

            //ds-snippet-start:eSign1Step5
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, returnUrl, signerClientId, pingUrl);

            // call the CreateRecipientView API
            ViewUrl results1 = _envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            // Don't use an iFrame!
            // State can be stored/recovered using the framework's session or a
            // query parameter on the returnUrl (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;

            // returning both the envelopeId as well as the url to be used for embedded signing
            return (envelopeId, redirectUrl);
            //ds-snippet-end:eSign1Step5
        }

        //ds-snippet-start:eSign1Step4
        public RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {
            // Data for this method
            // signerEmail
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global
            RecipientViewRequest viewRequest = new RecipientViewRequest();

            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = returnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's
            // authentication, you can include authenticate steps from DocuSign.
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope.
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // Signing Ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app,
            // NOTE: The pings will only be sent if the pingUrl is an https address
            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600"; // seconds
                viewRequest.PingUrl = pingUrl; // optional setting
            }

            return viewRequest;
        }

        //ds-snippet-start:eSign1Step2
        public EnvelopeDefinition MakeEnvelopeForEmbeddedSigning(string signerEmail, string signerName, string signerClientId, string docPdf)
        {
            // Data for this method
            // signerEmail
            // signerName
            // signerClientId -- class global
            // Config.docPdf
            byte[] buffer = System.IO.File.ReadAllBytes(docPdf);

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
            envelopeDefinition.EmailSubject = "Please sign this document";
            Document doc1 = new Document();

            string doc1B64 = Convert.ToBase64String(buffer);

            doc1.DocumentBase64 = doc1B64;
            doc1.Name = "Lorem Ipsum"; // can be different from actual file name
            doc1.FileExtension = "pdf";
            doc1.DocumentId = "3";

            // The order in the docs array determines the order in the envelope
            envelopeDefinition.Documents = new List<Document> { doc1 };

            // Create a signer recipient to sign the document, identified by name and email
            // We set the clientUserId to enable embedded signing for the recipient
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings.
            SignHere signHere1 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorXOffset = "10",
                AnchorYOffset = "20",
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipient to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
            };
            envelopeDefinition.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }

        public EnvelopeDefinition MakeEnvelopeViaEmail(string signerEmail, string signerName, string ccEmail, string ccName, string docDocx, string docPdf, string envStatus)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // Config.docDocx
            // Config.docPdf
            // RequestItemsService.Status -- the envelope status ('created' or 'sent')

            // document 1 (html) has tag **signature_1**
            // document 2 (docx) has tag /sn1/
            // document 3 (pdf) has tag /sn1/
            //
            // The envelope has two recipients.
            // recipient 1 - signer
            // recipient 2 - cc
            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read files from a local directory
            // The reads could raise an exception if the file is not available!
            string doc2DocxBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx));
            string doc3PdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));

            //ds-snippet-start:eSign2Step2
            EnvelopeDefinition env = new EnvelopeDefinition();
            env.EmailSubject = "Please sign this document set";

            // Create document objects, one per document
            Document doc1 = new Document();
            string b64 = Convert.ToBase64String(Document1(signerEmail, signerName, ccEmail, ccName));
            doc1.DocumentBase64 = b64;
            doc1.Name = "Order acknowledgement"; // can be different from actual file name
            doc1.FileExtension = "html"; // Source data format. Signed docs are always pdf.
            doc1.DocumentId = "1"; // a label used to reference the doc
            Document doc2 = new Document
            {
                DocumentBase64 = doc2DocxBytes,
                Name = "Battle Plan", // can be different from actual file name
                FileExtension = "docx",
                DocumentId = "2",
            };
            Document doc3 = new Document
            {
                DocumentBase64 = doc3PdfBytes,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "pdf",
                DocumentId = "3",
            };

            // The order in the docs array determines the order in the envelope
            env.Documents = new List<Document> { doc1, doc2, doc3 };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
            };

            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            CarbonCopy cc1 = new CarbonCopy
            {
                Email = ccEmail,
                Name = ccName,
                RecipientId = "2",
                RoutingOrder = "2",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            SignHere signHere1 = new SignHere
            {
                AnchorString = "**signature_1**",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20",
            };

            SignHere signHere2 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20",
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1, signHere2 },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
                CarbonCopies = new List<CarbonCopy> { cc1 },
            };
            env.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = envStatus;

            return env;
            //ds-snippet-end:eSign2Step2
        }

        public byte[] Document1(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            return Encoding.UTF8.GetBytes(
            " <!DOCTYPE html>\n" +
                "    <html>\n" +
                "        <head>\n" +
                "          <meta charset=\"UTF-8\">\n" +
                "        </head>\n" +
                "        <body style=\"font-family:sans-serif;margin-left:2em;\">\n" +
                "        <h1 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "            color: darkblue;margin-bottom: 0;\">World Wide Corp</h1>\n" +
                "        <h2 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "          margin-top: 0px;margin-bottom: 3.5em;font-size: 1em;\n" +
                "          color: darkblue;\">Order Processing Division</h2>\n" +
                "        <h4>Ordered by " + signerName + "</h4>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Email: " + signerEmail + "</p>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Copy to: " + ccName + ", " + ccEmail + "</p>\n" +
                "        <p style=\"margin-top:3em;\">\n" +
                "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n" +
                "        </p>\n" +
                "        <!-- Note the anchor tag for the signature field is in white. -->\n" +
                "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n" +
                "        </body>\n" +
                "    </html>");
        }
    }
}
