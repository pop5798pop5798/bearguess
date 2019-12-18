using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class nabobpayoutDto
    {
        public int comSn { get; set; }
        public int gameSn { get; set; }
        public int topicSn { get; set; }
        public int choiceSn { get; set; }
        public int userSn { get; set; }
        public int betSn { get; set; }
        public float Odds { get; set; }
        public float money { get; set; }
        public int unitSn { get; set; }
        public float realMoney { get; set; }
        public float? topicTotalMoney { get; set; }
        public float? correctTotalMoney { get; set; }
        public byte? isTrue { get; set; }
        public float? rake { get; set; }
        public int trueCount { get; set; }
        public string choiceStr { get; set; }
        public int outlay { get; set; }
        public int promote { get; set; }
        public int betnumber { get; set; }
        public int equalchoice { get; set; }
    }
}
