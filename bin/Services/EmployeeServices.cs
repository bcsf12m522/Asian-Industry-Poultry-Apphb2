using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ranglerz_project.Models;
using System.Web.Mvc;
using ranglerz_project.ModelsBO;


namespace ranglerz_project.Services
{
    
    public class EmployeeServices
    {
        Database1Entities1 db = new Database1Entities1();

        public Employee find(int id)
        {
            return db.Employees.Find(id);
        }
        public List<Employee> listEmployees()
        {
            return db.Employees.ToList();
        }
        public bool create(FormCollection collection ,HttpPostedFileBase image)
        {
            Employee emp = new Employee();

            if (image != null && image.ContentLength > 0)
            {


                emp.image_ = new byte[image.ContentLength];
                image.InputStream.Read(emp.image_, 0, image.ContentLength);
                emp.Name = collection["name"];
                emp.City = collection["city"];
                emp.location = collection["location"];
                emp.cnic = collection["cnic"];
                emp.email = collection["email"];
                emp.phone = (collection["phone"]);
                emp.status = collection["status"];
                emp.department = collection["department"];
                emp.designation = collection["designation"];
                emp.joining_date = Convert.ToDateTime(collection["joining"]);

                emp.created_at = DateTime.UtcNow;
                emp.updated_at = emp.created_at;
                emp.is_active = "Y";

                db.Employees.Add(emp);

                db.SaveChanges();
                return true;




            }




            else
            {

                return false;

            }

        }
        public bool Edit (FormCollection collection,HttpPostedFileBase image, int id)
        {
            try
            {


                Employee emp = db.Employees.Find(id);
                emp.Name = collection["name"];
                emp.City = collection["city"];
                emp.location = collection["location"];
                emp.cnic = collection["cnic"];
                emp.email = collection["email"];
                emp.phone = (collection["phone"]);
                emp.status = collection["status"];
                emp.department = collection["department"];
                emp.designation = collection["designation"];
                emp.joining_date = Convert.ToDateTime(collection["joining"]);

                emp.updated_at = DateTime.UtcNow;

                if (image != null && image.ContentLength > 0)
                {
                    emp.image_ = new byte[image.ContentLength];
                    image.InputStream.Read(emp.image_, 0, image.ContentLength);
                }
                db.SaveChanges();
                return true;
            }
            catch(Exception )
            {
                return false;
            }
        }
        public bool delete(int id) 
        {
            try
            {
                Employee emp = db.Employees.Find(id);
                emp.is_active = "N";
                db.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool Attendence(FormCollection collection, int id ,Attendence atd)
        {
            try
            {
                
                string attendence = collection["present"];
                atd.time_in_ = TimeSpan.Parse(collection["timeIn"]);
                atd.time_out = TimeSpan.Parse(collection["timeOut"]);

                if (attendence == "present")
                {
                    atd.present = 1;
                    atd.absent = 0;
                }
                else
                {
                    atd.absent = 1;
                    atd.present = 0;
                }
        
                db.SaveChanges();

                return true;
            }
            catch( Exception)
            {
            return false;
            }
        }
         
        public List<Attendence> listAttendecne(DateTime date)
        {

            return db.Attendences.Where(x => x.date == date).OrderBy(x => x.employee_Id).ToList();
        }
        public Attendence findAttendence(int id)
        {
            return (db.Attendences.Find(id));
        }
        public bool updateAttendence(FormCollection collection, int id , Attendence atd)
        {
            try
            {

                string attendence = collection["present"];
                atd.time_in_ = TimeSpan.Parse(collection["timeIn"]);
                atd.time_out = TimeSpan.Parse(collection["timeOut"]);

                if (attendence == "present")
                {
                    atd.present = 1;
                    atd.absent = 0;
                }
                else
                {
                    atd.absent = 1;
                    atd.present = 0;
                }

                db.SaveChanges();

                return true;
            }
            catch(Exception)
            {
                return false;
            }

        }
        public bool linkAttendence(List<Employee> employeeFullList, DateTime date)
        {
            foreach(var emp in employeeFullList)
                {
                    Attendence atd = new Attendence();
                    atd.date = date;
                    atd.absent = 1;
                    atd.present = 0;
                    atd.employee_Id = emp.Id;
                    atd.updated_at = date;
                    db.Attendences.Add(atd);

                }
            db.SaveChanges();

            return true;
        }
        public List<Attendence> listAttendeceFull ()
        {
           return db.Attendences.ToList();
        }
        public List<EmployeeAttendenceReports>  attendenceReports( )
        {
            List<Employee> listEmp = db.Employees.OrderBy(x => x.Id).ToList();
            List<EmployeeAttendenceReports> listReports = new List<EmployeeAttendenceReports>();
            foreach(var emp in listEmp)
            {
                int presents = db.Attendences.Where(x => x.present == 1 && x.employee_Id==emp.Id).Count();
                int absents = db.Attendences.Where(x => x.absent == 1 && x.employee_Id==emp.Id).Count();
                EmployeeAttendenceReports report = new EmployeeAttendenceReports();
                report.employee = emp;
                report.absents = absents;
                report.presents = presents;
                report.Name = emp.Name;
                listReports.Add(report);
           }
            return listReports;
        }
    }
}