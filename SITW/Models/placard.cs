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
    
    public partial class placard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public placard()
        {
            this.manager = "\'熊i猜\'";
            this.mail_bear = 0;
        }
    
        public int Id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Nullable<System.DateTime> real_time { get; set; }
        public string manager { get; set; }
        public Nullable<System.DateTime> time { get; set; }
        public Nullable<System.DateTime> endtime { get; set; }
        public string image { get; set; }
        public Nullable<int> p_class { get; set; }
        public Nullable<int> mail_bear { get; set; }
        public Nullable<int> getId { get; set; }
    }
}
