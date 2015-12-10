using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdDataWebApp.Models
{
    public class VmListDataResponse
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<AdDataWebService.Ad> MyData { get; set; }
    }
}