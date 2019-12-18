﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SITDto.Request
{
    public class SetWinnerReq
    {
        public string UserID { get; set; }
        public int comSn { get; set; }
        public int gameSn { get; set; }
        public int topicSn { get; set; }
        public gameDto game { get; set; }
        public List<choiceDto> choiceList { get; set; }
    }
}
