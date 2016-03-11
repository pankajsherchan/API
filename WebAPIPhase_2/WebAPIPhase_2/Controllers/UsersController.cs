﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using WebAPIPhase_2.Models;

namespace WebAPIPhase_2.Controllers
{
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private WebAPIPhase_2Context db = new WebAPIPhase_2Context();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignIn(User user)
        {

               
            var userInDatabase = db.Users.FirstOrDefault(u => u.Email == user.Email);


            if (ModelState.IsValid && userInDatabase != null)
            {
                bool verifyPassword = Crypto.VerifyHashedPassword(userInDatabase.Password, user.Password);
                if (verifyPassword == true)
                {

                    // creating authetication ticket
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    Session["userRole"] = user.Role;

                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    @ViewBag.Message = "Error.Ivalid login.";
                   
                }

            }

            ModelState.AddModelError("UserDoesNotExist", "Username or Password is Incorrect! Please try Again!!");
            return View(user);
        }

        public ActionResult Logout()
        {
            // This is the predefined SignOut method in FormAuthentication :p
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");

        }
      
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                
                user.Password = Crypto.HashPassword(user.Password);
                user.Role = "Customer";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Email,Password,Role")] User userRole)
        {
            if (ModelState.IsValid)
            {

               
                    userRole.Role = userRole.Role;
                    db.Entry(userRole).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
              
                //     else
                //{
                //    ModelState.AddModelError("", "You need to be admin Bro to change my data");
                //    @ViewBag.Message = "You need to be admin to edit";

                //    }
                
            
            
            return View(userRole);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
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
