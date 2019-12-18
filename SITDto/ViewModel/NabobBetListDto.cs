using SITDto.function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto.ViewModel
{
    public class NabobBetListDto
    {
        public int sn { get; set; }
        public int gamepostsn { get; set; }
        public int gameSn { get; set; }
        public string game { get; set; }
        public byte betModel { get; set; }       
        public List<NabobtopicDto> topic { get; set; }
        public double money { get; set; }
        public double realmoney { get; set; }
        public string userID { get; set; }
        public int comSn { get; set; }
        public int gameStatus { get; set; }
        public DateTime betDatetime { get; set; }



    }
}
