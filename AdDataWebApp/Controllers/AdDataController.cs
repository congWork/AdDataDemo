using AdDataWebApp.AdDataWebService;
using AdDataWebApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdDataWebApp.Controllers
{
    public class AdDataController : Controller
    {
        // GET: AdData
        public ActionResult Index(string startDatetime,string endDatetime,int? page,int pageSize=50)
        {
            return View();
        }
        public JsonResult ListDataForDatetimeRange(string startDatetime, string endDatetime, int? page, decimal? pageCoverage, int pageSize = 50, string selectedOption="")
        {
            List<Ad> result;
            DateTime _startDt;
            DateTime _endDt;
            //default values for startDateTime and endDatetime
            if (!DateTime.TryParse(startDatetime, out _startDt))
            {
                _startDt = new DateTime(2011, 1, 1);
            }

            if (!DateTime.TryParse(endDatetime, out _endDt))
            {
                _endDt = new DateTime(2011, 4, 1);
            }

            int currPage = 1;
            if (page != null) currPage = page.Value;
            int totalRecords = 0;

            //consume web service to get data
            using (AdDataServiceClient client = new AdDataServiceClient())
            {
                try {
                    var tempList = client.GetAdDataByDateRange(_startDt, _endDt).ToList();
                    switch (selectedOption)
                    {
                        //get data from A list of ads which appeared in the “Cover” position with 50% + page coverage, take 50 records per time
                        case "opt2":
                            decimal myVal = pageCoverage ?? (decimal)0.5;
                            var allData = tempList.Where(x => x.Position.ToLower() == "cover" && x.NumPages >= myVal).OrderBy(x => x.Brand.BrandName).ToList();
                            totalRecords = allData.Count();
                            result = allData.Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();

                            break;
                        //get a full list of the ads, take 50 records per time
                        default:
                            totalRecords = tempList.Count();
                            result = tempList.OrderBy(x => x.Brand.BrandName).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                            break;
                    }
                }
                catch
                {
                    return Json(new VmListDataResponse(), JsonRequestBehavior.AllowGet);
                }
               
            };

            int pageNumber = page ?? 1;        
            int totalPages = totalRecords / pageSize;
            if (totalPages <= 0)
            {
                totalPages = 1;
            }
            else
            {
                totalPages += 1;
            }
            //bind data to the view model
            VmListDataResponse r = new VmListDataResponse
            {
                TotalPages = totalPages,
                CurrentPage = currPage,
                MyData = result
            };
           
            return Json(r,JsonRequestBehavior.AllowGet);
        }
      
        
        public ActionResult TopFiveBrandsByPageCoverage(string startDatetime, string endDatetime)
        {
           
            DateTime _startDt;
            DateTime _endDt;

            if (!DateTime.TryParse(startDatetime, out _startDt))
            {
                _startDt = new DateTime(2011, 1, 1);
            }

            if (!DateTime.TryParse(endDatetime, out _endDt))
            {
                _endDt = new DateTime(2011, 4, 1);
            }

            List<VmTopFiveBrands> myResult = new List<VmTopFiveBrands>();
            try {
                using (AdDataServiceClient client = new AdDataServiceClient())
                {
                    //group the data by brand name
                    var tempList = from k in client.GetAdDataByDateRange(_startDt, _endDt)
                                   group k by k.Brand.BrandName into g
                                   select new
                                   {
                                       totalPageCoverage = g.Sum(x => x.NumPages),
                                       brandName = g.Key
                                   };
                    //take top 5
                    var result = tempList.OrderByDescending(x => x.totalPageCoverage).OrderBy(x => x.brandName).Take(5).ToList();
                    //bind data into the view model
                    foreach (var i in result)
                    {
                        myResult.Add(new VmTopFiveBrands
                        {
                            TotalPageCoverage = i.totalPageCoverage,
                            BrandName = i.brandName

                        });
                    }
                    return View(myResult);
                };
            }
            catch
            {
                return View(myResult);
            }
        }
      
        public ActionResult TopFiveAdsByPageCoverage(string startDatetime, string endDatetime)
        {

            DateTime _startDt;
            DateTime _endDt;

            if (!DateTime.TryParse(startDatetime, out _startDt))
            {
                _startDt = new DateTime(2011, 1, 1);
            }

            if (!DateTime.TryParse(endDatetime, out _endDt))
            {
                _endDt = new DateTime(2011, 4, 1);
            }

            List<VmListTopFiveAds> myRecs = new List<VmListTopFiveAds>();
            try
            {
                using (AdDataServiceClient client = new AdDataServiceClient())
                {
                    //The top five ads by page coverage amount, distinct by brand.
                    var tempList = client.GetAdDataByDateRange(_startDt, _endDt).Select(x =>
                        new
                        {
                            totalPageCoverage = x.NumPages,
                            brand = x.Brand
                        }).Distinct().ToList();

                    var result = tempList.OrderByDescending(x => x.totalPageCoverage).OrderBy(x => x.brand.BrandName).Take(5).ToList();

                    //bind data into the view model
                    foreach (var i in result)
                    {
                        myRecs.Add(new VmListTopFiveAds
                        {
                            TotalPageCoverage = i.totalPageCoverage,
                            MyBrand = i.brand

                        });
                    }
                    return View(myRecs);

                };
            }
            catch
            {
                return View(myRecs);
            }
           

        }
        

        // GET: AdData/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdData/Create
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

        // GET: AdData/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdData/Edit/5
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

        // GET: AdData/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdData/Delete/5
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
