using Microsoft.Owin;
using Owin;
using Weavy.Core.Services;
using Weavy.Core;
using Weavy.Web.Owin;

using System;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;


[assembly: OwinStartup(typeof(Weavy.Startup))]
namespace Weavy {

    /// <summary>
    /// OWIN Startup class that configures our application on application start.
    /// </summary>
    public partial class Startup {

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app) {
            app.UseWeavy();
            var googleClientId = ConfigurationService.AppSetting("GoogleClientID");
            var googleClientSecret = ConfigurationService.AppSetting("GoogleClientSecret");
            var googleDomain = ConfigurationService.AppSetting("GoogleDomain");

            if (googleClientId != null && googleClientSecret != null && googleDomain != null)
            {
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    AuthenticationType = "Google",
                    AuthenticationMode = AuthenticationMode.Passive,
                    Authority = "https://accounts.google.com",
                    Caption = "Google",
                    ClientId = googleClientId,
                    ClientSecret = googleClientSecret,
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (context) => {
                            // this ensures that the address used for sign in and sign out is picked up dynamically (it must still be registered in the Google developer console)
                            context.ProtocolMessage.RedirectUri = WeavyContext.Current.ApplicationUrl + "signin-google";

                            // set the hd parameter to optimize the OpenID Connect flow for users of the specified domain.  
                            context.ProtocolMessage.Parameters.Add("hd", googleDomain);

                            return Task.CompletedTask;
                        },
                        SecurityTokenValidated = (context) => {
                            // validate domain
                            string hd = context.AuthenticationTicket.Identity.FindFirst("hd")?.Value;
                            if (hd == null)
                            {
                                throw new SecurityTokenValidationException("No hosted domain (hd) found in claims.");
                            }
                            else if (!hd.Equals(googleDomain, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new SecurityTokenValidationException($"Hosted domain {hd} is not allowed.");
                            }
                            return Task.CompletedTask;
                        },
                        AuthenticationFailed = (context) => {
                            context.OwinContext.Response.Redirect(WeavyContext.Current.ApplicationPath + "error/unauthorized");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    },
                    Scope = "openid email profile",
                });
            }
        }
    }
}
