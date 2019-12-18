using SITDto;
using SITW.Helper;
using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SITW.Models.ViewModel
{
    public class GuessTopicModel
    {
       public string title { get; set; }
       public DateTime createDate { get; set; }
       public int valid { get; set; }
       public List<GuessChoiceModel> gchoice { get; set; }



    }
}