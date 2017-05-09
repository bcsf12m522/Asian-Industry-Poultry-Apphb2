using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Services;
using ranglerz_project.Models;

namespace ranglerz_project.Services
{
    public class Purchase_Order_Services
    {
        Database1Entities1 db = new Database1Entities1();
          public int lastPoOrder ()
        {
            try
            {
                int max = db.Temporary_Orders.Max(x => x.Id);
                return (max + 1);
            }
            catch
            {
                return 1;
            }
            
            
        }

        public List<Purchase_Order> listofPO()
        {
            return db.Purchase_Order.Where(x => x.is_active =="Y").ToList();
        }
        public void addPO(Purchase_Order obj)
        {
            db.Purchase_Order.Add(obj);
        }
        public void save()
        {
            db.SaveChanges();
        }
        public Purchase_Order find(int id)
        {
            return db.Purchase_Order.Find(id);
        }
        public List<Purchase_Order> listofPObyAccount(int number)
        {
            return db.Purchase_Order.Where(x => x.Account_Id == number && x.is_active=="Y").ToList();
        }
    }

 }
