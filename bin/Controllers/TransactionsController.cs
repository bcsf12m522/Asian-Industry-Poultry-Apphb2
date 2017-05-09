using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Services;
using ranglerz_project.Models;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using ranglerz_project.ModelsBO;
using System.Web.Helpers;
using PdfSharp;
using System.IO;

namespace ranglerz_project.Controllers
{
    [SessionCheck]

   

    public class TransactionsController : Controller
    {
        TransactionServices service = new TransactionServices();

        #region Except PDF
        public ActionResult index()
        {
            return View(service.allTransactions());
        
        }

        // Jounal Voucher Module //
        public ActionResult journal()
        {

   

      
            int check = (int)Session["JV"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "JV00" + num;
            return View(service.allTransactionaccounts());
        }


        public ActionResult journalPost(int id)
        {

            if (service.checkIDTrans() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }





            int check = (int)Session["JV"];

            if (check == 0)
            {
                return RedirectToAction("Index","Home");
            }

            TemporaryReport temp = service.findTemporaryReport(id);
            TemporaryReport tempTo = service.findTemporaryReport(id + 1);


            int amount = (int)temp.amount;
            string from = temp.from_account;
            string to = temp.to_account;


            try
            {
                

                String session = Session["name"].ToString();
                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
                Transaction tr = new Transaction();
                Transaction transTo = new Transaction(); ;

                tr.created_at = temp.created_at;
                tr.code = trans_from.id;
                tr.description = temp.description;
                tr.dr = amount;
                tr.cr = 0;
                tr.balance = (trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "Y";

                tr.from_account = from;
                tr.to_account = to;
                tr.status = "Journal voucher added";
                tr.voucher_type = "JV";
                tr.voucher_code = "JV111";
                tr.trans_acc_id = trans_from.id;
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


                tr.quantity = "Not Availabe";

                service.addTransaction(tr);
                temp.is_active = "N";
                //service.save();
                //trans_from.balance = tr.balance;
                
               // service.save();




                transTo.created_at = tempTo.created_at;
                transTo.code = trans_to.id;
                transTo.description = tempTo.description;
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = tempTo.updated_at;
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = to;
                transTo.status = "Journal voucher added";
                transTo.voucher_type = "JV";
                transTo.voucher_code = "JV111";
                transTo.trans_acc_id = trans_to.id;
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
                
                transTo.quantity = "Not Availabe";
                // End //


                service.addTransaction(transTo);
                tempTo.is_active = "N";



                ///// New change /////

                tr.unique_key = trans_from.id;
                tr.voucher_code = temp.voucher_code;

                // Last Edit //

                tr.Last_Edit = "None";
                transTo.Last_Edit = "None";

                /////////////////

                service.save();

                trans_to.balance = transTo.balance;
                transTo.unique_key = tr.unique_key;
                transTo.voucher_code = tr.voucher_code;
                trans_from.updated_at_ = tr.updated_at;
                trans_to.updated_at_ = tr.updated_at;
                //trans_from.cr_ = trans_from.cr_ + amount;
                //trans_to.dr_ = trans_to.dr_ + amount;

                service.save();

                service.balanceUpdation(trans_from.id, trans_to.id);



                return RedirectToAction("Supervision","USER_CRUD");

            }

               catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                validationErrors.Entry.Entity.GetType().FullName,
                                validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }
                    return RedirectToAction("Supervision","USER_CRUD");
                }




        }





        // Jounal Voucher END //



        // Report Module //

        public ActionResult history()
        {
            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(service.allTransactionaccounts());
        }
        [HttpGet]
        public ActionResult historyPost(string search, string dateStart, string dateEnd, string submitValue)
        {
            if (submitValue == "Search And Print")
            {
               return  RedirectToAction("ledgersPrint", new { Search = search, DateEnd = dateEnd, DateStart = dateStart });
            }

            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForLedgers(PermissionForAccounts.permisionLedgers,search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }

            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            DateTime date =  Convert.ToDateTime(dateStart);
            var balance = service.findOpeningBalancebeforeDate(date, search);
            var balance2 = service.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            ViewBag.firstOpeningbalance = balance2;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);

            ViewBag.MyList = allaccounts;
            return View(accounts);
        }


        ////// Print ///////////////

