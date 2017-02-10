using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSAP.Controllers
{
    public class PartnershipController : Controller
    {
        //
        // GET: /Partnership/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PDF()
        {
            return new ActionAsPdf("Index", new { name = "Giorgio" }) { FileName = "Test.pdf" };
        }

    }
}
