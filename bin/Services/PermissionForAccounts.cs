using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Models;

namespace ranglerz_project.Services
{
    public class PermissionForAccounts
    {
      public  static List<Permission> permisions;
      public static List<Reports_Permissions> permisionLedgers;
      public static List<Sale_Reports_Permissions> permisionSales;
      public static List<Purchase_Reports_Permissions> permisionPurchase;
      public static List<Expense_Reports_Permissions> permisionExpenses;
      public static List<Production_Reports_Permissions> permisionProductions;
      public static List<Main_Reports_Permissions> permisionMainReports;
      public static List<bankPayment_Reports_Permissions> permisionBankPaymentReports;

      public static bool permisionCheck(List<Permission> list, string from , string to)
      {
           int permisonCount = 0;

                    foreach (var check in PermissionForAccounts.permisions)
                    {

                        if (check.account_Name == from || check.account_Name == to)
                        {
                            permisonCount++;
                        }

                        if (permisonCount == 2)
                        {
                            return true;
                        }

                    }

                    return false;
      }
      public static bool permisionCheckForLedgers(List<Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionLedgers)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }


      public static bool permisionCheckForSales(List<Sale_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionSales)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

      public static bool permisionCheckForPurchases(List<Purchase_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionPurchase)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

      public static bool permisionCheckForExpenses(List<Expense_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionExpenses)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

      public static bool permisionCheckForProductions(List<Production_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionProductions)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

      public static bool permisionCheckForMainReports(List<Main_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionMainReports)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

      public static bool permisionCheckForBankPaymentReports(List<bankPayment_Reports_Permissions> list, string account)
      {
          int permisonCount = 0;

          foreach (var check in PermissionForAccounts.permisionBankPaymentReports)
          {

              if (check.account_name == account)
              {
                  permisonCount++;
              }

              if (permisonCount == 1)
              {
                  return true;
              }

          }

          return false;
      }

    }

}