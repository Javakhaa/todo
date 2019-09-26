using System;
using System.Linq;
using System.Web.Mvc;
using TDLView1.Helper;
using TDLView1.Models;

namespace TDLView1.Controllers
{
    public class UserController : Controller
    {

        // this is main first method for connection db
        public UserController()
        {
        }


        public ActionResult Registration()
        {
            return View();
        }

        // რეგისტრაციის ფორმა, ხელით გაწერილი ანოტაციებით
        [HttpPost]
        public ActionResult Registration(RegistrationViewModel registrationViewModel)
        {
            if (string.IsNullOrEmpty(registrationViewModel.Name)
                || string.IsNullOrEmpty(registrationViewModel.Lastname)
                || string.IsNullOrEmpty(registrationViewModel.Username)
                || string.IsNullOrEmpty(registrationViewModel.Password)
                || string.IsNullOrEmpty(registrationViewModel.RepeatPassword))
            {
                ViewBag.error = "შეავსე ყველა ველი";
                return View();
            }


            if (registrationViewModel.Password != registrationViewModel.RepeatPassword)
            {
                ViewBag.error = "პაროლები არ ემთხვევა";
                return View();
            }

            // კონფირმაციის გასვლამდე გავლილი რეგისტრაცია
            var notConfirmedUser = new User()
            {
                Username = registrationViewModel.Username,
                Name = registrationViewModel.Name,
                Lastname = registrationViewModel.Lastname,
                CreateDate = DateTime.Now,
                PasswordHash = AccountHelper.GetHash256ByString(registrationViewModel.Password + AccountHelper.AuthSecret),
            };

            using (var db = new DbConnectionDataContext())
            {
                db.Users.InsertOnSubmit(notConfirmedUser);
                db.SubmitChanges();
            }


            return RedirectToAction("Login");

        }

        //კონფირმაციის გავლა


        public ActionResult Login()
        {
            return View();
        }

        //  ლოგინის ფორმა 
        [HttpPost]
        public ActionResult Login(LoginViewModel loginviewmodel)
        {
            if (string.IsNullOrEmpty(loginviewmodel.Username) || string.IsNullOrEmpty(loginviewmodel.Password))
            {
                ViewBag.error = "შეავსეთ ყველა ველი";
                return RedirectToAction("Login");
            }

            string passwordHash = AccountHelper.GetHash256ByString(loginviewmodel.Password + AccountHelper.AuthSecret);
            User user = null;

            using (var db = new DbConnectionDataContext())
            {
                user = db.Users.SingleOrDefault(x => x.Username == loginviewmodel.Username && x.PasswordHash == passwordHash);
            }


            if (user == null)
            {
                ViewBag.error = "ასეთი მონაცემებით მომხმარებელი არ არსებობს";
                return View();

            }
            else
            {
                Session["user"] = user;
                return RedirectToAction("Organize", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

        // პაროლის აღდგენა

        public ActionResult ForgotPassword()
        {
            return View();
        }


        public ActionResult ResetPassword()
        {
            return View();
        }
    }

}