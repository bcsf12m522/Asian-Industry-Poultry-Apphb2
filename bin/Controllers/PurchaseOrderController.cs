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
    public class PurchaseOrderController : Controller
    {
       
        TransactionServices transactionService = new TransactionServices();
        Purchase_Order_Services service = new Purchase_Order_Services();
        Temporary_Order_Services tempServices = new Temporary_Order_Services();
        Database1Entities1 db = new Database1Entities1();



        public ActionResult Index()
        {
            List<Purchase_Order> list = service.listofPO();
            ViewBag.Accounts = db.TransactionAccounts.Where(x => x.is_active == "Y").ToList();
            return View(list);

        }
           

        public ActionResult Create()
        {
            int lastId = service.lastPoOrder();
            ViewBag.Code = "PO#0" + lastId;
            List<TransactionAccount> listOfAccounts = new List<TransactionAccount>();
            listOfAccounts = transactionService.allTransactionaccounts();
            return View(listOfAccounts);
        }

 

        public ActionResult CreatePurchaseOrder(int id)
        {
            try
            {
                Temporary_Orders temp = tempServices.findTempOrderById(id);
                int account_id = temp.Account_Id;
                TransactionAccount account = transactionService.findTransactionAccount(account_id);
                Purchase_Order s_order = new Purchase_Order();
               
                s_order.bags = temp.bags;
                s_order.broker_name = temp.broker_name;
                s_order.brokers_commision = temp.brokers_commision;
          
                s_order.company_sign = temp.company_sign;
                s_order.created_at_ = temp.created_at_;
                s_order.is_active = temp.is_active;
                s_order.vendor_sign = temp.vendor_sign;
                s_order.item = temp.item;
                s_order.party_name = account.name;
               
                s_order.po_number = temp.order_number;
                s_order.terms = temp.terms;
                s_order.uptdated_at = temp.created_at_;
                s_order.weight = temp.weight;
                s_order.rate = temp.rate;
                s_order.prepared_by = temp.prepared_by;
                s_order.approved_by = Session["username"].ToString();
                s_order.checked_by = temp.checked_by;
                s_order.Account_Id = account.id;
                service.addPO(s_order);                              
                service.save();
                Temporary_Orders TO = db.Temporary_Orders.Find(id);
                TO.is_active = "N";
                db.SaveChanges();
                db.SaveChanges();
                return RedirectToAction("listOfTempOrders","SaleOrder");
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

                return RedirectToAction("listOfTempOrders","SaleOrder");
            }
        }

   

        public ActionResult sale_order_by(int id)
        {
            List<Purchase_Order> po = service.listofPObyAccount(id);
            return PartialView("UpdatedPurchaseOrderList", po);
        }



        public ActionResult deletePurchaseOrder(int id)
        {
            Purchase_Order so = db.Purchase_Order.Find(id);
            so.is_active = "N";
            db.SaveChanges();
            List<Purchase_Order> list = db.Purchase_Order.Where(x => x.is_active == "Y").ToList();
            return PartialView("UpdatedPurchaseOrderList", list);
        }

    


  
    }
}
