using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        public ViewResult Index()
        {
            //var model = db.Movies.ToList();  //tolist dediğinde db de veri yoksa null değil boş liste gelir. eğer asenurable olsaydı null gelecekti.
            var model = GetList();
            //ViewBag.count = model.Count;
            ViewData["count"] = model.Count;
            return View(model);  //view içerisinde razor syntaxı kullanarak,listeleme işlemi yapılıyor. o yüzden viewe ihtiyacım var.
            //eğer bir veri taşıyacaksan view üzerinden yapmalısın.
            //dinamkik veri göstereceksen modeli taşımaya ihtiyacın var. viewe gönderiyorsun.
        }

        [NonAction]  //URL üzerinden getlist aksiyonunu görmemek için
        public List<Movie> GetList(bool removeSession = true)  //session sunucunun memorysini kullanarak verileri tutuyor. verilere o tarayıcı bazında erişebiliyorum.
        {
            List<Movie> entities;
            if (removeSession)
            {
                Session.Remove("movies");
            }
            if (Session["movies"]==null || removeSession)
            {
                entities = db.Movies.ToList();  //çektiğin verileri önce sessiona atman lazım. verileri memoryden çekerken senin attığın verileri getirecek çünkü.
                Session["movies"] = entities;
            }
            else
            {
                entities = Session["movies"] as List<Movie>;
            }
            return entities;
        }

        public ActionResult GetMoviesFromSession()  //movieleri bana hafızadan getir. methodun amacı bu.
        {
            var model = GetList(false);
            return View("Index", model);  //tek bir view'ı 2 aksiyonda kullanmış olduk. 
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Message = "Please enter movie information...";  //dynamic bir yapı. runtime sırasında çalışıyor.  viewbag. dan sonraki kısım değişken. 
            return View();
            //return new ViewResult(); bu şekilde de yazabilirsin.
        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name,int ProductionYear,string BoxOfficeReturn)  //formu almak için add,verileri kaydetmek için de add methodu yazdık. fakat biri get biri post oldu. 2. add methodu overload oldu artık imzaları farklı. [] içine yazma methoduna,action method selector deniyor. içerisine yapacağın işlemi yazıyorsun.
        {
            var entity = new Movie()
            {
                Name = Name,
                ProductionYear = ProductionYear.ToString(),
                BoxOfficeReturn =Convert.ToDouble(BoxOfficeReturn.Replace(",","."), CultureInfo.InvariantCulture)  
                //eğer kullanıcı 2,1 girerse program patlar. double virgül değil nokta kabul eder. o yüzden replace ile , yerine . yaptık.

            };
            db.Movies.Add(entity);
            db.SaveChanges();
            TempData["Info"] = "Record successfully saved to database.";
            return RedirectToAction("Index"); //ındex aksiyonuna yönlendirdim. kaydı ekleyince,listeyi güncelleyip yeniden göstermesi için indexi çağırdım.
        }

        //formu get ile al, verileri de post ile gönder. bunun için 2  method tanımla aynı isimli. ama biri overload edilecek. genel kullanım böyledir.



        public ActionResult Edit(int? id)  //edit=update işlemi. update için id ye ihtiyacım var. id null mı diye bakıyoruz.
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Id is required! ");
            }
            var model = db.Movies.Find(id.Value);
            List<SelectListItem> years = new List<SelectListItem>();
            SelectListItem year;
            for (int i = DateTime.Now.Year; i >= 1900; i--)
            {
                year = new SelectListItem() { Value = i.ToString(), Text = i.ToString() };
                years.Add(year);
            }
            ViewBag.Years = new SelectList(years, "Value", "Text", model.ProductionYear);
            return View(model);
          
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include ="Id, Name, ProductionYear")]Movie movie,string BoxOfficeReturn)
        {
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            entity.BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToRoute(new { controller = "Movies", action = "Index" });

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required !");
            }

            var model = db.Movies.FirstOrDefault(e => e.Id == id.Value);
            return View(model);
        }

        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int? id)
        {
            var entity = db.Movies.Find(id);
            db.Movies.Remove(entity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}