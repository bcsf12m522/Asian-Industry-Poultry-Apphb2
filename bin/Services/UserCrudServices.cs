using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.ModelsBO;
using ranglerz_project.Models;
using System.Web.Mvc;

namespace ranglerz_project.Services
{
    public class UserCrudServices
    {
        Database1Entities1 db = new Database1Entities1();
        public bool Create(FormCollection collection, HttpPostedFileBase image, User user)
        {
            string name = collection["name"];
            string username = collection["username"];
            string password = collection["password"];
            string cnic = collection["cnic"];
            string location = collection["address"];
            string email = collection["email"];
            string phone = collection["phone"];
            string city = collection["city"];

            int amount_limit = Convert.ToInt32(collection["amountLimit"]);
         

            if (name !=null && username != null && password !=null)
            {
              
                user.image = new byte[image.ContentLength];
                image.InputStream.Read(user.image, 0, image.ContentLength);
                user.name = name;
                user.username = username;
                user.password = password;
                user.location = location;
                user.email = email;
                user.city = city;
                user.phone = phone;
                user.cnic = cnic;
                user.amount_limit = amount_limit;
                             
            
                // Users Permission

                if (collection["view"] != null) { user.option_view = "Y"; } else { user.option_view = "N"; }
                if (collection["create"] != null) { user.option_create = "Y"; } else { user.option_create = "N"; }
                if (collection["delete"] != null) { user.option_delete = "Y"; } else { user.option_delete = "N"; }
                if (collection["edit"] != null) { user.option_edit = "Y"; } else { user.option_edit = "N"; }

                // Accounts Permission

                if (collection["accView"] != null) { user.account_view = "Y"; } else { user.account_view = "N"; }
                if (collection["accCreate"] != null) { user.account_create = "Y"; } else { user.account_create = "N"; }
                if (collection["accDelete"] != null) { user.account_delete = "Y"; } else { user.account_delete = "N"; }
                if (collection["accUpdate"] != null) { user.account_update = "Y"; } else { user.account_update = "N"; }

                // Employee Permission

                if (collection["empView"] != null) { user.emplyee_view = 1; }
                if (collection["empCreate"] != null) { user.employee_add = 1; }
                if (collection["empDelete"] != null) { user.employee_delete = 1; }
                if (collection["empUpdate"] != null) { user.employee_edit = 1; }

                // Reports Permission

                if (collection["viewAll"] != null) { user.all_reports = 1; }
                if (collection["viewSale"] != null) { user.sale_reports = 1; }
                if (collection["viewExpense"] != null) { user.expense_reports = 1; }
                if (collection["viewBankPayment"] != null) { user.bankPaymentReports = 1; }
                if (collection["viewPurchase"] != null) { user.purchase_reports = 1; }
                if (collection["trialBalance"] != null) { user.trial_balance = 1; }
                

                

                if (collection["editAll"] != null) { user.all_reports_edit = 1; }
                if (collection["editSale"] != null) { user.sale_reports_edit = 1; }
                if (collection["editExpense"] != null) { user.expense_reports_edit = 1; }
                if (collection["editPurchase"] != null) { user.purchase_reports_edit = 1; }
                // Vouchers Permission

                if (collection["jv"] != null) { user.JV = 1; }
                if (collection["br"] != null) { user.BR = 1; }
                if (collection["bp"] != null) { user.BP = 1; }
                if (collection["cr"] != null) { user.CR = 1; }
                if (collection["cp"] != null) { user.CP = 1; }
                if (collection["sv"] != null) { user.SV = 1; }
                if (collection["pv"] != null) { user.PV = 1; }
                if (collection["upsv"] != null) { user.UPSV = 1; }
                if (collection["uppv"] != null) { user.UPPV = 1; }
                if (collection["ev"] != null) { user.EV = 1; }

                // SuperVision


                if (collection["supervision"] != null) { user.supervision = 1; }
                if (collection["homeScreen"] != null) { user.homeScreen = 1; }


                // Goods Management Permission

                if (collection["viewGoods"] != null) { user.view_goods = 1; }
                if (collection["addGood"] != null) { user.add_goods = 1; }
                if (collection["editGood"] != null) { user.edit_goods = 1; }
                if (collection["deleteGood"] != null) { user.delete_goods = 1; }
                if (collection["addType"] != null) { user.add_goodTypes = 1; }
                if (collection["editType"] != null) { user.edit_goodTypes = 1; }
                if (collection["deleteType"] != null) { user.delete_goodTypes = 1; }
                if (collection["viewType"] != null) { user.view_goodTypes = 1; }
                if (collection["unitRate"] != null) { user.add_unitRate = 1; }

                // Attendence Permission

                if (collection["atdView"] != null) { user.view_attendence = 1; } 
                if (collection["atdAdd"] != null) { user.add_attedence = 1; }
                if (collection["atdEdit"] != null) { user.edit_attendence = 1 ; } 
                if (collection["atdReports"] != null) { user.attendence_reports = 1 ; } 

                // Production Permission

                if (collection["productionReports"] != null) { user.addProduction = 1; }
                if (collection["productionProtien"] != null) { user.production_fat = 1; }
                if (collection["productionCarbon"] != null) { user.production_carbon = 1; }
                if (collection["productionBleachOil"] != null) { user.production_bleach = 1; }

                // New Permission

                if (collection["mainReports"] != null) { user.Main_Reports = 1; } else { user.Main_Reports = 0; }
                if (collection["multiVoucher"] != null) { user.Multi_Vouchers = 1; } else { user.Multi_Vouchers = 0; }
                if (collection["unitRate"] != null) { user.Unit_Rate = 1; } else { user.Unit_Rate = 0; }
                if (collection["stockReports"] != null) { user.Stock_Reports = 1; } else { user.Stock_Reports = 0; }
                if (collection["stockSummary"] != null) { user.Stock_summary = 1; } else { user.Stock_summary = 0; }
                 

                /////


                if (collection["orders"] != null) { user.S_P_Orders = 1; }
                if (collection["pending"] != null) { user.S_P_PendingOrders = 1; }
                if (collection["pin"] != null) { user.account_pin = 1; }
                if (collection["wev"] != null) { user.WEV = 1; }
                    

                foreach (var v in db.TransactionAccounts.ToList())
                {
                    
                }




                user.updated_at = DateTime.UtcNow;
                user.created_at = DateTime.UtcNow;
                user.is_active = "Y";
                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public List<UserBO> allUsers()
        {
            List<User> users = db.Users.Where(x=>x.is_active=="Y").ToList();
            List<UserBO> usersbo = new List<UserBO>();
            foreach(User user in users)

            {
                UserBO userbo = new UserBO();
                userbo.name = user.name;
                userbo.id = user.Id;
                userbo.username = user.username;
                userbo.password = user.password;
                userbo.cnic = user.cnic;
                userbo.email = user.email;
                userbo.phone = user.phone;
                userbo.location = user.location;
                userbo.city = user.city;
                userbo.image = user.image;
                userbo.updated_at = user.updated_at;
                userbo.is_active = user.is_active;
                userbo.view = user.option_view;
                userbo.edit = user.option_edit;
                userbo.delete = user.option_delete;
                userbo.create = user.option_create;
                userbo.created_at = user.created_at;
                usersbo.Add(userbo);
            }
            return usersbo;
        }
        public User find(int id)
        {
            User user = db.Users.Find(id);
            return user;
        }
        public User findByString(string username)
        {
            User user = db.Users.First(x => x.username == username);
            return user;
        }
        public List<User> findAllUsers()
        {
            List<User> users = db.Users.Where(x=>x.is_active=="Y").ToList();
            return users;
        }
        public  void save()
        {
            db.SaveChanges();
        }
        public bool checkUsername(string username)
        {
            try
            {
                string user = db.Users.First(x => x.username == username & x.is_active =="Y").ToString();

                if (user == null)
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        public void addPermission(Permission permision)
        {
      
            db.Permissions.Add(permision);

        }
        public List<Permission> findPermissions(int id)
        {
            try
            {
                List<Permission> list = db.Permissions.Where(x => x.user_id == id && x.is_active==1).ToList();
                return list;
            }
            catch(Exception)
            {
                return null;
            }
        }
        public void inActivePermissions(int id)
        {
            List<Permission> list  =  db.Permissions.Where(x => x.user_id == id).ToList();
            foreach(var per in list)
            {
                per.is_active = 0;
            }
            db.SaveChanges();
        }
        public List<Reports_Permissions> findPermissionsLedgers(int id)
        {
            try
            {
                List<Reports_Permissions> list = db.Reports_Permissions.Where(x => x.user_id == id && x.is_active_ =="Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActivePermissions(int id)
        {
            List<Reports_Permissions> list = db.Reports_Permissions.Where(x => x.user_id == id & x.is_active_ =="Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ ="N";
            }
            db.SaveChanges();
        }

        public void AddledgersPermision(Reports_Permissions permission)
        {
            db.Reports_Permissions.Add(permission);
        }


        //////////////////// Main Reports ///////////////

        public List<Main_Reports_Permissions> findPermissionsMainReports(int id)
        {
            try
            {
                List<Main_Reports_Permissions> list = db.Main_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActiveMainReportsPermissions(int id)
        {
            List<Main_Reports_Permissions> list = db.Main_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddMainReportsPermision(Main_Reports_Permissions permission)
        {
            db.Main_Reports_Permissions.Add(permission);
        }

        //////////////////// Sale Reports ///////////////

        public List<Sale_Reports_Permissions> findPermissionsSaleReports(int id)
        {
            try
            {
                List<Sale_Reports_Permissions> list = db.Sale_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActiveSaleReportsPermissions(int id)
        {
            List<Sale_Reports_Permissions> list = db.Sale_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddSaleReportsPermision(Sale_Reports_Permissions permission)
        {
            db.Sale_Reports_Permissions.Add(permission);
        }

        //////////////////// Purchase Reports ///////////////

        public List<Purchase_Reports_Permissions> findPermissionsPurchaseReports(int id)
        {
            try
            {
                List<Purchase_Reports_Permissions> list = db.Purchase_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActivePurchaseReportsPermissions(int id)
        {
            List<Purchase_Reports_Permissions> list = db.Purchase_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddPurchaseReportsPermision(Purchase_Reports_Permissions permission)
        {
            db.Purchase_Reports_Permissions.Add(permission);
        }


        //////////////////// Expense Reports ///////////////

        public List<Expense_Reports_Permissions> findPermissionsExpenseReports(int id)
        {
            try
            {
                List<Expense_Reports_Permissions> list = db.Expense_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActiveExpenseReportsPermissions(int id)
        {
            List<Expense_Reports_Permissions> list = db.Expense_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddExpenseReportsPermision(Expense_Reports_Permissions permission)
        {
            db.Expense_Reports_Permissions.Add(permission);
        }

        //////////////////// Production Reports ///////////////

        public List<Production_Reports_Permissions> findPermissionsProductionReports(int id)
        {
            try
            {
                List<Production_Reports_Permissions> list = db.Production_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActiveProductionReportsPermissions(int id)
        {
            List<Production_Reports_Permissions> list = db.Production_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddProductionReportsPermision(Production_Reports_Permissions permission)
        {
            db.Production_Reports_Permissions.Add(permission);
        }

        //////////////////// Bank Payment Reports ///////////////

        public List<bankPayment_Reports_Permissions> findPermissionsbankPaymentReports(int id)
        {
            try
            {
                List<bankPayment_Reports_Permissions> list = db.bankPayment_Reports_Permissions.Where(x => x.user_id == id && x.is_active_ == "Y").ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void deActivebankPaymentReportsPermissions(int id)
        {
            List<bankPayment_Reports_Permissions> list = db.bankPayment_Reports_Permissions.Where(x => x.user_id == id & x.is_active_ == "Y").ToList();
            foreach (var per in list)
            {
                per.is_active_ = "N";
            }
            db.SaveChanges();
        }

        public void AddbankPaymentReportsPermision(bankPayment_Reports_Permissions permission)
        {
            db.bankPayment_Reports_Permissions.Add(permission);
        }


        public List<TemporaryReport> findtemporaryReports(string  userID)
        {
            List<TemporaryReport> list = db.TemporaryReports.Where(x => x.user == userID & x.is_active =="Y").ToList();
            return list;
        }



    }
}