using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Models;

namespace ranglerz_project.Services
{
    public class TransactionServices
    {
        Database1Entities1 db = new Database1Entities1();
         

        public TransactionAccount findJvTransaction(string name)
        {
            try{
                 TransactionAccount tr = db.TransactionAccounts.Where(x=>x.name==name & x.is_active =="Y").First();
                 return tr;

            }
            catch(Exception )
            {
                return null;
            }
        }
        public void save()
        {
            db.SaveChanges();
        }
        public void addTransaction(Transaction tr)
        {
            db.Transactions.Add(tr);
        }
        public List<Transaction> allTransactions()
        {
            return db.Transactions.Where(x=>x.is_active=="Y").ToList();
        }
        public List<TransactionAccount> allTransactionaccounts()
        {
            return db.TransactionAccounts.Where(x=>x.is_active=="Y").ToList();
        }
         public List<Transaction> findTransactions(string search)
        {
            return db.Transactions.Where(x => x.voucher_code == search).ToList();
        }
         public List<TransactionAccount> findTransactionAccounts(string search)
         {
             return db.TransactionAccounts.Where(x => x.name == search & x.is_active=="Y").ToList();
         }
          public User findUser(string username)
         {
             return db.Users.Where(x => x.username == username & x.is_active=="Y").First();
         }
        public Transaction findTransaction(int id)
          {
              return db.Transactions.Find(id);
          }
        public List<Good> allGoods()
        {
            return db.Goods.Where(x=>x.is_active=="Y").ToList();
        }
        public Good findGoodByName(string name)
        {
            try
            {
                return (db.Goods.Where(x => x.good_Name == name & x.is_active == "Y").First());
            }
            catch
            {
                return null;
            }
        }

        public void addGood(Good good)
        {
            db.Goods.Add(good);
            return;
        }
        public void addUnitRate(UnitRate good)
        {
            db.UnitRates.Add(good);
            return;
        }
        public double findRate(int id,string name)
        {

            try
            {
                UnitRate unit = db.UnitRates.Where(x => x.account_id == id & x.good== name & x.is_active == "Y").First();
                return unit.rate;
            }
           
            catch(Exception e)
            {
                return 0;
            }
          
  
        }

        public void findAndSetUnitRates(int id,string good)
        {
            try
            {
                List<UnitRate> ratess = db.UnitRates.Where(x => x.account_id == id && x.good == good && x.is_active=="Y").ToList();

                foreach (UnitRate rate in ratess)
                {
                    rate.is_active = "N";
                    rate.updated_at = DateTime.UtcNow;
                }
                db.SaveChanges();
            }

            catch (Exception )
            {
                return ;
            }


            return ;
        }
        public List<Transaction> allSaleVoucher()
        {
            try
            {
                return db.Transactions.Where(x => x.voucher_type == "SV111").ToList();
            }
            catch(Exception)
            {
                return null;
            }
        }
        public Good findGood(string name)
        {
            return db.Goods.Where(x => x.good_Name == name & x.is_active == "Y").First();
        }
        public Good findGood(int id)
        {
            return db.Goods.Find(id);
        }
        public List<Good_Types> allGoodTypes()
        {
            return db.Good_Types.ToList();
        }
        public TransactionAccount findTransactionAccount(int id)
        {
            return db.TransactionAccounts.Find(id);
        }
        public void addExpenseVoucher(ExpenseTransaction tr)
        {
            db.ExpenseTransactions.Add(tr);
        }
        public List<ExpenseTransaction> allExpenseVoucherTransaction()
        {
            return db.ExpenseTransactions.ToList();
        }
        
        public ExpenseTransaction findExpenseVoucher(int id)
        {
            return db.ExpenseTransactions.Find(id);
        }
        public TemporaryReport findTemporaryReport(int id)
        {
          return  db.TemporaryReports.Find(id);
        }
        public List<HeadAccount> allHeadAccounts()
        {
            return db.HeadAccounts.Where(x => x.main_id == 4 & x.is_active == "Y").ToList();
        }
        public List<SubHeadAccount> allSubHeadAccounts()
        {
            return db.SubHeadAccounts.Where(x => x.main_id == 4 & x.is_active == "Y").ToList();
        }
        public HeadAccount findHeadAccounts(string search)
        {
            return db.HeadAccounts.Where(x => x.name == search & x.is_active == "Y").First();
        }
        public SubHeadAccount findSubHeadAccounts(string search)
        {
            return db.SubHeadAccounts.Where(x => x.name == search & x.is_active == "Y").First();
        }
        public List<Transaction> findTransactionsByGoods(string search)
        {
            try
            {

                return db.Transactions.Where(x => x.good_ == search & x.is_active == "Y").ToList();
            }
            catch
            {
                return null;
            }
        }
        public int findAccountBalance(string search)
        {
            return Convert.ToInt32(db.TransactionAccounts.Where(x => x.name == search & x.is_active == "Y").First().parent);
           
        }
        public static int getMaxId()
        {
            Database1Entities1 db = new Database1Entities1();
            if (db.TemporaryReports.Count() !=0)
            {
                int num = db.TemporaryReports.Max(x => x.Id);
                return num++;
            }
            else
            {
                return 1;
            }
        }
        public static int getMaxIdForProduction()
        {
            Database1Entities1 db = new Database1Entities1();
            if (db.Transactions.Count() != 0)
            {
                int num = db.Transactions.Max(x => x.Id);
                return num++;
            }
            else
            {
                return 1;
            }
        }

