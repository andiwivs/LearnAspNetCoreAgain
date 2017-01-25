using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWorld.ViewModels;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {

        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _worldRepository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository worldRepository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _worldRepository = worldRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactVM model)
        {

            if (model.Email.Contains("aol.com"))
                ModelState.AddModelError(String.Empty, "We don't support AOL addresses");

            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From The World", model.Message);

                ModelState.Clear();

                ViewBag.UserMessage = "Message Sent OK";
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
