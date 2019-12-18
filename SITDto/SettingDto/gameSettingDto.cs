using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class gameSettingDto
    {
        public Nullable<int> sn { get; set; }
        [DisplayName("下注金額")]
        public Nullable<int> betnumber{ get; set; }
        [DisplayName("主題")]
        public string title { get; set; }
        [DisplayName("可超過額度")]
        public Nullable<int> promote { get; set; }
        [DisplayName("手續費(%)")]
        public Nullable<float> outlay { get; set; }
        [DisplayName("先知獎勵(%)")]
        public Nullable<int> allocation { get; set; }

    }
}


