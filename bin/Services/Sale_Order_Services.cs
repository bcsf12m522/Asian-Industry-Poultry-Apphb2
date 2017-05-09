using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Services;
using ranglerz_project.Models;
namespace ranglerz_project.Services
{
    public class Sale_Order_Services
    {
        Database1Entities1 db = new Database1Entities1();

        public int lastSoOrder ()
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

        public List<Sale_Order> listofSO()
        {
            return db.Sale_Order.Where(x => x.is_active =="Y").ToList();
        }
        public void addSO(Sale_Order obj)
        {
            db.Sale_Order.Add(obj);
        }
        public void save()
        {
            db.SaveChanges();
        }

        public  Sale_Order find(int id)
        {
            return db.Sale_Order.Find(id);
        }
        public List<Sale_Order> listofOrderss(int number)
        {
            return db.Sale_Order.Where(x => x.Account_Id == number && x.is_active=="Y").ToList();
        }
    }
}