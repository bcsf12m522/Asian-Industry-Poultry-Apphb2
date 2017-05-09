using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Models;
using ranglerz_project.Services;
using System.IO;
using ranglerz_project.ModelsBO;

namespace ranglerz_project.Controllers
{
    [SessionCheck]
    public class EmployeeController : Controller
    {

        EmployeeServices service = new EmployeeServices();
     

        public ActionResult Index()
        {
            int check = (int)Session["emplyee_view"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(service.listEmployees());
        }
        public ActionResult Detail(int id)
        {
            return View(service.find(id));
        }

        //
        // GET: /Employee/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Employee/Create

        public ActionResult Create()
        {
            int check = (int)Session["employee_add"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //
        // POST: /Employee/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection,HttpPostedFileBase image)
        {
            int check = (int)Session["employee_add"];
            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            bool flag = service.create(collection, image);


            if (flag)
            {

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        //
        // GET: /Employee/Edit/5

        public ActionResult Edit(int id)
        {
            int check = (int)Session["employee_edit"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View(service.find(id));
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, HttpPostedFileBase image)
        {
            int check = (int)Session["employee_edit"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            
            
                bool flag = service.Edit(collection, image, id);

                if (flag)
                {
                    return RedirectToAction("Index");
                }
            
            else
            {
                return RedirectToAction("Edit");
            }
        }

        //
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id)
        {
            int check = (int)Session["employee_delete"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }


            service.delete(id);

            if(service.delete(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
 
        }

    

         
        public ActionResult AttendenceSearch(int id)
        {

            int check = (int)Session["view_attendence"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult Attendence ( FormCollection collection)
        {
            DateTime date = Convert.ToDateTime(collection["date"]).Date;
            ViewBag.Date = date;
            List<Employee> employeeFullList = service.listEmployees();
            List<Attendence> AttendencesList = service.listAttendecne(date);
            if(AttendencesList.Count == 0)
            {
                service.linkAttendence(employeeFullList, date);

                return View(service.listAttendecne(date));      
            }

            return View(AttendencesList);
        }
        [HttpPost]
        public ActionResult AttendencePost(FormCollection collection, int id)
        {


            int check = (int)Session["add_attedence"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Attendence atd = service.findAttendence(id);
           
            DateTime date = Convert.ToDateTime(collection["date"]).Date;

            ViewBag.Date = date;

            if (service.Attendence(collection,id,atd))
            {
                return View(service.listAttendecne(date));
            }

            return View(service.listAttendecne(date));
        }

        //[HttpPost]
        public ActionResult ViewAttendence(DateTime date)
        {
            int check = (int)Session["view_attendence"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Employee> employeeFullList = service.listEmployees();
            ViewBag.Date = date;
            DateTime serachDate = date.Date;
            List<Attendence> AttendencesList = service.listAttendecne(date);
            if (AttendencesList.Count == 0)
            {
                service.linkAttendence(employeeFullList, date);
                return View(service.listAttendecne(date));
            }
            return View(AttendencesList);

        }

        [HttpPost]
        public ActionResult UpdateAttendence(FormCollection collection, int id)
        {
            int check = (int)Session["edit_attendence"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Attendence atd = service.findAttendence(id);
            DateTime date = Convert.ToDateTime(collection["date"]).Date;
            ViewBag.Date = date;
        if(service.updateAttendence(collection,id,atd))
        {
            return RedirectToAction("AttendenceSearch", new {id = 2 });
        }

        return RedirectToAction("ViewAttendence", new { date = date });
        }
        public ActionResult AttendenceReports()
        {
            int check = (int)Session["add_attedence"];

            if (check == 0)
            {
                return RedirectToAction("Index", "Home");
            }




           List<EmployeeAttendenceReports> reports =  service.attendenceReports();
           
               if(reports.Count != 0)
               {
                   return View(reports);
               }
           
          return Json(new { status = "Error", message = "No Record Found" });
        }
    }
}
