using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.ModelsBO;
using ranglerz_project.Models;
using System.Web.Mvc;


namespace ranglerz_project.Services
{
    public class AccountServices
    {
       static  Database1Entities1 db = new Database1Entities1();

        public bool checkName (string name)
       {
          List<TransactionAccount> list = db.TransactionAccounts.Where(x => x.name == name & x.is_active =="Y").ToList();
            if(list.Count >=1)
            {
                return true;
            }
            else
            {
                return false;
            }

       }

        public List<MainAccount> allAccounts()
        {

            return (db.MainAccounts.ToList());

        }
        public void add(MainAccount main)
        {

            db.MainAccounts.Add(main);
            db.SaveChanges();

        }
        public void addTransAcc(TransactionAccount trans)
        {

            db.TransactionAccounts.Add(trans);
            db.SaveChanges();

        }
        public void addHeadAcc(HeadAccount head)
        {

            db.HeadAccounts.Add(head);
            db.SaveChanges();
        
        }
        public void addSubHeadAcc(SubHeadAccount sub)
        {

            db.SubHeadAccounts.Add(sub);
            db.SaveChanges();

        }
        public MainAccount findMainAcc(int id)
        {
            return db.MainAccounts.Find(id);
        }
        public HeadAccount findHeadAcc(int id)
        {
            return (db.HeadAccounts.Find(id));
        }
        public SubHeadAccount findSubHeadAcc(int id)
        {
            return (db.SubHeadAccounts.Find(id));
        }
        public MainAccount findMainHead(int id)
        {
            HeadAccount head = db.HeadAccounts.Find(id);
            MainAccount main = db.MainAccounts.Find(head.main_id);
            return main;
        }
        public int lastHeadAccID()
        {
            try
            {
                int id = db.HeadAccounts.Max(x => x.head_id);
                return id;
            }
            catch(Exception )
            {
                return 0;
            }
            
        }
        public int lastSubHeadAccID()
        {
            try
            {
                int id = db.SubHeadAccounts.Max(x => x.sub_head_id);

                return id;
            }
            catch(Exception )
            {
                return 0;
            }
        }
        public void save()
        {
            db.SaveChanges();

        }
        public List<HeadAccount> listHeadAcc(int id)
        {
            List<HeadAccount> headAccounts = db.HeadAccounts.Where(x => x.main_id == id & x.is_active == "Y").ToList();
            return headAccounts;
        }
        public List<SubHeadAccount> listSubHeadAcc(int id)
        {
            List<SubHeadAccount> headAccounts = db.SubHeadAccounts.Where(x => x.main_id == id & x.is_active == "Y").ToList();
            return headAccounts;
        }
        public int lastTransID()
        {
            try
            {
                int id = db.Transactions.Max(x => x.Id);
                return id;
            }
            catch (Exception )
            {
                return 0;
            }
            
        }
        public List<TransactionAccount> allTransactionAccounts()
        {
            return db.TransactionAccounts.Where(x=>x.is_active == "Y").ToList();
        }

        public List<Good_Types> allGoodTypes()
        {
            return db.Good_Types.Where(x => x.is_active == "Y").ToList();
        }


        public static void addRistrictedAccount( string name)
        {
            int transAcc = db.TransactionAccounts.Count();
            int ristricAcc = db.User_Ristriction_Accounts.Count();

            if (transAcc - ristricAcc > 1)
            {
                List<TransactionAccount> listOfTransactionAccount = db.TransactionAccounts.Where(x => x.is_active == "Y").ToList();
                foreach (var acc in listOfTransactionAccount)
                {
                    User_Ristriction_Accounts uRA = new User_Ristriction_Accounts();
                    uRA.Name_ = acc.name;
                    uRA.is_active = 1;
                    db.User_Ristriction_Accounts.Add(uRA);
                }
            }
            else
            {
                User_Ristriction_Accounts account = new User_Ristriction_Accounts();
                account.Name_ = name;
                account.is_active = 1;
                db.User_Ristriction_Accounts.Add(account);
               
            }

            db.SaveChanges();
        }
        public static List<User_Ristriction_Accounts> allURAcc()
        {

            return db.User_Ristriction_Accounts.ToList(); 
        }
        public int totalAccounts ()
        {
            try
            {
                int count = db.TransactionAccounts.Where(x=>x.is_active=="Y").Count();
                return count;
            }
            catch(Exception)
            {
                return 0 ;
            }
        }
        public int totalTemporaryReports()
        {
            try
            {
                int count = db.TemporaryReports.Where(x => x.is_active =="Y").Count();
                return count/2;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int totalEmployees()
        {
            try
            {
                int count = db.Employees.Where(x => x.is_active == "Y").Count();
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void addPinAccounts(Pin_Accounts pin)
        {
            db.Pin_Accounts.Add(pin);
        }
        public List<Pin_Accounts> findPinAccountsOfUser(int id)
        {
            List<Pin_Accounts> list = db.Pin_Accounts.Where(x => x.User_id == id && x.User_type == "User" & x.is_active == 1).ToList();
            return list;
        }

        public List<Pin_Accounts> findPinAccountsOfAdmin(int id)
        {
            List<Pin_Accounts> list = db.Pin_Accounts.Where(x => x.Admin_id == id && x.User_type == "Admin" & x.is_active == 1).ToList();
            return list;
        }
        
        public void deActivePinAccountsOfAdmin(int id)
        {
            List<Pin_Accounts> list = db.Pin_Accounts.Where(x => x.Admin_id == id & x.User_type == "Admin" & x.is_active == 1).ToList();
            foreach(var item in list )
            {
                item.is_active = 0;
            }
            db.SaveChanges();
        }
        public void deActivePinAccountsOfUser(int id)
        {
            List<Pin_Accounts> list = db.Pin_Accounts.Where(x => x.User_id == id & x.User_type == "User" & x.is_active == 1).ToList();
            foreach (var item in list)
            {
                item.is_active = 0;
            }
            db.SaveChanges();
        }
        public TransactionAccount findTransactionAccount(int id)
        {
            return db.TransactionAccounts.Find(id);
        }
        public Pin_Accounts findPinAccount(int id)
        {
            return db.Pin_Accounts.Find(id);
        }

        

    }
}