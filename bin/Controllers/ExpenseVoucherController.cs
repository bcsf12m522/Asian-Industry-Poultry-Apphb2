using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.ModelsBO;
using ranglerz_project.Services;

namespace ranglerz_project.Controllers
{

      [SessionCheck]
    public class ExpenseVoucherController : Controller
    {

        //
        // GET: /ExpenseVoucher/
        TransactionServices service = new TransactionServices();
        AccountServices serviceAccount = new AccountServices();
       

        public ActionResult Index()
        {

            ViewBag.from = service.allTransactionaccountsForExpense();
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "EV00" + num;
            return View(serviceAccount.findMainAcc(4));
        }

        
        

        public ActionResult ExpensePost(int id)
        {
            TemporaryReport temp = service.findTemporaryReport(id);
            TemporaryReport tempTo = service.findTemporaryReport(id+1);
            int transAccID = temp.trans_acc_id;
            int subHeadID = 0;
            int headID = 0;
            string from = temp.from_account;
            string to = temp.to_account;
            try
            {
                int amount = (int)temp.amount;

              
              
              
                
                String session = Session["name"].ToString();


                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
            
                Transaction tr = new Transaction();
                Transaction transTo = new Transaction();

                tr.created_at = temp.created_at;
                tr.code = trans_from.id;
                tr.description = temp.description;
                tr.dr = amount;
                tr.cr = 0;
                tr.balance = (trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "Y";

                tr.from_account = from;
                tr.to_account = trans_to.name;
                tr.status ="1";
                tr.voucher_type = "EV";
                tr.trans_acc_id = trans_from.id;
              //  tr.sub_head_id = trans_from.sub_head_id;
              //  tr.head_id = trans_from.head_id;
              //  tr.main_id = trans_from.main_id;
                if (session != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    tr.supervisor = user.name;
                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    tr.supervisor = Session["username"].ToString();
                }


                tr.user_id = temp.user_id;
                tr.admin_id = temp.admin_id;


                tr.voucher_code = temp.voucher_code;
                tr.to_account = trans_to.name;
                service.addTransaction(tr);
                trans_from.balance =(int) tr.balance;
                tr.unique_key = trans_from.id;
                temp.is_active = "N";
                //service.save();




                transTo.created_at = tempTo.created_at;
                transTo.code = trans_to.id;
                transTo.description = tempTo.description;
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = tempTo.updated_at;
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = trans_to.name;
                transTo.status = "1";
                transTo.voucher_type = "EV";
        
                transTo.trans_acc_id = trans_to.id;
               // transTo.sub_head_id = trans_to.sub_head_id;
               // transTo.head_id = trans_to.head_id;
               // transTo.main_id = trans_to.main_id;
                if (session != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    transTo.supervisor = user.name;
                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    transTo.supervisor = Session["username"].ToString();
                }


                transTo.user_id = temp.user_id;
                transTo.admin_id = temp.admin_id;

             
                // End //
                transTo.voucher_code = tr.voucher_code;
                transTo.to_account = tr.to_account;
                service.addTransaction(transTo);
                trans_to.balance =(int) transTo.balance;
                transTo.unique_key = tr.unique_key;
                tempTo.is_active = "N";
                trans_from.updated_at_ = tr.updated_at;
                trans_to.updated_at_ = tr.updated_at;
                //trans_from.cr_ = trans_from.cr_ + amount;
                //trans_to.dr_ = trans_to.dr_ + amount;
                tr.Last_Edit = "None";
                transTo.Last_Edit = "None";

 

                service.save();


                service.balanceUpdation(trans_from.id, trans_to.id);


                return RedirectToAction("Supervision","USER_CRUD");

            } 

           catch
            {

                return RedirectToAction("Index");
            }

           
           
      
        }

        
        

        public ActionResult ExpenseReports()
        {
            int check = (int)Session["expense_reports"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.headAccounts = service.allHeadAccounts();
            ViewBag.subHeadAccounts = service.allSubHeadAccounts();
            return View(service.allTransactionaccounts());
        }

        
        

        [HttpPost]
        public ActionResult ExpenseReportsPost(string search, string dateStart, string dateEnd)
        {
            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForExpenses(PermissionForAccounts.permisionExpenses, search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }


            int check = (int)Session["expense_reports"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            DateTime date = Convert.ToDateTime(dateStart);
            var balance = service.findOpeningBalancebeforeDate(date, search);
            var balance2 = service.findAccountBalance(search);
            ViewBag.openingBalance = balance + balance2;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
            return View(accounts);
        }


        [HttpPost]
        public ActionResult ExpenseReportsPostHead(string search, string dateStart, string dateEnd)
        {
            HeadAccount accounts = service.findHeadAccounts(search);
         //   var balance = service.findAccountBalance(search);
         //   ViewBag.openingBalance = balance;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.headAccounts = service.allHeadAccounts();
            return View(accounts);
        }


        [HttpPost]
        public ActionResult ExpenseReportsPostSub(string search, string dateStart, string dateEnd)
        {
            SubHeadAccount accounts = service.findSubHeadAccounts(search);
          //  var balance = service.findAccountBalance(search);
          //  ViewBag.openingBalance = balance;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.subHeadAccounts = service.allSubHeadAccounts();
            return View(accounts);
        }

        //
        // GET: /ExpenseVoucher/Edit/5

        public ActionResult Edit(int id)
        {
            int allReports = Convert.ToInt32(Session["expense_reports_edit"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }


            Transaction transEdit = service.findTransaction(id);
            ViewBag.MyList = service.allTransactionaccounts();
            DateTime Date = Convert.ToDateTime(transEdit.created_at);
            ViewBag.date = Date.ToShortDateString();
            return View(transEdit);
        }

        [HttpPost]

        public ActionResult Edit(int id, FormCollection collection)
        {
            // For User Ristriction 
            int allReports = Convert.ToInt32(Session["all_reports_edit"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }


            Database1Entities1 db = new Database1Entities1();


            int amount = Convert.ToInt32(collection["amount"]);
            string fromAccount = collection["from"];
            string toAccount = collection["to"];



            TransactionAccount fromAccountTrans = db.TransactionAccounts.Where(x => x.name == fromAccount && x.is_active == "Y").First();
            TransactionAccount toAccountTrans = db.TransactionAccounts.Where(x => x.name == toAccount && x.is_active == "Y").First();

            Transaction transOdd = new Transaction();
            Transaction transEven = new Transaction();

            if (id % 2 == 0)
            {
                transOdd = db.Transactions.Find(id - 1);
                transEven = db.Transactions.Find(id);
            }
            else
            {
                transOdd = db.Transactions.Find(id);
                transEven = db.Transactions.Find(id + 1);
            }

            transOdd.trans_acc_id = fromAccountTrans.id;
            transOdd.dr = amount;
            transOdd.cr = 0;
            transEven.trans_acc_id = toAccountTrans.id;
            transEven.cr = amount;
            transEven.dr = 0;

            
            transOdd.code = transOdd.trans_acc_id;
            transEven.code = transEven.trans_acc_id;

            transOdd.from_account = fromAccountTrans.name;
            transEven.from_account = fromAccountTrans.name;
            transOdd.to_account = toAccountTrans.name;
            transEven.to_account = toAccountTrans.name;

            transOdd.description = collection["description"];
            transEven.description = transOdd.description;
            transOdd.updated_at = Convert.ToDateTime(collection["date"]);
            //transEven.updated_at = Convert.ToDateTime(collection["DebitDate"]);
            transEven.updated_at = transOdd.updated_at;
            transOdd.Last_Edit = Session["username"].ToString();
            transEven.Last_Edit = Session["username"].ToString();
            if (transOdd.voucher_type == "WEV")
            {
                transOdd.net_weight = Convert.ToInt32(collection["weight"]);
                transEven.net_weight = transOdd.net_weight;
            }
            db.SaveChanges();

            service.balanceUpdation(fromAccountTrans.id, toAccountTrans.id);

            db.SaveChanges();

            return RedirectToAction("ExpenseReports");

        }
       
  

        
   

       


    }
}