        public List<MainAccount> allMainAccounts()
        {
            return db.MainAccounts.Where(x => x.is_active == "Y").ToList();
        }
        public MainAccount findMainAccount(string search)
        {
            return db.MainAccounts.Where(x => x.name == search & x.is_active == "Y").First();
        }
        public TransactionAccount findSaleAccount(int id)
        {
            TransactionAccount account = db.TransactionAccounts.Find(id);
            return account;
        }
        //public User findAdmin(string username)
        //{
        //    return db.Users.Where(x => x.username == username & x.is_active == "Y").First();
        //}
        public List<TransactionAccount> allTransactionaccountsForExpense()
        {
            return db.TransactionAccounts.Where(x => x.type_ == "Expense" || x.type_ == "Cash" & x.is_active == "Y").ToList();
        }
        public List<TransactionAccount> AllStockAccounts()
        {
            return db.TransactionAccounts.Where(x => x.type_ =="Goods Account" & x.is_active=="Y").ToList();
        }
        public int findAccountWeight(string search)
        {
            return Convert.ToInt32(db.TransactionAccounts.Where(x => x.name == search & x.is_active == "Y").First().opening_weight);

        }

        public int findOpeningWeightBeforeDate(string enddate,string searching)
        {
            DateTime date = Convert.ToDateTime(enddate);
            TransactionAccount account = db.TransactionAccounts.Where(x => x.name == searching & x.is_active == "Y").First();
            List<Transaction> transactions = account.Transactions.Where(x => x.created_at < date & x.is_active == "Y").ToList();
            int weight = Convert.ToInt32(account.opening_weight);
            int balance = weight;
            int totalBalance = 0;
            foreach (var t in transactions)
            {
                if (t.net_weight != null)
                {

                    if (t.extra == "Sale" && account.main_id != 12 || t.net_weight == null)
                    {
                        continue;
                    }

                    if (t.is_active != "Y")
                    {
                        continue;
                    }
                    if (account.id == 118 && t.voucher_type == "PRV")
                    {
                        continue;
                    }

                    if (t.voucher_type == "SV" || t.voucher_type == "UPSV")
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else if (t.voucher_type == "PRV" && t.Id % 2 == 1)
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else if (t.voucher_type == "WEV" && t.Id % 2 == 1)
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else
                    {
                        balance = (int)(balance + t.net_weight);
                    }

                    totalBalance = totalBalance + balance;
                }
            }
            return balance;
        }
                                                    
                                  



         

      
        public int findOpeningBalancebeforeDate(DateTime date, string search)
        {
            TransactionAccount account = db.TransactionAccounts.Where(x => x.name == search & x.is_active =="Y").First();
            List<Transaction> transactions = account.Transactions.Where(x => x.created_at < date && x.is_active == "Y").ToList();
            int balance = Convert.ToInt32(account.parent);
            foreach(var tr in transactions)
            {
                balance = balance + tr.cr;
                balance = balance - tr.dr;
            }
            return balance;
        }

