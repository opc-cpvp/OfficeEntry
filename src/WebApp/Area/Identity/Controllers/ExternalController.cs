﻿using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Area.Identity.Controllers
{
    [Route("{controller}/{action}")]
    public class ExternalController : Controller
    {
        private readonly ILogger<ExternalController> _logger;

        public ExternalController(ILogger<ExternalController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            // we only have one option for logging in and it's an external provider
            return RedirectToAction("Challenge", "External", new { provider = NegotiateDefaults.AuthenticationScheme, returnUrl });
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            if (NegotiateDefaults.AuthenticationScheme != provider)
            {
                throw new Exception("Invalid authentication provider.");
            }

            return await ProcessNegociateLoginAsync(returnUrl);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            return Redirect(returnUrl);
        }

        private async Task<IActionResult> ProcessNegociateLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(NegotiateDefaults.AuthenticationScheme);

            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, treating windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("Callback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", NegotiateDefaults.AuthenticationScheme },
                    },
                    ExpiresUtc = new DateTimeOffset(DateTime.UtcNow.AddDays(365)),
                    AllowRefresh = true,
                    IsPersistent = true
                };

                var id = new ClaimsIdentity(NegotiateDefaults.AuthenticationScheme);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.FindFirst(ClaimTypes.PrimarySid).Value));
#pragma warning disable CA1416
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));
                id.AddClaim(new Claim(ClaimTypes.Name, wp.Identity.Name));
#pragma warning restore CA1416

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return Redirect(props.RedirectUri);
            }
            else
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return Challenge(NegotiateDefaults.AuthenticationScheme);
            }
        }
    }
}