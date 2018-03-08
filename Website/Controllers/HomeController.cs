using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController :Controller
    {
        public ActionResult Index()
        {
            return View(new CompoundGraph(1000,1,5,10,25));
        }

        [HttpPost]
        public ActionResult Index(float startingCaptial , float lowInterest , float highInterest , int intervals , int periods)
        {
            CompoundGraph graph = new CompoundGraph(startingCaptial , lowInterest , highInterest , intervals , periods);
            return View(graph);
        }
    }
}