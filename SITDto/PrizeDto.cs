using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class PrizeDto
    {
        public int sn { get; set; }
        public int userSn { get; set; }
        public int unitSn { get; set; }
        public Nullable<int> gameSn { get; set; }
        public Nullable<int> topicSn { get; set; }
        public Nullable<int> choiceSn { get; set; }
        public float assets { get; set; }
        public int type { get; set; }
        public System.DateTime inpdate { get; set; }
        public int promote { get; set; }
        public float poolall { get; set; }
        public float outlayall { get; set; }
    }
}


