using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using ranglerz_project.ModelsBO;

namespace ranglerz_project.Controllers
{
    [SessionCheck]

    public class UsersController : Controller
    {
        
        Database1Entities1 db = new Database1Entities1();
        AccountServices service = new AccountServices();
        TransactionServices servicess = new TransactionServices();

        public ActionResult Index(int id)
        {
            User user = new User();
            UserCrudServices service = new UserCrudServices();
            user = service.find(id);
            List<PinAccounts> listOfPinAccounts = new List<PinAccounts>();
            List<Pin_Accounts> list = db.Pin_Accounts.Where(x => x.User_id == id && x.User_type == "User" & x.is_active == 1).ToList();
            foreach (var account in list)
            {
                PinAccounts pin = new PinAccounts();
                pin.Name = account.Account_name;
                pin.id = account.Account_id;
                TransactionAccount transAccount = db.TransactionAccounts.Find(account.Account_id); 
                pin.Balance = (int)transAccount.balance;
                pin.Updated_Time = transAccount.updated_at_.ToShortDateString();
                listOfPinAccounts.Add(pin);
            }

            int count = listOfPinAccounts.Count;

            ViewBag.pinAccounts = listOfPinAccounts;
            
            return View(user);
        }

     

        
      

     

        
        

        

    
  
    }
}
