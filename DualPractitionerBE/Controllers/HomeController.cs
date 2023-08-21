using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    [Route("api/home/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _service;

        /// <summary>
        /// Constructor for Home controller
        /// </summary>
        public HomeController(ILogger<HomeController> logger, IServiceProvider service)
        {
            _service = service;
            _logger = logger;
        }
        ///// <summary>
        ///// Index
        ///// </summary>
        //[HttpGet("index")]
        //public IActionResult Index()
        //{
        //    return View();
        //}
        ///// <summary>
        ///// Home
        ///// </summary>
        //[HttpGet("home")]
        //public IActionResult Home()
        //{
        //    return View();
        //}

        //[HttpGet("seha-account/login")]
        //public IActionResult SehaLogin(int Id)
        //{
        //    return RedirectToAction("Login", "SehaAccount", new { id = Id});
        //}
    }
}
