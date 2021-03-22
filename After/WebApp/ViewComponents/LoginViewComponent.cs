using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebApp.ViewComponents
{
    public class LoginViewComponent : ViewComponent
    {
        public LoginViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            //get the user name from claims
            if (User.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims;
                string userName = identity.Claims.FirstOrDefault(i => i.Type == "name").Value;

                LoginModel model = new LoginModel
                {
                    IsLoggedIn = User.Identity.IsAuthenticated,
                    UserName = userName
                };
                return View(model);
            }
            else
                return View(new LoginModel());
        }
    }

    public class LoginModel
    {
        public bool IsLoggedIn { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
    }
}
