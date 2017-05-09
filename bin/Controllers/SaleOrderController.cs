using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Routing;
namespace ranglerz_project.Controllers
{


    public class SaleOrderController : Controller
    {

        TransactionServices transactionService = new TransactionServices();
        Sale_Order_Services service = new Sale_Order_Services();
        Temporary_Order_Services tempServices = new Temporary_Order_Services();
        Database1Entities1 db = new Database1Entities1();

        #region SALE ORDERS 

        public ActionResult Index()
        {

            List<Sale_Order> list = service.listofSO();
            ViewBag.Accounts = db.TransactionAccounts.Where(x => x.is_active == "Y").ToList();
            return View(list);
        }



        public ActionResult CreateSaleOrder()
        {
            int lastId = service.lastSoOrder();
            ViewBag.Code = "SO#0" + lastId;
            List<TransactionAccount> listOfAccounts = new List<TransactionAccount>();
            listOfAccounts = transactionService.allTransactionaccounts();
            return View(listOfAccounts);
        }



        public ActionResult Create(int id)
        {
            try
            {
                Temporary_Orders temp = tempServices.findTempOrderById(id);
                int account_id = temp.Account_Id;
                TransactionAccount account = transactionService.findTransactionAccount(account_id);
                Sale_Order s_order = new Sale_Order();
              
                s_order.bags = temp.bags;
                s_order.broker_name = temp.broker_name;
                s_order.brokers_commision = temp.brokers_commision;
             
                s_order.company_sign = temp.company_sign;
                s_order.created_at_ = temp.created_at_;
                s_order.is_active = temp.is_active;
                s_order.vendor_sign = temp.vendor_sign;
                s_order.item = temp.item;
                s_order.party_name = account.name;
              
                s_order.so_number = temp.order_number;
                s_order.terms = temp.terms;
                s_order.uptdated_at = temp.created_at_;
                s_order.weight = temp.weight;
                s_order.rate = temp.rate;
                s_order.prepared_by = temp.prepared_by;
                s_order.approved_by = Session["username"].ToString();
                s_order.checked_by = temp.checked_by;
                s_order.Account_Id = account.id;
                service.addSO(s_order);
                service.save();

                Temporary_Orders TO = db.Temporary_Orders.Find(id);
                TO.is_active = "N";
                db.SaveChanges();
                return RedirectToAction("listOfTempOrders");
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

                return RedirectToAction("listOfTempOrders");
            }
        }


        

        public ActionResult sale_order_by(int id)
        {
            List<Sale_Order> so = service.listofOrderss(id);
            return PartialView("UpdatedSaleOrderList", so);
        }

        public ActionResult deleteSaleOrder(int id)
        {
            Sale_Order so = db.Sale_Order.Find(id);
            so.is_active = "N";
            db.SaveChanges();
            List<Sale_Order> list = db.Sale_Order.Where(x=>x.is_active=="Y").ToList();
            return PartialView("UpdatedSaleOrderList",list);
        }

        public bool checkSaleOrder(string good , int account)
        {
            int so = db.Sale_Order.Where(x => x.item == good && x.Account_Id == account && x.is_active == "Y").ToList().Count;
            if(so >0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region TEMPORARY ORDERS

        public ActionResult listOfTempOrders()
        {

            List<Temporary_Orders> list = tempServices.listOfTemporaryOrders();

            return View(list);
        }


        [HttpPost]
        public ActionResult CreateTempOrder(FormCollection collection)
        {
            try
            {
                int account_id = Convert.ToInt32(collection["party_name"]);
                TransactionAccount account = transactionService.findTransactionAccount(account_id);
                Temporary_Orders s_order = new Temporary_Orders();
                string actionType = collection["actionType"];
              
                s_order.bags = collection["bags"];
                s_order.broker_name = collection["broker_name"];
                s_order.brokers_commision = collection["commisions"];              
                s_order.company_sign = "N/A";
                s_order.created_at_ = Convert.ToDateTime(collection["date"]);
                s_order.is_active = "Y";
                s_order.vendor_sign = "N/A";
                s_order.item = collection["item"];
                s_order.party_name = account.name;               
                s_order.order_number = collection["code"];
                s_order.terms = collection["terms"];
                s_order.uptdated_at = s_order.created_at_;
                s_order.weight = Convert.ToInt32(collection["weight"]);
                s_order.rate = float.Parse(collection["rate"]);
                s_order.type = Convert.ToInt32(collection["type"]);
                s_order.approved_by = Session["username"].ToString();
                s_order.checked_by = Session["username"].ToString();
                s_order.prepared_by = Session["username"].ToString();
                s_order.Account_Id = account.id;
                db.Temporary_Orders.Add(s_order);
                db.SaveChanges();

                if (actionType == "Add and Print")
                {
                    return RedirectToAction("printOrder", new RouteValueDictionary(s_order));
                }

                else 
                {

                    if (s_order.type == 1)
                    {
                        return RedirectToAction("CreateSaleOrder");
                    }
                    else
                    {
                        return RedirectToAction("Create", "PurchaseOrder");
                    }
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

                return RedirectToAction("listOfTempOrders");
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Accounts = db.TransactionAccounts.Where(x => x.is_active=="Y").ToList();
            Temporary_Orders temp = tempServices.findTempOrderById(id);
            return View(temp);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            int account_id = Convert.ToInt32(collection["party_name"]);
            TransactionAccount account = transactionService.findTransactionAccount(account_id);
            Temporary_Orders s_order = db.Temporary_Orders.Find(id);
            DateTime date = s_order.uptdated_at;
            bool flag =DateTime.TryParse(collection["date"], out date);
            s_order.bags = collection["bags"];
            s_order.broker_name = collection["broker_name"];
            s_order.brokers_commision = collection["commisions"];
            s_order.company_sign = "N/A";
            if (flag != false)
            {
                s_order.uptdated_at = date;
            }
            s_order.is_active = "Y";
            s_order.vendor_sign = "N/A";
            s_order.item = collection["item"];
            s_order.party_name = account.name;

            s_order.terms = collection["terms"];
          
            s_order.weight = Convert.ToInt32(collection["weight"]);
            s_order.rate = float.Parse(collection["rate"]);
            s_order.type = Convert.ToInt32(collection["type"]);        
            s_order.checked_by = Session["username"].ToString();          
            s_order.Account_Id = account.id;
       
            db.SaveChanges();
            return RedirectToAction("listOfTempOrders");
        }

        public ActionResult deleteTempOrder(int id)
        {
            Temporary_Orders temp = db.Temporary_Orders.Find(id);
            temp.is_active = "N";
            db.SaveChanges();
            List<Temporary_Orders> list = db.Temporary_Orders.Where(x => x.is_active == "Y").ToList();
            return PartialView("UpdatedTempOrderList", list);
        }

        #endregion

        public ActionResult printOrder(Temporary_Orders order)
        {
            return View(order);
        }
    }
    
}
