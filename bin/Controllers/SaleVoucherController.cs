using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using System.Web.Script.Serialization;

namespace ranglerz_project.Controllers
{
    [SessionCheck]
    public class SaleVoucherController : Controller
    {
        TransactionServices service = new TransactionServices();

        #region Vouchers and Reports
        public ActionResult Index()
        
        {
            int check = (int)Session["SV"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.myList = service.allGoods();
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "SV00" + num;
            return View(service.allTransactionaccounts());
        }


        


        public ActionResult AddSaleVoucher(int id)
        {

            if (service.checkID() == false)
            {
                return Content("IDS Conflict Error Occured While Procsesing Request");
            }





            TemporaryReport temp = service.findTemporaryReport(id);
            int nextID = id + 1;
            TemporaryReport tempTo = service.findTemporaryReport(nextID);
            string from = temp.from_account;
            string to = temp.to_account;
            int fromID =(int) temp.tax;
            double rate = 0;
            int idForRate = 0;
            float unitPrice = 0;
            if (temp.voucher_type =="UPSV" || temp.voucher_type =="UPPV")
            {
                try
                {
                    float checkingUnit = float.Parse(temp.unit_price);
                }
                catch (Exception)
                {
                    if (temp.builty_no_ != null)
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
                int checking =(int)temp.builty_no_;
            }
            catch (Exception)
            {
                if (temp.voucher_type == "SV")
                {
                    return RedirectToAction("Index","SaleVoucher");
                }
                else
                {
                    return RedirectToAction("unitPriceSV","SaleVoucher");
                }
            }


            int builty = Convert.ToInt32(temp.builty_no_);
           
            try
            {
                
 
                String session = Session["name"].ToString();
                TransactionAccount transFrom = new TransactionAccount();
                TransactionAccount trans_to = new TransactionAccount();
               
                Good good = service.findGood(fromID);
                if (temp.voucher_type=="UPPV" || temp.voucher_type=="PV")
                {
                    transFrom = service.findJvTransaction(from);
                    trans_to = good.TransactionAccount;
                    idForRate = transFrom.id;
                }
                else if (temp.voucher_type=="SV" || temp.voucher_type=="UPSV")
                {
                    transFrom = good.TransactionAccount;
                    trans_to = service.findJvTransaction(to);
                    idForRate = trans_to.id;
                }
                Transaction tr = new Transaction();
                Transaction transTo = new Transaction();
                if (temp.voucher_type == "UPSV" || temp.voucher_type =="UPPV")
                {
                    unitPrice = float.Parse(temp.unit_price);
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

                tr.truck = temp.truck;
                tr.builty_no_ = temp.builty_no_;
                tr.weight_empty = temp.weight_empty;
                tr.weight_load = temp.weight_load;
                tr.net_weight = temp.net_weight;
                tr.quantity = tr.net_weight.ToString();
                tr.good_ = good.good_Name;

                amount = Convert.ToInt32(rate * tr.net_weight);

                tr.bags = temp.bags;
                tr.created_at = temp.created_at;
                tr.updated_at = tr.created_at;
                tr.is_active = "Y";
                tr.code = transFrom.id;
                tr.description = temp.description;
                tr.cr = 0;
                tr.dr = amount;
                tr.balance = (transFrom.balance - amount);



                tr.from_account = from;
                tr.to_account = to;
                //transFrom.dr_ = amount;

                if (temp.voucher_type =="SV" || temp.voucher_type =="UPSV")
                {
                    if (temp.voucher_type == "SV")
                    {
                        tr.status = "Sale voucher added";
                        tr.voucher_type = "SV";
                     //   tr.voucher_code = "SV111";
                    }
                    else
                    {
                        tr.status = "Unit Price Sale Voucher Added";
                        tr.voucher_type = "UPSV";
                   //     tr.voucher_code = "UPSV111";
                    }
                }
                else
                {
                    if (temp.voucher_type == "PV")
                    {
                        tr.status = "Purchase Voucher added";
                   //     tr.voucher_code = "PV111";
                        tr.voucher_type = "PV";
                        tr.grn = temp.grn;
                        tr.deduction = temp.deduction;
                    }
                    else
                    {
                        tr.status = "Unit Price Purchase Voucher added";
                   //     tr.voucher_code = "UPPV111";
                        tr.voucher_type = "UPPV";
                        tr.grn = temp.grn;
                        tr.deduction = temp.deduction;

                    }
                }
                tr.trans_acc_id = transFrom.id;
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
                tr.unit_price = temp.unit_price;
                service.addTransaction(tr);
                transFrom.balance = tr.balance;
                tr.unique_key = transFrom.id;
                temp.is_active = "N";
               



                transTo.truck = tempTo.truck;
                transTo.builty_no_ = tempTo.builty_no_;
                transTo.weight_empty = tempTo.weight_empty;
                transTo.weight_load = tempTo.weight_load;
                transTo.net_weight = tempTo.net_weight;
                transTo.bags = tempTo.bags;
                transTo.created_at = tempTo.created_at;
                transTo.updated_at = tr.created_at;
                transTo.is_active = "Y";
                transTo.quantity = tr.quantity;
                transTo.good_ = good.good_Name;


                transTo.code = trans_to.id;
                transTo.description = tempTo.description;
                transTo.dr = 0;
                transTo.cr = amount;
                transTo.balance = (trans_to.balance + amount);



                transTo.from_account = from;
                transTo.to_account = to;
                if (temp.voucher_type == "SV" || temp.voucher_type == "UPSV")
                {
                    if (temp.voucher_type=="SV")
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
                    if (temp.voucher_type=="PV")
                    {
                        transTo.status = "Purchase Voucher added";
                        transTo.voucher_code = "PV111";
                        transTo.voucher_type = "PV";
                        transTo.grn = tempTo.grn;
                        transTo.deduction = tempTo.deduction;
                    }
                    else
                    {
                        transTo.status = "Unit Price Purchase Voucher added";
                        transTo.voucher_code = "UPPV111";
                        transTo.voucher_type = "UPPV";
                        transTo.grn = tempTo.grn;
                        transTo.deduction = tempTo.deduction;

                    }
                }
                transTo.trans_acc_id = trans_to.id;
                //trans_to.cr_ = amount;
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
                transTo.voucher_code = temp.voucher_code;
                transTo.unit_price = tr.unit_price;
                service.addTransaction(transTo);
                trans_to.balance = transTo.balance;
                transTo.unique_key = tr.unique_key;
                tempTo.is_active = "N";
                trans_to.updated_at_ = tr.updated_at;
                transFrom.updated_at_ = tr.updated_at;
          
                // for Weight maintenece //

                //if (tr.voucher_type == "PV" || tr.voucher_type != "UPPV")


                //    if (transFrom.WEIGHT != null)
                //    {
                //        int weight = Convert.ToInt32(transFrom.WEIGHT);
                //        weight = (int)(weight - temp.net_weight);
                //        transFrom.WEIGHT = weight.ToString();
                //    }

                //    if (trans_to.WEIGHT != null)
                //    {
                //        int weight = Convert.ToInt32(trans_to.WEIGHT);
                //        weight = (int)(weight + temp.net_weight);
                //        trans_to.WEIGHT = weight.ToString();
                //    }
                

                

                //////////////////////////

                tr.Last_Edit = "None";
                transTo.Last_Edit = "None";

 


                service.save();



                #region Sale and Purchase order


                if (temp.order_number !=0 && temp.order_number !=-1 && temp.order_number !=null)
                {
                    Database1Entities1 db = new Database1Entities1();
                    if (temp.order_code == "SO")
                    {
                        Sale_Order so = db.Sale_Order.Find(temp.order_number);
                        so.weight = (int)(so.weight - temp.net_weight);
                        if(so.weight <= 0)
                        {
                            so.is_active = "N";
                        }
                    }
                    else if (temp.order_code =="PO")
                    {
                        Purchase_Order po = db.Purchase_Order.Find(temp.order_number);
                        po.weight = (int)(po.weight - temp.net_weight);
                        if (po.weight <= 0)
                        {
                            po.is_active = "N";
                        }
                    }
                    db.SaveChanges();
                }

                #endregion

                service.balanceUpdation(transFrom.id, trans_to.id);


                //////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////

                if (tr.voucher_type == "SV" || tr.voucher_type =="UPSV")
                {

                    Transaction transaction = new Transaction();
                    Transaction transactionTo = new Transaction();
                    TransactionAccount newTr = new TransactionAccount();
                    try{

                        newTr = service.findSaleAccount(transFrom.sale_account_id);
                    }
                    catch(Exception)
                    {
                           return Content("<script language='javascript' type='text/javascript'>alert('Link sale Account not Found !');</script>");
                    }

                    transaction.truck = tr.truck;
                    transaction.builty_no_ = tr.builty_no_;
                    transaction.weight_empty = tr.weight_empty;
                    transaction.weight_load = tr.weight_load;
                    transaction.net_weight = tr.net_weight;

                    transaction.quantity = tr.quantity;
                    tr.good_ = good.good_Name;

                    amount = Convert.ToInt32(rate * tr.net_weight);

                    transaction.bags = tr.bags;
                    transaction.created_at = tr.created_at;
                    transaction.updated_at = tr.created_at;
                    transaction.is_active = "Y";
                    transaction.code = transFrom.id;
                    transaction.description = tr.description;
                    transaction.cr = 0;
                    transaction.dr = amount;
                    transaction.balance = (transFrom.balance - amount);



                    transaction.from_account = transFrom.name;
                    transaction.to_account = trans_to.name;
                    //transFrom.dr_ = amount;
                    transaction.status = "Sale voucher added";
                    transaction.voucher_type = "SV";
                    transaction.voucher_code = tr.voucher_code;
                    transaction.trans_acc_id = transFrom.id;

                    if (session != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transaction.supervisor = user.name;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transaction.supervisor = Session["username"].ToString();
                    }


                    transaction.user_id = temp.user_id;
                    transaction.admin_id = temp.admin_id;


                    transaction.from_account = transFrom.name;
                    transaction.to_account = newTr.name;
                    transaction.voucher_code = tr.voucher_code;
                    transaction.unique_key = good.Id;
                    transaction.Last_Edit = tr.Last_Edit;
                    service.addTransaction(transaction);
                 
                    transactionTo.truck = tr.truck;
                    transactionTo.builty_no_ = tr.builty_no_;
                    transactionTo.weight_empty = tr.weight_empty;
                    transactionTo.weight_load = tr.weight_load;
                    transactionTo.net_weight = tr.net_weight;
                    transactionTo.bags = tr.bags;
                    transactionTo.created_at = tr.created_at;
                    transactionTo.updated_at = tr.created_at;
                    transactionTo.is_active = "Y";
                    transactionTo.quantity = tr.quantity;
                    transactionTo.good_ = tr.good_;
                    transactionTo.code = newTr.id;
                    transactionTo.description = tr.description;
                    transactionTo.dr = 0;
                    transactionTo.cr = amount;
                    transactionTo.balance = (trans_to.balance + amount);
                    transactionTo.from_account = transFrom.name;

                    transaction.to_account = newTr.name;
                    transactionTo.status = "Sale voucher added";
                    transactionTo.voucher_type = "SV";
                    transactionTo.trans_acc_id = newTr.id;
                    //newTr.cr_ = amount;

                    if (session != "Admin")
                    {
                        string username = Session["username"].ToString();
                        User user = service.findUser(username);
                        ViewBag.userName = user.name;
                        transactionTo.supervisor = user.name;
                    }
                    else
                    {
                        ViewBag.userName = Session["username"].ToString();
                        transactionTo.supervisor = Session["username"].ToString();
                    }


                    transactionTo.user_id = temp.user_id;
                    transactionTo.admin_id = temp.admin_id;


                    transactionTo.from_account = transFrom.name;
                    transactionTo.to_account = newTr.name;

                    transactionTo.Last_Edit = tr.Last_Edit;

                    service.addTransaction(transactionTo);
                    newTr.balance = newTr.balance + amount;
                    newTr.updated_at_ = tr.updated_at;
                    //newTr.dr_ = newTr.dr_ + amount;
                    int weight = Convert.ToInt32(newTr.WEIGHT);
                    int newWieght =(int)(weight + tr.net_weight);
                    newTr.WEIGHT = newWieght.ToString();
                    transactionTo.voucher_code = tr.voucher_code;

                    transaction.extra = "Sale"; // for dicriminate sale accounts //
                    transactionTo.extra = "Sale";

                    service.save();


                    service.balanceUpdation(transFrom.id, newTr.id);

                }


                ////////////////////////////////////////////////////////////////////////////
                return RedirectToAction("Supervision","USER_CRUD");

            }

            catch
            {
                if (temp.voucher_type == "SV" || temp.voucher_type == "UPSV")
                {
                    if (unitPrice == 0)
                    {
                        return RedirectToAction("Supervision", "USER_CRUD");
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Supervision", "USER_CRUD");
                        //return RedirectToAction("unitPriceSV");
                    }
                }
                else
                {
                    if (unitPrice == 0)
                    {
                        return RedirectToAction("purchaseVoucher");
                    }
                    else
                    {
                        return RedirectToAction("unitPricePV");
                    }
                }
            }




        }



        public ActionResult unitRate()

        {
            int check = (int)Session["add_unitRate"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Good> allGoods = service.allGoods();
            ViewBag.myList = allGoods ;
            return View(service.allTransactionaccounts());
        }

  

       
        [HttpPost]
        public ActionResult addUnitRate(FormCollection collection)
        {
            int check = (int)Session["add_unitRate"];
            string good = collection["good"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            
            string name = collection["account"];
            TransactionAccount transAccount = service.findJvTransaction(name);
            service.findAndSetUnitRates(transAccount.id, good);
            UnitRate uRate = new UnitRate();
            uRate.account_id = transAccount.id;
            uRate.good = collection["good"];
            uRate.rate =float.Parse( collection["rate"]);
            uRate.is_active = "Y";
            uRate.updated_at = DateTime.UtcNow;
            uRate.created_at = uRate.updated_at;
            service.addUnitRate(uRate);
            service.save();
            return RedirectToAction("unitRate");
        }


        /*
               [HttpPost]
               public ActionResult addGood(FormCollection collection)
               {
                   Good goods = new Good();
                   string name = collection["transactionAccount"];
                   TransactionAccount trans = service.findJvTransaction(name);
                   goods.good_Name = collection["name"];
                   goods.good_Type=collection["goodType"];
                   goods.is_active = "Y";
                   goods.transaction_account = trans.id;

                   service.addGood(goods);
                   service.save();
                   return RedirectToAction("index", "Admin");
               }

              public ActionResult viewGoods()
               {
                   return View(service.allGoods());
               }

               public ActionResult editGood(int id)
               {
                   ViewBag.typeList = service.allGoodTypes();
                   return View(service.findGood(id));
               }

               [HttpPost]
               public ActionResult editGoodPost(int id)
               {
                   Good good = service.findGood(id);
                   good.good_Name = Request.Form["name"];
                   good.good_Type = Request.Form["goodType"];
                   service.save();
                   return RedirectToAction("viewGoods");
               }
        

               public ActionResult deleteGood(int id)
               {
                  Good good= service.findGood(id);
                  good.is_active = "N";
                  service.save();
                  return RedirectToAction("viewGoods");
               }
               */
        public ActionResult salesReport()
        {
            int check =(int) Session["sale_reports"];
            if(check==0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(service.allTransactionaccounts());
        }

        [HttpPost]
        public ActionResult salesReportPost(string search, string dateStart, string dateEnd)
        {
               if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForSales(PermissionForAccounts.permisionSales,search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }

            int check = (int)Session["sale_reports"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            var balance = service.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
            return View(accounts);
        }

        [HttpPost]
        public ActionResult salesReportPostItemWise(int id, string dateStart, string dateEnd, string item)
        {
            TransactionAccount tr = service.findTransactionAccount(id);
            string search = tr.name;

               if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForSales(PermissionForAccounts.permisionSales,search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }

            int check = (int)Session["sale_reports"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.item = item;
            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            var balance = service.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
            return View(accounts);
        }
        public ActionResult purchaseVoucher()
        {
            int check = (int)Session["PV"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.myList = service.allGoods();
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "PV00" + num;

            return View(service.allTransactionaccounts());

        }
        

        public ActionResult unitPriceSV()
        {
            int check = (int)Session["UPSV"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.myList = service.allGoods();
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "UPSV00" + num;

            return View(service.allTransactionaccounts());
        }
        

        public ActionResult unitPricePV()
        {
            int check = (int)Session["UPPV"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.myList = service.allGoods();
            int num = TransactionServices.getMaxId();
            ViewBag.Code = "UPPV00" + num;

            return View(service.allTransactionaccounts());

        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////////////////////////



        // Transaction Edit Module //

        public ActionResult Edit(int id)
        {
            int allReports = Convert.ToInt32(Session["sale_reports_edit"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.myList = service.allGoods();
            ViewBag.Accounts = service.allTransactionaccounts();
            Transaction tr = service.findTransaction(id);
            ViewBag.type = tr.voucher_type;
            return View(service.findTransaction(id));
        }



        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            int allReports = Convert.ToInt32(Session["sale_reports_edit"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Database1Entities1 db = new Database1Entities1();
            string fromAccount = collection["from"];
            string toAccount = collection["to"];        
            Transaction transEdit = new Transaction();
            Transaction transNext = new Transaction();
            TransactionAccount fromAccountTrans = new TransactionAccount();
            TransactionAccount toAccountTrans = new TransactionAccount();

            if(id %2 ==0)
            {
                transEdit = db.Transactions.Find(id - 1);
                transNext = db.Transactions.Find(id);
            }
            else
            {
                transEdit = db.Transactions.Find(id);
                transNext = db.Transactions.Find(id+1);
            }          
           
            Good trans_from = service.findGoodByName(fromAccount);

            if(transEdit.voucher_type=="PV" || transEdit.voucher_type=="UPPV")
            {
                 fromAccountTrans = db.TransactionAccounts.Where(x => x.name == toAccount & x.is_active=="Y").First();
                 toAccountTrans = trans_from.TransactionAccount;
            }
            else
            {
                fromAccountTrans = trans_from.TransactionAccount;
                toAccountTrans = db.TransactionAccounts.Where(x => x.name == toAccount & x.is_active=="Y").First();
            }
         
            int amount = 0;
            double rate = 0;

            //if(transEdit.voucher_type =="PV" || transEdit.voucher_type =="UPPV")
            //{
            //    rate = service.findRate(fromAccountTrans.id, trans_from.good_Name);
            //}
            //else
            //{
            //    rate = service.findRate(toAccountTrans.id, trans_from.good_Name);
            //}

             
           

            //if(transEdit.voucher_type=="UPPV" || transEdit.voucher_type =="UPSV")
            //{
                transEdit.unit_price = collection["unitPrice"];
                transNext.unit_price = collection["unitPrice"];
                rate = float.Parse(transEdit.unit_price);
            //}
            transEdit.truck = collection["truck"];
            transEdit.builty_no_ = Convert.ToInt32(collection["builty"]);
            transEdit.weight_empty = Convert.ToInt32(collection["empty"]);
            transEdit.weight_load = Convert.ToInt32(collection["load"]);
            transEdit.net_weight = Convert.ToInt32(collection["net"]);
            transEdit.quantity = transEdit.net_weight.ToString();
            transEdit.good_ = trans_from.good_Name;
            transEdit.deduction = collection["deduction"];
            transEdit.grn = collection["grn"];

            transNext.truck = collection["truck"];
            transNext.builty_no_ = Convert.ToInt32(collection["builty"]);
            transNext.weight_empty = Convert.ToInt32(collection["empty"]);
            transNext.weight_load = Convert.ToInt32(collection["load"]);
            transNext.net_weight = Convert.ToInt32(collection["net"]);
            transNext.quantity = transEdit.net_weight.ToString();
            transNext.good_ = trans_from.good_Name;
            transNext.deduction = collection["deduction"];
            transNext.grn = collection["grn"];
           
            if(rate !=0)
            {
                amount = Convert.ToInt32(rate * transEdit.net_weight);
            }
            else
            {
                amount = transEdit.dr;
            }


            if (collection["clientWeight"] != null)
            {

                int client_weight = Convert.ToInt32(collection["clientWeight"]);

                transEdit.client_s_weight = client_weight;
                transNext.client_s_weight = client_weight;
         
                if (client_weight != 0)
                {
                    amount = Convert.ToInt32(rate * client_weight);
                }
            }


            transEdit.bags = collection["bags"];
            transEdit.created_at = Convert.ToDateTime(collection["date"]);
            transNext.created_at = transEdit.created_at;
            transEdit.dr = amount;
            transNext.cr = amount;

            // For linking transaction //

            transEdit.trans_acc_id = fromAccountTrans.id;
            transNext.trans_acc_id = toAccountTrans.id;

         
            //////////////////////////////////////

            transEdit.from_account = fromAccountTrans.name;
            transEdit.to_account = toAccountTrans.name;
            transNext.to_account = toAccountTrans.name;
            transNext.from_account = fromAccountTrans.name;
   
           
            transNext.client_s_weight = transEdit.client_s_weight;
            transEdit.description = collection["description"];
            transNext.description = collection["description"];
            db.SaveChanges();

            service.balanceUpdation(fromAccountTrans.id,toAccountTrans.id);

            if (transEdit.voucher_type == "SV" || transEdit.voucher_type == "UPSV")
            {
                return RedirectToAction("salesReport");
            }
            else
            {
                return RedirectToAction("purchaseReport");
            }
        }





        // Transaction Edit Module End  //




        [HttpPost]
        public string AjaxCall(int RouteID)
        {

           Database1Entities1 db = new Database1Entities1();
           string vendrList = "<option value=''>-- Select Vendor--*</option>";
           var goods = db.UnitRates.Where(x => x.account_id == RouteID & x.is_active=="Y").ToList();
            foreach(var good in goods)
            {
                vendrList += "<option value='" + good.good + "'>" + good.good + "</option>";
            }

            return vendrList;

          //  return new JsonResult (goods);  
            
        }

        

      

        public ActionResult salesReportByItem()
        {
            int check = (int)Session["sale_reports"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(service.allGoods());
        }

        [HttpPost]
        public ActionResult salesReportByItemPost(string search,string dateStart, string dateEnd)
        {
            //if (Session["name"] != "Admin")
            //{


            //    if (PermissionForAccounts.permisionCheckForSales(PermissionForAccounts.permisionSales, search) == false)
            //    {
            //        return RedirectToAction("index", "Home");
            //    }
            //}

            int check = (int)Session["sale_reports"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mylist = service.allGoods();
            List<Transaction> accounts = new List<Transaction>();
            accounts = service.findTransactionsByGoods(search);
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            return View (accounts);
        }



        public ActionResult purchaseReport()
        {
            int check = (int)Session["purchase_reports"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.type = "PV";
            return View(service.allTransactionaccounts());
        }

        [HttpPost]
        public ActionResult purchaseReportPost(string search, string dateStart, string dateEnd)
        {
            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForPurchases(PermissionForAccounts.permisionPurchase,search) == false)
                {
                    return RedirectToAction("index", "Home");
                }
            }



            int check = (int)Session["purchase_reports"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.search = search;
            List<TransactionAccount> accounts = service.findTransactionAccounts(search);
            var balance = service.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            List<TransactionAccount> allaccounts = service.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);
            ViewBag.MyList = allaccounts;
            return View(accounts);
        }

        #endregion

        #region Sale and Purchase Order

        [HttpPost]
        public string FindSaleOrder(int AccountId, string Item)
        {

            Database1Entities1 db = new Database1Entities1();
            string vendrList = null;
            string no = "Not Availabe";
            string value = "0";
            var sale_orders = db.Sale_Order.Where(x => x.Account_Id == AccountId & x.item == Item & x.is_active == "Y").ToList();

            if (sale_orders.Count == 0)
            {
                vendrList += "<option value='" + value + "'>" + no + "</option>";

            }
            else
            {
                foreach (var good in sale_orders)
                {
                    vendrList += "<option value='" + good.Id + "'>" + good.so_number + "</option>";
                }
            }
            return vendrList;

            //  return new JsonResult (goods);  

        }


        [HttpPost]
        public string FindPurchaseOrder(int AccountId, string Item)
        {

            Database1Entities1 db = new Database1Entities1();
            string vendrList = null;
            string no = "Not Availabe";
            string value = "0";
            var goods = db.Purchase_Order.Where(x => x.Account_Id == AccountId & x.item == Item & x.is_active == "Y").ToList();

            if (goods.Count == 0)
            {
                vendrList += "<option value='" + value + "'>" + no + "</option>";

            }
            else
            {
                foreach (var good in goods)
                {
                    vendrList += "<option value='" + good.Id + "'>" + good.po_number + "</option>";
                }
            }
            return vendrList;

            //  return new JsonResult (goods);  

        }

        #endregion
    }
}
