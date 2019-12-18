using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto
{
    public class bonusSettingDto
    {
        public Nullable<int> sn { get; set; }
        public Nullable<int> settingsn { get; set; }
        [DisplayName("龍擊殺數")]
        public Nullable<int> Quantity { get; set; }
        [DisplayName("是否擊殺大龍")]
        public bool bonus { get; set; }
        [DisplayName("獎勵(倍)")]
        public Nullable<int> BonusRatio { get; set; }
        [DisplayName("是否為獎池")]
        public bool pool { get; set; }
    }
}


