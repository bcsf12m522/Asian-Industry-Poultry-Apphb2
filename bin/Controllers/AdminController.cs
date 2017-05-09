using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using Humanizer;
using ranglerz_project.Services;
using ranglerz_project.ModelsBO;
namespace ranglerz_project.Controllers
{
    [SessionCheck]
    //[OutputCache(NoStore = true, Duration = 0)]
    public class AdminController : Controller
    {
        Database1Entities1 db = new Database1Entities1();
        AccountServices service = new AccountServices();
        TransactionServices services_transactions = new TransactionServices();


       
        public ActionResult index(int id)
        {
            //bool status = services_transactions.setAllBalances();
            Admin admin = db.Admins.Find(id);
            ViewBag.totalAccounts = service.totalAccounts();
            ViewBag.totalEmployees = service.totalEmployees();
            ViewBag.totaltemporaryReports = service.totalTemporaryReports();
            List<Pin_Accounts> listOfPinAccounts = new List<Pin_Accounts>();
            listOfPinAccounts = service.findPinAccountsOfAdmin(id); 
            return View(listOfPinAccounts);
           
        }
        public ActionResult Create()
        {
            User user = new User();

            return View();
        }
        public ActionResult Details()
        {
            List<User> allUsers = db.Users.ToList();
            return View(allUsers);
        }
        public ActionResult PinAccountDetail(int id)
        {
            TransactionAccount trans = services_transactions.findTransactionAccount(id);
            return View(trans);
        }
        public ActionResult UnpinAccount(int id)
        {
            Pin_Accounts pin = service.findPinAccount(id);
            pin.is_active = 0;
            service.save();
            int identity = Convert.ToInt32(Session["adminId"]);
            return RedirectToAction("index", new {ID = identity });
        }
        
       
       
    }
}
