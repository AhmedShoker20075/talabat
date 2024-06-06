using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repositry.Identity
{
    public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUsersAsync(UserManager<AppUser> _userManager)
		{
			if(_userManager.Users.Count() == 0)
			{
				var user = new AppUser()
				{
					DisplayName = "Ahmed Sayed",
					Email = "ahmed.sayed20075@gmail.com",
					UserName = "ahmed.sayed20075",
					PhoneNumber = "01028200798"
				};
					
				await _userManager.CreateAsync(user,"P@$$W0rd");
			}
		}
	}
}
