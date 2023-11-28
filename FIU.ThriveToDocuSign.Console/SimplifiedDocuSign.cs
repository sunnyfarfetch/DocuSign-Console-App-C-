using DocuSign.CodeExamples.Authentication;
using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using ESignature.Examples;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.IO;
using DocuSign.CodeExamples.Common;


namespace FIU.ThriveToDocuSign.Console
{
    public class SimplifiedDocuSign
    {
        //static readonly string DevCenterPage = "https://developers.docusign.com/platform/auth/consent";
        //public static void Main(string[] args)
        //{
        //    OAuthToken accessToken = null;
        //    var CLIENT_ID = "8c442e32-9937-4a31-b52e-87b8e8247611";
        //    var USER_ID = "735f09b7-0213-44e3-a4a8-824f4660f7bf";
        //    var AUTH_SERVER = "account-d.docusign.com";
        //    var PRIVATE_KEY_FILE = DsHelper.ReadFileContent(@"..\..\..\private_demo2.key");

        //    try
        //    {
        //        accessToken = JwtAuth.AuthenticateWithJwt("ESignature", CLIENT_ID, USER_ID, AUTH_SERVER, PRIVATE_KEY_FILE);
        //    }
        //    catch (ApiException apiExp)
        //    {
        //        // Consent for impersonation must be obtained to use JWT Grant
        //        if (apiExp.Message.Contains("consent_required"))
        //        {
        //            // Caret needed for escaping & in windows URL
        //            string caret = "";
        //            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //            {
        //                caret = "^";
        //            }

        //            // build a URL to provide consent for this Integration Key and this userId
        //            string url = "https://" + ConfigurationManager.AppSettings["AuthServer"] + "/oauth/auth?response_type=code" + caret + "&scope=impersonation%20signature" + caret +
        //                "&client_id=" + ConfigurationManager.AppSettings["ClientId"] + caret + "&redirect_uri=" + DevCenterPage;
        //            System.Console.WriteLine($"Consent is required - launching browser (URL is {url.Replace(caret, "")})");

        //            // Start new browser window for login and consent to this app by DocuSign user
        //            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //            {
        //                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = false });
        //            }
        //            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //            {
        //                Process.Start("xdg-open", url);
        //            }
        //            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //            {
        //                Process.Start("open", url);
        //            }

        //            System.Console.ForegroundColor = System.ConsoleColor.Red;
        //            System.Console.WriteLine("Unable to send envelope; Exiting. Please rerun the System.Console app once consent was provided");
        //            System.Console.ForegroundColor = System.ConsoleColor.White;
        //            Environment.Exit(-1);
        //        }
        //    }

        //    var docuSignClient = new DocuSignClient();
        //    docuSignClient.SetOAuthBasePath(ConfigurationManager.AppSettings["AuthServer"]);
        //    UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
        //    Account acct = userInfo.Accounts.FirstOrDefault();

        //    System.Console.WriteLine("Welcome to the JWT Code example! ");
        //    System.Console.Write("Enter the signer's email address: ");
        //    string signerEmail = System.Console.ReadLine();
        //    System.Console.Write("Enter the signer's name: ");
        //    string signerName = System.Console.ReadLine();
        //    System.Console.Write("Enter the carbon copy's email address: ");
        //    string ccEmail = System.Console.ReadLine();
        //    System.Console.Write("Enter the carbon copy's name: ");
        //    string ccName = System.Console.ReadLine();
        //    string docDocx = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_salary.docx");
        //    string docPdf = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_lorem.pdf");
        //    System.Console.WriteLine("");
        //    string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, docDocx, docPdf, "sent");
        //    System.Console.WriteLine("");
        //    System.Console.ForegroundColor = System.ConsoleColor.Green;
        //    System.Console.WriteLine($"Successfully sent envelope with envelopeId {envelopeId}");
        //    System.Console.WriteLine("");
        //    System.Console.WriteLine("");
        //    System.Console.ForegroundColor = ConsoleColor.White;
        //    Environment.Exit(0);
        //}

    }
}
