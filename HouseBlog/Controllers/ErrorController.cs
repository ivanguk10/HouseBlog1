using HouseBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HouseBlog.Controllers
{
    public class Error : Controller 
    {
        private readonly ILogger<Error> _logger;

        public Error(ILogger<Error> loggerr)
        {
            _logger = loggerr;
        }

        public IActionResult Index(int? statusCode = null)
        {
            ViewData["Error"] = statusCode;
            _logger.LogError($"Error. Status code {statusCode}");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ErrorPro()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
