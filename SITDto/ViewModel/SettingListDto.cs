using SITDto.function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto.ViewModel
{
    public class SettingListDto
    {
        //初始設定
        public List<bonusSettingDto> bonussettingList { get; set; }
        public List<gameSettingDto> gamesettingList { get; set; }
        public List<topicSettingDto> topicsettingList { get; set; }
        
    }
}
