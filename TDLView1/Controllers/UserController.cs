using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TDLView1.Helper;
using TDLView1.Models;

namespace TDLView1.Controllers
{
    public class UserController : Controller
    {
        DbConnectionDataContext db;

        // this is main first method for connection db
        public AccountController()
        {
            if (db == null)
            {
                db = new DbConnectionDataContext();
            }
        }


        public ActionResult Registration()
        {
            return View();
        }

        // რეგისტრაციის ფორმა, ხელით გაწერილი ანოტაციებით
        [HttpPost]
        public ActionResult Registration(RegistrationViewModel registrationViewModel)
        {
            if (string.IsNullOrEmpty(registrationViewModel.Name) || string.IsNullOrEmpty(registrationViewModel.SurName) || string.IsNullOrEmpty(registrationViewModel.Email) || string.IsNullOrEmpty(registrationViewModel.Password) || string.IsNullOrEmpty(registrationViewModel.RepeatPassword))
            {
                ViewBag.error = "შეავსე ყველა ველი";
                return View();
            }

            if (!registrationViewModel.Email.Contains("@"))
            {
                ViewBag.error = "ელ-ფოსტა არ არის ვალიდური";
            }

            if (registrationViewModel.Password != registrationViewModel.RepeatPassword)
            {
                ViewBag.error = "პაროლები არ ემთხვევა";
                return View();
            }

            // კონფირმაციის გასვლამდე გავლილი რეგისტრაცია
            var notConfirmedUser = new NotConfirmedUser()
            {
                Name = registrationViewModel.Name,
                SurName = registrationViewModel.SurName,
                Email = registrationViewModel.Email,
                CreateDate = DateTime.Now,
                Password = AccountHelper.GetHash256ByString(registrationViewModel.Password + AccountHelper.AuthSecret),
                ConfirmationCode = AccountHelper.RandomString(),
                RequestIp = Request.UserHostAddress,
            };
            db.NotConfirmedUsers.InsertOnSubmit(notConfirmedUser);
            db.SubmitChanges();


            return RedirectToAction("Login");

        }

        //კონფირმაციის გავლა

        public ActionResult ConfirmationCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmationCode(string confirmationCode)
        {
            return View();
        }

        public ActionResult Confirmation(string id)
        {
            var notConfirmedUser = db.NotConfirmedUsers.FirstOrDefault(x => x.ConfirmationCode == id);

            db.Users.InsertOnSubmit(
                new User()
                {
                    Name = notConfirmedUser.Name,
                    SurName = notConfirmedUser.SurName,
                    Email = notConfirmedUser.Email,
                    Password = notConfirmedUser.Password,
                    RequestIp = notConfirmedUser.RequestIp,
                    CreateDate = DateTime.Now
                });


            db.NotConfirmedUsers.DeleteAllOnSubmit(
                db.NotConfirmedUsers.Where(x => x.Email == notConfirmedUser.Email));
            db.SubmitChanges();


            return RedirectToAction("Login");
        }


        public ActionResult Login()
        {
            return View();
        }

        //  ლოგინის ფორმა 
        [HttpPost]
        public ActionResult Login(LoginViewModel loginviewmodel)
        {
            if (string.IsNullOrEmpty(loginviewmodel.Email) || string.IsNullOrEmpty(loginviewmodel.Password))
            {
                ViewBag.error = "შეავსეთ ყველა ველი";
                return RedirectToAction("Logined");
            }



            string password = AccountHelper.GetHash256ByString(loginviewmodel.Password + AccountHelper.AuthSecret);
            var user = db.Users.FirstOrDefault(x => x.Email == loginviewmodel.Email && x.Password == password);

            if (user == null)
            {
                ViewBag.error = "ასეთი მონაცემებით მომხმარებელი არ არსებობს";
                return View();

            }
            else
            {
                Session["user"] = user;
                return RedirectToAction("Logined", "Home");
            }

            public ActionResult Logout()
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            // პაროლის აღდგენა

            public ActionResult ForgotPassword()
            {
                return View();
            }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {

            var user = db.Users.FirstOrDefault(x => x.Email == EmailID);
            var resetPasswordUserTable = new ResetPasswordUser();
            if (user == null)
            {
                return RedirectToAction("Registration");
            }
            else
            {
                resetPasswordUserTable.EmailID = user.Email;
                resetPasswordUserTable.ConfirmationCode = AccountHelper.RandomString();
                db.ResetPasswordUsers.InsertOnSubmit(resetPasswordUserTable);
                db.SubmitChanges();

                return RedirectToAction("ForgotPass");
            }


        }

        public ActionResult ForgotPass(string id)
        {

            var user = db.ResetPasswordUsers.FirstOrDefault(x => x.ConfirmationCode == id);

            if (user == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                return RedirectToAction("ResetPassword");
            }

        }
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]

        public ActionResult ResetPassword(string Email)
        {



            var RecoverPassword = db.Users.FirstOrDefault(x => x.Password == AccountHelper.GetHash256ByString(resetPasswordViewModel.Password + AccountHelper.AuthSecret));
            RecoverPassword.Password = AccountHelper.GetHash256ByString(resetPasswordViewModel.Password + AccountHelper.AuthSecret);
            db.SubmitChanges();

            return RedirectToAction("Login");

        }

    
    }
    
}