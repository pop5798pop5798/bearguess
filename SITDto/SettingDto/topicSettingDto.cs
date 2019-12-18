using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SITDto
{
    public class topicSettingDto
    {
        [DisplayName("序號")]
        public int id { get; set; }
        [Required(ErrorMessage = "請輸入題目名")]
        [DisplayName("題目名")]
        public string topicsName { get; set; }
        [DisplayName("英文題目名")]
        public string enName { get; set; }
        [Required(ErrorMessage = "請輸入選項類型")]
        [DisplayName("選項類型")]
        public Nullable<bool> choiceType { get; set; }
        [DisplayName("選項類型")]
        public string choiceTypeStr {
            get
            {
                switch (choiceType)
                {
                    case false:
                        return "隊伍選項";
                    case true:
                        return "自定義";                
                    default:
                        return "";
                }
            }

        }
        [DisplayName("題目啟用")]
        public Nullable<byte> valid { get; set; }

        public Nullable<int> gametype { get; set; }
        public List<choiceSettingDto> choicsettingList { get; set; }
        public string image { get; set; }
        public string hoverImage { get; set; }
        public Nullable<int> Model { get; set; }

        public Nullable<int> autotype { get; set; }
        public Nullable<int> numberType { get; set; }
    }
}


