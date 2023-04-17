using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MVC_1.Areas_DataBase_Controllers_

{
    [Area("DataBase")]
    [Route("/database/[action]")]
    public class DbManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}