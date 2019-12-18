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
    public class GuessViewModel
    {
        public guess gs { get; set; }
        public List<GuessTopicModel> gtopics { get; set; }


    }
}