//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SITW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Missions
    {
        public int sn { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public System.DateTime sdate { get; set; }
        public System.DateTime edate { get; set; }
        public int valid { get; set; }
        public int cycle { get; set; }
        public string imgURL { get; set; }
    }
}
