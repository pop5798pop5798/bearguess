using SITW.Filter;
using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Net.Mime;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNet.Identity;
using SITW.Models;
using System.Data;

namespace SITW.Controllers
{

    public class PreferentialController : Controller
    {

        [MissionFilter]
        public ActionResult Index()
        {
            List<Preferential> pft = new PreferentialRepository().getAll();


            return View(pft);
        }
        public ActionResult Create()
        {

            Preferential pf = new Preferential();
            return View(pf);
        }

        [HttpPost]
        public ActionResult Create(Preferential pf)
        {
            try
            {
                pf.offerModel = 3;
                pf.inpdate = DateTime.Now;
                pf.vaild = 1;
                new PreferentialRepository().Create(pf);
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var preferential = new PreferentialRepository().Get((int)id);
            return View(preferential);
        }

        [HttpPost]
        public ActionResult Edit(Preferential pft)
        {
            try
            {
                //var preferential = new PreferentialRepository().Get((int)id);
                pft.inpdate = DateTime.Now;
                new PreferentialRepository().Update(pft);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public string FirstAssets(int money)
        {
            try
            {
                //var preferential = new PreferentialRepository().Get((int)id);
                var pft = new PreferentialRepository().Get(1);
                pft.assets = money;
                new PreferentialRepository().Update(pft);
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        // GET: WebMall/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }
            else
            {
                //var category = this.newsRepository.Get(id.Value);
                var mall = new PreferentialRepository().Get((int)id);
                return View(mall);
            }
        }

        // POST: WebMall/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                // TODO: Add delete logic here
                var mall = new PreferentialRepository().Get((int)id);
                new PreferentialRepository().Delete(mall);
                return RedirectToAction("Index");
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id });
            }
            //return RedirectToAction("Mall");
        }



    }
}