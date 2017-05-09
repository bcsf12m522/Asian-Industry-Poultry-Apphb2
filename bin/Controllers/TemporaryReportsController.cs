using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using Humanizer;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Routing;
using ranglerz_project.ModelsBO;


namespace ranglerz_project.Controllers
{
  //  [SuperVisionCheck]
    [SessionCheck]
    public class TemporaryReportsController : Controller
    {
        TransactionServices service = new TransactionServices();
        Database1Entities1 db = new Database1Entities1();


        [HttpGet]
         [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult ViewTempReports()
        {
          
            return View(db.TemporaryReports.Where(x=>x.is_active=="Y").ToList());
        }

        [HttpPost]
        public ActionResult journalPost(FormCollection coll)
        {
           if(service.checkID()==false)
           {
               return Content("IDS Conflict Error Occured While Procsesing Request");
           }
            
            FormCollection collection = coll;
            
            //int checkingCode = Permissions.checkVoucherNumber(collection["voucherCode"]);
            //if(checkingCode ==0)
            //{
            //    return Content("<script language='javascript' type='text/javascript'>alert('Voucher Already Exist !! Please Refresh Page ');</script>");
            //}
            int amount = Convert.ToInt32(collection["amount"]);
            string from = collection["from"];
            string to = collection["to"];
            string type = "JV";
            string code = "JV111";
     

            try
           {
               //if (Session["name"] != "Admin")  MultiVoucherPost
               //{
               //    int permisonCount = 0;

               //    foreach (var check in PermissionForAccounts.permisions)
               //    {

               //        if (check.account_Name == from || check.account_Name == to)
               //        {
               //            permisonCount++;
               //        }
               //        if (permisonCount == 2)
               //        {
               //            break;
               //        }

               //    }

               //    if (permisonCount < 2)
               //    {
               //        return RedirectToAction("index", "Home");
               //    }
               //}


                String session = Session["username"].ToString();
                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
                TemporaryReport tr = new TemporaryReport();
                TemporaryReport transTo = new TemporaryReport(); ;

                tr.created_at = Convert.ToDateTime(collection["date"]);
                tr.code = trans_from.id;
                tr.description = collection["description"];
                tr.dr = Convert.ToInt32(amount);
                tr.cr = 0;
            
                tr.balance =(int) (trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "N";

                tr.from_account = from;
                tr.to_account = to;
                tr.status = "Journal voucher added";
                tr.voucher_type = "JV";
                tr.trans_acc_id = trans_from.id;
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    tr.user_id = user.Id;


                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    tr.admin_id = Convert.ToInt32(Session["adminId"]);
                }

                tr.quantity = "Not Availabe";
                temporarySave.saveTempReport(coll, code, type, session, tr);
                temporarySave.add(tr);
                //temporarySave.save();
                tr.voucher_code = collection["voucherCode"];
                //temporarySave.save();



                transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                transTo.code = trans_to.id;
                transTo.description = collection["description"];
                transTo.dr = 0;
                transTo.cr =Convert.ToInt32( amount);
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = Convert.ToDateTime(collection["date"]);
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = to;
                transTo.status = "Journal voucher added";
                transTo.voucher_type = "JV";
                transTo.voucher_code = "JV111";
                transTo.trans_acc_id = trans_to.id;
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    transTo.user_id = user.Id;
                }


                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    transTo.admin_id =Convert.ToInt32(Session["adminId"]);
                }

                transTo.quantity = "Not Availabe";
                // End //

                temporarySave.saveTempReport(coll, code, type, session, transTo);
                temporarySave.add(transTo);
                transTo.voucher_code = tr.voucher_code;
                temporarySave.save();

                string actionType = collection["actionType"];



                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintJournal", new { id = tr.Id });
                }

                return RedirectToAction("journal","Transactions");

            } 

