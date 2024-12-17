// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user != null)
            //{
            //    user.LastLogoutTime = DateTime.UtcNow;
            //    await _signInManager.UserManager.UpdateAsync(user);
            //}


            {
                // Log the logout activity
                var dbContext = HttpContext.RequestServices.GetService<ApplicationDbContext>();
                dbContext.ActivityLog.Add(new UserActivityLog
                {
                    UserId = user.Id,
                    Activity = "Logged out",
                    Timestamp = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
