﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HabitatForHumanity.Models;
using HabitatForHumanity.ViewModels;

namespace HabitatForHumanity.Controllers
{
    public class TimeSheetController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        // GET: TimeSheet
        //public ActionResult Index()
        //{
        //    return View(db.timeSheets.ToList());
        //}

        // GET: TimeSheet/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeSheet timeSheet = db.timeSheets.Find(id);
        //    if (timeSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(timeSheet);
        //}


        #region PunchIn

        [AuthorizationFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult PunchIn([Bind(Include = "userId,projectId,orgId")] PunchInVM punchInVM)
        public ActionResult PunchIn(PunchInVM punchInVM)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (punchInVM.orgId < 1)
                    {
                        punchInVM.orgId = 1; // force the -1 to be org #1, relies on orgId 1 == "Individual"
                    }
                    TimeSheet sheet = new TimeSheet();
                    sheet.user_Id = punchInVM.userId;
                    sheet.project_Id = punchInVM.projectId;
                    sheet.clockInTime = DateTime.Now;
                    sheet.clockOutTime = DateTime.Today.AddDays(1);
                    sheet.org_Id = punchInVM.orgId;

                    //TODO: check error code?
                    ReturnStatus st = Repository.PunchIn(sheet);
                    if (st.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = "punchin action" });
                    }
                    //    return RedirectToAction("VolunteerPortal", "User");
                }


                return RedirectToAction("VolunteerPortal", "User", new { justPunched = 1 });
            }
            catch
            {
                return View("Error");
            }

        }
        #endregion

        #region PunchOut

        [AuthorizationFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult PunchOut([Bind(Include = "Id,user_Id,project_Id,org_id,clockInTime,clockOutTime")] TimeSheet timeSheet)
        public ActionResult PunchOut(PunchOutVM punchOutVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TimeSheet timeSheet = new TimeSheet();
                    timeSheet.Id = punchOutVM.timeSheetNumber;
                    timeSheet.user_Id = punchOutVM.userNumber;
                    timeSheet.project_Id = punchOutVM.projectNumber;
                    timeSheet.org_Id = punchOutVM.orgNumber;
                    timeSheet.clockInTime = punchOutVM.inTime;
                    timeSheet.clockOutTime = DateTime.Now;

                    //TODO: add handling to ensure timesheet was properly updated
                    Repository.UpdateTimeSheet(timeSheet);

                    return RedirectToAction("VolunteerPortal", "User", new { justPunched = 1 });
                }
                return View(punchOutVM);
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion

        #region Create
        // GET: TimeSheet/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}


        //// POST: TimeSheet/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,user_Id,project_Id,clockInTime,clockOutTime")] TimeSheet timeSheet)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.timeSheets.Add(timeSheet);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(timeSheet);
        //}
        #endregion

        #region Edit
        // GET: TimeSheet/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeSheet timeSheet = db.timeSheets.Find(id);
        //    if (timeSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(timeSheet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,user_Id,project_Id,clockInTime,clockOutTime")] TimeSheet timeSheet)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(timeSheet).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(timeSheet);
        //}
        #endregion

        #region Delete
        // GET: TimeSheet/Delete/5
        [AdminFilter]
        [AuthorizationFilter]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TimeSheet timeSheet = db.timeSheets.Find(id);
                if (timeSheet == null)
                {
                    return HttpNotFound();
                }
                return View(timeSheet);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: TimeSheet/Delete/5
        [AdminFilter]
        [AuthorizationFilter]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TimeSheet timeSheet = db.timeSheets.Find(id);
                db.timeSheets.Remove(timeSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
            catch
            {
                //not sure what to put here
            }
        }

    }
}
