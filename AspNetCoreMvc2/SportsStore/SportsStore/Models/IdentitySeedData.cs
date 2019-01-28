using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SportsStore.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";


        //public static async void EnsurePopulated(IApplicationBuilder app)
        //{
        //    UserManager<IdentityUser> userManager = app.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();
        //    IdentityUser user = await userManager.FindByIdAsync(adminUser);

        //    if (user == null)
        //    {
        //        user = new IdentityUser("Admin");
        //        await userManager.CreateAsync(user, adminPassword);
        //    }
        //}


        //ch12 edit:
        //Rather than obtaining the UserManager<IdentityUser> service itself, the EnsurePopulated method receives the object as an argument.
        //This allows me to integrate the database seeding in the AccountController class

        public static async Task EnsurePopulated(UserManager<IdentityUser> userManager)
        {
            IdentityUser user = await userManager.FindByIdAsync(adminUser);

            if (user == null)
            {
                user = new IdentityUser("Admin");
                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
