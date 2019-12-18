using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SITW.Models
{
    [XmlRoot(ElementName = "SmilePayEinvoice")]
    public class SmilePayEinvoice
    {
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "Desc")]
        public string Desc { get; set; }
        [XmlElement(ElementName = "Grvc")]
        public string Grvc { get; set; }
        [XmlElement(ElementName = "orderno")]
        public string Orderno { get; set; }
        [XmlElement(ElementName = "data_id")]
        public string Data_id { get; set; }
        [XmlElement(ElementName = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }
        [XmlElement(ElementName = "RandomNumber")]
        public string RandomNumber { get; set; }
        [XmlElement(ElementName = "InvoiceDate")]
        public string InvoiceDate { get; set; }
        [XmlElement(ElementName = "InvoiceTime")]
        public string InvoiceTime { get; set; }
        [XmlElement(ElementName = "CarrierID")]
        public string CarrierID { get; set; }
    }


}
