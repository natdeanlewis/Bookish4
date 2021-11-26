﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bookish4._DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bookish4.Web2.Models;

namespace Bookish4.Web2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Console.WriteLine();
            // LibraryDatabaseClient.GetAllLoans(1);
            // Console.WriteLine();
            // LibraryDatabaseClient.Search("am");
            // Console.WriteLine();
            // LibraryDatabaseClient.CheckAvailability(4);
            var list = LibraryDatabaseClient.GetAllBooks();

            var viewModels = list.Select(b => new BookViewModel(b)).ToList();
            return View(viewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}