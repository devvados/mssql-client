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
    public class EmpsController : Controller
    {
        private StaffDataBaseEntities db = new StaffDataBaseEntities();

        // GET: Emps
        public ActionResult Index()
        {
            var emps = db.Emps.Include(e => e.Deps).Include(e => e.Positions);
            return View(emps.ToList());
        }

        // GET: Emps/Details/5
        public ActionResult Details(int? id)
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
            return View(emps);
        }

        public ActionResult CreateVacation(int? id)
        {
            return RedirectToAction("Create", "Vacations", new { id = id });
        }
        public ActionResult VacationEdit(int? id)
        {
            return RedirectToAction("Edit", "Vacations", new { id = id });
        }
        public ActionResult VacationDelete(int? id)
        {
            return RedirectToAction("Delete", "Vacations", new { id = id });
        }
        public ActionResult VacationDetails(int? id)
        {
            return RedirectToAction("Details", "Vacations", new { id = id });
        }

        // GET: Emps/Create
        public ActionResult Create()
        {
            ViewBag.DepID = new SelectList(db.Deps, "DepartamentID", "DepartamentName");
            ViewBag.PosID = new SelectList(db.Positions, "PositionID", "PositionName");
            return View();
        }

        // POST: Emps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,Name,Surname,Patronymic,PosID,VacationsCount,DepID")] Emps emps)
        {
            if (ModelState.IsValid)
            {
                db.Emps.Add(emps);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepID = new SelectList(db.Deps, "DepartamentID", "DepartamentName", emps.DepID);
            ViewBag.PosID = new SelectList(db.Positions, "PositionID", "PositionName", emps.PosID);
            return View(emps);
        }

        // GET: Emps/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.DepID = new SelectList(db.Deps, "DepartamentID", "DepartamentName", emps.DepID);
            ViewBag.PosID = new SelectList(db.Positions, "PositionID", "PositionName", emps.PosID);
            return View(emps);
        }

        // POST: Emps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,Name,Surname,Patronymic,PosID,VacationsCount,DepID")] Emps emps)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emps).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepID = new SelectList(db.Deps, "DepartamentID", "DepartamentName", emps.DepID);
            ViewBag.PosID = new SelectList(db.Positions, "PositionID", "PositionName", emps.PosID);
            return View(emps);
        }

        // GET: Emps/Delete/5
        public ActionResult Delete(int? id)
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
            return View(emps);
        }

        // POST: Emps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Emps emps = db.Emps.Find(id);
            db.Emps.Remove(emps);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
