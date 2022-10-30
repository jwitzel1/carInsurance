using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Web.UI;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public async Task<ActionResult> Index()
        {
            return View(await db.Insurances.ToListAsync());
        }

        // GET: Insuree/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = await db.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: Insuree/Admin
        //public ActionResult Admin => View();
        
        // GET: Insuree for Admin
        public async Task<ActionResult> Admin()
        {
            return View(await db.Insurances.ToListAsync());
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,Get_Full_All_Reason_Coverage,Quote,QuoteDate")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                //insurance.Quote do the math
                //get temp vars
                decimal quoteVal = 50.00m;

                DateTime dateOfBirth = insurance.DateOfBirth;
                DateTime bDay = dateOfBirth;
                var now = DateTime.Now;
                int age = now.Year - bDay.Year;
                if (now.Month < bDay.Month || (now.Month == bDay.Month && now.Day < bDay.Day)) { age--; }
                if(age <= 18) { quoteVal += 100; }
                 else if(age <= 25) { quoteVal += 50; }
                 else { quoteVal += 25; }
                if(insurance.CarYear > 2015 || insurance.CarYear < 2000) { quoteVal += 25; }
                if ((string)insurance.CarMake == "Porsche")
                {
                    if(insurance.CarModel=="911 Carrera") { quoteVal += 50; } else { quoteVal += 25; }
                }
                quoteVal+= (insurance.SpeedingTickets*10);
                if (insurance.DUI) { quoteVal = (decimal)(quoteVal)
                                                * 1.25m; }
                if (insurance.Get_Full_All_Reason_Coverage) { quoteVal = (decimal)(quoteVal)
                                                * 1.5m; }

                insurance.Quote = quoteVal;
                db.Insurances.Add(insurance);
                Console.WriteLine("Start query...quote is for " + quoteVal);
                await db.SaveChangesAsync();
                Console.WriteLine("End query...");
                return RedirectToAction("Index");
            }

            return View(insurance);
        }

        // GET: Insuree/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = await db.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,Get_Full_All_Reason_Coverage,Quote,QuoteDate")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insurance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(insurance);
        }

        // GET: Insuree/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = await db.Insurances.FindAsync(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Insurance insurance = await db.Insurances.FindAsync(id);
            db.Insurances.Remove(insurance);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
