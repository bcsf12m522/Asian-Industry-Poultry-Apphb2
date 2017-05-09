using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
namespace ranglerz_project.Controllers
{
    public class HomeController : Controller
    {

        Database1Entities1 db = new Database1Entities1();
        AccountServices service = new AccountServices();
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]

        public ActionResult checkLogin(string username, string password, string Permission)
        {
            Login_Check loginCheck= new Login_Check();

                try
                    {

                if (loginCheck.check(username, password, Permission) != null)
                {
                
                    LoginBO loginBo = loginCheck.check(username, password, Permission);

                    string value = loginBo.user;


                    if (value == "Admin")
                    {

                        Admin admin = (Admin)loginBo.obj;
                        Session["adminId"] = admin.Id;
                        Session["username"] = admin.username;
                        Session["name"] = "Admin";
                        Session["create"] = "Y";
                        Session["edit"] = "Y";
                        Session["delete"] = "Y";
                        Session["view"] = "Y";
                        Session["accView"] = "Y";
                        Session["accCreate"] = "Y";
                        Session["accDelete"] = "Y";
                        Session["accUpdate"] = "Y";

                        Session["JV"] = 1;
                        Session["BR"] = 1;
                        Session["BP"] = 1;
                        Session["CR"] = 1;
                        Session["CP"] = 1;
                        Session["SV"] = 1;
                        Session["PV"] = 1;
                        Session["UPSV"] = 1;
                        Session["UPPV"] = 1;
                        Session["EV"] = 1;



                        Session["emplyee_view"] = 1;
                        Session["employee_add"] = 1;
                        Session["employee_edit"] = 1;
                        Session["employee_delete"] = 1;
                        Session["all_reports_edit"] = 1;
                        Session["sale_reports_edit"] = 1;
                        Session["expense_reports_edit"] = 1;



                        Session["all_reports"] = 1;
                        Session["sale_reports"] = 1;
                        Session["expense_reports"] = 1;
                        Session["trialBalance"] = 1;

                        Session["supervision"] = 1;


                        Session["view_goods"] = 1;
                        Session["add_goods"] = 1;
                        Session["edit_goods"] = 1;
                        Session["delete_goods"] = 1;
                        Session["view_attendence"] = 1;
                        Session["edit_attendence"] = 1;
                        Session["add_attedence"] = 1;
                        Session["attendence_reports"] = 1;
                        Session["add_goodTypes"] = 1;
                        Session["edit_goodTypes"] = 1;
                        Session["delete_goodTypes"] = 1;
                        Session["view_goodTypes"] = 1;
                        Session["add_unitRate"] = 1;
                        Session["homeScreen"] = 1;

                        Session["addProduction"] = 1;
                        Session["bankPaymentReports"] = 1;

                        Session["productionCarbon"] = 1;
                        Session["productionBleach"] = 1;
                        Session["productionFat"] = 1;
                        Session["ID"] = admin.Id;
                        Session["purchase_reports"] = 1;
                        Session["purchase_reports_edit"] = 1;

                        // new permisions

                        Session["mainReportss"] = 1;
                        Session["multiVouchers"] = 1;
                        Session["unitratess"] = 1;
                        Session["stockReportss"] = 1;
                        Session["stockSummary"] = 1;

                        Session["pin"] = 1;
                        Session["pending"] = 1;
                        Session["orders"] = 1;
                        Session["wev"] = 1;
 
                        var Base64 = Convert.ToBase64String(admin.image_admin);
                        var imgsrc = string.Format("data:img/gif;base64,{0}", Base64);
                        Session["image"] = imgsrc;

                        return RedirectToAction("index", "Admin", new { id = admin.Id });


                    }
                

                else if (value == "User")
                {
                    
                        User user = (User)loginBo.obj;



                        Session["username"] = user.username;
                        Session["name"] = user.name;
                        Session["create"] = user.option_create;
                        Session["edit"] = user.option_edit;
                        Session["delete"] = user.option_delete;
                        Session["view"] = user.option_view;
                        Session["accView"] = user.account_view;
                        Session["accCreate"] = user.account_create;
                        Session["accDelete"] = user.account_delete;
                        Session["accUpdate"] = user.account_update;

                        Session["JV"] = user.JV;
                        Session["BR"] = user.BR;
                        Session["BP"] = user.BP;
                        Session["CR"] = user.CR;
                        Session["CP"] = user.CP;
                        Session["SV"] = user.SV;
                        Session["PV"] = user.PV;
                        Session["UPSV"] = user.UPSV;
                        Session["UPPV"] = user.UPPV;
                        Session["EV"] = user.EV;



                        Session["emplyee_view"] = user.emplyee_view;
                        Session["employee_add"] = user.employee_add;
                        Session["employee_edit"] = user.employee_edit;
                        Session["employee_delete"] = user.employee_delete;
                        Session["all_reports_edit"] = user.all_reports_edit;
                        Session["sale_reports_edit"] = user.sale_reports_edit;
                        Session["expense_reports_edit"] = user.expense_reports_edit;



                        Session["all_reports"] = user.all_reports;
                        Session["sale_reports"] = user.sale_reports;
                        Session["expense_reports"] = user.expense_reports;
                        Session["trialBalance"] = user.trial_balance;


                        Session["supervision"] = user.supervision;
                        Session["homeScreen"] = user.homeScreen;

                        Session["view_goods"] = user.view_goods;
                        Session["add_goods"] = user.add_goods;
                        Session["edit_goods"] = user.edit_goods;
                        Session["delete_goods"] = user.delete_goods;
                        Session["view_attendence"] = user.view_attendence;
                        Session["edit_attendence"] = user.edit_attendence;
                        Session["add_attedence"] = user.add_attedence;
                        Session["attendence_reports"] = user.attendence_reports;
                        Session["add_goodTypes"] = user.add_goodTypes;
                        Session["edit_goodTypes"] = user.edit_goodTypes;
                        Session["delete_goodTypes"] = user.delete_goodTypes;
                        Session["view_goodTypes"] = user.view_goodTypes;
                        Session["add_unitRate"] = user.add_unitRate;

                        Session["homeScreen"] = user.homeScreen;
                        Session["addProduction"] = user.addProduction;
                        Session["bankPaymentReports"] = user.bankPaymentReports;


                        Session["productionCarbon"] = user.production_carbon;
                        Session["productionBleach"] = user.production_bleach;
                        Session["productionFat"] = user.production_fat;

                        Session["purchase_reports"] = user.purchase_reports;
                        Session["purchase_reports_edit"] = user.purchase_reports_edit;

                        Session["amount_limit"] = user.amount_limit;
                        Session["user_id"] = user.Id;
                        Session["ID"] = user.Id;
                        var Base64 = Convert.ToBase64String(user.image);
                        var imgsrc = string.Format("data:img/gif;base64,{0}", Base64);
                        Session["image"] = imgsrc;
                        PermissionForAccounts.permisions = db.Permissions.Where(x => x.user_id == user.Id && x.is_active==1).ToList();
                        PermissionForAccounts.permisionLedgers = db.Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ =="Y").ToList();
                        PermissionForAccounts.permisionSales = db.Sale_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        PermissionForAccounts.permisionPurchase = db.Purchase_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        PermissionForAccounts.permisionExpenses= db.Expense_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        PermissionForAccounts.permisionProductions = db.Production_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        PermissionForAccounts.permisionMainReports = db.Main_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        PermissionForAccounts.permisionBankPaymentReports = db.bankPayment_Reports_Permissions.Where(x => x.user_id == user.Id & x.is_active_ == "Y").ToList();
                        Session["userId"] = user.Id;



                        Session["mainReportss"] = user.Main_Reports;
                        Session["multiVouchers"] =user.Multi_Vouchers;
                        Session["unitratess"] = user.Unit_Rate;
                        Session["stockReportss"] = user.Stock_Reports;
                        Session["stockSummary"] = user.Stock_summary;


                        Session["pin"] = user.account_pin;
                        Session["pending"] = user.S_P_PendingOrders;
                        Session["orders"] = user.S_P_Orders;
                        Session["wev"] = user.WEV;

                        return RedirectToAction("index", "Users", new { id = user.Id });

                    }
                   
                }
                   

            }
                catch (Exception)
                {
                    return RedirectToAction("index");
                }
            return RedirectToAction("index", "Home");
        }




        public ActionResult ChangePasswordAdmin(int id)
        {

            Admin admin = db.Admins.Find(id);
            return View(admin);
        }
        [HttpPost]

        public ActionResult  ChangePasswordAdmin(int id, string password)
        {
            Admin admin = db.Admins.Find(id);
            admin.password = password;
            db.SaveChanges();
            return RedirectToAction("index","Home");
        }


        public ActionResult ChangePassword(int id)
        {

            User user = db.Users.Find(id);
            return View(user);
        }
        [HttpPost]

        public ActionResult ChangePassword(int id, string password)
        {
            User user = db.Users.Find(id);
            user.password = password;
            db.SaveChanges();
            return RedirectToAction("index","Home");
        }


          [SessionCheck]
        public ActionResult PinAccounts(int id)
        {
            ViewBag.id = id;
            if (Session["name"] != "Admin")
            {
                List<Pin_Accounts> listOfAccounts = service.findPinAccountsOfUser(id);
                ViewBag.pinAccounts = listOfAccounts;
            }
            else
            {
                List<Pin_Accounts> listOfAccounts = service.findPinAccountsOfAdmin(id);
                ViewBag.pinAccounts = listOfAccounts;
            }
            return View(service.allTransactionAccounts());
        }

        [HttpPost]

        public ActionResult PinAccounts(int id, FormCollection collection)
        {
            if (Session["name"] != "Admin")
            {
                service.deActivePinAccountsOfUser(id);
            }
            else
            {
                service.deActivePinAccountsOfAdmin(id);
            }
            foreach (var accName in service.allTransactionAccounts())
            {
               
                if (Request.Form[accName.name] != null)
                {
                    Pin_Accounts pin = new Pin_Accounts();

                    if (Session["name"] != "Admin")
                    {
                        string value = Request.Form[accName.name];    
                        pin.User_id =id;
                        pin.Account_id = accName.id;
                        pin.User_type = "User";
                        pin.Account_name = accName.name;
                        pin.is_active = 1;
                   
                    }
                    else
                    {
                        string value = Request.Form[accName.name];
                        pin.Admin_id = id;
                        pin.Account_id = accName.id;
                        pin.User_type = "Admin";
                        pin.Account_name = accName.name;
                        pin.is_active = 1;
                
                    }

                    service.addPinAccounts(pin);
                    service.save();
                    
                }


            }
            return RedirectToAction("PinAccounts");
        }


          [SessionCheck]
        public ActionResult logout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Index");
        }
       
        //public ActionResult setBalance()
        //{
        //    Database1Entities1 db = new Database1Entities1();
        //    List<TransactionAccount> list = db.TransactionAccounts.Where(x => x.id == 168 || x.id == 169 || x.id == 170 || x.id == 171 || x.id == 172 || x.id == 173 || x.id == 174 || x.id == 175 || x.id == 176 || x.id == 177 || x.id == 178 || x.id == 197 || x.id == 198 || x.id == 199 || x.id == 213 || x.id == 214 || x.id == 215 || x.id == 216 ).ToList();
        //    foreach(var acc in list)
        //    {
        //        acc.parent = acc.balance.ToString();
        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("index","Home");
        //}

        public ActionResult allAccounts()
        {
           
            return View( db.TransactionAccounts.Where(x => x.is_active == "Y").ToList());
        }
        public ActionResult allTemporaryReports()
        {

            return View(db.TemporaryReports.Where(x => x.Id >190 && x.is_active == "Y").ToList());
        }

        public ActionResult allTransactions()
        {

            return View(db.Transactions.Where(x => x.is_active == "Y").ToList());
        }

    }
}
       
    
   
     
