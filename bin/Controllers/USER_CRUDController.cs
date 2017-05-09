using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Services;
using ranglerz_project.ModelsBO;
using ranglerz_project.Models;

namespace ranglerz_project.Controllers
{
     [SessionCheck]
    public class USER_CRUDController : Controller
    {
         TransactionServices serviceAccounts = new TransactionServices();
         UserCrudServices service = new UserCrudServices();
       
        [HttpGet]
        public ActionResult Create()
        {
            UserCrudServices service = new UserCrudServices();
            
            string name=(string)Session["name"];
            List<TransactionAccount> allacc = serviceAccounts.allTransactionaccounts();
            if (name == "Admin")
            {
                return View(allacc);
            }
            else
            {
                string username = (string)(Session["username"]);
                User user = service.findByString(username);

                if (user.option_create == "Y")
                {
                    return View(allacc);
                }
            }
            
       
           return  RedirectToAction("Index", "Home");
            
        }

        [HttpPost]
        public ActionResult createPost(HttpPostedFileBase image,string name, string username, string password, string view, string create, string delete, string edit, string accView, string accCreate, string accUpdate, string accDelete , FormCollection collection)
        {
                UserCrudServices services = new UserCrudServices();

                string names = (string)Session["name"];
                string usernames = (string)Session["username"];
        
                if (services.checkUsername(username) == false && image != null && image.ContentLength > 0)
                {
                    User user = new User();

                    if (services.Create(collection, image, user))
                    {
                        foreach (var accName in serviceAccounts.allTransactionaccounts())
                        {
                            if(Request.Form[accName.name]!= null)
                            
                            {
                                string value = Request.Form[accName.name];
                                Permission p = new Permission();
                                p.user_id = user.Id;
                                p.account_Name = accName.name;
                                p.urls = "N/A";
                                services.addPermission(p);
                            }

                          
                        }

                        services.save();

                        return RedirectToAction("viewUsers");
                    }   
                }
        
                
                    return RedirectToAction("Create");      
        }

        public ActionResult viewUsers()
        {
            string sesion = (string)(Session["name"]);
            

            UserCrudServices service = new UserCrudServices();

            if (sesion=="Admin")
            {

                if (service.allUsers() != null)
                {
                    List<UserBO> usersbo = service.allUsers();
                    return View(usersbo);
                }
               
            }

            return RedirectToAction("userRistrictedView");
        }

        public ActionResult userRistrictedView()
        {
            Database1Entities1 db = new Database1Entities1();

            UserCrudServices service = new UserCrudServices();

            string username = (string)(Session["username"]);

            User user = service.findByString(username);

            RestrictUser rUser = new RestrictUser();
            rUser.allusers = service.findAllUsers();
            rUser.edit = user.option_edit;
            rUser.delete = user.option_delete;
      
            return View(rUser);
        }






        public ActionResult Edit(int id)
        {
            UserCrudServices service = new UserCrudServices();
            List<TransactionAccount> allacc = serviceAccounts.allTransactionaccounts();
            ViewBag.AllAccounts = allacc;
            string name = (string)Session["name"];

            if (name == "Admin")
            {
                User user = service.find(id);
                List<Permission> listPermisons = service.findPermissions(id);
                ViewBag.Permissions = listPermisons;
                return View(user);
            }
            string username = (string)(Session["username"]);
            User users = service.findByString(username);
            if (users.option_edit == "Y")
            {

                User user = service.find(id);
                List<Permission> listPermisons = service.findPermissions(id);
                ViewBag.Permissions = listPermisons;
                return View(user);

            }

            return RedirectToAction("Index", "Home");
            
        }

       

