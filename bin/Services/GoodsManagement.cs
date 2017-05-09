using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Models;

namespace ranglerz_project.Services
{
    public class GoodsManagement
    {
        Database1Entities1 db = new Database1Entities1();
        
        public void addGoodTypes( Good_Types goodType)
        {
            db.Good_Types.Add(goodType);
        }
       
        public void save()
        {
            db.SaveChanges();
        }
         public List<Good_Types> listGoodTypes()
         {
             return db.Good_Types.Where(x=>x.is_active=="Y").ToList();
         }
        public void addtransactionAccount(TransactionAccount tr)
         {
             db.TransactionAccounts.Add(tr);
         }
        public void addGoods(Good good)
        {
            db.Goods.Add(good);
        }
        public List<Good> listOfGoods()
        {
            return db.Goods.Where(x=>x.is_active=="Y").ToList();
        }
        public Good findGood( int id)
        {
            return db.Goods.Find(id);
        }
        public Good_Types findGoodType(int id)
        {
            return db.Good_Types.Find(id);
        }
        public UnitRate findUnitRateByID(int id)
        {
            return db.UnitRates.Find(id);
        }

    }
}