        public bool balanceUpdation(int id1, int id2)
        {
          try{

            TransactionAccount account1 = db.TransactionAccounts.Find(id1);
            TransactionAccount account2 = db.TransactionAccounts.Find(id2);
            int balance1 =Convert.ToInt32(account1.parent);
            int balance2 =Convert.ToInt32(account2.parent);
            int weight1 = Convert.ToInt32(account1.opening_weight);
            int weight2 = Convert.ToInt32(account2.opening_weight);
            int Debit1 =0;
            int Credit1=0;
            int Debit2 =0;
            int Credit2=0;
            foreach(var tr in account1.Transactions.Where(x=>x.is_active=="Y"))
                {
                    if (tr.extra == "Sale" && account1.main_id != 12)
                    {
                        continue;
                    }

                    Credit1 = Credit1 + tr.cr;
                    Debit1 = Debit1 + tr.dr;
                    if(account1.type_ == "Goods Account")
                    {
                        if (tr.net_weight != null)
                        {
                            if (account1.id == 118 && tr.voucher_type == "PRV")
                            {
                                continue;
                            }

                            if (tr.voucher_type == "SV" || tr.voucher_type == "UPSV")
                            {
                                weight1 = (int)(weight1 - tr.net_weight);
                            }
                            else if (tr.voucher_type =="PRV" && tr.Id % 2 != 0)
                            {
                                weight1 = (int)(weight1 - tr.net_weight);
                            }
                            else if (tr.voucher_type == "WEV" && tr.Id %2 !=0)
                            {
                                weight1 = (int)(weight1 - tr.net_weight); 
                            }
                            else if(tr.voucher_type=="WEV" && tr.Id %2 ==0)
                            {
                                weight1 = (int)(weight1 + tr.net_weight); 
                            }

                            else if (tr.voucher_type == "PV" || tr.voucher_type == "UPPV" || tr.voucher_type == "PRV")
                            {
                                weight1 = (int)(weight1 + tr.net_weight);
                            }
                        }
                    }
                }

               foreach(var tr in account2.Transactions.Where(x=>x.is_active=="Y"))
                {
                    if (tr.extra == "Sale" && account2.main_id != 12)
                    {
                        continue;
                    }

                    Credit2 = Credit2 + tr.cr;
                    Debit2 = Debit2 + tr.dr;
                    if(account2.type_ == "Goods Account")
                    {
                        if (tr.net_weight != null)
                        {
                            if (account2.id == 118 && tr.voucher_type == "PRV")
                            {
                                continue;
                            }

                            if (tr.voucher_type == "SV" || tr.voucher_type == "UPSV")
                            {
                                weight2 = (int)(weight2 - tr.net_weight);
                            }
                            else if (tr.voucher_type == "PRV" && tr.Id % 2 != 0)
                            {
                                weight2 = (int)(weight2 - tr.net_weight);
                            }
                            else if (tr.voucher_type == "WEV" && tr.Id % 2 != 0)
                            {
                                weight2 = (int)(weight2 - tr.net_weight);
                            }
                            else if (tr.voucher_type == "WEV" && tr.Id % 2 == 0)
                            {
                                weight2 = (int)(weight2 + tr.net_weight);
                            }

                            else if (tr.voucher_type == "PV" || tr.voucher_type == "UPPV" || tr.voucher_type == "PRV")
                            {
                                weight2 = (int)(weight2 + tr.net_weight);
                            }
                        }
                    }
                }
             account1.balance = (balance1 + Credit1)- Debit1;
             account2.balance = (balance2 + Credit2)- Debit2;
             account1.WEIGHT  = weight1.ToString();
             account2.WEIGHT  = weight2.ToString();
             db.SaveChanges();
             return true;
        }
            catch(Exception)
          {
                return false;
          }
     }

        public bool setAllBalances()
        {
            try
            {

                List<TransactionAccount> accounts = db.TransactionAccounts.Where(x => x.is_active == "Y").ToList();
                foreach (var acc in accounts)
                {
                    int Debit = 0;
                    int Credit = 0;
                    int balance = Convert.ToInt32(acc.parent);
                    int weight = Convert.ToInt32(acc.opening_weight);

                    foreach (var tr in acc.Transactions.Where(x => x.is_active == "Y"))
                    {
                        if (tr.extra == "Sale" && acc.main_id != 12)
                        {
                            continue;
                        }

                        Credit = Credit + tr.cr;
                        Debit = Debit + tr.dr;
                        if (acc.type_ == "Goods Account")
                        {
                            if (tr.net_weight != null)
                            {

                                if (tr.voucher_type == "SV" || tr.voucher_type == "UPSV")
                                {
                                    weight = (int)(weight - tr.net_weight);
                                }
                                else if (tr.voucher_type == "PRV" && tr.Id % 2 != 0)
                                {
                                    weight = (int)(weight - tr.net_weight);
                                }

                                else if (tr.voucher_type == "PV" || tr.voucher_type == "UPPV" || tr.voucher_type == "PRV")
                                {
                                    weight = (int)(weight + tr.net_weight);
                                }
                            }
                        }
                    }

                    acc.balance = (balance + Credit) - Debit;
                    acc.WEIGHT = weight.ToString();
                    db.SaveChanges();
                }
                return true;
            }
             
            catch(Exception)
            {
                return false;
            }

        }
        public bool checkID()
        {
            int id = db.TemporaryReports.Max(x => x.Id);
            
            if(id % 2 == 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        public bool checkIDTrans()
        {
            int id = db.Transactions.Max(x => x.Id);

            if (id % 2 == 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public int findOpeningWeightBeforeDates(string enddate, string searching)
        {
            string strs = null;
            DateTime date = Convert.ToDateTime(enddate);
            TransactionAccount account = db.TransactionAccounts.Where(x => x.name == searching & x.is_active == "Y").First();
            List<Transaction> transactions = account.Transactions.Where(x => x.created_at < date & x.is_active == "Y").ToList();
            int weight = Convert.ToInt32(account.opening_weight);
            int balance = weight;
            int totalBalance = 0;
            foreach (var t in transactions)
            {
                if (t.net_weight != null)
                {

                    if (t.extra == "Sale" && account.main_id != 12 || t.net_weight == null)
                    {
                        continue;
                    }

                    if (t.is_active != "Y")
                    {
                        continue;
                    }
                    if (account.id == 118 && t.voucher_type == "PRV")
                    {
                        continue;
                    }

                    if (t.voucher_type == "SV" || t.voucher_type == "UPSV")
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else if (t.voucher_type == "PRV" && t.Id % 2 == 1)
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else if (t.voucher_type == "WEV" && t.Id % 2 == 1)
                    {
                        balance = (int)(balance - t.net_weight);
                    }
                    else
                    {
                        balance = (int)(balance + t.net_weight);
                    }

                    totalBalance = totalBalance + balance;
                }
            }
            return balance;
        }
    }
}