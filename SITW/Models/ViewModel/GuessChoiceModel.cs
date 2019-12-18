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
    public class GuessChoiceModel
    {
        public string choiceStr { get; set; }
        public Nullable<byte> isTrue { get; set; }

    }
}