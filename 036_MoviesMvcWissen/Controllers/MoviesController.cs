using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var model = db.Movies.ToList();  //tolist dediğinde db de veri yoksa null değil boş liste gelir. eğer asenurable olsaydı null gelecekti.
          
            return View(model);  //view içerisinde razor syntaxı kullanarak,listeleme işlemi yapılıyor. o yüzden viewe ihtiyacım var.
            //eğer bir veri taşıyacaksan view üzerinden yapmalısın.
            //dinamkik veri göstereceksen modeli taşımaya ihtiyacın var. viewe gönderiyorsun.
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name,string ProductionYear,string BoxOfficeReturn)  //formu almak için add,verileri kaydetmek için de add methodu yazdık. fakat biri get biri post oldu. 2. add methodu overload oldu artık imzaları farklı. [] içine yazma methoduna,action method selector deniyor. içerisine yapacağın işlemi yazıyorsun.
        {
            var entity = new Movie()
            {
                Name = Name,
                ProductionYear = ProductionYear,
                BoxOfficeReturn =Convert.ToDouble(BoxOfficeReturn.Replace(",","."), CultureInfo.InvariantCulture)  
                //eğer kullanıcı 2,1 girerse program patlar. double virgül değil nokta kabul eder. o yüzden replace ile , yerine . yaptık.

            };
            db.Movies.Add(entity);
            db.SaveChanges();
            return RedirectToAction("Index"); //ındex aksiyonuna yönlendirdim. kaydı ekleyince,listeyi güncelleyip yeniden göstermesi için indexi çağırdım.
        }

        //formu get ile al, verileri de post ile gönder. bunun için 2  method tanımla aynı isimli. ama biri overload edilecek. genel kullanım böyledir.
    }
}