        [HttpPost]
        public ActionResult editPost(int id, FormCollection collection, HttpPostedFileBase image)
        {
            try
            {
                UserCrudServices service = new UserCrudServices();
                User user = new User();
                user = service.find(id);
                user.name = collection["name"];
                user.username = collection["username"];
                user.password = collection["password"];
                user.email = collection["email"];
                user.city = collection["city"];
                user.location = collection["address"];
                user.cnic = collection["cnic"];
                user.phone = collection["phone"];
                user.amount_limit = Convert.ToInt32(collection["amountlimit"]);
                user.updated_at = DateTime.UtcNow;

                if (image != null && image.ContentLength > 0)
                {
                    user.image = new byte[image.ContentLength];
                    image.InputStream.Read(user.image, 0, image.ContentLength);
                }

// Users Permission
 
                if (collection["view"] != null) { user.option_view = "Y"; } else { user.option_view = "N"; }
                if (collection["create"] != null) { user.option_create = "Y"; } else { user.option_create = "N"; }
                if (collection["delete"] != null) { user.option_delete = "Y"; } else { user.option_delete = "N"; }
                if (collection["edit"] != null) { user.option_edit = "Y"; } else { user.option_edit = "N"; }

// Accounts Permission

                if (collection["accView"] != null) { user.account_view = "Y"; } else { user.account_view = "N"; }
                if (collection["accCreate"] != null) { user.account_create = "Y"; } else { user.account_create = "N"; }
                if (collection["accDelete"] != null) { user.account_delete= "Y"; } else { user.account_delete = "N"; }
                if (collection["accUpdate"] != null) { user.account_update = "Y"; } else { user.account_update = "N"; }

// Employee Permission

                if (collection["empView"] != null) { user.emplyee_view = 1; } else { user.emplyee_view = 0; }
                if (collection["empCreate"] != null) { user.employee_add = 1; } else { user.employee_add = 0; }
                if (collection["empDelete"] != null) { user.employee_delete = 1; } else { user.employee_delete = 0; }
                if (collection["empUpdate"] != null) { user.employee_edit = 1; } else { user.employee_edit = 0; }

// Reports Permission

                if (collection["viewAll"] != null) { user.all_reports = 1; } else { user.all_reports = 0; }
                if (collection["viewSale"] != null) { user.sale_reports = 1; } else { user.sale_reports = 0; }
                if (collection["viewExpense"] != null) { user.expense_reports = 1; } else { user.expense_reports = 0; }
                if (collection["trialBalance"] != null) { user.trial_balance = 1; } else { user.trial_balance = 0;}

                if (collection["viewPurchase"] != null) { user.purchase_reports = 1; }else{ user.purchase_reports = 0; }
                if (collection["editPurchase"] != null) { user.purchase_reports_edit = 1; }else{ user.purchase_reports_edit = 0; }

                if (collection["editAll"] != null) { user.all_reports_edit = 1; } else { user.all_reports_edit = 0; }
                if (collection["editSale"] != null) { user.sale_reports_edit = 1; } else { user.sale_reports_edit = 0; }
                if (collection["editExpense"] != null) { user.expense_reports_edit = 1; } else { user.expense_reports_edit = 0; }

                if (collection["viewBankPayment"] != null) { user.bankPaymentReports = 1; } else { user.bankPaymentReports = 0; }

                // productionReports

                if (collection["productionReports"] != null) { user.addProduction = 1; } else { user.addProduction = 0; }

// Vouchers Permission

                if (collection["jv"] != null) { user.JV = 1; } else { user.JV = 0; }
                if (collection["br"] != null) { user.BR = 1; } else { user.BR = 0; }
                if (collection["bp"] != null) { user.BP = 1; } else { user.BP = 0; }
                if (collection["cr"] != null) { user.CR = 1; } else { user.CR = 0; }
                if (collection["cp"] != null) { user.CP = 1; } else { user.CP = 0; }
                if (collection["sv"] != null) { user.SV = 1; } else { user.SV = 0; }
                if (collection["pv"] != null) { user.PV = 1; } else { user.PV = 0; }
                if (collection["upsv"] != null) { user.UPSV = 1; } else { user.UPSV = 0; }
                if (collection["uppv"] != null) { user.UPPV = 1; } else { user.UPPV = 0; }
                if (collection["ev"] != null) { user.EV = 1; } else { user.EV = 0; }



// SuperVision


                if (collection["supervision"] != null) { user.supervision = 1; } else { user.supervision = 0; }
                if (collection["homeScreen"] != null) { user.homeScreen = 1; } else { user.homeScreen = 0; }

 // Goods Management Permission

                if (collection["viewGoods"] != null) { user.view_goods = 1; } else { user.view_goods = 0; }
                if (collection["addGood"] != null) { user.add_goods = 1; } else { user.add_goods = 0; }
                if (collection["editGood"] != null) { user.edit_goods = 1; } else { user.edit_goods = 0; }
                if (collection["deleteGood"] != null) { user.delete_goods = 1; } else { user.delete_goods = 0; }
                if (collection["addType"] != null) { user.add_goodTypes = 1; } else { user.add_goodTypes = 0; }
                if (collection["editType"] != null) { user.edit_goodTypes = 1; } else { user.edit_goodTypes = 0; }
                if (collection["deleteType"] != null) { user.delete_goodTypes = 1; } else { user.delete_goodTypes = 0; }
                if (collection["viewType"] != null) { user.view_goodTypes = 1; } else { user.view_goodTypes = 0; }
                if (collection["unitRate"] != null) { user.add_unitRate = 1; } else { user.add_unitRate = 0; }

  // Attendence Permission

                if (collection["atdView"] != null) { user.view_attendence = 1; } else { user.view_attendence = 0; }
                if (collection["atdAdd"] != null) { user.add_attedence = 1; } else { user.add_attedence = 0; }
                if (collection["atdEdit"] != null) { user.edit_attendence = 1; } else { user.edit_attendence = 0; }
                if (collection["atdReports"] != null) { user.attendence_reports = 1; } else { user.attendence_reports = 0; }

 // Production Permission

                if (collection["productionReports"] != null) { user.addProduction = 1; } else { user.addProduction = 0; }
                if (collection["productionProtien"] != null) { user.production_fat = 1; } else { user.production_fat = 0; }
                if (collection["productionCarbon"] != null) { user.production_carbon = 1; } else { user.production_carbon = 0; }
                if (collection["productionBleachOil"] != null) { user.production_bleach = 1; } else { user.production_bleach = 0; }

// New Permission


                if (collection["mainReports"] != null) { user.Main_Reports = 1; } else { user.Main_Reports = 0; }
                if (collection["multiVouchers"] != null) { user.Multi_Vouchers = 1; } else { user.Multi_Vouchers = 0; }
                if (collection["unitRates"] != null) { user.Unit_Rate = 1; } else { user.Unit_Rate = 0; }
                if (collection["stockReports"] != null) { user.Stock_Reports = 1; } else { user.Stock_Reports = 0; }
                if (collection["stockSummary"] != null) { user.Stock_summary = 1; } else { user.Stock_summary = 0; }


                if (collection["orders"] != null) { user.S_P_Orders = 1; } else { user.S_P_Orders = 0; }
                if (collection["pending"] != null) { user.S_P_PendingOrders = 1; } else { user.S_P_PendingOrders = 0; }
                if (collection["pin"] != null) { user.account_pin = 1; } else { user.account_pin = 0; }
                if (collection["wev"] != null) { user.WEV = 1; } else { user.WEV = 0; }

                Database1Entities1 db = new Database1Entities1();
                UserCrudServices services = new UserCrudServices();
                services.inActivePermissions(user.Id);
                foreach (var accName in serviceAccounts.allTransactionaccounts())
                {
                    if (Request.Form[accName.name] != null)
                    {
                        string value = Request.Form[accName.name];
                        Permission permit = new Permission();
                        permit.user_id = user.Id;
                        permit.account_Name = accName.name;
                        permit.urls = "N/A";
                        permit.is_active = 1;
                        db.Permissions.Add(permit);
                        db.SaveChanges();
          
                    }


                }


                service.save();

                return RedirectToAction("viewUsers");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            UserCrudServices service = new UserCrudServices();

            string name = (string)Session["name"];

            if (name == "Admin")
            {
                User user = service.find(id);
                user.is_active = "N";
                user.updated_at = DateTime.UtcNow;
                service.save();
                return RedirectToAction("viewUsers");
                
            }

            else
            {
                try
                {


                    string username = (string)(Session["username"]);
                    User users = service.findByString(username);
                    if (users.option_delete == "Y")
                    {
                        User user = service.find(id);
                        user.is_active = "N";
                        user.updated_at = DateTime.UtcNow;
                        service.save();
                        return RedirectToAction("viewUsers");
                    }

                    return RedirectToAction("Index", "Home");


                }
                catch
                {
                    return View();
                }
            }
        }

        public ActionResult Active(int id)
        {
            UserCrudServices service = new UserCrudServices();
            User user = service.find(id);
            user.is_active = "Y";
            user.updated_at = DateTime.UtcNow;
            service.save();
            return RedirectToAction("viewUsers");    
        }

         public ActionResult ReportsPermissions()
        {
       
              List<User> users = service.findAllUsers();
              return View(users);
        }

         [HttpGet]
         public ActionResult ReportsPermssionGet(string user)
        {

            int id = Convert.ToInt32(user);
            User userRecord = service.find(id);
            List<User> users = service.findAllUsers();
            return View(userRecord);
        }
         [HttpGet]
         public ActionResult Ledgers(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsLedgers(id);
             List <TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult Ledgers(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActivePermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Reports_Permissions permit = new Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ ="Y";
                     service.AddledgersPermision(permit);
                     service.save();
                 }


             }


             return RedirectToAction("ReportsPermissions");
             
             
             
         }

