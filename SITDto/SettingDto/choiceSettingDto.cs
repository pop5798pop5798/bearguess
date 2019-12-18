using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class choiceSettingDto
    {
        public int id { get; set; }
        [DisplayName("選項名")]
        public string choiceName { get; set; }
        [DisplayName("英文選項名")]
        public string eName { get; set; }
        public Nullable<int> cNumberType { get; set; }
        public Nullable<byte> valid { get; set; }
        public Nullable<int> topiceSetting { get; set; }

    }
}


