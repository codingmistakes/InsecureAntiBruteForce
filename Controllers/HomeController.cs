using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CaptchaReplay.Models;
using CaptchaReplay.Utility;
using Microsoft.AspNetCore.Http;

namespace CaptchaReplay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string FAILED_ATTEMPTS = "failed_attempts";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string captcha)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return View("Index");
            }

            ViewBag.Username = username;

            // check possible brute-force attacks
            int failedtries = 1;

            if (HttpContext.Session.GetInt32(FAILED_ATTEMPTS).HasValue)
            {
                failedtries = HttpContext.Session.GetInt32(FAILED_ATTEMPTS).Value;
            }
            else
            {
                HttpContext.Session.SetInt32(FAILED_ATTEMPTS, 0);
            }

            if (failedtries >= 2)
            {
                if (Authenticator.CheckCaptcha(captcha))
                {
                    if (Authenticator.Authenticate(username, password))
                    {
                        HttpContext.Session.SetInt32(FAILED_ATTEMPTS, 0);
                        return View("Success");
                    }
                    else
                    {
                        ViewBag.ShowCaptcha = true;
                        ViewBag.Result = "Username or password is wrong, please enter again!";
                    }
                }
                else
                {
                    ViewBag.ShowCaptcha = true;
                    ViewBag.Result = "Please enter correct CAPTCHA!";
                }
            }
            else
            {
                if (Authenticator.Authenticate(username, password))
                {
                    HttpContext.Session.SetInt32(FAILED_ATTEMPTS, 0);
                    return View("Success");
                }
                else
                {
                    failedtries = HttpContext.Session.GetInt32(FAILED_ATTEMPTS).Value;
                    HttpContext.Session.SetInt32(FAILED_ATTEMPTS, ++failedtries);
                    ViewBag.Result = "Username or password is wrong, please enter again!";
                }
            }

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
