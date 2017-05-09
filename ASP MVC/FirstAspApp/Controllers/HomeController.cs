using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FirstAspApp.Models;

namespace FirstAspApp.Controllers
{
    public class HomeController : Controller
    {
        ViewModel vm = new ViewModel() { Name = "Max" };

        public ActionResult Index()
        {
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(string name)
        {
            vm.Name = name;
            return View(vm);
        }
    }
}