         // Main Reports //

         [HttpGet]
         public ActionResult MainReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsMainReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult MainReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActiveMainReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Main_Reports_Permissions permit = new Main_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddMainReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }



         // sale Reports //

         [HttpGet]
         public ActionResult SaleReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsSaleReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult SaleReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActiveSaleReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Sale_Reports_Permissions permit = new Sale_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddSaleReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }



         // purchase Reports //

         [HttpGet]
         public ActionResult PurchaseReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsPurchaseReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult PurchaseReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActivePurchaseReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Purchase_Reports_Permissions permit = new Purchase_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddPurchaseReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }




         // Expense Reports //

         [HttpGet]
         public ActionResult ExpenseReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsExpenseReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult ExpenseReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActiveExpenseReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Expense_Reports_Permissions permit = new Expense_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddExpenseReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }


         // Production Reports //

         [HttpGet]
         public ActionResult ProductionReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsProductionReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult ProductionReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActiveProductionReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     Production_Reports_Permissions permit = new Production_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddProductionReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }


         // BankPayment Reports //

         [HttpGet]
         public ActionResult BankPaymentReports(int id)
         {
             User userRecord = service.find(id);
             ViewBag.AllAccounts = serviceAccounts.allTransactionaccounts();
             ViewBag.Permissions = service.findPermissionsbankPaymentReports(id);
             List<TransactionAccount> listOfAccounts = serviceAccounts.allTransactionaccounts();
             return View(userRecord);
         }
         [HttpPost]
         public ActionResult BankPaymentReports(int id, FormCollection collection)
         {
             User userRecord = service.find(id);
             service.deActivebankPaymentReportsPermissions(id);
             foreach (var accName in serviceAccounts.allTransactionaccounts())
             {
                 if (Request.Form[accName.name] != null)
                 {
                     string value = Request.Form[accName.name];
                     bankPayment_Reports_Permissions permit = new bankPayment_Reports_Permissions();
                     permit.user_id = userRecord.Id;
                     permit.account_name = accName.name;
                     permit.is_active_ = "Y";
                     service.AddbankPaymentReportsPermision(permit);
                     service.save();
                 }

             }

             return RedirectToAction("ReportsPermissions");
         }

         public ActionResult Supervision()
         {
             //if (Session["name"] != "Admin")
             //{
             //    return View(service.findAllUsers());
             //}
             //else
             //{
             //    return RedirectToAction("ViewTempReports", "TemporaryReports");
             //}
             return View(service.findAllUsers());
         }

         [HttpGet]
        
         [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
         public ActionResult SupervisionPost(string user)
         {
             int orignalId = Convert.ToInt32(user);
             User users = service.find(orignalId);
             List<TemporaryReport> listOfTemporaryReports = service.findtemporaryReports(users.username);
             ViewBag.allUsers = service.findAllUsers();
             return View(listOfTemporaryReports);
         }
    }
}
