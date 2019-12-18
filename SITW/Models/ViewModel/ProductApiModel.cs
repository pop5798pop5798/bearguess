using SITDto.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SITW.Models.ViewModel
{
    public class ProductApiModel
    {
        public cfgUnit unit { get; set; }
        public Product product { get; set; }
        public List<Product> producList { get; set; }
        public ProductRecord productRecord { get; set; }
        public int firstType { get; set; }
        public List<int> firstTypeArray { get; set; }
        public Preferential preferential {get;set;}

    }

    public class PProductViewModel
    {
        public int id { get; set; }
        public string ProductName { get; set; }
        public Nullable<double> Price { get; set; }
        public string ProductContent { get; set; }
        public Nullable<int> unitSn { get; set; }
        public Nullable<int> type { get; set; }
        public Nullable<byte> valid { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> inpdate { get; set; }
        public string image { get; set; }
        public Nullable<double> transform { get; set; }
        public string Pname{ get; set; }
        public Nullable<double> offer { get; set; }



    }
}