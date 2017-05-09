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
    public class GoodsManagementController : Controller
    {
        GoodsManagement service = new GoodsManagement();
        TransactionServices serviceTransaction = new TransactionServices();
        AccountServices serviceAccount = new AccountServices();

        [HttpGet]
        public ActionResult GoodTypes()
        {
            int check = (int)Session["add_goodTypes"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult GoodTypes(FormCollection collection)
        {
            try
            {
                Good_Types types = new Good_Types();
                types.type = collection["name"];
                types.code = "N/A";
                types.is_active = "Y";
                service.addGoodTypes(types);
                service.save();
                return RedirectToAction("ListGoodTypes");

            }
            catch (Exception)
            {
                return RedirectToAction("ListGoodTypes");
            }
        }
        public ActionResult EditGoodType(int id)
        {
            int check = (int)Session["edit_goodTypes"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(service.findGoodType(id));

        }
        [HttpPost]
        public ActionResult EditGoodType(int id, FormCollection collection)
        {
            int check = (int)Session["edit_goodTypes"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Good_Types types = service.findGoodType(id);
            types.type = collection["name"];
            types.code = "N/A";
            service.save();
            return RedirectToAction("ListGoodTypes");

        }
        public ActionResult DeleteGoodType(int id)
        {
            int check = (int)Session["delete_goodTypes"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Good_Types goodType = service.findGoodType(id);
            goodType.is_active = "N";
            service.save();
            return RedirectToAction("ListGoodTypes");
        }


        public ActionResult ListGoodTypes()
        {
            int check = (int)Session["view_goodTypes"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(service.listGoodTypes());
        }

        public ActionResult ListOfGoods()
        {

            int check = (int)Session["view_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }



            return View(service.listOfGoods());
        }


        //
        // GET: /GoodsManagement/Create

        public ActionResult Create()
        {

            int check = (int)Session["add_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }



            ViewBag.goodTypes = service.listGoodTypes();
            return View(serviceTransaction.allTransactionaccounts());
        }

        //
        // POST: /GoodsManagement/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {

            int check = (int)Session["add_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                Good good = new Good();
                good.created_at = DateTime.UtcNow;
                good.updated_at = good.created_at;
                good.good_Name = collection["name"];
                good.good_Type = collection["type"];
                //   good.vendor = Convert.ToInt32(collection["vendor"]);
                good.unit = collection["unit"];
                good.is_active = "Y";
                TransactionAccount tr = new TransactionAccount();
                var mainaccount = collection["mainAccount"];
                if (mainaccount == "1")
                {
                    tr.main_id = 3;
                    tr.head_id = 11;
                    tr.sub_head_id = 34;

                }
                else if (mainaccount == "2")
                {
                    tr.main_id = 4;
                    tr.head_id = 21;
                    tr.sub_head_id = 44;
                }
                else
                {
                    tr.main_id = 4;
                    tr.head_id = 21;
                    tr.sub_head_id = 45;
                }

                tr.created_at_ = DateTime.UtcNow;
                tr.updated_at_ = tr.created_at_;
                tr.description = "Opening New Account";
                tr.cr_ = 0;
                tr.dr_ = 0;
                tr.balance = 0;
                tr.code = "0";
                tr.type_ = "Goods Account";
                tr.is_active = "Y";
                tr.name = collection["name"] + " Main " + " Account";
                tr.opening_weight = "0";
                tr.WEIGHT = "0";
                service.addtransactionAccount(tr);
                service.save();

                good.transaction_account = tr.id;
                service.addGoods(good);
                service.save();
                tr.code = "0" + tr.main_id + "00" + tr.head_id + "000" + tr.sub_head_id + "0000" + tr.id;
                tr.parent = "0";
                service.save();

                if (mainaccount == "1")
                {

                    SubHeadAccount subHead = new SubHeadAccount();
                    subHead.head_id = 16;
                    subHead.main_id = 12;
                    subHead.parent = "Sales of Products";
                    subHead.code = subHead.head_id.ToString();
                    subHead.name = collection["name"] + " Sale ";
                    subHead.is_active = "Y";
                    subHead.update_at = DateTime.UtcNow;
                    subHead.created_at = DateTime.UtcNow;
                    serviceAccount.addSubHeadAcc(subHead);
                    service.save();

                    subHead.code = subHead.sub_head_id.ToString();

                    TransactionAccount transNew = new TransactionAccount();
                    transNew.type_ = "income";
                    transNew.cr_ = 0;
                    transNew.dr_ = 0;
                    transNew.balance = 0;
                    transNew.head_id = 16;
                    transNew.sub_head_id = subHead.sub_head_id;
                    transNew.main_id = 12;
                    transNew.description = "Opening new Account";
                    transNew.name = collection["name"] + " Main " + " Sale " + " Account ";
                    transNew.is_active = "Y";

                    transNew.updated_at_ = DateTime.UtcNow;
                    transNew.created_at_ = transNew.updated_at_;
                    transNew.code = "0";
                    transNew.opening_weight = "0";
                    transNew.WEIGHT = tr.WEIGHT;
                    serviceAccount.addTransAcc(transNew);
                    transNew.code = "0" + transNew.main_id + "00" + transNew.head_id + "000" + transNew.sub_head_id + "0000" + transNew.id;
                    transNew.sale_account_id = tr.id;
               
                    transNew.parent = "0";
                    service.save();
                    tr.sale_account_id = transNew.id;
                   
                    service.save();
                }

             


                AccountServices.addRistrictedAccount(tr.name);

                return RedirectToAction("ListOfGoods");
                
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

                return RedirectToAction("Create");

            }
        }
        public ActionResult Edit(int id)
        {

            int check = (int)Session["edit_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }


            ViewBag.transactionAccounts = serviceTransaction.allTransactionaccounts();
            ViewBag.goodTypes = service.listGoodTypes();

            return View(service.findGood(id));
        }

        //
        // POST: /GoodsManagement/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {

            int check = (int)Session["edit_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }




            try
            {
                Good good = service.findGood(id);
                good.updated_at = good.created_at;
                good.good_Name = collection["name"];
                good.good_Type = collection["type"];

                good.unit = collection["unit"];
                service.save();

                return RedirectToAction("listOfGoods");
            }
            catch
            {
                return View("Edit");
            }
        }

        //
        // GET: /GoodsManagement/Delete/5

        public ActionResult Delete(int id)
        {

            int check = (int)Session["delete_goods"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }




            Good good = service.findGood(id);
            good.is_active = "N";
            good.TransactionAccount.is_active = "N";
            service.save();
            return RedirectToAction("listOfGoods");
        }

        public ActionResult DateRange(int id)
        {
            ViewBag.id = id;

            return View();

        }


        [HttpGet]
        public ActionResult AddProduction(int id, string dateStart, string dateEnd)
        {
            Database1Entities1 db = new Database1Entities1();
            Good good = new Good();
            TransactionAccount account = new TransactionAccount();
            DateTime dateStarts = Convert.ToDateTime(dateStart);
            DateTime dateEnds = Convert.ToDateTime(dateEnd);
            int code = TransactionServices.getMaxId();
            ViewBag.Code = "PRV11" + code;

            int weight = 0;

            if (id == 1)
            {

                good = service.findGood(35); // 1 is ID of GOOD(chicken waste) in GoodManagemnet Table

                account = serviceTransaction.findTransactionAccount(good.transaction_account);

                List<Transaction> list = db.Transactions.Where(x => x.created_at >= dateStarts & x.created_at <= dateEnds & x.good_ == good.good_Name & x.voucher_type == "PV" & x.Id % 2 == 0 & x.is_active=="Y").ToList();

                foreach (var trans in list)
                {
                    if (trans.net_weight != null)
                    {
                        weight = (int)(weight + trans.net_weight);
                    }
                }

            }

            else if(id ==2)

             {
                good = service.findGood(46); // 12 is ID of GOOD (used tyre) in GoodManagemnet Table
                account = serviceTransaction.findTransactionAccount(good.transaction_account);

                List<Transaction> list = db.Transactions.Where(x => x.created_at >= dateStarts & x.created_at <= dateEnds & x.good_ == good.good_Name & x.voucher_type == "PV" & x.Id % 2 == 0 & x.is_active=="Y").ToList();

                foreach (var trans in list)
                {
                    weight = (int)(weight + trans.net_weight);
                }

            }

            else
            {
                good = service.findGood(39); // 10 is ID of GOOD (Chicken Oil) in GoodManagemnet Table
                account = serviceTransaction.findTransactionAccount(good.transaction_account);

                List<Transaction> list = db.Transactions.Where(x => x.created_at >= dateStarts & x.created_at <= dateEnds & x.good_ == good.good_Name & x.voucher_type == "PV" || x.voucher_type =="PRV" & x.Id % 2 == 0 & x.is_active =="Y").ToList();

                foreach (var trans in list)
                {
                    weight = (int)(weight + trans.net_weight);
                }

            }
            ViewBag.good = good.good_Name;
            ViewBag.ID = id;
            ViewBag.weight = weight;
            ViewBag.dateStart = dateStarts.Date;
            ViewBag.dateEnd = dateEnds.Date;
            return View(account);
        }

        [HttpPost]
        public ActionResult AddProduction(FormCollection collection)
        {
            try
            {
                TransactionServices service2 = new TransactionServices();

                if (service2.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }



                Database1Entities1 db = new Database1Entities1();
                int checking = 0;
                int id = Convert.ToInt32(collection["id"]);
                if (id == 1)
                {
                    int weight = Convert.ToInt32(collection["weight"]);
                    string accountName = collection["vendor"];
                    string dateSart = collection["dateStart"];
                    string dateEnd = collection["dateEnd"];
                    string good = collection["good"];
                    string description = collection["description"];
                    float protien = float.Parse(collection["protien"]);
                    float fat = float.Parse(collection["fat"]);
                    int totalProtien = Convert.ToInt32((protien / 100) * weight);
                    int totalFat = Convert.ToInt32((fat / 100) * weight);
                    string protienGood = "Protein";
                    string fatGood = "Crude Oil (Fat)";
                    Good goodProtien = db.Goods.Where(x => x.good_Name == protienGood & x.is_active =="Y").First();
                    Good goodFatOil = db.Goods.Where(x => x.good_Name == fatGood & x.is_active == "Y").First();
                    TransactionAccount protienTransAcc = db.TransactionAccounts.Find(goodProtien.transaction_account);
                    TransactionAccount fatOilTransAcc = db.TransactionAccounts.Find(goodFatOil.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == accountName).First();
                    double rate = 0;
                    // Finding Rate //
                    try
                    {
                        rate = Convert.ToInt32(collection["protienRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Protien !');</script>");
                    }

                    int amount = Convert.ToInt32(rate * totalProtien);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport trTo = new TemporaryReport();
                    tr.created_at =Convert.ToDateTime(collection["date"]);
                    tr.updated_at = tr.created_at;
                    tr.description = description;
                    tr.net_weight = totalProtien;
                    tr.is_active = "Y";
                    tr.good_ = protienGood;
                    tr.from_account = VendorAcc.name;
                    tr.to_account = protienTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amount;
                    //protienTransAcc.balance = VendorAcc.balance + amount;
                    tr.dr = amount;
                    tr.cr = 0;
                    tr.voucher_code = collection["code"];
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = totalProtien;
                    ///////

                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = description;
                    trTo.net_weight = totalProtien;
                    trTo.is_active = "Y";
                    trTo.good_ = protienGood;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = protienTransAcc.name;
                    trTo.cr = amount;
                    trTo.dr = 0;
                    trTo.voucher_code = collection["code"];
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = protienTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = totalProtien;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                     
                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
                        tr.user = username;
                    }
                    else
                    {
                    
                        tr.admin_id = 1;
                        trTo.admin_id = 1;
                        tr.user = Session["username"].ToString();
                    }
                    tr.pvr_type = 1;
                    trTo.pvr_type = 1;
                    tr.item = good;
                    trTo.item = good;
                    trTo.user = tr.user;
                    tr.amount = amount;
                    trTo.amount = amount;
                    db.TemporaryReports.Add(tr);
                    db.TemporaryReports.Add(trTo);
                    db.SaveChanges();



                    ///////////////////////////////////
                    double rateOil = 0;
                    // Finding Rate //
                    try
                    {
                        rateOil = Convert.ToInt32(collection["fatRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Fat/Chicken Oil !');</script>");
                    }

                    int amountFat = Convert.ToInt32(rateOil * totalFat);
                    TemporaryReport trans = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();
                    trans.created_at = Convert.ToDateTime(collection["date"]);
                    trans.updated_at = trans.created_at;
                    trans.description = description;
                    trans.net_weight = totalFat;
                    trans.is_active = "Y";
                    trans.good_ = fatGood;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = fatOilTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amountFat;
                    //fatOilTransAcc.balance = VendorAcc.balance + amountFat;
                    trans.dr = amountFat;
                    trans.cr = 0;
                    trans.voucher_code = collection["code"];
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = totalFat;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = description;
                    transTo.net_weight = totalFat;
                    transTo.is_active = "Y";
                    transTo.good_ = fatGood;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = fatOilTransAcc.name;
                    transTo.cr = amountFat;
                    transTo.dr = 0;
                    transTo.voucher_code = collection["code"];
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = fatOilTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = totalFat;

                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);

                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                        trans.user = username;
                    }
                    else
                    {

                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                        trans.user = Session["username"].ToString();
                    }
                    trans.pvr_type = 4;
                    transTo.pvr_type = 4;
                    trans.item = good;
                    transTo.item = good;
                    transTo.user = trans.user;
                    trans.amount = amount;
                    transTo.amount = amount;
                    db.TemporaryReports.Add(trans);
                    db.TemporaryReports.Add(transTo);

                    db.SaveChanges();

                    return RedirectToAction("ListOfGoods");
                }
                else if (id == 2)
                {
                    int weight = Convert.ToInt32(collection["weight"]);
                    string accountName = collection["vendor"];
                    string dateSart = collection["dateStart"];
                    string dateEnd = collection["dateEnd"];
                    string good = collection["good"];
                    string description = collection["description"];
                    float carbon = float.Parse(collection["carbon"]);
                    float crudeOil = float.Parse(collection["crudeOil"]);
                    float wire = float.Parse(collection["wire"]);
                    int totalCarbon = Convert.ToInt32((carbon / 100) * weight);
                    int totalCrudeOil = Convert.ToInt32((crudeOil / 100) * weight);
                    int totalWire = Convert.ToInt32((wire / 100) * weight);
                    string carbonGood = "Carbon";
                    string crudeOilGood = "Tyre Oil";
                    string wireGood = "Tyre Wire";
                    Good goodCarbon = db.Goods.Where(x => x.good_Name == carbonGood & x.is_active == "Y").First();
                    Good goodCrudeOil = db.Goods.Where(x => x.good_Name == crudeOilGood & x.is_active == "Y").First();
                    Good goodWire = db.Goods.Where(x => x.good_Name == wireGood & x.is_active == "Y").First();
                    TransactionAccount carbonTransAcc = db.TransactionAccounts.Find(goodCarbon.transaction_account);
                    TransactionAccount crudeOilTransAcc = db.TransactionAccounts.Find(goodCrudeOil.transaction_account);
                    TransactionAccount wireTransAcc = db.TransactionAccounts.Find(goodWire.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == accountName).First();
                    double rate = 0;
                    // Finding Rate //
                    try
                    {
                        rate = Convert.ToInt32(collection["carbonRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Protien !');</script>");
                    }

                    int amount = Convert.ToInt32(rate * totalCarbon);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport trTo = new TemporaryReport();
                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.updated_at = tr.created_at;
                    tr.description = description;
                    tr.net_weight = totalCarbon;
                    tr.is_active = "Y";
                    tr.good_ = carbonGood;
                    tr.from_account = VendorAcc.name;
                    tr.to_account = carbonTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amount;
                    //carbonTransAcc.balance = VendorAcc.balance + amount;
                    tr.dr = amount;
                    tr.cr = 0;
                    tr.voucher_code = collection["code"];
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = totalCarbon;
                    ///////

                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = description;
                    trTo.net_weight = totalCarbon;
                    trTo.is_active = "Y";
                    trTo.good_ = carbonGood;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = carbonTransAcc.name;
                    trTo.cr = amount;
                    trTo.dr = 0;
                    trTo.voucher_code = collection["code"];
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = carbonTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = totalCarbon;
                    if (Session["name"] !="Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
                        tr.user = username;
                    }
                    else
                    {
                        tr.user = Session["username"].ToString();
                        tr.admin_id = 1;
                        trTo.admin_id = 1;
                    }
                    tr.pvr_type = 1;
                    trTo.pvr_type = 1;
                    tr.item = good;
                    trTo.item = good;
                    trTo.user = tr.user;
                    tr.amount = amount;
                    trTo.amount = amount;
                    db.TemporaryReports.Add(tr);
                    db.TemporaryReports.Add(trTo);
                    db.SaveChanges();



                    ///////////////////////////////////


                    double rateOil = 0;
                    // Finding Rate //
                    try
                    {
                        rateOil = Convert.ToInt32(collection["crudeOilRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Fat/Chicken Oil !');</script>");
                    }

                    int amountFat = Convert.ToInt32(rateOil * totalCrudeOil);
                    TemporaryReport trans = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();
                    trans.created_at = Convert.ToDateTime(collection["date"]);
                    trans.updated_at = trans.created_at;
                    trans.description = description;
                    trans.net_weight = totalCrudeOil;
                    trans.is_active = "Y";
                    trans.good_ = crudeOilGood;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = crudeOilTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amountFat;
                    //crudeOilTransAcc.balance = VendorAcc.balance + amountFat;
                    trans.dr = amountFat;
                    trans.cr = 0;
                    trans.voucher_code = collection["code"];
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = totalCrudeOil;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = description;
                    transTo.net_weight = totalCrudeOil;
                    transTo.is_active = "Y";
                    transTo.good_ = crudeOilGood;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = crudeOilTransAcc.name;
                    transTo.cr = amountFat;
                    transTo.dr = 0;
                    transTo.voucher_code = collection["code"];
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = crudeOilTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = totalCrudeOil;

                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                        trans.user = username;
                    }
                    else
                    {
                        trans.user = Session["username"].ToString();
                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                    }
                    trans.pvr_type = 4;
                    transTo.pvr_type = 4;
                    trans.item = good;
                    transTo.item = good;
                    transTo.user = trans.user;
                    trans.amount = amount;
                    transTo.amount = amount;
                    db.TemporaryReports.Add(trans);
                    db.TemporaryReports.Add(transTo);
                    db.SaveChanges();


                    ////////////////////////////////

                    double rateWire = 0;
                    // Finding Rate //
                    try
                    {
                        rateWire = Convert.ToInt32(collection["wireRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Fat/Chicken Oil !');</script>");
                    }

                    int amountWire = Convert.ToInt32(rateWire * totalWire);
                    TemporaryReport transWire = new TemporaryReport();
                    TemporaryReport transWireTo = new TemporaryReport();
                    transWire.created_at = Convert.ToDateTime(collection["date"]);
                    transWire.updated_at = transWire.created_at;
                    transWire.description = description;
                    transWire.net_weight = totalWire;
                    transWire.is_active = "Y";
                    transWire.good_ = wireGood;
                    transWire.from_account = VendorAcc.name;
                    transWire.to_account = wireTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amountWire;
                    //wireTransAcc.balance = VendorAcc.balance + amountWire;
                    transWire.dr = amountWire;
                    transWire.cr = 0;
                    transWire.voucher_code = collection["code"];
                    transWire.voucher_type = "PRV";
                    transWire.status = "Production Voucher is Added";
                    transWire.trans_acc_id = VendorAcc.id;
                    transWire.code = VendorAcc.id;
                    transWire.net_weight = totalWire;

                    ///////

                    transWireTo.created_at = transWire.created_at;
                    transWireTo.updated_at = transWire.updated_at;
                    transWireTo.description = description;
                    transWireTo.net_weight = totalWire;
                    transWireTo.is_active = "Y";
                    transWireTo.good_ = wireGood;
                    transWireTo.from_account = VendorAcc.name;
                    transWireTo.to_account = wireTransAcc.name;
                    transWireTo.cr = amountWire;
                    transWireTo.dr = 0;
                    transWireTo.voucher_code = collection["code"];
                    transWireTo.voucher_type = "PRV";
                    transWireTo.status = "Production Voucher is Added";
                    transWireTo.trans_acc_id = wireTransAcc.id;
                    transWireTo.code = VendorAcc.id;
                    transWireTo.net_weight = totalWire;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        transWire.user_id = user.Id;
                        transWire.user_id = user.Id;
                        transWire.user = username;
                    }
                    else
                    {
                        transWire.user = Session["username"].ToString();
                        transWire.admin_id = 1;
                        transWireTo.admin_id = 1;
                    }
                    transWire.pvr_type = 4;
                    transWireTo.pvr_type = 4;
                    transWire.item = good;
                    transWireTo.item = good;
                    transWireTo.user = transWire.user;
                    transWire.amount = amount;
                    transWireTo.amount = amount;
                    db.TemporaryReports.Add(transWire);
                    db.TemporaryReports.Add(transWireTo);




             

                    db.SaveChanges();


                    return RedirectToAction("ListOfGoods");


                }
                else
                {
                    int ffa  = 0;

                    try
                    {
                        ffa = Convert.ToInt32(collection["ffa"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid FFA Value!');</script>");
                    }
                   
                    int weight = Convert.ToInt32(collection["weight"]);
                    string accountName = collection["vendor"];
                    string dateSart = collection["dateStart"];
                    string dateEnd = collection["dateEnd"];
                    string good = collection["good"];
                    string description = collection["description"];
                    float bleachOil = float.Parse(collection["bleachOil"]);
                    float fatAcid = float.Parse(collection["fatAcid"]);
                    int totalBleachOil= Convert.ToInt32((bleachOil / 100) * weight);
                    int totalFatAcid = Convert.ToInt32((fatAcid / 100) * weight);
                    string bleachOilGood = "Bleach Oil";
                    string fatAcidGood = "Fat Acid";
                    Good goodBleachOil = db.Goods.Where(x => x.good_Name == bleachOilGood & x.is_active == "Y").First();
                    Good goodFatAcid = db.Goods.Where(x => x.good_Name == fatAcidGood & x.is_active == "Y").First();
                    TransactionAccount bleachOilTransAcc = db.TransactionAccounts.Find(goodBleachOil.transaction_account);
                    TransactionAccount fatAcidTransAcc = db.TransactionAccounts.Find(goodFatAcid.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == accountName).First();
                    double rate = 0;
                    // Finding Rate //
                    try
                    {
                        rate = Convert.ToInt32(collection["bleachOilRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Protien !');</script>");
                    }

                    int amount = Convert.ToInt32(rate * totalBleachOil);
                    TemporaryReport tr = new TemporaryReport();
                    TemporaryReport trTo = new TemporaryReport();
                    tr.created_at = Convert.ToDateTime(collection["date"]);
                    tr.updated_at = tr.created_at;
                    tr.description = description;
                    tr.net_weight = totalBleachOil;
                    tr.is_active = "Y";
                    tr.good_ = bleachOilGood;
                    tr.from_account = VendorAcc.name;
                    tr.to_account = bleachOilTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amount;
                    //bleachOilTransAcc.balance = VendorAcc.balance + amount;
                    tr.dr = amount;
                    tr.cr = 0;
                    tr.voucher_code = collection["code"];
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = totalBleachOil;
                    ///////

                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = description;
                    trTo.net_weight = totalBleachOil;
                    trTo.is_active = "Y";
                    trTo.good_ = bleachOilGood;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = bleachOilTransAcc.name;
                    trTo.cr = amount;
                    trTo.dr = 0;
                    trTo.voucher_code = collection["code"];
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = bleachOilTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = totalBleachOil;
                    tr.extra =Convert.ToString (ffa);
                    trTo.extra = tr.extra;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
                        tr.user = username;
                    }
                    else
                    {
                        tr.user = Session["username"].ToString();
                        tr.admin_id = 1;
                        trTo.admin_id = 1;
                    }
                    tr.pvr_type = 1;
                    trTo.pvr_type = 1;
                    tr.item = good;
                    trTo.item = good;
                    trTo.user = tr.user;
                    tr.amount = amount;
                    trTo.amount = amount;
                    db.TemporaryReports.Add(tr);
                    db.TemporaryReports.Add(trTo);
                    db.SaveChanges();



                    ///////////////////////////////////
                    double rateOil = 0;
                    // Finding Rate //
                    try
                    {
                        rateOil = Convert.ToInt32(collection["fatAcidRate"]);
                    }
                    catch (Exception)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Invalid Rate of Fat/Chicken Oil !');</script>");
                    }

                    int amountFat = Convert.ToInt32(rateOil * totalFatAcid);
                    TemporaryReport trans = new TemporaryReport();
                    TemporaryReport transTo = new TemporaryReport();
                    trans.created_at = Convert.ToDateTime(collection["date"]);
                    trans.updated_at = trans.created_at;
                    trans.description = description;
                    trans.net_weight = totalFatAcid;
                    trans.is_active = "Y";
                    trans.good_ = fatAcidGood;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = fatAcidTransAcc.name;
                    //VendorAcc.balance = VendorAcc.balance - amountFat;
                    //fatAcidTransAcc.balance = VendorAcc.balance + amountFat;
                    trans.dr = amountFat;
                    trans.cr = 0;
                    trans.voucher_code = collection["code"];
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = totalFatAcid;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = description;
                    transTo.net_weight = totalFatAcid;
                    transTo.is_active = "Y";
                    transTo.good_ = fatAcidGood;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = fatAcidTransAcc.name;
                    transTo.cr = amountFat;
                    transTo.dr = 0;
                    transTo.voucher_code = collection["code"];
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = fatAcidTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = totalFatAcid;
                    trans.extra = tr.extra;
                    transTo.extra = tr.extra;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                        trans.user = username;
                    }
                    else
                    {
                        trans.user = Session["username"].ToString();
                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                    }
                    trans.pvr_type = 4;
                    transTo.pvr_type = 4;
                    trans.item = good;
                    transTo.item = good;
                    transTo.user = trans.user;
                    transTo.amount = amount;
                    trans.amount = amount;
                    db.TemporaryReports.Add(trans);
                    db.TemporaryReports.Add(transTo);



                    db.SaveChanges();

                    return RedirectToAction("ListOfGoods");


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
            return RedirectToAction("ListOfGoods");
        }

        public ActionResult history()
        {
            int allReports = Convert.ToInt32(Session["all_reports"]);

            if (allReports == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(serviceTransaction.allTransactionaccounts());
        }
        [HttpPost]
        public ActionResult historyPost(string search, string dateStart, string dateEnd)
        {
            if (Session["name"] != "Admin")
            {


                if (PermissionForAccounts.permisionCheckForProductions(PermissionForAccounts.permisionProductions, search) == false)
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
            List<TransactionAccount> accounts = serviceTransaction.findTransactionAccounts(search);
            var balance = serviceTransaction.findAccountBalance(search);
            ViewBag.openingBalance = balance;
            List<TransactionAccount> allaccounts = serviceTransaction.allTransactionaccounts();
            ViewBag.start = Convert.ToDateTime(dateStart);
            ViewBag.end = Convert.ToDateTime(dateEnd);

            ViewBag.MyList = allaccounts;
            return View(accounts);
        }
        public ActionResult Production(int id)
        {
            try
            {
                TransactionServices service2 = new TransactionServices();

                if (service2.checkIDTrans() == false)
                {
                    return Content("IDS Conflict Error Occured While Procsesing Request");
                }



                Database1Entities1 db = new Database1Entities1();
                TemporaryReport temp = db.TemporaryReports.Find(id);
                TemporaryReport tempTo = db.TemporaryReports.Find(id + 1);
                TemporaryReport temp2nd = db.TemporaryReports.Find(id + 2);
                TemporaryReport temp2ndTo = db.TemporaryReports.Find(id + 3);

                if (temp.from_account == "Chicken Waste Purchase Main  Account")
                {

                    string protienGood = "Protein";
                    string fatGood = "Crude Oil (Fat)";
                    Good goodProtien = db.Goods.Where(x => x.good_Name == protienGood & x.is_active== "Y").First();
                    Good goodFatOil = db.Goods.Where(x => x.good_Name == fatGood & x.is_active == "Y").First();
                    TransactionAccount protienTransAcc = db.TransactionAccounts.Find(goodProtien.transaction_account);
                    TransactionAccount fatOilTransAcc = db.TransactionAccounts.Find(goodFatOil.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == temp.from_account).First();
                    double rate = 0;
                    // Finding Rate //
                 
                    Transaction tr = new Transaction();
                    Transaction trTo = new Transaction();
                    tr.created_at = temp.created_at;
                    tr.updated_at = tr.created_at;
                    tr.description = temp.description;
                    tr.net_weight = temp.net_weight ;
                    tr.is_active = "Y";
                    tr.good_ = temp.good_ ;
                    tr.from_account = temp.from_account;
                    tr.to_account = temp.to_account;
                    VendorAcc.balance = VendorAcc.balance - temp.dr;
                    protienTransAcc.balance = protienTransAcc.balance + temp.dr;
                    tr.dr =(int)(temp.dr);
                    tr.cr = 0;
                    tr.voucher_code = temp.voucher_code;
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = temp.net_weight;
                    ///////


                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = tempTo.description;
                    trTo.net_weight = tempTo.net_weight;
                    trTo.is_active = "Y";
                    trTo.good_ = tempTo.good_;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = protienTransAcc.name;
                    trTo.cr =(int)(tempTo.cr);
                    trTo.dr = 0;
                    trTo.voucher_code = tempTo.voucher_code;
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = protienTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = tempTo.net_weight;

                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);

                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
               
                    }
                    else
                    {

                        tr.admin_id = 1;
                        trTo.admin_id = 1;
         
                    }


                    tr.Last_Edit = "None";
                    trTo.Last_Edit = "None";
                    db.Transactions.Add(tr);
                    db.Transactions.Add(trTo);
                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(VendorAcc.id,protienTransAcc.id);

                    ///////////////////////////////////
                   

                  
                    Transaction trans = new Transaction();
                    Transaction transTo = new Transaction();
                    trans.created_at = temp.created_at;
                    trans.updated_at = trans.created_at;
                    trans.description = temp2nd.description;
                    trans.net_weight = temp2nd.net_weight;
                    trans.is_active = "Y";
                    trans.good_ = temp2nd.good_;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = fatOilTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp2nd.dr;
                    fatOilTransAcc.balance = fatOilTransAcc.balance + temp2nd.dr;
                    trans.dr =(int)temp2nd.dr;
                    trans.cr = 0;
                    trans.voucher_code = temp2nd.voucher_code;
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = temp2nd.net_weight;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = temp2ndTo.description;
                    transTo.net_weight = temp2ndTo.net_weight;
                    transTo.is_active = "Y";
                    transTo.good_ = temp2ndTo.good_;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = fatOilTransAcc.name;
                    transTo.cr = (int)temp2ndTo.cr;
                    transTo.dr = 0;
                    transTo.voucher_code = temp2ndTo.voucher_code;
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = fatOilTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = temp2ndTo.net_weight;

                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);

                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                      
                    }
                    else
                    {

                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                      
                    }

                    trans.Last_Edit = "None";
                    transTo.Last_Edit = "None";

                    db.Transactions.Add(trans);
                    db.Transactions.Add(transTo);
                    temp.is_active = "N";
                    temp2nd.is_active = "N";
                    temp2ndTo.is_active = "N";
                    tempTo.is_active = "N";
                    fatOilTransAcc.updated_at_ = trans.updated_at;
                    protienTransAcc.updated_at_ = trans.updated_at;
                    VendorAcc.updated_at_ = trans.updated_at;


                    /////////////////// Weight Management  ////////////////

                    int weightFat = Convert.ToInt32(fatOilTransAcc.WEIGHT);
                    weightFat =(int)(weightFat + trTo.net_weight);
                    fatOilTransAcc.WEIGHT = weightFat.ToString();

                    int weightProtien = Convert.ToInt32(protienTransAcc.WEIGHT);
                    weightProtien =(int)(weightProtien + transTo.net_weight);
                    protienTransAcc.WEIGHT = weightProtien.ToString();





                    ///////////////////////////////////////////////////////














                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(VendorAcc.id, fatOilTransAcc.id);

                    return RedirectToAction("ViewTempReports", "TemporaryReports");
                }
                else if (temp.from_account == "Tyre Waste Main  Account")
                {
                  
                  
                    string carbonGood = "Carbon";
                    string crudeOilGood = "Tyre Oil";
                    string wireGood = "Tyre Wire";
                    Good goodCarbon = db.Goods.Where(x => x.good_Name == carbonGood & x.is_active =="Y").First();
                    Good goodCrudeOil = db.Goods.Where(x => x.good_Name == crudeOilGood & x.is_active =="Y").First();
                    Good goodWire = db.Goods.Where(x => x.good_Name == wireGood & x.is_active =="Y").First();
                    TransactionAccount carbonTransAcc = db.TransactionAccounts.Find(goodCarbon.transaction_account);
                    TransactionAccount crudeOilTransAcc = db.TransactionAccounts.Find(goodCrudeOil.transaction_account);
                    TransactionAccount wireTransAcc = db.TransactionAccounts.Find(goodWire.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == temp.from_account).First();

                    Transaction tr = new Transaction();
                    Transaction trTo = new Transaction();
                    tr.created_at = temp.created_at;
                    tr.updated_at = tr.created_at;
                    tr.description = temp.description;
                    tr.net_weight = temp.net_weight;
                    tr.is_active = "Y";
                    tr.good_ = temp.good_;
                    tr.from_account = VendorAcc.name;
                    tr.to_account = carbonTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp.dr;
                    carbonTransAcc.balance = carbonTransAcc.balance + temp.dr;
                    tr.dr =(int)temp.dr;
                    tr.cr = 0;
                    tr.voucher_code = temp.voucher_code;
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = temp.net_weight;
                    ///////

                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = tempTo.description;
                    trTo.net_weight = tempTo.net_weight;
                    trTo.is_active = "Y";
                    trTo.good_ = tempTo.good_;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = carbonTransAcc.name;
                    trTo.cr = (int)tempTo.cr;
                    trTo.dr = 0;
                    trTo.voucher_code = tempTo.voucher_code;
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = carbonTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = tempTo.net_weight;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
             
                    }
                    else
                    {
                       
                        tr.admin_id = 1;
                        trTo.admin_id = 1;
                    }

                    tr.Last_Edit = "None";
                    trTo.Last_Edit = "None";

                    db.Transactions.Add(tr);
                    db.Transactions.Add(trTo);
                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(carbonTransAcc.id, VendorAcc.id);

                    ///////////////////////////////////



                    Transaction trans = new Transaction();
                    Transaction transTo = new Transaction();
                    trans.created_at = temp.created_at;
                    trans.updated_at = trans.created_at;
                    trans.description = temp2nd.description;
                    trans.net_weight = temp2nd.net_weight;
                    trans.is_active = "Y";
                    trans.good_ = crudeOilGood;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = crudeOilTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp2nd.dr;
                    crudeOilTransAcc.balance = crudeOilTransAcc.balance + temp2nd.dr;
                    trans.dr = (int)temp2nd.dr;
                    trans.cr = 0;
                    trans.voucher_code = temp2nd.voucher_code;
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = temp2nd.net_weight;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = temp2ndTo.description;
                    transTo.net_weight = temp2ndTo.net_weight;
                    transTo.is_active = "Y";
                    transTo.good_ = crudeOilGood;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = crudeOilTransAcc.name;
                    transTo.cr =(int)temp2ndTo.cr;
                    transTo.dr = 0;
                    transTo.voucher_code = temp2ndTo.voucher_code;
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = crudeOilTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = temp2ndTo.net_weight;

                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                        
                    }
                    else
                    {
                   
                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                    }

                    trans.Last_Edit = "None";
                    transTo.Last_Edit = "None";

                    
                    db.Transactions.Add(trans);
                    db.Transactions.Add(transTo);
                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(crudeOilTransAcc.id, VendorAcc.id);

                    ////////////////////////////////

                    TemporaryReport temp3rd = db.TemporaryReports.Find(id + 4);
                    TemporaryReport temp3rdTo = db.TemporaryReports.Find(id + 5);


                    Transaction transWire = new Transaction();
                    Transaction transWireTo = new Transaction();
                    transWire.created_at = temp.created_at;
                    transWire.updated_at = transWire.created_at;
                    transWire.description = temp3rd.description;
                    transWire.net_weight = temp3rd.net_weight;
                    transWire.is_active = "Y";
                    transWire.good_ = wireGood;
                    transWire.from_account = VendorAcc.name;
                    transWire.to_account = wireTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp3rd.dr;
                    wireTransAcc.balance = wireTransAcc.balance + temp3rd.dr;
                    transWire.dr = (int)temp3rd.dr;
                    transWire.cr = 0;
                    transWire.voucher_code = temp3rd.voucher_code;
                    transWire.voucher_type = "PRV";
                    transWire.status = "Production Voucher is Added";
                    transWire.trans_acc_id = VendorAcc.id;
                    transWire.code = VendorAcc.id;
                    transWire.net_weight = temp3rd.net_weight;

                    ///////

                    transWireTo.created_at = transWire.created_at;
                    transWireTo.updated_at = transWire.updated_at;
                    transWireTo.description = temp3rdTo.description;
                    transWireTo.net_weight = temp3rdTo.net_weight;
                    transWireTo.is_active = "Y";
                    transWireTo.good_ = wireGood;
                    transWireTo.from_account = VendorAcc.name;
                    transWireTo.to_account = wireTransAcc.name;
                    transWireTo.cr = (int)temp3rdTo.cr;
                    transWireTo.dr = 0;
                    transWireTo.voucher_code =temp3rdTo.voucher_code;
                    transWireTo.voucher_type = "PRV";
                    transWireTo.status = "Production Voucher is Added";
                    transWireTo.trans_acc_id = wireTransAcc.id;
                    transWireTo.code = VendorAcc.id;
                    transWireTo.net_weight = temp3rdTo.net_weight;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        transWire.user_id = user.Id;
                        transWire.user_id = user.Id;
                     
                    }
                    else
                    {
                      
                        transWire.admin_id = 1;
                        transWireTo.admin_id = 1;
                    }

                    transWire.Last_Edit = "None";
                    transWireTo.Last_Edit = "None";


                    db.Transactions.Add(transWire);
                    db.Transactions.Add(transWireTo);
                    temp.is_active = "N";
                    temp2nd.is_active = "N";
                    temp2ndTo.is_active = "N";
                    tempTo.is_active = "N";
                    temp3rd.is_active = "N";
                    temp3rdTo.is_active = "N";
                    carbonTransAcc.updated_at_ = trans.updated_at;
                    crudeOilTransAcc.updated_at_ = trans.updated_at;
                    wireTransAcc.updated_at_ = trans.updated_at;
                    VendorAcc.updated_at_ = trans.updated_at;



                    /////////////////// Weight Management  ////////////////

                    int weightWire = Convert.ToInt32(wireTransAcc.WEIGHT);
                    weightWire =(int)(weightWire + transWireTo.net_weight);
                    wireTransAcc.WEIGHT = weightWire.ToString();

                    int weightCarbon = Convert.ToInt32(carbonTransAcc.WEIGHT);
                    weightCarbon =(int)(weightCarbon + trTo.net_weight);
                    carbonTransAcc.WEIGHT = weightCarbon.ToString();

                    int weightCrude = Convert.ToInt32(crudeOilTransAcc.WEIGHT);
                    weightCrude =(int)(weightCrude + transTo.net_weight);
                    crudeOilTransAcc.WEIGHT = weightCrude.ToString();


                    ///////////////////////////////////////////////////////







                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(wireTransAcc.id, VendorAcc.id);

                    return RedirectToAction("Supervision", "USER_CRUD");


                }
                else
                {
                 
                    string bleachOilGood = "Bleach Oil";
                    string fatAcidGood = "Fat Acid";
                    Good goodBleachOil = db.Goods.Where(x => x.good_Name == bleachOilGood & x.is_active =="Y").First();
                    Good goodFatAcid = db.Goods.Where(x => x.good_Name == fatAcidGood & x.is_active =="Y").First();
                    TransactionAccount bleachOilTransAcc = db.TransactionAccounts.Find(goodBleachOil.transaction_account);
                    TransactionAccount fatAcidTransAcc = db.TransactionAccounts.Find(goodFatAcid.transaction_account);
                    TransactionAccount VendorAcc = db.TransactionAccounts.Where(x => x.name == temp.from_account).First();



                    Transaction tr = new Transaction();
                    Transaction trTo = new Transaction();
                    tr.created_at = temp.created_at;
                    tr.updated_at = tr.created_at;
                    tr.description = temp.description;
                    tr.net_weight = temp.net_weight;
                    tr.is_active = "Y";
                    tr.good_ = temp.good_;
                    tr.from_account = VendorAcc.name;
                    tr.to_account = bleachOilTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp.dr;
                    bleachOilTransAcc.balance = bleachOilTransAcc.balance + temp.dr;
                    tr.dr =(int)temp.dr;
                    tr.cr = 0;
                    tr.voucher_code = temp.voucher_code;
                    tr.voucher_type = "PRV";
                    tr.status = "Production Voucher is Added";
                    tr.trans_acc_id = VendorAcc.id;
                    tr.code = VendorAcc.id;
                    tr.net_weight = temp.net_weight;
                    ///////

                    trTo.created_at = tr.created_at;
                    trTo.updated_at = tr.updated_at;
                    trTo.description = tempTo.description;
                    trTo.net_weight = tempTo.net_weight;
                    trTo.is_active = "Y";
                    trTo.good_ = tempTo.good_;
                    trTo.from_account = VendorAcc.name;
                    trTo.to_account = bleachOilTransAcc.name;
                    trTo.cr = (int)tempTo.cr;
                    trTo.dr = 0;
                    trTo.voucher_code = tempTo.voucher_code;
                    trTo.voucher_type = "PRV";
                    trTo.status = "Production Voucher is Added";
                    trTo.trans_acc_id = bleachOilTransAcc.id;
                    trTo.code = VendorAcc.id;
                    trTo.net_weight = tempTo.net_weight;
                    tr.extra = temp.extra;
                    trTo.extra = tempTo.extra;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        tr.user_id = user.Id;
                        trTo.user_id = user.Id;
                      
                    }
                    else
                    {
                       
                        tr.admin_id = 1;
                        trTo.admin_id = 1;
                    }


                    tr.Last_Edit = "None";
                    trTo.Last_Edit = "None";

                    db.Transactions.Add(tr);
                    db.Transactions.Add(trTo);
                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(bleachOilTransAcc.id, VendorAcc.id);

                    ///////////////////////////////////



                    Transaction trans = new Transaction();
                    Transaction transTo = new Transaction();
                    trans.created_at = temp.created_at;
                    trans.updated_at = trans.created_at;
                    trans.description = temp2nd.description;
                    trans.net_weight = temp2nd.net_weight;
                    trans.is_active = "Y";
                    trans.good_ = temp2nd.good_;
                    trans.from_account = VendorAcc.name;
                    trans.to_account = fatAcidTransAcc.name;
                    VendorAcc.balance = VendorAcc.balance - temp2nd.dr;
                    fatAcidTransAcc.balance = fatAcidTransAcc.balance + temp2nd.dr;
                    trans.dr =(int)temp2nd.dr;
                    trans.cr = 0;
                    trans.voucher_code = temp2nd.voucher_code;
                    trans.voucher_type = "PRV";
                    trans.status = "Production Voucher is Added";
                    trans.trans_acc_id = VendorAcc.id;
                    trans.code = VendorAcc.id;
                    trans.net_weight = temp2nd.net_weight;

                    ///////

                    transTo.created_at = trans.created_at;
                    transTo.updated_at = trans.updated_at;
                    transTo.description = temp2ndTo.description;
                    transTo.net_weight = temp2ndTo.net_weight;
                    transTo.is_active = "Y";
                    transTo.good_ = temp2ndTo.good_;
                    transTo.from_account = VendorAcc.name;
                    transTo.to_account = fatAcidTransAcc.name;
                    transTo.cr =(int)temp2ndTo.cr;
                    transTo.dr = 0;
                    transTo.voucher_code = temp2ndTo.voucher_code;
                    transTo.voucher_type = "PRV";
                    transTo.status = "Production Voucher is Added";
                    transTo.trans_acc_id = fatAcidTransAcc.id;
                    transTo.code = VendorAcc.id;
                    transTo.net_weight = temp2ndTo.net_weight;
                    trans.extra = temp2nd.extra;
                    transTo.extra = temp2ndTo.extra;
                    if (Session["name"] != "Admin")
                    {
                        TransactionServices services = new TransactionServices();
                        string username = Session["username"].ToString();
                        User user = services.findUser(username);
                        trans.user_id = user.Id;
                        transTo.user_id = user.Id;
                     
                    }
                    else
                    {
                      
                        trans.admin_id = 1;
                        transTo.admin_id = 1;
                    }

                    trans.Last_Edit = "None";
                    transTo.Last_Edit = "None";

                    db.Transactions.Add(trans);
                    db.Transactions.Add(transTo);
                    temp.is_active = "N";
                    tempTo.is_active = "N";
                    temp2nd.is_active = "N";
                    temp2ndTo.is_active = "N";
                    bleachOilTransAcc.updated_at_ = trans.updated_at;
                    fatAcidTransAcc.updated_at_ = trans.updated_at;
                    VendorAcc.updated_at_ = trans.updated_at;


                    int weightWire = Convert.ToInt32(bleachOilTransAcc.WEIGHT);
                    weightWire = (int)(weightWire + trTo.net_weight);
                    bleachOilTransAcc.WEIGHT = weightWire.ToString();

                    int weightCarbon = Convert.ToInt32(fatAcidTransAcc.WEIGHT);
                    weightCarbon = (int)(weightCarbon + transTo.net_weight);
                    fatAcidTransAcc.WEIGHT = weightCarbon.ToString();




                    db.SaveChanges();

                    serviceTransaction.balanceUpdation(fatAcidTransAcc.id, VendorAcc.id);

                    return RedirectToAction("Supervision", "USER_CRUD");


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
            return RedirectToAction("Supervision", "USER_CRUD");
        }

        public ActionResult viewUnitRates()
        {
            Database1Entities1 db = new Database1Entities1();
            return View(db.UnitRates.Where(x=>x.is_active =="Y").ToList());
        }
        public ActionResult EditUnitRate(int id)
        {
            Database1Entities1 db = new Database1Entities1();
            UnitRate unit = db.UnitRates.Find(id);
            return View(unit);
        }

        [HttpPost]
        public ActionResult  EditUnitRate(int id, FormCollection collection)
        {
            Database1Entities1 db = new Database1Entities1();
            UnitRate unit = db.UnitRates.Find(id);
            unit.rate = float.Parse(collection["rate"]);
            unit.updated_at = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("viewUnitRates");
        }
        [HttpGet]
        public ActionResult DeleteUnitRate(int id)
        {
            UnitRate unitRate = service.findUnitRateByID(id);
            unitRate.is_active = "N";
            service.save();
            return RedirectToAction("viewUnitRates");
        }
    }
}
