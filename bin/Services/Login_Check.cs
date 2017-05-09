using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;

namespace ranglerz_project.Services
{
    public class Login_Check
    {
        Database1Entities1 db = new Database1Entities1();

        public LoginBO check(string username, string password, string Permission)
        {
            string value = null;

            LoginBO loginBo = new LoginBO();


            if (Permission == "Admin")
            {
               
                    Admin admin = db.Admins.First(x => x.username == username && x.password == password);


                    if (admin.username == username && admin.password == password)
                    {
                        value = "Admin";
                        loginBo.obj = admin;
                        loginBo.user = value;
                        return loginBo;
                    }
                
             
            }

            else if (Permission == "User")
            {
                try
                {
                    User user = db.Users.First(x => x.username == username && x.password == password && x.is_active=="Y");
                    
                    value = "User";
                    loginBo.obj = user;
                    loginBo.user = value;

                    return loginBo;
                }
                catch (Exception)
                {
                    
                }

            }
            return null;

        }
    }
}