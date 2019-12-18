using SITDto.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SITW.Models.ViewModel
{
    public class ProductRecordModel
    {
        public string change_name { get; set; }
        public string change_email { get; set; }
        public string change_address { get; set; }
        public string change_phone { get; set; }
        public string change_recipient { get; set; }
        public Product produc { get; set; }
        public ProductRecord productRecord { get; set; }
        public float pay { get; set; }

       
    }
}