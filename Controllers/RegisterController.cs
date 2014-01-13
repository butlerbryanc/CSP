using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using CSP.Data;
using System.Data.Entity.Validation;

namespace CSP.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Register/Details/5

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]

        public ActionResult LogIn(CSP.Data.User user)
        {
            if(IsValid(user.email, user.password))
            {
                FormsAuthentication.SetAuthCookie(user.email, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login details were wrong.");
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(CSP.Data.User model)
        {
                if (ModelState.IsValid)
                {
                    CSPEntities context = new CSPEntities();
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encrypPass = crypto.Compute(model.password);
                    User newUser = new User();
                    newUser.email = model.email;

                    newUser.password = encrypPass;
                    newUser.password_salt = crypto.Salt;
                    newUser.first_name = model.first_name;
                    newUser.last_name = model.last_name;
                    context.Users.Add(newUser);
                    context.SaveChanges();
                    return RedirectToAction("Thanks", model);  
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct.");
                    return View(model);
                }
    
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsValid = false;

            CSPEntities context = new CSPEntities();

            IEnumerable<User> customQuery =
                from user in context.Users
                where user.email == email
                select user;

            if (customQuery != null)
            {
                foreach (User u in customQuery)
                {
                    if (u.password == crypto.Compute(password, u.password_salt))
                    {
                        IsValid = true;
                    }
                }

            }

            return IsValid;
            


        }   
      
        public ActionResult Thanks(User model)
        {
            return View(model);
        }

    }
}
