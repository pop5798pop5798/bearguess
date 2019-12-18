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

    }
}