        public ActionResult ledgersPrint(string search, string dateStart, string dateEnd)
        {
            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForLedgers(PermissionForAccounts.permisionLedgers, search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }

            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            DateTime date = Convert.ToDateTime(dateStart);
            var balance = service.findOpeningBalancebeforeDate(date, search);
            var balance2 = service.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            ViewBag.firstOpeningbalance = balance2;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
             
            //// for Account Code //

            TransactionAccount accountTrans = service.findJvTransaction(search);
            ViewBag.AccountCode = accountTrans.code;
            ViewBag.MainAccountCode = accountTrans.MainAccount.code;
            ViewBag.MainTitle = accountTrans.MainAccount.name;

            //// for Dates ///

            ViewBag.FromDate = Convert.ToDateTime(dateStart).ToShortDateString();
            ViewBag.ToDate = Convert.ToDateTime(dateEnd).ToShortDateString();

            return View(accounts);
        }








        ///////////////////////////


        // Report Module End  //


        // Transaction Edit Module //

        #region Edit


        public ActionResult Edit(int id)
        {
            int allReports = Convert.ToInt32(Session["all_reports_edit"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            Transaction transEdit2 = new Transaction();
            Transaction transEdit = new Transaction();
            if (id % 2 != 0)
            {
                transEdit = service.findTransaction(id);
                transEdit2 = service.findTransaction(id + 1);
            }
            else
            {
                transEdit = service.findTransaction(id - 1);
                transEdit2 = service.findTransaction(id);
            }

            // for Debit date //


            ViewBag.debitDate = transEdit2.created_at;

            ///////////////////

            if (transEdit.voucher_type == "SV" || transEdit.voucher_type == "PV" || transEdit.voucher_type == "UPSV" || transEdit.voucher_type == "UPPV")
            {
                return RedirectToAction("Edit", "SaleVoucher", new { id = id });
            }
            else
            {


                ViewBag.MyList = service.allTransactionaccounts();

                return View(transEdit);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            

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
            TransactionAccount toAccountTrans = db.TransactionAccounts.Where(x=>x.name==toAccount && x.is_active=="Y").First();

            Transaction transOdd = new Transaction();
            Transaction transEven = new Transaction();

            if(id%2==0)
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

            if(transOdd.voucher_type=="BP" && transEven.voucher_type=="BP")
            {
                transOdd.cheque_number = collection["chequeNo"];
                transEven.cheque_number = transOdd.cheque_number;
                string chequeType = collection["chequeType"];
                
                if(chequeType=="cross")
                {
                    transOdd.cross = chequeType;
                    transEven.cross = chequeType;
                    transOdd.cash = null;
                    transEven.cash = null;
                }
                else if(chequeType=="cash")
                {
                    transOdd.cash = chequeType;
                    transEven.cash = chequeType;
                    transOdd.cross = null;
                    transEven.cross = null;

                }

                transOdd.tax_orignal = float.Parse(collection["tax"]);
                transOdd.tax = (int)((transOdd.tax_orignal * amount) / 100);
                transEven.tax_orignal = transOdd.tax_orignal;
                transEven.tax = transOdd.tax;

                Transaction transEvenTax = db.Transactions.Find(transEven.Id + 2);
                Transaction transOddTax = db.Transactions.Find(transOdd.Id +2);

                if (transEvenTax.description == "WHT" && transOddTax.description == "WHT")
                {

                    transOddTax.trans_acc_id = transOdd.trans_acc_id;
                    transOddTax.cheque_number = transOdd.cheque_number;
                    transOddTax.cash = transOdd.cash;
                    transOddTax.cross = transOdd.cross;
                    transOddTax.dr = (int)(transOdd.tax);
                    transOddTax.cr = transOdd.cr;

                    transEvenTax.trans_acc_id = transEven.trans_acc_id;
                    transEvenTax.cheque_number = transEven.cheque_number;
                    transEvenTax.cash = transEven.cash;
                    transEvenTax.cross = transEven.cross;
                    transEvenTax.cr = (int)(transEven.tax);
                    transEvenTax.dr = transEven.dr;

                    transOddTax.updated_at = Convert.ToDateTime(collection["date"]);
                    transEvenTax.updated_at = Convert.ToDateTime(collection["Debitdate"]);
                    transOddTax.created_at = transOddTax.updated_at;
                    transEvenTax.created_at = transEvenTax.updated_at;
                    transOddTax.from_account = fromAccountTrans.name;
                    transEvenTax.from_account = fromAccountTrans.name;
                    transOddTax.to_account = toAccountTrans.name;
                    transEvenTax.to_account = toAccountTrans.name;

                    transOddTax.Last_Edit = Session["username"].ToString();
                    transEvenTax.Last_Edit = Session["username"].ToString();

                    transOddTax.code = transOddTax.trans_acc_id;
                    transEvenTax.code = transEvenTax.trans_acc_id;
                }
            }

            transOdd.code = transOdd.trans_acc_id;
            transEven.code = transEven.trans_acc_id;

            transOdd.from_account = fromAccountTrans.name;
            transEven.from_account = fromAccountTrans.name;
            transOdd.to_account = toAccountTrans.name;
            transEven.to_account = toAccountTrans.name;

            transOdd.description = collection["description"];
            transEven.description = transOdd.description;
            transOdd.updated_at =Convert.ToDateTime(collection["date"]);
            transEven.updated_at = Convert.ToDateTime(collection["DebitDate"]);
            transOdd.created_at = transOdd.updated_at;
            transEven.created_at = transEven.updated_at;
            transOdd.Last_Edit = Session["username"].ToString();
            transEven.Last_Edit = Session["username"].ToString();
            if(transOdd.voucher_type =="WEV")
            {
                transOdd.net_weight = Convert.ToInt32(collection["weight"]);
                transEven.net_weight = transOdd.net_weight;
            }
            db.SaveChanges();
           
            service.balanceUpdation(fromAccountTrans.id, toAccountTrans.id);

            return RedirectToAction("history");

        }



        // Transaction Edit Module End  //

        #endregion

        public ActionResult bank()
        {
            int check = (int)Session["BR"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "BV00" + num;

            return View(service.allTransactionaccounts());
        }

        public ActionResult bankPost(int id)
        {
            if (service.checkIDTrans() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }




            int check = (int)Session["BR"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }




            TemporaryReport temp = service.findTemporaryReport(id);
            TemporaryReport tempTo = service.findTemporaryReport(id + 1);
            int amount = (int)temp.amount;
            string from = temp.from_account;
            string to = temp.to_account;
            try
            {
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
                tr.to_account = to;
                tr.status = "Bank voucher added";
                tr.voucher_type = "BV";
                tr.voucher_code = "BV111";
                tr.trans_acc_id = trans_from.id;
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


                tr.admin_id = temp.admin_id;
                tr.user_id = temp.user_id;
                tr.quantity = "Not Availabe";

                service.addTransaction(tr);
                //trans_from.balance = tr.balance;
                tr.unique_key = trans_from.id;
                tr.voucher_code = temp.voucher_code;
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
                transTo.to_account = to;
                transTo.status = "Bank voucher added";
                transTo.voucher_type = "BV";
                transTo.voucher_code = "BV111";
                transTo.trans_acc_id = trans_to.id;
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
                transTo.quantity = "Not Availabe";
                // End //

                service.addTransaction(transTo);
                trans_to.balance = transTo.balance;
                transTo.unique_key = tr.unique_key;
                transTo.voucher_code = tr.voucher_code;
                tempTo.is_active = "N";
                trans_from.updated_at_ = tr.updated_at;
                trans_to.updated_at_ = tr.updated_at;
                //trans_from.cr_ = trans_from.cr_ + amount;
                //trans_to.dr_ = trans_to.dr_ + amount;

                tr.Last_Edit = "None";
                transTo.Last_Edit = "None";

 

                service.save();

                service.balanceUpdation(trans_from.id, trans_to.id);

                return RedirectToAction("Supervision", "USER_CRUD");

            }

            catch
            {

                return RedirectToAction("Supervision", "USER_CRUD");
            }




        }
        public ActionResult bankPayment()
        {
            int check = (int)Session["BP"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            int num = TransactionServices.getMaxId();
            ViewBag.Code = "BP00" + num;

            return View(service.allTransactionaccounts());
        }

        public ActionResult bankPaymentPost(int id)
        {
            {
                if (service.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }


                int check = (int)Session["BP"];

                if (check == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                TemporaryReport temp = service.findTemporaryReport(id);
                TemporaryReport tempTo = service.findTemporaryReport(id + 1);
                TemporaryReport tempTax = service.findTemporaryReport(id + 2);
                TemporaryReport tempToTax = service.findTemporaryReport(id + 3);

                int amount = (int)temp.amount;
                string from = temp.from_account;

                string to = temp.to_account;
                try
                {
                    String session = Session["name"].ToString();


                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    Transaction tr = new Transaction();
                    Transaction transTo = new Transaction();

                    tr.created_at = temp.created_at;
                    tr.code = trans_from.id;
                    tr.description = temp.description;
                    tr.dr =(int)(temp.dr);
                    tr.cr = 0;
                    tr.balance = (trans_from.balance - tr.dr);
                    trans_from.balance = tr.balance;
                    tr.updated_at = tr.created_at;
                    tr.tax = temp.tax;
                    tr.is_active = "Y";

                    tr.from_account = from;
                    tr.to_account = to;
                    tr.status = "Bank Payment voucher added";
                    tr.voucher_type = "BP";
                    tr.voucher_code = "BP111";
                    tr.trans_acc_id = trans_from.id;
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

                    tr.quantity = "Not Availabe";

                    tr.cheque_no = temp.cheque_no;
                    tr.cash = temp.cash;  
                    tr.cross = temp.cross;
                    tr.extra = temp.extra;
                    tr.tax = temp.tax;
                    tr.tax_orignal = temp.tax_orignal;
                    service.addTransaction(tr);
                    tr.unique_key = trans_from.id;
                    tr.voucher_code = temp.voucher_code;
                    temp.is_active = "N";
              




                    transTo.created_at = tempTo.created_at;
                    transTo.code = trans_to.id;
                    transTo.description = tempTo.description;
                    transTo.dr = 0;
                    transTo.cr =(int) (tempTo.cr);
                    transTo.balance = (trans_to.balance + tempTo.cr);
                    trans_to.balance = transTo.balance;
                    transTo.updated_at = tempTo.updated_at;
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.tax = tr.tax;
                    transTo.status = "Bank Payment voucher added";
                    transTo.voucher_type = "BP";
                    transTo.voucher_code = "BP111";
                    transTo.trans_acc_id = trans_to.id;

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

                    transTo.quantity = "Not Availabe";
                    transTo.cheque_no = tempTo.cheque_no;
                    transTo.cash = tempTo.cash;
                   
               
                    transTo.cross = tempTo.cross;
     
                    transTo.extra = tempTo.extra;

                    // End //

                    service.addTransaction(transTo);
                    transTo.unique_key = tr.unique_key;
                    transTo.tax = tr.tax;
                    transTo.voucher_code = tr.voucher_code;
                    transTo.tax_orignal = temp.tax_orignal;
                    tempTo.is_active = "N";

                    tr.Last_Edit = "None";
                    transTo.Last_Edit = "None";

 


                    service.save();

                    /////////////////////////////////////////////

                    Transaction trTax = new Transaction();
                    Transaction transToTax = new Transaction();

                    trTax.created_at = tempTax.created_at;
                    trTax.code = trans_from.id;
                    trTax.description = tempTax.description;
                    trTax.dr = (int)(tempTax.dr);
                    trTax.cr = 0;
                    trTax.balance = (trans_from.balance - trTax.dr);
                    trans_from.balance = trTax.balance;
                    trTax.updated_at = trTax.created_at;
                    trTax.tax = tempTax.tax;
                    trTax.is_active = "Y";

                    trTax.from_account = from;
                    trTax.to_account = to;
                    trTax.status = "Bank Payment voucher added";
                    trTax.voucher_type = "BP";
                    trTax.voucher_code = "BP111";
                    trTax.trans_acc_id = trans_from.id;

                    if (session != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        trTax.supervisor = user.name;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        trTax.supervisor = Session["username"].ToString();
                    }


                    trTax.user_id = temp.user_id;
                    trTax.admin_id = temp.admin_id;

                    trTax.quantity = "Not Availabe";

                    trTax.cheque_no = tempTax.cheque_no;
                    trTax.cash = tempTax.cash;
                    trTax.cross = tempTax.cross;
                    trTax.extra = tempTax.extra;
                    trTax.tax = tr.tax;
                    trTax.tax_orignal = temp.tax_orignal;
                    trTax.voucher_code = tr.voucher_code;
                    service.addTransaction(trTax);
                    trTax.unique_key = trans_from.id;
                    tempTax.is_active = "N";
                    


                    transToTax.created_at = tempToTax.created_at;
                    transToTax.code = trans_to.id;
                    transToTax.description = tempToTax.description;
                    transToTax.dr = 0;
                    transToTax.cr = (int)(tempToTax.cr);
                    transToTax.balance = (trans_to.balance + tempToTax.cr);
                    trans_to.balance = transToTax.balance;
                    transToTax.updated_at = tempToTax.updated_at;
                    transToTax.is_active = "Y";

                    transToTax.from_account = from;
                    transToTax.to_account = to;
                    transToTax.tax = trTax.tax;
                    transToTax.status = "Bank Payment voucher added";
                    transToTax.voucher_type = "BP";
                    transToTax.voucher_code = "BP111";
                    transToTax.trans_acc_id = trans_to.id;

                    if (session != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transToTax.supervisor = user.name;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transToTax.supervisor = Session["username"].ToString();
                    }


                    transToTax.user_id = temp.user_id;
                    transToTax.admin_id = temp.admin_id;

                    transToTax.quantity = "Not Availabe";
                    transToTax.cheque_no = tempToTax.cheque_no;
                    transToTax.cash = tempToTax.cash;


                    transToTax.cross = tempToTax.cross;

                    transToTax.extra = tempToTax.extra;

                   
                    transToTax.tax_orignal = temp.tax_orignal;
                    transToTax.tax = tr.tax;
                    transToTax.voucher_code = tr.voucher_code;
                    service.addTransaction(transToTax);
                    transToTax.unique_key = trTax.unique_key;
                    tempToTax.is_active = "N";
                    trans_to.updated_at_ = tr.updated_at;
                    trans_from.updated_at_ = tr.updated_at;
                    //trans_from.cr_ = trans_from.cr_ + amount;
                    //trans_to.dr_ = trans_to.dr_ + amount;
                    tr.cheque_number = temp.cheque_number;
                    transTo.cheque_number = temp.cheque_number;
                    trTax.cheque_number = temp.cheque_number;
                    transToTax.cheque_number = temp.cheque_number;


                    trTax.Last_Edit = "None";
                    transToTax.Last_Edit = "None";

 
                   
                    service.save();


                    service.balanceUpdation(trans_from.id, trans_to.id);


                    ////////////////////////////////////////////

                    return RedirectToAction("Supervision", "USER_CRUD");

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                validationErrors.Entry.Entity.GetType().FullName,
                                validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }

                    return RedirectToAction("Supervision", "USER_CRUD");
                }

                




            }
        }


        public ActionResult cashPayment()
        {
            int check = (int)Session["CP"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "CP00" + num;
            return View(service.allTransactionaccounts());
        }
        public ActionResult cashPaymentPost(int id)
        {
            {
                if (service.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }


                int check = (int)Session["CP"];

                if (check == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                TemporaryReport temp = service.findTemporaryReport(id);
                TemporaryReport tempTo = service.findTemporaryReport(id + 1);
                int amount = (int)temp.amount;
                string from = temp.from_account;
                string to = temp.to_account;
                try
                {
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
                    tr.to_account = to;
                    tr.status = "Cash Payment voucher added";
                    tr.voucher_type = "CP";
                    tr.voucher_code = "CP111";
                    tr.trans_acc_id = trans_from.id;
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

                    tr.quantity = "Not Availabe";

                    service.addTransaction(tr);
                    trans_from.balance = tr.balance;
                    tr.unique_key = trans_from.id;
                    tr.voucher_code = temp.voucher_code;
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
                    transTo.to_account = to;
                    transTo.status = "Cash Payment voucher added";
                    transTo.voucher_type = "CP";
                    transTo.voucher_code = "CP111";
                    transTo.trans_acc_id = trans_to.id;
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
                    transTo.quantity = "Not Availabe";
                    // End //

                    service.addTransaction(transTo);
                    trans_to.balance = transTo.balance;
                    transTo.unique_key = tr.unique_key;
                    transTo.voucher_code = tr.voucher_code;
                    tempTo.is_active = "N";
                    trans_from.updated_at_ = tr.updated_at;
                    trans_to.updated_at_ = tr.updated_at;
                    //trans_from.cr_ = trans_from.cr_ + amount;
                    //trans_to.dr_ = trans_to.dr_ + amount;

                    tr.Last_Edit = "None";
                    transTo.Last_Edit = "None";

 

                    service.save();


                    service.balanceUpdation(trans_from.id, trans_to.id);



                    return RedirectToAction("Supervision", "USER_CRUD");

                }

                catch
                {

                    return RedirectToAction("Supervision", "USER_CRUD");
                }




            }
        }

        public ActionResult cashReceive()
        {
            int check = (int)Session["CR"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            int num = TransactionServices.getMaxId();
            ViewBag.Code = "CR00" + num;

            return View(service.allTransactionaccounts());
        }

        public ActionResult cashReceivedPost(int id)
        {
            {

                if (service.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }

                int check = (int)Session["CR"];

                if (check == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                TemporaryReport temp = service.findTemporaryReport(id);
                TemporaryReport tempTo = service.findTemporaryReport(id + 1);
                int amount = (int)temp.amount;
                string from = temp.from_account;
                string to = temp.to_account;
                try
                {
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
                    tr.to_account = to;
                    tr.status = "Cash Received voucher added";
                    tr.voucher_type = "CR";
                    tr.voucher_code = "CR111";
                    tr.trans_acc_id = trans_from.id;
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

                    tr.quantity = "Not Availabe";

                    service.addTransaction(tr);
                    trans_from.balance = tr.balance;
                    tr.unique_key = trans_from.id;
                    tr.voucher_code = temp.voucher_code;
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
                    transTo.to_account = to;
                    transTo.status = "Cash Received voucher added";
                    transTo.voucher_type = "CR";
                    transTo.voucher_code = "CR111";
                    transTo.trans_acc_id = trans_to.id;
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

                    transTo.quantity = "Not Availabe";
                    // End //

                    service.addTransaction(transTo);
                    trans_to.balance = transTo.balance;
                    transTo.unique_key = tr.unique_key;
                    transTo.voucher_code = tr.voucher_code;
                    tempTo.is_active = "N";
                    trans_from.updated_at_ = tr.updated_at;
                    trans_to.updated_at_ = tr.updated_at;
                    //trans_from.cr_ = trans_from.cr_ + amount;
                    
                     //trans_to.dr_ = trans_to.dr_ + amount;

                    tr.Last_Edit = "None";
                    transTo.Last_Edit = "None";

 

                    service.save();


                    service.balanceUpdation(trans_from.id, trans_to.id);



                    return RedirectToAction("Supervision", "USER_CRUD");

                }

                catch
                {

                    return RedirectToAction("Supervision", "USER_CRUD");
                }




            }
        }

        public ActionResult weightExpense()
        {
            //int check = (int)Session["CR"];

            //if (check == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            int num = TransactionServices.getMaxId();
            ViewBag.Code = "WEV00" + num;

            return View(service.allTransactionaccounts());
        }
        public ActionResult weightExpensePost(int id)
        {
            {

                if (service.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }

                //int check = (int)Session["CR"];

                //if (check == 0)
                //{
                //    return RedirectToAction("Index", "Home");
                //}

                TemporaryReport temp = service.findTemporaryReport(id);
                TemporaryReport tempTo = service.findTemporaryReport(id + 1);
                int amount = (int)temp.amount;
                string from = temp.from_account;
                string to = temp.to_account;
                try
                {
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
                    tr.to_account = to;
                    tr.status = "Weight Expense Voucher Added";
                    tr.voucher_type = "WEV";
                    tr.voucher_code = "WEV111";
                    tr.trans_acc_id = trans_from.id;
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

                    tr.quantity = "Not Availabe";
                    tr.net_weight = temp.net_weight;
                    service.addTransaction(tr);
                    trans_from.balance = tr.balance;
                    tr.unique_key = trans_from.id;
                    tr.voucher_code = temp.voucher_code;
                 
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
                    transTo.to_account = to;
                    transTo.status = "Weight Expense Voucher Added";
                    transTo.voucher_type = "WEV";
                    transTo.voucher_code = "WEV111";
                    transTo.trans_acc_id = trans_to.id;
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

                    transTo.quantity = "Not Availabe";
                    // End //
                    transTo.net_weight = temp.net_weight;
                    service.addTransaction(transTo);
                    trans_to.balance = transTo.balance;
                    transTo.unique_key = tr.unique_key;
                    transTo.voucher_code = tr.voucher_code;
                  
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

                    return RedirectToAction("Supervision","USER_CRUD");
                }




            }
        }

        public ActionResult BankPaymentReport()
        {
            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(service.allTransactionaccounts());

        }
        [HttpPost]
        public ActionResult BankPaymentReportPost(string search, string dateStart, string dateEnd)
        {



            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }





            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForBankPaymentReports(PermissionForAccounts.permisionBankPaymentReports, search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }


            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            DateTime date = Convert.ToDateTime(dateStart);
            var balance = service.findOpeningBalancebeforeDate(date, search);
            var balance2 = service.findAccountBalance(search);
            ViewBag.openingBalance = balance + balance2;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);

            ViewBag.MyList = allaccounts;
            return View(accounts);
        }


        public ActionResult mainReports()
        {
            //int check = (int)Session["expense_reports"];

            //if (check == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            Database1Entities1 db = new Database1Entities1();
            ViewBag.headAccounts = db.HeadAccounts.Where(x => x.is_active == "Y" & x.MainAccount.is_active=="Y").ToList();
            ViewBag.subHeadAccounts = db.SubHeadAccounts.Where(x => x.is_active == "Y" & x.HeadAccount.is_active=="Y" & x.MainAccount.is_active=="Y").ToList();
            ViewBag.MainAccounts = db.MainAccounts.Where(x => x.is_active == "Y").ToList();
            return View(service.allTransactionaccounts());
        }




        [HttpPost]
        public ActionResult mainReportsPost(string search, string dateStart, string dateEnd)
        {
            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForMainReports(PermissionForAccounts.permisionMainReports, search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }



            //int check = (int)Session["expense_reports"];
            
            //if (check == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            DateTime date = Convert.ToDateTime(dateStart);
            var balance = service.findOpeningBalancebeforeDate(date, search);
            var balance2 = service.findAccountBalance(search);
            ViewBag.openingBalance = balance+balance2;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
            return View(accounts);
        }


        [HttpPost]
        public ActionResult mainReportsPostHead(string search, string dateStart, string dateEnd)
        {
            Database1Entities1 db = new Database1Entities1();
            HeadAccount accounts = service.findHeadAccounts(search);
            //   var balance = service.findAccountBalance(search);
            //   ViewBag.openingBalance = balance;
            ViewBag.search = search;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.HeadAccounts = db.HeadAccounts.Where(x=>x.MainAccount.is_active=="Y"& x.is_active=="Y").ToList();
            return View(accounts);
        }


        [HttpPost]
        public ActionResult mainReportsPostSub(string search, string dateStart, string dateEnd)
        {
            Database1Entities1 db = new Database1Entities1();
            SubHeadAccount accounts = service.findSubHeadAccounts(search);
            //  var balance = service.findAccountBalance(search);
            //  ViewBag.openingBalance = balance;
            ViewBag.search = search;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.subHeadAccounts = db.SubHeadAccounts.Where(x => x.is_active == "Y" & x.HeadAccount.is_active =="Y" & x.MainAccount.is_active=="Y").ToList();
            return View(accounts);
        }



        [HttpPost]
        public ActionResult mainReportsMain(string search, string dateStart, string dateEnd)
        {
            MainAccount accounts = service.findMainAccount(search);
            //  var balance = service.findAccountBalance(search);
            //  ViewBag.openingBalance = balance;
            ViewBag.search = search;
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MainAccounts = service.allMainAccounts();
            return View(accounts);
        }

        public ActionResult Detail (int id)
        {

            return View(service.findTransaction(id));
        }

        public ActionResult TrialBalanceReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TrialBalanceReportPost( string dateStart )
        {
            List<TransactionAccount> accounts = service.allTransactionaccounts();
            ViewBag.date = Convert.ToDateTime(dateStart);
            return View(accounts);
        }

        public ActionResult MultiVoucher()
        {
            int num = TransactionServices.getMaxId();
            ViewBag.LastID = num;
            ViewBag.Code = "JV00" + num;
            return View(service.allTransactionaccounts());
        }

        public ActionResult MultiVoucher2()
        {
            int num = TransactionServices.getMaxId();
            ViewBag.LastID = num;
            ViewBag.Code = "JV00" + num;
            return View(service.allTransactionaccounts());
        }

        public ActionResult Delete(int ID, string SEARCH, string START, string END)
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

            if((tr.voucher_type =="SV" && trTo.voucher_type =="SV") || (tr.voucher_type=="UPSV" && trTo.voucher_type=="UPSV"))
            {
                TransactionAccount acc = service.findTransactionAccount(trTo.trans_acc_id);
                int count = acc.Sale_Order.Count;
                if(count > 0)
                {
                    List<Sale_Order> listOfSaleOrders = acc.Sale_Order.Where(x => x.item == trTo.good_ && x.is_active=="Y").ToList();
                    if(listOfSaleOrders.Count >0)
                    {
                        foreach(var order in listOfSaleOrders)
                        {
                            order.weight = (int)(order.weight + trTo.net_weight);
                        }
                    }
                }
            }

            else if ((tr.voucher_type == "PV" && trTo.voucher_type == "PV") || (tr.voucher_type == "UPPV" && trTo.voucher_type == "UPPV"))
            {
                TransactionAccount acc = service.findTransactionAccount(trTo.trans_acc_id);
                int count = acc.Sale_Order.Count;
                if (count > 0)
                {
                    List<Purchase_Order> listOfPurchaseOrders = acc.Purchase_Order.Where(x => x.item == trTo.good_ && x.is_active == "Y").ToList();
                    if (listOfPurchaseOrders.Count > 0)
                    {
                        foreach (var order in listOfPurchaseOrders)
                        {
                            order.weight = (int)(order.weight + trTo.net_weight);
                        }
                    }
                }
            }

            service.save();

            return RedirectToAction("historyPost", new { search = SEARCH, dateStart = START, dateEnd = END });

        }


        //////////////////////////////////////////////////////////////////////////////////////
        //[HttpPost]
        //public void ExportData( string search,string dateStart, string dateEnd)  
        //{  
        //    Database1Entities1 db = new Database1Entities1();
        //    TransactionAccount transactionAccount = db.TransactionAccounts.Where(x=>x.name == search & x.is_active =="Y").First();
        //    DateTime start = Convert.ToDateTime(dateStart);
        //    DateTime end = Convert.ToDateTime(dateEnd);
        //    List<Transaction> allTransactions = new List<Transaction>();
        //    List<Transaction> filterTransaction = new List<Transaction>(); 
        //    allTransactions = transactionAccount.Transactions.Where(x => x.created_at >= start & x.created_at <= end & x.is_active == "Y").ToList();
           
        //    foreach(Transaction tr in allTransactions)
        //    {
        //        if (tr.extra == "Sale" && transactionAccount.main_id != 12)
        //        {
        //            continue;
        //        }
        //        else if (tr.cr == 0 && tr.dr == 0)
        //        {
        //            continue;
        //        }
        //        else if (tr.is_active != "Y")
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            filterTransaction.Add(tr);
        //        }
        //    }

        //    WebGrid webGrid = new WebGrid(filterTransaction, canPage: true, rowsPerPage: 90);  
        //    string gridData = webGrid.GetHtml(  
        //        columns: webGrid.Columns(
        //        webGrid.Column(columnName: "Id", header: "ID"),
        //        webGrid.Column(columnName: "from_account", header: "From"),
        //        webGrid.Column(columnName: "to_account", header: "To"),
        //        webGrid.Column(columnName: "created_at", header: "Date"),  
        //        webGrid.Column(columnName: "voucher_type", header: "Type"),  
        //        webGrid.Column(columnName: "voucher_code", header: "Code"),  
        //        webGrid.Column(columnName: "cr", header: "Credit"),
        //        webGrid.Column(columnName: "dr", header: "Debit"),
        //        webGrid.Column(columnName: "description", header: "Description")
        //        //new{ Name="Brij", Email="brij@techbrij.com", Phone="111-222-3333" }
                
        //    )).ToString();  
  
        //    Response.ClearContent();  
        //    Response.AddHeader("content-disposition","attachment; filename=Ledgers-Details.xls");  
        //    Response.ContentType = "applicatiom/excel";  
        //    Response.Write(gridData);  
        //    Response.End();  
        //}


        public ActionResult salary()
        {

            //int check = (int)Session["JV"];

            //if (check == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "SLRV00" + num;
            return View(service.allTransactionaccounts());
        }


        public ActionResult salaryPost(int id)
        {
            //int check = (int)Session["JV"];

            //if (check == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }



            TemporaryReport temp = service.findTemporaryReport(id);
            TemporaryReport tempTo = service.findTemporaryReport(id + 1);


            int amount = (int)temp.amount;
            string from = temp.from_account;
            string to = temp.to_account;


            try
            {


                String session = Session["name"].ToString();
                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
                Transaction tr = new Transaction();
                Transaction transTo = new Transaction(); ;

                tr.created_at = temp.created_at;
                tr.code = trans_from.id;
                tr.description = temp.description;
                tr.dr = amount;
                tr.cr = 0;
                tr.balance = (trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "Y";

                tr.from_account = from;
                tr.to_account = to;
                tr.status = "Salary voucher added";
                tr.voucher_type = "SLRV";
                tr.voucher_code = "SLRV111";
                tr.trans_acc_id = trans_from.id;
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


                tr.quantity = "Not Availabe";

                service.addTransaction(tr);
                temp.is_active = "N";
                //service.save();
                //trans_from.balance = tr.balance;

                // service.save();




                transTo.created_at = tempTo.created_at;
                transTo.code = trans_to.id;
                transTo.description = tempTo.description;
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = tempTo.updated_at;
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = to;
                transTo.status = "Salary voucher added";
                transTo.voucher_type = "SLRV";
                transTo.voucher_code = "SLRV111";
                transTo.trans_acc_id = trans_to.id;
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

                transTo.quantity = "Not Availabe";
                // End //


                service.addTransaction(transTo);
                tempTo.is_active = "N";



                ///// New change /////

                tr.unique_key = trans_from.id;
                tr.voucher_code = temp.voucher_code;

                // Last Edit //

                tr.Last_Edit = "None";
                transTo.Last_Edit = "None";

                /////////////////

                service.save();

                trans_to.balance = transTo.balance;
                transTo.unique_key = tr.unique_key;
                transTo.voucher_code = tr.voucher_code;
                trans_from.updated_at_ = tr.updated_at;
                trans_to.updated_at_ = tr.updated_at;
                //trans_from.cr_ = trans_from.cr_ + amount;
                //trans_to.dr_ = trans_to.dr_ + amount;

                service.save();

                service.balanceUpdation(trans_from.id, trans_to.id);



                return RedirectToAction("Supervision", "USER_CRUD");

            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("Supervision", "USER_CRUD");
            }




        }


#endregion

        
    }  
}  


    

