using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SITW.Models.ViewModel;

namespace SITW.Models.ViewModel
{
    public class ProblemViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string comment { get; set; }
        public string image_1 { get; set; }
        public string image_2 { get; set; }
        public string image_3 { get; set; }
        public Nullable<System.DateTime> inpdate { get; set; }
        public Nullable<byte> valid { get; set; }
        public string userId { get; set; }
        public HttpPostedFileBase image1 { get; set; }
        public HttpPostedFileBase image2 { get; set; }
        public HttpPostedFileBase image3 { get; set; }



    }

}

