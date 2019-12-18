using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class choiceStrDto
    {
        public int sn { get; set; }
        public int choiceSn { get; set; }
        [DisplayName("幣別")]
        public int unitSn { get; set; }
        [DisplayName("題目選項")]
        public string choiceStr { get; set; }
        public string eChoiceStr { get; set; }
        public int truecount{ get; set; }
    }
}


