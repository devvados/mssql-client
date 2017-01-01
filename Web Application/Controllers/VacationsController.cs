using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class VacationsController : Controller
    {
        private StaffDataBaseEntities db = new StaffDataBaseEntities();

        // GET: Vacations
        public ActionResult Index()
        {
            var vacations = db.Vacations.Include(v => v.Emps);
            return View(vacations.ToList());
        }

        // GET: Vacations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacations vacations = db.Vacations.Find(id);
            if (vacations == null)
            {
                return HttpNotFound();
            }
            return View(vacations);
        }

        // GET: Vacations/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emps emps = db.Emps.Find(id);
            if (emps == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Emps, "EmployeeID", "Name", emps.EmployeeID);

            return View();
        }

        // POST: Vacations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VacationID,BeginDate,EndDate,EmpID")] Vacations vacations)
        {
            if (ModelState.IsValid)
            {
                db.Vacations.Add(vacations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpID = new SelectList(db.Emps, "EmployeeID", "Name", vacations.EmpID);
            return View(vacations);
        }

        // GET: Vacations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacations vacations = db.Vacations.Find(id);
            if (vacations == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Emps, "EmployeeID", "Name", vacations.EmpID);
            return View(vacations);
        }

        // POST: Vacations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VacationID,BeginDate,EndDate,EmpID")] Vacations vacations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Emps");
            }
            ViewBag.EmpID = new SelectList(db.Emps, "EmployeeID", "Name", vacations.EmpID);
            return View(vacations);
        }

        // GET: Vacations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacations vacations = db.Vacations.Find(id);
            if (vacations == null)
            {
                return HttpNotFound();
            }
            return View(vacations);
        }

        // POST: Vacations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vacations vacations = db.Vacations.Find(id);
            db.Vacations.Remove(vacations);
            db.SaveChanges();
            return RedirectToAction("Index","Emps");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
