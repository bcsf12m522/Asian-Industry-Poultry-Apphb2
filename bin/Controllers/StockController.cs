using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using System.Web.Routing;
using ranglerz_project.ModelsBO;


namespace ranglerz_project.Controllers
{
    [SessionCheck]
    public class StockController : Controller
    {
        
        TransactionServices service = new TransactionServices();
        Database1Entities1 db = new Database1Entities1();
        StockServiceClass stockService = new StockServiceClass();
        public ActionResult Index()
        {
            //foreach (var trans in service.allTransactionaccounts())
            //{
            //    trans.code = "0" + trans.main_id + "00" + trans.head_id + "000" + trans.sub_head_id + "0000" + trans.id;
            //}
            
            service.save();

            return View(service.allTransactionaccounts());
        }    

        public ActionResult StockReport()
        {
           
            return View(service.allTransactionaccounts());
        }

       

        [HttpGet]
       
        public ActionResult StockReportPost(string search, string dateStart, string dateEnd)
        {
            
            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            var weight = service.findAccountWeight(search);
            var weighttssss = weight;
            ViewBag.openingBalance = weighttssss;
            ViewBag.openingweight = service.findOpeningWeightBeforeDates(dateStart, search);
            List<TransactionAccount> allaccounts = service.AllStockAccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);

            ViewBag.MyList = allaccounts;
            return View(accounts);
        }
           

        public ActionResult Delete(int ID, string SEARCH,string START,string END)
        {
            int id = ID;
            Transaction tr = new Transaction();
            Transaction trTo = new Transaction();
            if (id % 2 == 0)
            {
                tr = service.findTransaction(id - 1);
                trTo = service.findTransaction(id);
            }
            else
            {
                tr = service.findTransaction(id);
                trTo = service.findTransaction(id + 1);
            }

            TransactionAccount transFrom = service.findJvTransaction(tr.to_account);
            TransactionAccount transTo = service.findJvTransaction(tr.to_account);

            transFrom.balance = transFrom.balance + tr.dr;
            transTo.balance = transTo.balance - tr.cr;

            if (tr.net_weight != null)
            {
                transFrom.WEIGHT = (Convert.ToInt32(transFrom.WEIGHT) + tr.net_weight).ToString();
                transTo.WEIGHT = (Convert.ToInt32(transTo.WEIGHT) - tr.net_weight).ToString();
            }

            tr.is_active = "N";
            trTo.is_active = "N";

            service.save();

            return RedirectToAction("StockReportPost", new {search = SEARCH, dateStart = START, dateEnd = END   }); 

        }

        public ActionResult Routes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Routes( string dateStart)
        {
            DateTime date = Convert.ToDateTime(dateStart);
            
            return View(stockService.routeReports(date));
        }


        public ActionResult RoutesOil()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RoutesOil(string dateStart)
        {
            DateTime date = Convert.ToDateTime(dateStart);

            return View(stockService.routeReports(date));
        }
    }
}
