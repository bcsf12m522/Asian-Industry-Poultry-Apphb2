using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.ModelsBO;
using ranglerz_project.Models;
using ranglerz_project.Services;
namespace ranglerz_project.Services
{
    public class StockServiceClass
    {
        Database1Entities1 db = new Database1Entities1();
        public List<RoutesReport> routeReports(DateTime date)
        {
            List<RoutesReport> listRouteReport = new List<RoutesReport>();
            RoutesReport routeReport = new RoutesReport();

            for (int i = 0; i < 31; i++)
            {
                RoutesReport routeReportObj = new RoutesReport();
                List<int> listofweights = new List<int>();

                for (int j = 136; j <=145; j++)
                {
                    int weight = 0;

                    if ((db.Transactions.Where(x => x.Id == j & x.created_at == date & x.is_active == "Y")) != null)
                    {
                        List<Transaction> transactions = db.Transactions.Where(x => x.trans_acc_id == j & x.created_at == date & x.is_active == "Y").ToList();

                        foreach (var tr in transactions)
                        {
                            if (tr.net_weight != null)
                            {
                                weight = (int)(weight + tr.net_weight);
                            }
                        }
                        
                    } 
                    
                    


                    if (weight == null)
                    {
                        weight = 0;
                    }

                    listofweights.Add(weight);                          
                }
                date = date.AddDays(1);

                int dateNumber =(int)date.Day;

              

                routeReportObj.routeTotalWeight = listofweights;
                routeReportObj.AccountID = i;
                listRouteReport.Add(routeReportObj);

                if (dateNumber == 1)
                {
                    break;
                }
            }
            return listRouteReport;
        }
    }
}