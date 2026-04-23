// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Cailean_Razvan_Zboruri.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager; // <-- AM ADĂUGAT USER MANAGER

        // Asigură-te că și constructorul le include pe toate 3:
        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            // AM SCOS [EmailAddress] PENTRU A PERMITE ȘI NUME SIMPLE (fără @)
            [Required(ErrorMessage = "Te rugăm să introduci Email-ul sau Numele de Utilizator.")]
            [Display(Name = "Email sau Utilizator")]
            public string UserNameOrEmail { get; set; }

            [Required(ErrorMessage = "Parola este obligatorie.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Ține-mă minte")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // 1. Căutăm userul în baza de date MAI ÎNTÂI DUPĂ EMAIL
                var user = await _userManager.FindByEmailAsync(Input.UserNameOrEmail);

                // 2. Dacă nu îl găsim după email, ÎL CĂUTĂM DUPĂ NUMELE DE UTILIZATOR
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(Input.UserNameOrEmail);
                }

                // 3. Dacă am găsit un cont valid (indiferent cum am căutat)
                if (user != null)
                {
                    // Ne logăm folosind proprietatea corectă (user.UserName)
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                }

                // Dacă userul nu a fost găsit sau parola e greșită
                ModelState.AddModelError(string.Empty, "Date de conectare incorecte.");
                return Page();
            }

            return Page();
        }
    }
}
