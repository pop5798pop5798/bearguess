using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class betCountDto
    {
        public Nullable<int> betSn { get; set; }
        public Nullable<int> topicSn { get; set; }
        public int unitSn { get; set; }
        public string choiceStr { get; set; }
        public Nullable<int> choiceCount { get; set; }
        public Nullable<int> allcount { get; set; }
    }
}


