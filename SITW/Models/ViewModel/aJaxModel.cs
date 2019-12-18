using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITW.Models.ViewModel
{
    public class aJaxModel
    {
        public bool isTrue { get; set; }
        public int ErrorCode { get; set; }
        private string errmsg { get; set; }
        public string Title { get; set; }
        public string userId { get; set; }
        public string ErrorMsg
        {
            get
            {
                if (!string.IsNullOrEmpty(errmsg))
                    return errmsg;
                string ret = "";
                switch (ErrorCode)
                {
                    case 100:
                        ret = "無權限";
                        break;
                    case 500:
                        ret = "系統錯誤";
                        break;
                }
                return ret;
            }
            set
            {
                errmsg = value;
            }
        }
    }
}
