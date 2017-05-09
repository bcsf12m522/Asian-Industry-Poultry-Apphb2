using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Models;
namespace ranglerz_project.Services
{
    public class Temporary_Order_Services
    {

        Database1Entities1 db = new Database1Entities1();

        public List<Temporary_Orders> listOfTemporaryOrders()
        {
           return db.Temporary_Orders.Where(x=>x.is_active =="Y").ToList();
        }
        public Temporary_Orders findTempOrderById(int id)
        {
            return db.Temporary_Orders.Find(id);
        }
        public List<Temporary_Orders> findTempOrderByAccountId(int id)
        {
            return db.Temporary_Orders.Where(x => x.Account_Id == id && x.is_active=="Y").ToList();
        }
        public void addTempOrder (Temporary_Orders obj)
        {
            db.Temporary_Orders.Add(obj);
        }

    }
}