            catch
            {

                return RedirectToAction("journal","Transactions");
            }



        }
      
        public ActionResult PrintJournal(int id)
        {
            TemporaryReport temp = service.findTemporaryReport(id);

            int amount = (int)temp.amount;
            string amountWord = amount.ToWords();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            ViewBag.amount = myTI.ToTitleCase(amountWord);

            if (id != 0)
            {
                return View(temp);
            }
            return null;
        }
               
        [HttpPost]
        public ActionResult bankPost(FormCollection collection)
        {

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }

            int amount=0;
            try
            { 
                amount = Convert.ToInt32(collection["amount"]); 
            }
            catch (Exception)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Invalid Amount !');</script>");
            }

            string from = collection["from"];
            string to = collection["to"];
            string type = "BV";
            string code = "BV111";
            try
            {
                //if (Session["name"] != "Admin")
                //{
                   
                    
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions,from,to)==false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                String session = Session["username"].ToString();


                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
                TemporaryReport tr = new TemporaryReport();
                TemporaryReport transTo = new TemporaryReport();

                tr.created_at = Convert.ToDateTime(collection["date"]);
                tr.code = trans_from.id;
                tr.description = collection["description"];
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
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    tr.user_id = user.Id;


                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    tr.admin_id = 1;
                }

                temporarySave.saveTempReport(collection, code, type, session, tr);
                temporarySave.add(tr);
                //temporarySave.save();
                tr.voucher_code = collection["voucherCode"];
                //temporarySave.save();






                transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                transTo.code = trans_to.id;
                transTo.description = collection["description"];
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = Convert.ToDateTime(collection["date"]);
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = to;
                transTo.status = "Bank voucher added";
                transTo.voucher_type = "BV";
                transTo.voucher_code = "BV111";
                transTo.trans_acc_id = trans_to.id;
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    transTo.user_id = user.Id;
                }


                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    transTo.admin_id = 1;
                }

                temporarySave.saveTempReport(collection, code, type, session, transTo);
                temporarySave.add(transTo);
                transTo.voucher_code = tr.voucher_code;
                temporarySave.save();

                 string actionType = collection["actionType"];

                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintJournal", new { id = tr.Id });
                }

               

                return RedirectToAction("bank", "Transactions");

            }

            catch
            {

                return RedirectToAction("bank","Transactions");
            }

           
           
        
        }


        public ActionResult bankPaymentPost(FormCollection collection)
        {
            {
                if (service.checkID() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }

                int amount = Convert.ToInt32(collection["amount"]);
                string from = collection["from"];
                string to = collection["to"];
                string type = "BP";
                string code = "BP111";
                float tax = 0;
                if (collection["chequeType"] !="none")
                {
                    try
                    {
                        tax = float.Parse(collection["tax"]);
                    }

                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Tax !');</script>");
                    }
                }
                else
                {
                    tax = 0;
                }

                //if (Session["name"] != "Admin")
                //{
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, to) == false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                try
                {
                    float taxCalculate = (tax / 100) * amount;

                    String session = Session["username"].ToString();

                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport ();

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                 
                    tr.dr =Convert.ToInt32(amount - taxCalculate);
                    tr.cr = 0;
                    tr.balance = (trans_from.balance - tr.dr);
               
                    tr.updated_at = tr.created_at;
       
                    tr.is_active = "Y";

                    tr.from_account = from;
                    tr.to_account = to;
                    tr.status = "Bank Payment voucher added";
                    tr.voucher_type = "BP";
                    tr.voucher_code = "BP111";
                    tr.trans_acc_id = trans_from.id;

                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]); 
                    }

                    tr.quantity = "Not Availabe";

                    tr.cheque_no = 1;
                    tr.cheque_number = collection["chequeNo"];

                    if (collection["chequeType"] == "cash")
                    {
                        tr.cash = "cash";
                       
                    }
                    else if (collection["chequeType"] == "cross")
                    {
                        tr.cross = "cross";
                  
                    }
                    else
                    {
                        tr.extra = "none";
                    }

                    temporarySave.saveTempReport(collection, code, type, session, tr);
                    tr.amount = tr.dr;
                    tr.description = collection["description"];
                    tr.tax_orignal = tax;
                    tr.tax =(int)(taxCalculate);
                    temporarySave.add(tr);
                    //temporarySave.save();
                    tr.voucher_code = collection["voucherCode"];         
                    tr.unique_key = trans_from.id;
                    //temporarySave.save();




                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                   
                    transTo.dr = 0;
                    transTo.cr = Convert.ToInt32 (amount - taxCalculate);
                    transTo.balance = (trans_to.balance + transTo.cr);
              
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.tax = tr.tax;
                    transTo.status = "Bank Payment voucher added";
                    transTo.voucher_type = "BP";
                    transTo.voucher_code = "BP111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    transTo.cheque_no = 1;
                    transTo.cheque_number = tr.cheque_number;
                    if (collection["chequeType"] == "cash")
                    {
                        transTo.cash = "cash";
                       
                    }
                    else if (collection["chequeType"] == "cross")
                    {
                        transTo.cross = "cross";        
                    }
                    else
                    {
                        transTo.extra = "none";
                    }

                 

                    temporarySave.saveTempReport(collection, code, type, session, transTo);
                    transTo.description = collection["description"];
                    transTo.amount = transTo.cr;
                    transTo.tax = (int)(taxCalculate);
                    transTo.tax_orignal = tax;
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;
                    temporarySave.save();

               
                    transTo.unique_key = tr.unique_key;
                 



                    //////////////////////////////////////////////////////////////

                 
                    TemporaryReport tr1 = new TemporaryReport();
                    TemporaryReport transTo1 = new TemporaryReport();

                    tr1.created_at = Convert.ToDateTime(collection["date"]);
                    tr1.code = trans_from.id;
                  
                    tr1.dr =(int)(taxCalculate);
                    tr1.cr = 0;
                    tr1.balance = (trans_from.balance - tr1.dr);
                 
                    tr1.updated_at = tr1.created_at;
                  
                    tr.is_active = "Y";

                    tr1.from_account = from;
                    tr1.to_account = to;
                    tr1.status = "Bank Payment voucher added";
                    tr1.voucher_type = "BP";
                    tr1.voucher_code = "BP111";
                    tr1.trans_acc_id = trans_from.id;

                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr1.user_id = user.Id;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr1.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr1.quantity = "Not Availabe";

                    tr1.cheque_no = 1;
                    tr1.cheque_number = tr.cheque_number;

                    if (collection["chequeType"] == "cash")
                    {
                        tr1.cash = "cash";
                        tr1.tax = (int)taxCalculate;
                    }
                    else if (collection["chequeType"] == "cross")
                    {
                        tr1.cross = "cross";
                        tr1.tax = (int)taxCalculate;
                    }
                    else
                    {
                        tr1.extra = "none";
                    }

                    temporarySave.saveTempReport(collection, code, type, session, tr1);
                    tr1.description = "WHT";
                    tr1.amount = tr1.dr;
                    tr1.tax = (int)(taxCalculate);
                    tr1.tax_orignal = tax;
                    temporarySave.add(tr1);
                    tr1.voucher_code = tr.voucher_code;
                    //temporarySave.save();

                    tr1.unique_key = trans_from.id;





                    transTo1.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo1.code = trans_to.id;
                 
                    transTo1.dr = 0;
                    transTo1.cr =(int)(taxCalculate);
                    transTo1.balance = (trans_to.balance + transTo1.cr);
                  
                    transTo1.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo1.is_active = "Y";

                    transTo1.from_account = from;
                    transTo1.to_account = to;
                    transTo1.tax = tr1.tax;
                    transTo1.status = "Bank Payment voucher added";
                    transTo1.voucher_type = "BP";
                    transTo1.voucher_code = "BP111";
                    transTo1.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo1.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo1.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo1.quantity = "Not Availabe";
                    transTo1.cheque_no = 1;
                    transTo1.cheque_number = tr.cheque_number;
                    if (collection["chequeType"] == "cash")
                    {
                        transTo1.cash = "cash";

                    }
                    else if (collection["chequeType"] == "cross")
                    {
                        transTo1.cross = "cross";
                    }
                    else
                    {
                        transTo1.extra = "none";
                    }

             

                    temporarySave.saveTempReport(collection, code, type, session, transTo1);
                    transTo1.description = "WHT";
                    transTo1.amount = transTo1.cr;
                    transTo1.tax = (int)(taxCalculate);
                    transTo1.tax_orignal = tax;
                    transTo1.unique_key = transTo.unique_key;
                    temporarySave.add(transTo1);
                    transTo1.voucher_code = tr.voucher_code;
                    temporarySave.save();


              
                    service.save();

 

                    /////////////////////////////////////////////////////////////


                    


                    string actionType = collection["actionType"];

                    if (actionType == "Add and Print")
                    {
                        return RedirectToAction("PrintJournal", new { id = tr.Id });
                    }


                    return RedirectToAction("bankPayment", "Transactions");

                }

                catch
                {

                    return RedirectToAction("bankPayment","Transactions");
                }

            }
        }
        [HttpPost]
        public ActionResult cashPaymentPost(FormCollection collection)
        {
            {
                if (service.checkID() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }

                int amount = Convert.ToInt32(collection["amount"]);
                string from = collection["from"];
                string to = collection["to"];
                string code = "CP111";
                string type = "CP";
                //if (Session["name"] != "Admin")
                //{
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, to) == false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                try
                {
                    String session = Session["username"].ToString();


                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                    tr.description = collection["description"];
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
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr.quantity = "Not Availabe";

                    
                 
                    tr.unique_key = trans_from.id;
                    temporarySave.saveTempReport(collection, code, type, session, tr);
                    temporarySave.add(tr);
                    //temporarySave.save();
                    //service.save();
                    tr.voucher_code = collection["voucherCode"];
                    //temporarySave.save();


                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = amount;
                    transTo.balance = (trans_to.balance + amount);
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.status = "Cash Payment voucher added";
                    transTo.voucher_type = "CP";
                    transTo.voucher_code = "CP111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    // End //

                    
                
                    transTo.unique_key = transTo.unique_key;
                    temporarySave.saveTempReport(collection, code, type, session, transTo);
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;
                    temporarySave.save();
                    service.save();


                    string actionType = collection["actionType"];

                    if (actionType == "Add and Print")
                    {
                        return RedirectToAction("PrintJournal", new { id = tr.Id });
                    }



                    return RedirectToAction("cashPayment", "Transactions");

                }

                catch
                {

                    return RedirectToAction("cashPayment","Transactions");
                }




            }
        }
        public ActionResult cashReceivedPost(FormCollection collection)
        {
            {
                if (service.checkID() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }

                int amount = Convert.ToInt32(collection["amount"]);
                string from = collection["from"];
                string to = collection["to"];
                string code = "CR111";
                string type = "CR";
                //if (Session["name"] != "Admin")
                //{
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, to) == false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                try
                {
                    String session = Session["username"].ToString();


                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                    tr.description = collection["description"];
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
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr.quantity = "Not Availabe"; 
                    tr.unique_key = trans_from.id;
                    temporarySave.saveTempReport(collection, code, type, session, tr);
                    temporarySave.add(tr);
                    //temporarySave.save();
                    //service.save();
                    tr.voucher_code = collection["voucherCode"];
                    //temporarySave.save();


                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = amount;
                    transTo.balance = (trans_to.balance + amount);
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.status = "Cash Received voucher added";
                    transTo.voucher_type = "CR";
                    transTo.voucher_code = "CR111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = "Admin";
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    temporarySave.saveTempReport(collection, code, type, session, transTo);
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;
                    temporarySave.save();
                    transTo.unique_key = transTo.unique_key;
                    service.save();

                    string actionType = collection["actionType"];

                    if (actionType == "Add and Print")
                    {
                        return RedirectToAction("PrintJournal", new { id = tr.Id });
                    }


                    return RedirectToAction("cashReceive", "Transactions");

                }

                catch
                {

                    return RedirectToAction("cashReceive","Transactions");
                }




            }
        }

        public ActionResult weightExpensePost(FormCollection collection)
        {
            {

                if (service.checkID() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }


                int amount = Convert.ToInt32(collection["amount"]);
                int weight = Convert.ToInt32(collection["weight"]);
                string from = collection["from"];
                string to = collection["to"];
                string code = "WEV111";
                string type = "WEV";
                //if (Session["name"] != "Admin")
                //{
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, to) == false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                try
                {
                    String session = Session["username"].ToString();


                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                    tr.description = collection["description"];
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
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr.quantity = "Not Availabe";
                    tr.unique_key = trans_from.id;
                    tr.net_weight = weight;
                    temporarySave.saveTempReport(collection, code, type, session, tr);
                    temporarySave.add(tr);
                    //temporarySave.save();
                    //service.save();
                    tr.voucher_code = collection["voucherCode"];
                    //temporarySave.save();


                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = amount;
                    transTo.balance = (trans_to.balance + amount);
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.status = "Weight Expense Voucher Added";
                    transTo.voucher_type = "WEV";
                    transTo.voucher_code = "WEV111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = "Admin";
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    transTo.net_weight = tr.net_weight;
                    temporarySave.saveTempReport(collection, code, type, session, transTo);
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;
                    temporarySave.save();
                    transTo.unique_key = transTo.unique_key;
                    service.save();

                    string actionType = collection["actionType"];

                    if (actionType == "Add and Print")
                    {
                        return RedirectToAction("PrintJournal", new { id = tr.Id });
                    }


                    return RedirectToAction("weightExpense","Transactions");

                }

                catch
                {

                    return RedirectToAction("weightExpense","Transactions");
                }




            }
        }
        public ActionResult AddSaleVoucher(FormCollection collection)
        {

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }

                string fromId = (collection["from"]);
                int toId = Convert.ToInt32(collection["to"]);
                TransactionAccount acc = service.findTransactionAccount(toId);
                Good goods = service.findGood(fromId);
                string from = goods.TransactionAccount.name;
                string to = acc.name;
                double rate = 0;
                int idForRate = 0;
                float unitPrice = 0;
                //if (Session["name"] != "Admin")
                //{
                //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, to) == false)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}
                if (collection["unitPrice"] != null)
                {
                    try
                    {
                        float checkingUnit = float.Parse(collection["unitPrice"]);
                    }

                    catch (Exception)
                    {
                        if (collection["builty"] != null)
                        {
                            return RedirectToAction("unitPriceSV", "SaleVoucher");
                        }
                        else
                        {
                            return RedirectToAction("unitPricePV", "SaleVoucher");
                        }
                    }
                }
                try
                {
                    int checking = Convert.ToInt32(collection["builty"]);
                }
                catch (Exception)
                {
                    if (collection["unitPrice"] == null)
                    {
                        return RedirectToAction("Index", "SaleVoucher");
                    }
                    else
                    {
                        return RedirectToAction("unitPriceSV", "SaleVoucher");
                    }
                }


                int builty = Convert.ToInt32(collection["builty"]);
                if (collection["unitPrice"] != null)
                {
                     unitPrice = float.Parse(collection["unitPrice"]);
                }
                try
                {


                    String session = Session["username"].ToString();
                    TransactionAccount transFrom = new TransactionAccount();
                    TransactionAccount trans_to = new TransactionAccount();

                    Good good = service.findGood(fromId);
                    if (builty <= 0)
                    {
                        transFrom = service.findJvTransaction(to);
                        trans_to = good.TransactionAccount;
                        idForRate = transFrom.id;
                    }
                    else if (builty > 0)
                    {
                        transFrom = good.TransactionAccount;
                        trans_to = service.findJvTransaction(to);
                        idForRate = trans_to.id;
                    }
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();
                    // to get good ID on other Sale Voucher //
                    tr.tax = good.Id;
                    transTo.tax = good.Id;
                    // // //
                    if (unitPrice != 0)
                    {
                        rate = unitPrice;
                    }
                    else
                    {
                        try
                        {
                            rate = service.findRate(idForRate, good.good_Name);
                        }

                        catch (Exception)
                        {
                            return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate !');</script>");
                        }

                      
                    }
                    int amount = 0;

                    tr.truck = collection["truck"];
                    tr.builty_no_ = Convert.ToInt32(collection["builty"]);
                    tr.weight_empty = Convert.ToInt32(collection["empty"]);
                    tr.weight_load = Convert.ToInt32(collection["load"]);
                    tr.net_weight = Convert.ToInt32(collection["net"]);
                   
                    tr.quantity = tr.net_weight.ToString();
                    tr.good_ = good.good_Name;

                    amount = Convert.ToInt32(rate * tr.net_weight);

                    tr.bags = collection["bags"];
                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.updated_at = tr.created_at;
                    tr.is_active = "Y";
                    tr.code = transFrom.id;
                    tr.description = collection["description"];
                    tr.cr = 0;
                    tr.dr = amount;
                    tr.balance = (transFrom.balance - amount);



                    tr.from_account = transFrom.name;
                    tr.to_account = trans_to.name;
                    //transFrom.dr_ = amount;

                    if (tr.builty_no_ != 0)
                    {
                        if (unitPrice == 0)
                        {
                            tr.status = "Sale voucher added";
                            tr.voucher_type = "SV";
                            tr.voucher_code = "SV111";
                        }
                        else
                        {
                            tr.status = "Unit Price Sale Voucher Added";
                            tr.voucher_type = "UPSV";
                            tr.voucher_code = "UPSV111";
                        }
                    }
                    else
                    {
                        if (unitPrice == 0)
                        {
                            tr.status = "Purchase Voucher added";
                            tr.voucher_code = "PV111";
                            tr.voucher_type = "PV";
                            tr.grn = collection["grn"];
                            tr.deduction = collection["deduction"];
                        }
                        else
                        {
                            tr.status = "Unit Price Purchase Voucher added";
                            tr.voucher_code = "UPPV111";
                            tr.voucher_type = "UPPV";
                            tr.grn = collection["grn"];
                            tr.deduction = collection["deduction"];

                        }
                    }
                    tr.trans_acc_id = transFrom.id;

                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }

                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }



                    temporarySave.saveTempReport(collection, tr.voucher_code, tr.voucher_type, session, tr);
                    tr.amount = amount;
                    tr.from_account = transFrom.name;
                    tr.to_account = trans_to.name;
                    //tr.unit_price = collection["unitPrice"];
                    tr.unit_price = unitPrice.ToString();
                    temporarySave.add(tr);
                    //temporarySave.save();

                    tr.voucher_code = collection["voucherCode"];                    
                    tr.unique_key = good.Id;
                    //service.save();



                    transTo.truck = collection["truck"];
                    transTo.builty_no_ = Convert.ToInt32(collection["builty"]);
                    transTo.weight_empty = Convert.ToInt32(collection["empty"]);
                    transTo.weight_load = Convert.ToInt32(collection["load"]);
                    transTo.net_weight = Convert.ToInt32(collection["net"]);
                    
                    transTo.bags = collection["bags"];
                    transTo.created_at = Convert.ToDateTime(collection["date"]);
                    transTo.updated_at = tr.created_at;
                    transTo.is_active = "Y";
                    transTo.quantity = tr.quantity;
                    transTo.good_ = tr.good_;


                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = amount;
                    transTo.balance = (trans_to.balance + amount);



                    transTo.from_account = transFrom.name;
                    transTo.to_account = trans_to.name;
                    if (transTo.builty_no_ != 0)
                    {
                        if (unitPrice == 0)
                        {

                            transTo.status = "Sale voucher added";
                            transTo.voucher_type = "SV";
                            transTo.voucher_code = "SV111";
                        }
                        else
                        {
                            transTo.status = "Unit Price Sale Voucher Added";
                            transTo.voucher_type = "UPSV";
                            transTo.voucher_code = "UPSV111";
                        }
                    }
                    else
                    {
                        if (unitPrice == 0)
                        {
                            transTo.status = "Purchase Voucher added";
                            transTo.voucher_code = "PV111";
                            transTo.voucher_type = "PV";
                            transTo.grn = collection["grn"];
                            transTo.deduction = collection["deduction"];
                        }
                        else
                        {
                            transTo.status = "Unit Price Purchase Voucher added";
                            transTo.voucher_code = "UPPV111";
                            transTo.voucher_type = "UPPV";
                            transTo.grn = collection["grn"];
                            transTo.deduction = collection["deduction"];

                        }
                    }
                    transTo.trans_acc_id = trans_to.id;
                    //trans_to.cr_ = amount;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }


                    // End //

                    temporarySave.saveTempReport(collection, tr.voucher_code, tr.voucher_type, session, transTo);
                    transTo.amount = amount;
                    transTo.from_account = transFrom.name;
                    transTo.to_account = trans_to.name;

                    transTo.unit_price = tr.unit_price;
                    transTo.unique_key = good.Id;


                    #region// Sale order and Purchase order //

                    if (collection["order_num"] != null && collection["order_num"] != "")
                    {
                        int order_num = 0;
                        bool check = int.TryParse(collection["order_num"], out order_num);
                        tr.order_number = order_num;

                        if (tr.voucher_type == "SV" || tr.voucher_type =="UPSV")
                        {
                            tr.order_code = "SO";
                        }
                        else if (tr.voucher_type == "PV" || tr.voucher_type=="UPPV")
                        {
                            tr.order_code = "PO";
                        }
                    }
                    else
                    {
                        tr.order_number = -1;
                        tr.order_code = "Error check Order not clicked";
                    }

                    transTo.order_number = tr.order_number;
                    transTo.order_code = tr.order_code;

                    #endregion










                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;
                    temporarySave.save();

                    
             
                    service.save();


                    // new Addtion of Transaction //

                    

                   


                    string actionType = collection["actionType"];

                    if (actionType == "Add and Print")
                    {
                        return RedirectToAction("PrintSale", new { id = tr.Id });
                    }



                    if (builty != 0)
                    {
                         if (unitPrice == 0)
                        {
                            return RedirectToAction("Index", "SaleVoucher");
                        }
                        else
                        {
                            return RedirectToAction("unitPriceSV", "SaleVoucher");
                        }
                    }
                    else
                    {
                        if (unitPrice == 0)
                        {
                            return RedirectToAction("purchaseVoucher", "SaleVoucher");
                        }
                        else
                        {
                            return RedirectToAction("unitPricePV", "SaleVoucher");
                        }
                    }

                  




                }

                  

                catch
                {
                    if (builty != 0)
                    {
                        if (unitPrice == 0)
                        {
                            return RedirectToAction("Index", "SaleVoucher");
                        }
                        else
                        {
                            return RedirectToAction("unitPriceSV", "SaleVoucher");
                        }
                    }
                    else
                    {
                        if (unitPrice == 0)
                        {
                            return RedirectToAction("purchaseVoucher", "SaleVoucher");
                        }
                        else
                        {
                            return RedirectToAction("unitPricePV", "SaleVoucher");
                        }
                    }
                }



        }


        public ActionResult PrintSale(int id)
        {
            TemporaryReport temp = service.findTemporaryReport(id);
            int amount =(int) temp.net_weight;
            string amountWord = amount.ToWords();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            ViewBag.amount = myTI.ToTitleCase(amountWord);
            if (id != 0)
            {
                return View(temp);
            }
            return null;
        }

        public ActionResult ExpensePost(FormCollection collection)
        {

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }


            int transAccID = 0;
            int subHeadID = 0;
            int headID = 0;
            string from = collection["from"];
            string to = collection["transactionAccount"];
            string head = collection["head"];
            string subhead = collection["subHead"];
            string tranAcc = collection["transactionAccount"];
            //if (Session["name"] != "Admin")
            //{
            //    if (PermissionForAccounts.permisionCheck(PermissionForAccounts.permisions, from, from) == false)
            //    {
            //        return RedirectToAction("index", "Home");
            //    }
            //}

            try
            {
                int amount = Convert.ToInt32(collection["amount"]);

                if (int.TryParse(head, out headID))
                {
                    headID = Convert.ToInt32(head);
                }
                if (int.TryParse(subhead, out subHeadID))
                {
                    int check = subHeadID;

                    subHeadID = Convert.ToInt32(subhead);
                }
                if (!(int.TryParse(tranAcc, out transAccID)))
                {
                    RedirectToAction("Index","ExpenseVoucher");
                }

                String session = Session["username"].ToString();


                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findTransactionAccount(transAccID);

                TemporaryReport tr = new TemporaryReport();
                TemporaryReport transTo = new TemporaryReport();

                tr.created_at = Convert.ToDateTime(collection["date"]);
                tr.code = trans_from.id;
                tr.description = collection["description"];
                tr.dr = amount;
                tr.cr = 0;
                tr.balance = (trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "Y";

                tr.from_account = from;
                
                tr.status = "1";
                tr.voucher_type = "EV";
                tr.voucher_code = "1212";
                tr.trans_acc_id = trans_from.id ;



                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    tr.user_id = user.Id;


                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    tr.admin_id = Convert.ToInt32(Session["adminId"]);
                }



                temporarySave.saveTempReport(collection, tr.voucher_code, tr.voucher_type, session, tr);
                tr.to_account = trans_to.name;
                temporarySave.add(tr);
                //temporarySave.save();
                tr.amount = amount;
                
                tr.unique_key = trans_from.id;
                tr.voucher_code = collection["voucherCode"];
                
                //service.save();




                transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                transTo.code = trans_to.id;
                transTo.description = collection["description"];
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = Convert.ToDateTime(collection["date"]);
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = trans_to.name;
                transTo.status = "1";
                transTo.voucher_type = "EV";
                transTo.voucher_code = "1212";
                transTo.trans_acc_id = trans_to.id;

                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    transTo.user_id = user.Id;
                }


                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                }


                // End //

                temporarySave.saveTempReport(collection, tr.voucher_code, tr.voucher_type, session, transTo);
                transTo.to_account = tr.to_account;
                temporarySave.add(transTo);

                temporarySave.save();
                transTo.amount = amount;
             
                transTo.unique_key = tr.unique_key;
                transTo.voucher_code = tr.voucher_code;
                service.save();


                string actionType = collection["actionType"];

                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintJournal", new { id = tr.Id });
                }



                return RedirectToAction("Index","ExpenseVoucher");

            }

            catch
            {

                return RedirectToAction("Index","ExpenseVoucher");
            }




        }

        public ActionResult Delete (int id)
        {
            TemporaryReport temp = db.TemporaryReports.Find(id);
            TemporaryReport tempTo = db.TemporaryReports.Find(id+1);
            if(temp.voucher_type =="PRV")
            {

                if (temp.from_account == "Tyre Waste Main  Account")
                {
                    TemporaryReport temp2nd = db.TemporaryReports.Find(id + 2);
                    TemporaryReport temp2ndTo = db.TemporaryReports.Find(id + 3);
                    TemporaryReport temp3rd = db.TemporaryReports.Find(id + 4);
                    TemporaryReport temp3rdTo = db.TemporaryReports.Find(id + 5);
                    temp2nd.is_active = "N";
                    temp2ndTo.is_active = "N";
                    temp3rd.is_active = "N";
                    temp3rdTo.is_active = "N";
                }
                else
                {
                       
                        TemporaryReport temp2nd = db.TemporaryReports.Find(id + 2);
                        TemporaryReport temp2ndTo = db.TemporaryReports.Find(id + 3);
                        temp2nd.is_active = "N";
                        temp2ndTo.is_active = "N";
                }
                 
            }
            else if(temp.voucher_type=="BP")
            {
                TemporaryReport temp2nd = db.TemporaryReports.Find(id + 2);
                TemporaryReport temp2ndTo = db.TemporaryReports.Find(id + 3);
                temp2nd.is_active = "N";
                temp2ndTo.is_active = "N";
            }
            temp.is_active ="N";
            tempTo.is_active ="N";
            db.SaveChanges();
            return RedirectToAction("ViewTempReports");
        }


        [HttpPost]
        public ActionResult MultiVoucherPost(FormCollection coll)
        {
        

            FormCollection collection = coll;
            int amount = Convert.ToInt32(collection["amount1"]);
            string from = collection["from"];
            string to = collection["account1"];
            string type = "JV";
            string code = "JV111";
            int idForPrint = 0;
            int lastID =Convert.ToInt32(collection["voucherLastID"]);

            try
            {
                //if (Session["name"] != "Admin") 
                //{
                //    int permisonCount = 0;

                //    foreach (var check in PermissionForAccounts.permisions)
                //    {

                //        if (check.account_Name == from || check.account_Name == to)
                //        {
                //            permisonCount++;
                //        }
                //        if (permisonCount == 2)
                //        {
                //            break;
                //        }

                //    }

                //    if (permisonCount < 2)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                for (int i = 0; i < 5; i++)
                {

                    string voucherCode = "JV00" + lastID;

                    if (i == 1)
                    {
                        if (collection["account2"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount2"]);
                            to = collection["account2"];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (i == 2)
                    {
                        if (collection["account3"] !="")
                        {
                            amount = Convert.ToInt32(collection["amount3"]);
                            to = collection["account3"];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (i == 3)
                    {
                        if (collection["account4"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount4"]);
                            to = collection["account4"];
                        }
                        else
                        {
                            break;
                        }
                    }
                   else if (i == 4)
                    {
                        if (collection["account5"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount5"]);
                            to = collection["account5"];
                        }
                        else
                        {
                            break;
                        }
                    }
                   



                    String session = Session["username"].ToString();
                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport(); ;

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                    tr.description = collection["description"];
                    tr.dr = Convert.ToInt32(amount);
                    tr.cr = 0;

                    tr.balance = (int)(trans_from.balance - amount);
                    tr.updated_at = tr.created_at;
                    tr.is_active = "Y";

                    tr.from_account = from;
                    tr.to_account = to;
                    tr.status = "Journal voucher added";
                    tr.voucher_type = "JV";
                    tr.trans_acc_id = trans_from.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr.quantity = "Not Availabe";
                    temporarySave.saveTempReport(coll, code, type, session, tr);
                    tr.amount = amount;
                    tr.to_account = to;
                    temporarySave.add(tr);
                    //temporarySave.save();
                    tr.voucher_code = voucherCode;
                    //temporarySave.save();



                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = Convert.ToInt32(amount);
                    transTo.balance = (trans_to.balance + amount);
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.status = "Journal voucher added";
                    transTo.voucher_type = "JV";
                    transTo.voucher_code = "JV111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    // End //

                    temporarySave.saveTempReport(coll, code, type, session, transTo);
                    transTo.amount = amount;
                    transTo.to_account = to;
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;

                    if (service.checkID() == false)
                    {
                        return Content("IDS Conflict Error Occured While Procsesing Request");
                    }


                    temporarySave.save();
                    idForPrint = tr.Id;
                    lastID = lastID + 1;
                }
                string actionType = collection["actionType"];



                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintJournal", new { id = idForPrint });
                }

                return RedirectToAction("MultiVoucher","Transactions");

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

                return RedirectToAction("MultiVoucher", "Transactions");

            }

            //catch
            //{

            //    return RedirectToAction("MultiVoucher","Transactions");
            //}



        }


        [HttpPost]
        public ActionResult MultiVoucherPost2(FormCollection coll)
        {


            FormCollection collection = coll;
            int amount = Convert.ToInt32(collection["amount1"]);
            string to = collection["to"];
            string from = collection["account1"];
            string type = "JV";
            string code = "JV111";
            int idForPrint = 0;
            int lastID = Convert.ToInt32(collection["voucherLastID"]);
            PrintOfMultiVoucher multiVoucherPrint = new PrintOfMultiVoucher();
            List<int> amounts = new List<int>();
            List<string> accounts = new List<string>();
            List<string> codes = new List<string>();
            int totalAmount = 0;
            int totalentries = 0;
    
            try
            {
                //if (Session["name"] != "Admin") 
                //{
                //    int permisonCount = 0;

                //    foreach (var check in PermissionForAccounts.permisions)
                //    {

                //        if (check.account_Name == from || check.account_Name == to)
                //        {
                //            permisonCount++;
                //        }
                //        if (permisonCount == 2)
                //        {
                //            break;
                //        }

                //    }

                //    if (permisonCount < 2)
                //    {
                //        return RedirectToAction("index", "Home");
                //    }
                //}

                for (int i = 0; i < 5; i++)
                {

                    string voucherCode = "JV00" + lastID;

                    if (i == 1)
                    {
                        if (collection["account2"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount2"]);
                            from = collection["account2"];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (i == 2)
                    {
                        if (collection["account3"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount3"]);
                            from = collection["account3"];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (i == 3)
                    {
                        if (collection["account4"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount4"]);
                            from = collection["account4"];
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (i == 4)
                    {
                        if (collection["account5"] != "")
                        {
                            amount = Convert.ToInt32(collection["amount5"]);
                            from = collection["account5"];
                        }
                        else
                        {
                            break;
                        }
                    }




                    String session = Session["username"].ToString();
                    TransactionAccount trans_from = service.findJvTransaction(from);
                    TransactionAccount trans_to = service.findJvTransaction(to);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport(); ;

                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.code = trans_from.id;
                    tr.description = collection["description"];
                    tr.dr = Convert.ToInt32(amount);
                    tr.cr = 0;

                    tr.balance = (int)(trans_from.balance - amount);
                    tr.updated_at = tr.created_at;
                    tr.is_active = "Y";

                    tr.from_account = from;
                    tr.to_account = to;
                    tr.status = "Journal voucher added";
                    tr.voucher_type = "JV";
                    tr.trans_acc_id = trans_from.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        tr.user_id = user.Id;


                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        tr.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    tr.quantity = "Not Availabe";
                    temporarySave.saveTempReport(coll, code, type, session, tr);
                    tr.amount = amount;
                    tr.to_account = to;
                    tr.from_account = from;
                    temporarySave.add(tr);
                    //temporarySave.save();
                    tr.voucher_code = voucherCode;
                    //temporarySave.save();



                    transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                    transTo.code = trans_to.id;
                    transTo.description = collection["description"];
                    transTo.dr = 0;
                    transTo.cr = Convert.ToInt32(amount);
                    transTo.balance = (trans_to.balance + amount);
                    transTo.updated_at = Convert.ToDateTime(collection["date"]);
                    transTo.is_active = "Y";

                    transTo.from_account = from;
                    transTo.to_account = to;
                    transTo.status = "Journal voucher added";
                    transTo.voucher_type = "JV";
                    transTo.voucher_code = "JV111";
                    transTo.trans_acc_id = trans_to.id;
                    if (Session["name"] != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transTo.user_id = user.Id;
                    }


                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                    }

                    transTo.quantity = "Not Availabe";
                    // End //

                    temporarySave.saveTempReport(coll, code, type, session, transTo);
                    transTo.amount = amount;
                    transTo.to_account = to;
                    transTo.from_account = from;
                    temporarySave.add(transTo);
                    transTo.voucher_code = tr.voucher_code;

                    if (service.checkID() == false)
                    {
                        return Content("IDS Conflict Error Occured While Procsesing Request");
                    }


                    temporarySave.save();
                    idForPrint = tr.Id;
                    lastID = lastID + 1;

                    amounts.Add(amount);
                    accounts.Add(from);
                    codes.Add(tr.voucher_code);
                    totalAmount = totalAmount + amount;
                    totalentries++;
                    multiVoucherPrint.date = tr.created_at.ToShortDateString();
                    multiVoucherPrint.discription = tr.description;
                }


                multiVoucherPrint.account = accounts;
                multiVoucherPrint.amount = amounts;
                multiVoucherPrint.voucher_code = codes;
                multiVoucherPrint.mainAccount = to;
                multiVoucherPrint.totalAmount = totalAmount;
                multiVoucherPrint.totalEntries = totalentries;
                multiVoucherPrint.type = "MultiCredit";
               

             
                
                string actionType = collection["actionType"];



                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintMultiVoucher", new RouteValueDictionary(multiVoucherPrint));
                }

                return RedirectToAction("MultiVoucher","Transactions");

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

                return RedirectToAction("MultiVoucher2","Transactions");

            }

            //catch
            //{

            //    return RedirectToAction("MultiVoucher","Transactions");
            //}



        }

        public ActionResult PrintMultiVoucher(PrintOfMultiVoucher multivoucher)
        {

            int amount = (int)multivoucher.totalAmount;
            string amountWord = amount.ToWords();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            ViewBag.amount = myTI.ToTitleCase(amountWord);
            return View(multivoucher);           
        }

        [HttpPost]
        public ActionResult salaryPost(FormCollection coll)
        {

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }


            FormCollection collection = coll;
            int amount = Convert.ToInt32(collection["amount"]);
            string from = collection["from"];
            string to = collection["to"];
            string type = "JV";
            string code = "JV111";
            try
            {               
                String session = Session["username"].ToString();
                TransactionAccount trans_from = service.findJvTransaction(from);
                TransactionAccount trans_to = service.findJvTransaction(to);
                TemporaryReport tr = new TemporaryReport();
                TemporaryReport transTo = new TemporaryReport(); ;

                tr.created_at = Convert.ToDateTime(collection["date"]);
                tr.code = trans_from.id;
                tr.description = collection["description"];
                tr.dr = Convert.ToInt32(amount);
                tr.cr = 0;

                tr.balance = (int)(trans_from.balance - amount);
                tr.updated_at = tr.created_at;
                tr.is_active = "N";

                tr.from_account = from;
                tr.to_account = to;
                tr.status = "Journal voucher added";
                tr.voucher_type = "JV";
                tr.trans_acc_id = trans_from.id;
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    tr.user_id = user.Id;


                }
                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    tr.admin_id = Convert.ToInt32(Session["adminId"]);
                }

                tr.quantity = "Not Availabe";
                temporarySave.saveTempReport(coll, code, type, session, tr);
                temporarySave.add(tr);
                tr.voucher_code = collection["voucherCode"];
             

                transTo.created_at = Convert.ToDateTime(collection["Debitdate"]);
                transTo.code = trans_to.id;
                transTo.description = collection["description"];
                transTo.dr = 0;
                transTo.cr = Convert.ToInt32(amount);
                transTo.balance = (trans_to.balance + amount);
                transTo.updated_at = Convert.ToDateTime(collection["date"]);
                transTo.is_active = "Y";

                transTo.from_account = from;
                transTo.to_account = to;
                transTo.status = "Journal voucher added";
                transTo.voucher_type = "JV";
                transTo.voucher_code = "JV111";
                transTo.trans_acc_id = trans_to.id;
                if (Session["name"] != "Admin")
                {
                    string username = Session["username"].ToString();
                    User user = service.findUser(username);
                    ViewBag.userName = user.name;
                    transTo.user_id = user.Id;
                }


                else
                {
                    ViewBag.userName = Session["username"].ToString();
                    transTo.admin_id = Convert.ToInt32(Session["adminId"]);
                }

                transTo.quantity = "Not Availabe";
                // End //

                temporarySave.saveTempReport(coll, code, type, session, transTo);
                temporarySave.add(transTo);
                transTo.voucher_code = tr.voucher_code;
                temporarySave.save();

                string actionType = collection["actionType"];



                if (actionType == "Add and Print")
                {
                    return RedirectToAction("PrintJournal", new { id = tr.Id });
                }

                return RedirectToAction("salary", "Transactions");

            }

            catch
            {

                return RedirectToAction("salary", "Transactions");
            }



        }
    }
}
