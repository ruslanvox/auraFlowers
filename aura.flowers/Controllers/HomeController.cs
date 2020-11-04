using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aura.flowers.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aura.flowers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using website.services.Interfaces;
using website.core.Models.Email;
using website.core.Models.GoogleRecaptcha;

namespace aura.flowers.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IGoogleRecaptchaService _googleRecaptchaService;
        private readonly IOptions<EmailConfiguration> _emailConfiguration;
        private readonly IOptions<GoogleRecaptchaConfiguration> _googleRecaptchaConfiguration;

        public HomeController(IEmailService emailService,
            IGoogleRecaptchaService googleRecaptchaService,
            IOptions<EmailConfiguration> emailConfiguration,
            IOptions<GoogleRecaptchaConfiguration> googleRecaptchaConfiguration)
        {
            _emailService = emailService;
            _googleRecaptchaService = googleRecaptchaService;
            _emailConfiguration = emailConfiguration;
            _googleRecaptchaConfiguration = googleRecaptchaConfiguration;
        }

        /// <summary>
        /// Main index page.
        /// </summary>
        /// <returns>Main index page view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Privacy and cookies page/subpage.
        /// </summary>
        /// <returns>Privacy and cookies page/subpage view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Set selected language.
        /// </summary>
        /// <param name="culture">Culture name.</param>
        /// <param name="returnUrl">Return url string.</param>
        /// <returns>Localized view for selected language.</returns>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Send message from customer in contact us modal window.
        /// </summary>
        /// <param name="model">Contact us modal window view model.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromForm] ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await _googleRecaptchaService
                    .IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _googleRecaptchaConfiguration.Value.Secret))
                {
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA");
                    return Json(new { error = "google-recaptcha-error" });
                }

                _emailService.Send(new EmailMessage
                {
                    FromAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = "Request from Aura.Flowers site.", Address = _emailConfiguration.Value.MailFrom }
                    },
                    ToAddresses = new List<EmailAddress>
                    {
                        new EmailAddress{ Name = "Aura.Flowers sales manager", Address = _emailConfiguration.Value.MailTo }
                    },
                    Subject = "Aura.Flowers order." +
                              (model.SelectedProductId != 0 ? $" Product type {(ProductTypes)model.SelectedProductId}" :
                                  string.Empty),
                    Content = "<h3>Good day, Ekaterina!</h3>" + $"<p>{model.Name} is interested of your web site.</p>" +
                              (model.SelectedProductId != 0 ? $"<p>Customer selected product type {(ProductTypes)model.SelectedProductId}.</p>" :
                                  string.Empty) +
                              $"<p>Customer e-mail: {model.Email}.</p><p>Customer message was: {model.Message}</p>"
                });

                return Json(new { success = true });
            }

            return Json(new { error = "backend-validation-error" });
        }

        /// <summary>
        /// Error page.
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}