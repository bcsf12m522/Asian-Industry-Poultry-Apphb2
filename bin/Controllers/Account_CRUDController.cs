using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace ranglerz_project.Controllers
{
      [SessionCheck]
    public class Account_CRUDController : Controller
    {
        AccountServices service = new AccountServices();

        public ActionResult Index()
        {
            string accView = (string)Session["accView"];
            
            

                if (accView == "Y")
                {
                    int maxId = service.lastHeadAccID();
                    ViewData["max"] = maxId + 1;
                    int subMaxId = service.lastSubHeadAccID();
                    ViewData["submax"] = subMaxId + 1;
                    int transMaxID = service.lastTransID();
                    ViewData["transmax"] = transMaxID + 1;
                    ViewBag.myList = service.allTransactionAccounts();
                    ViewBag.typeList = service.allGoodTypes();
                    return View(service.allAccounts());
                }
                if (Session["name"] != "Admin")
                {
                    int id = Convert.ToInt32(Session["userId"]);
                    return RedirectToAction("Index", "Home", new { id = id });
                }
                else
                {
                    int id = Convert.ToInt32(Session["adminId"]);
                    return RedirectToAction("Index", "Home", new { id = id });
                }
                        
           
        }
          
        public ActionResult Create()
        {
            string create =(string)Session["accCreate"];
            if (create == "Y")
            {
                return View();
            }
            return RedirectToAction("Home", "index");
        }


       [HttpPost]
        public ActionResult CreatePost( FormCollection collection)
        {
            try
            {
                MainAccount mainAccount = new MainAccount();
                AccountServices service = new AccountServices();
                mainAccount.code = collection["code"];
                mainAccount.code = "0"+ mainAccount.code;
                mainAccount.name = collection["name"];
                mainAccount.is_active = "Y";
                mainAccount.update_at = DateTime.UtcNow;
                mainAccount.created_at = DateTime.UtcNow;
                service.add(mainAccount);

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Create");
            }
        }

       [HttpPost]
       public ActionResult CreatePostHead( string id,FormCollection collection)
       {


                int identity = int.Parse(id);
              
               AccountServices service = new AccountServices();
               
               HeadAccount headAccount = new HeadAccount();
               MainAccount mainAccount = service.findMainAcc(identity);
               headAccount.main = mainAccount.name;
               headAccount.code = collection["code"];
               headAccount.name = collection["name"];
               headAccount.main_id = identity;
               headAccount.is_active = "Y";
               headAccount.update_at = DateTime.UtcNow;
               headAccount.created_at = DateTime.UtcNow;
               headAccount.code = "0" + headAccount.main_id + "00" + headAccount.code;
               service.addHeadAcc(headAccount);

               return RedirectToAction("Index");
        
        
       }

       [HttpPost]
       public ActionResult CreatePostSubHead(string id, FormCollection collection)
       {
       

           int identity = int.Parse(id);

           AccountServices service = new AccountServices();
           MainAccount main = service.findMainHead(identity);
           HeadAccount head = service.findHeadAcc(identity);
           SubHeadAccount sub = new SubHeadAccount();
           sub.head_id = head.head_id;
           sub.main_id = head.main_id;
           sub.parent = head.name;
           sub.code = collection["code"];
           sub.name = collection["name"];
           sub.is_active = "Y";
           sub.update_at = DateTime.UtcNow;
           sub.created_at = DateTime.UtcNow;
           sub.code = "0" + sub.main_id + "00" + sub.head_id + "000" + sub.code;
           service.addSubHeadAcc(sub);
           return RedirectToAction("Index");


       }

       [HttpPost]
       public ActionResult CreatePostTrans(string id, FormCollection collection)
       {
           try
           {

           
           AccountServices service = new AccountServices();
           string name = collection["name"];
          if(service.checkName(name) == true)
          {
              return Content("<script language='javascript' type='text/javascript'>alert('Name Already Exists , Try Again with diffrent name !');</script>");
          }


           int identity = int.Parse(id);

         
           SubHeadAccount sub = service.findSubHeadAcc(identity);
           TransactionAccount trans = new TransactionAccount();
           
           trans.type_ = collection["Type"];
           if (collection["OB"] == "Cr.")
           {
               trans.cr_ = Convert.ToInt32(collection["amount"]);
               trans.dr_ = 0;
               trans.balance = trans.cr_ * (-1);
           }
           else
           {
               trans.dr_ = Convert.ToInt32(collection["amount"]);
               trans.cr_ = 0;
               trans.balance = trans.dr_;
           }
           trans.head_id = sub.head_id;
           trans.main_id = sub.main_id;
           trans.sub_head_id = sub.sub_head_id;
           trans.description = "Opening new Account";
           trans.WEIGHT = collection["weight"];
           trans.opening_weight = trans.WEIGHT;
           trans.name = collection["name"];
           trans.is_active = "Y";
           
           trans.updated_at_ = DateTime.UtcNow;
           trans.created_at_ = Convert.ToDateTime(collection["date"]);
           // for Opening Balance store
           trans.parent = trans.balance.ToString();
           trans.code = "0";
           service.addTransAcc(trans);
          // AccountServices.addRistrictedAccount(trans.name);
           
           trans.code = "0" + trans.main_id + "00" + trans.head_id + "000" + trans.sub_head_id + "0000" + trans.id;
           service.save();
           if (sub.sub_head_id == 34) // finished good inventories id 
           {
               SubHeadAccount subHead = new SubHeadAccount();
               subHead.head_id = 16;
               subHead.main_id = 12;
               subHead.parent = "Sales of Products";
               subHead.code = "0" + sub.main_id + "00" + sub.head_id + "000";
               subHead.name = collection["name"] + " Sale ";
               subHead.is_active = "Y";
               subHead.update_at = DateTime.UtcNow;
               subHead.created_at = DateTime.UtcNow;
               service.addSubHeadAcc(subHead);
               service.save();

               subHead.code = subHead.sub_head_id.ToString();
               subHead.code ="0" + sub.main_id + "00" + sub.head_id + "000" + sub.sub_head_id;
               TransactionAccount transNew = new TransactionAccount();
               transNew.type_ = "income";
               transNew.cr_ = 0;
               transNew.dr_ = 0;
               transNew.balance = 0;
               transNew.head_id = 16;
               transNew.sub_head_id = subHead.sub_head_id;
               transNew.main_id = 12;
               transNew.description = "Opening new Account";
               transNew.name = collection["name"] + " Sale " + " Account ";
               transNew.is_active = "Y";

               transNew.updated_at_ = DateTime.UtcNow;
               transNew.created_at_ = Convert.ToDateTime(collection["date"]);
               transNew.code = "0";
               transNew.WEIGHT = trans.WEIGHT;
               transNew.opening_weight = trans.opening_weight;
               service.addTransAcc(transNew);
               transNew.code = "0" + transNew.main_id + "00" + transNew.head_id + "000" + transNew.sub_head_id + "0000" + transNew.id;
               transNew.sale_account_id = trans.id;
               transNew.parent = trans.parent;
               service.save();
               trans.sale_account_id = transNew.id;
               service.save();
           }
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

           }
           return RedirectToAction("Index");


       }

  
       
        
        public ActionResult Edit(int id)
        {
            return View();
        }

     
// Edit functionality start //

        public ActionResult EditTransactionAccount(int id)
        {
            TransactionAccount tr = service.findTransactionAccount(id);           
            return View(service.findTransactionAccount(id));
        }
        [HttpPost]
        public ActionResult EditTransactionAccount(int id, FormCollection collection)
        {
            try{

                TransactionAccount trans = service.findTransactionAccount(id);
                trans.type_ = collection["Type"];
                int openingamount = Convert.ToInt32(collection["amount"]);
                if (collection["OB"] == "Cr.")
                {
                    trans.cr_ = Convert.ToInt32(collection["amount"]);
                    trans.dr_ = 0;
                    trans.balance = trans.cr_ * (-1);
                    openingamount = openingamount * (-1);
                }
                else
                {
                    trans.dr_ = Convert.ToInt32(collection["amount"]);
                    trans.cr_ = 0;
                    trans.balance = trans.dr_;
              
                }

                trans.name = collection["name"];
                trans.opening_weight = collection["weight"];
                trans.updated_at_ = Convert.ToDateTime(collection["date"]);
                // for Opening Balance store
                trans.parent = openingamount.ToString();
        
                service.save();
              
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

            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                
                MainAccount main = service.findMainAcc(id);
                main.name = collection["name"];
                main.update_at = DateTime.UtcNow;
                service.save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult EditHeadAcc(int id, FormCollection collection)
        {
            try
            {

                HeadAccount head = service.findHeadAcc(id);
                head.name = collection["name"];
                head.update_at = DateTime.UtcNow;
                service.save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult EditSubHeadAcc(int id, FormCollection collection)
        {
            try
            {

               SubHeadAccount head = service.findSubHeadAcc(id);
                head.name = collection["name"];
                head.update_at = DateTime.UtcNow;
                service.save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }











       // Edit END //

        public ActionResult check()
        {
            return View();
        }

 
          // Delete Start //

      
        public ActionResult Delete(int id)
        {
            try
            {
                MainAccount main = service.findMainAcc(id);
                DateTime updateTime = DateTime.UtcNow;
                main.is_active ="N";
                main.update_at = updateTime;
                
                foreach(HeadAccount head in main.HeadAccounts)
                {
                    head.is_active = "N";
                    head.update_at = updateTime;
                    foreach (SubHeadAccount sub in head.SubHeadAccounts)
                    {
                        sub.is_active = "N";
                        sub.update_at = updateTime;
                        foreach (TransactionAccount trans in sub.TransactionAccounts)
                        {
                            trans.is_active = "N";
                            trans.updated_at_ = updateTime;
                        }
                    }

                }
               
                service.save();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteHead(int id)
        {
            try
            {
                HeadAccount main = service.findHeadAcc(id);
                DateTime updateTime = DateTime.UtcNow;
                main.is_active = "N";
                main.update_at = updateTime;

            
                foreach (SubHeadAccount sub in main.SubHeadAccounts)
                {
                    sub.is_active = "N";
                    sub.update_at = updateTime;
                    foreach (TransactionAccount trans in sub.TransactionAccounts)
                    {
                        trans.is_active = "N";
                        trans.updated_at_ = updateTime;
                    }
                }

                service.save();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult DeleteSub(int id)
        {
            try
            {
                SubHeadAccount main = service.findSubHeadAcc(id);
                DateTime updateTime = DateTime.UtcNow;
                main.is_active = "N";
                main.update_at = updateTime;
                foreach (TransactionAccount trans in main.TransactionAccounts)
                {
                    trans.is_active = "N";
                    trans.updated_at_ = updateTime;
                }
                service.save();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
          public ActionResult DeleteTransactionAccount(int id)
        {
            TransactionAccount tr = service.findTransactionAccount(id);
            tr.is_active = "N";
            service.save();
            return RedirectToAction("index");
        }



        // Delete END //

















    }
}
