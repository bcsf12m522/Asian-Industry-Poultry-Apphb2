﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ranglerz_project.Services;
using ranglerz_project.Models;

namespace ranglerz_project.Controllers
{
    [SessionCheck]
    public class PrintController : Controller
    {
        TransactionServices servicesTransaction = new TransactionServices();
        AccountServices serviceAccounts = new AccountServices();
        //
        // GET: /Print/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Print/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Print/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Print/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Print/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Print/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Print/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Print/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
