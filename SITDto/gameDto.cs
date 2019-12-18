using SITDto.function;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace SITDto
{
    public class gameDto
    {
        public int sn { get; set; }
        public string hashSn
        {
            get
            {
                return new encrypt().Encrypt(sn.ToString());
            }
            set
            {
                string strSn = new encrypt().Decrypt(value);
                int s;
                if (int.TryParse(strSn, out s))
                    sn = s;

            }
        }
        public string md5GameSn
        {
            get
            {
                MD5 md5 = MD5.Create();
                byte[] source = Encoding.Default.GetBytes(sn.ToString());
                byte[] crypto = md5.ComputeHash(source);
                return Convert.ToBase64String(crypto);
            }
        }
        public Nullable<int> comSn { get; set; }
        public Nullable<int> userSn { get; set; }
        public string userId { get; set; }

        [Required]
        [DisplayName("競猜開始日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> sdate { get; set; }

        [Required]
        [DisplayName("競猜終止日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> edate { get; set; }

        [Required]
        [DisplayName("競猜主題")]
        public string title { get; set; }

        [DisplayName("競猜備註")]
        public string comment { get; set; }

        public Nullable<byte> gameStatus { get; set; }

        [DisplayName("競猜狀態")]
        public string Status
        {
            get
            {
                switch (gameStatus)
                {
                    case 0:
                        return "開盤";
                    case 1:
                        return "未開盤";
                    case 2:
                        return "封盤";
                    case 3:
                        return "已派彩";
                    case 4:
                        return "已設定結果";
                    default:
                        return "";
                }
            }
        }
        [Display(Name ="遊戲模式")]
        [Required]
        public Nullable<byte> betModel { get; set; }
        public string betModelString
        {
            get
            {
                switch(betModel)
                {
                    case 1:
                        return "精準預測";
                    case 2:
                        return "總彩池競猜";
                    case 5:
                        return "龍的傳人";
                    case 6:
                        return "百倍大串燒";
                    case 7:
                        return "總彩池-走地";
                    case 10:
                        return "電競猜猜";
                    default:
                        return "";
                }
            }
        }

        public string betDetails
        {
            get
            {
                switch (betModel)
                {
                    case 1:
                        return "Details";
                    case 2:
                        return "Details";
                    case 5:
                        return "PrizePoolDetails";
                    case 6:
                        return "NabobDetails";
                    case 7:
                        return "Details";
                    default:
                        return "";
                }
            }
        }

        public string betAdminDetails
        {
            get
            {
                switch (betModel)
                {
                    case 1:
                        return "DetailsAdmin";
                    case 2:
                        return "DetailsAdmin";
                    case 5:
                        return "DPDetailsAdmin";
                    case 6:
                        return "DetailsAdmin";
                    case 7:
                        return "DetailsAdmin";
                    case 10:
                        return "DetailsAdmin";
                    default:
                        return "";
                }
            }
        }

        public string betEditDetails
        {
            get
            {
                switch (betModel)
                {
                    case 1:
                        return "Edit";
                    case 2:
                        return "Edit";
                    case 5:
                        return "DPEdit";
                    case 6:
                        return "Edit";
                    case 7:
                        return "Edit";
                    case 10:
                        return "Edit";
                    default:
                        return "";
                }
            }
        }


        [DisplayName("比賽時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> gamedate { get; set; }

        [DisplayName("比賽地點")]
        public string gameplace { get; set; }
        public Nullable<byte> valid { get; set; }

        [DisplayName("抽成比例(扣5% 輸入5)")]
        public Nullable<double> rake { get; set; }

        [DisplayName("題目清單")]
        public List<topicDto> topicList {get;set;}
        [DisplayName("獎金清單")]
        public List<bonusDto> bonusList { get; set; }

        [DisplayName("可否下注")]
        public bool canbet
        {
            get
            {
                if (_canbet.HasValue)
                    return _canbet.Value;
                bool c = true;
                if (gameStatus != 0)
                    c = false;
                if (DateTime.Now < sdate)
                    c = false;
                if (DateTime.Now > edate)
                    c = false;
                return c;
            }
            set
            {
                _canbet = value;
            }
        }

        private bool? _canbet { get; set; }
        [DisplayName("累積下注金額")]
        public float betmoneyall { get; set; }
        [DisplayName("先知獎勵(%)")]
        public int allocation { get; set; }

        public int live { get; set; }
    }
}
