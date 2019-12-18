using SITDto;
using SITW.Helper;
using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SITW.Models.ViewModel
{

    public class GameSyncViewModel
    {
        public GameSyncViewModel(gameDto gd, string encryptedKey)
        {
            this.sn = gd.sn;
            this.title = gd.title;
            this.comment = gd.comment;
            this.edate = gd.edate;
            this.canbet = gd.canbet;
            this.encryptedKey = encryptedKey;
            this.gamedate = gd.gamedate;
            this.gameplace = gd.gameplace;
            this.topicList = new List<TopicSyncViewModel>();
            foreach (topicDto td in gd.topicList)
            {
                this.topicList.Add(new TopicSyncViewModel(td,encryptedKey,gd));
            }
        }

        public string md5GameSn
        {
            get
            {
                if (string.IsNullOrEmpty(_md5GameSn))
                {
                    MD5 md5 = MD5.Create();
                    byte[] source = Encoding.Default.GetBytes(sn.ToString());
                    byte[] crypto = md5.ComputeHash(source);
                    string smd5 = Convert.ToBase64String(crypto);
                    string pattern = @"([^0-9a-zA-Z])+";
                    Regex rgx = new Regex(pattern);
                    smd5 = rgx.Replace(smd5, "");
                    return smd5;
                }
                else
                {
                    return _md5GameSn;
                }
            }
            set
            {
                _md5GameSn = value;
            }
        }

        private string _md5GameSn { get; set; }
        private int sn { get; set; }
        public bool canbet { get; set; }
        public string title { get; set; }
       
        public string comment { get; set; }

        public Nullable<DateTime> gamedate { get; set; }
        public string gameplace { get; set; }
        public DateTime? edate { get; set; }
        public string encryptedKey { private get; set; }

        public List<TopicSyncViewModel> topicList { get; set; }
    }

    public class TopicSyncViewModel
    {
        public TopicSyncViewModel(topicDto td, string encryptedKey,gameDto gd)
        {
            this.sn = td.sn;
            this.title = td.title;
            this.bigtitle = gd.title;
            this.comment = td.comment;
            this.edate = td.edate;
            this.canbet = (td.walk == 1 && gd.gameStatus == 0) ? true : td.canbet;
            this.encryptedKey = encryptedKey;                   
            this.poolall = td.poolall;
            this.promote = td.promote;
            this.outlay = td.outlay;
            this.image = td.image;
            this.hoverImage = td.hoverImage;
            this.walk = td.walk;
            this.valid = td.valid;
            this.choiceList = new List<ChoiceViewModel>();
            this.betcountlist = new List<BetCountViewModel>();
            this.betallcountlist = new List<BetAllCountViewModel>();
            this.live = gd.live;
            this.betmodel = gd.betModel;
            
            //龍的傳人才新增
            if(gd.betModel == 5)
            {
                string[] dgon = { "火龍", "地龍", "風龍", "水龍", "大龍" };
                for (var i = 0; i < dgon.Length; i++)
                {
                    this.betcountlist.Add(new BetCountViewModel(dgon[i], i));
                }

                foreach (BetCountViewModel bvl in this.betcountlist)
                {
                    foreach (betCountDto bed in td.betcountList)
                    {
                        if (bvl.dgonname == bed.choiceStr)
                        {
                            int cct = (int)bed.choiceCount;
                            bvl.donly[cct].dgonRate += bed.allcount;
                        }
                    }
                }
            }

                foreach (choiceDto cd in td.choiceList)
                {
                    this.choiceList.Add(new ChoiceViewModel(cd, this.encryptedKey));
                }
                foreach (ChoiceViewModel d in this.choiceList)
                {
                    if (gd.betModel == 5)
                    {
                        d.betball += Math.Round(td.poolall / 150000, 2);
                    }
                    d.betMoneyRate = Math.Round((d.betMoney * 100 / this.choiceList.Sum(p => p.betMoney)) - 0.005, 2);
                }

            
            
        }

        public string md5TopicSn
        {
            get
            {
                if (string.IsNullOrEmpty(_md5TopicSn))
                {
                    MD5 md5 = MD5.Create();
                    byte[] source = Encoding.Default.GetBytes(sn.ToString());
                    byte[] crypto = md5.ComputeHash(source);
                    string smd5 = Convert.ToBase64String(crypto);
                    string pattern = @"([^0-9a-zA-Z])+";
                    Regex rgx = new Regex(pattern);
                    smd5 = rgx.Replace(smd5, "");
                    return smd5;
                }
                else
                {
                    return _md5TopicSn;
                }
            }
            set
            {
                _md5TopicSn = value;
            }
        }

        private string _md5TopicSn { get; set; }
        private int sn { get; set; }
        public bool canbet { get; set; }
        public double? Odds { get; set; }
        public string title { get; set; }
        public string bigtitle { get; set; }
        public string comment { get; set; }
        public DateTime? edate { get; set; }
        public string encryptedKey { private get; set; }
        public float poolall { get; set; }
        public int? promote { get; set; }
        public double? outlay { get; set; }
        public string image { get; set; }
        public string hoverImage { get; set; }
        public int? walk { get; set; }
        public byte? valid { get; set; }
        public int live { get; set; }
        public byte? betmodel { get; set; }
        public List<ChoiceViewModel> choiceList { get; set; }
        public List<BetCountViewModel> betcountlist { get; set; }
        public List<BetAllCountViewModel> betallcountlist { get; set; }
    }

    public class BetAllCountViewModel {

        public Nullable<int> betSn { get; set; }
        public Nullable<int> topicSn { get; set; }
        public int unitSn { get; set; }
        public string choiceStr { get; set; }
        public Nullable<int> choiceCount { get; set; }
        public Nullable<int> allcount { get; set; }
    }


    public class BetCountViewModel
    {
        public BetCountViewModel(string bcd,int bcdid)
        {
            this.dgonname = bcd;
            this.dgonsn = bcdid;
            this.donly = new List<betcountonly>();
            for (int i = 0;i<5;i++)
            {
                betcountonly beonly = new betcountonly(null);
                beonly.dgonnumber = i + 1;
                beonly.dgonRate = 0;
                this.donly.Add(new betcountonly(beonly));

            }
            


        }

        public string dgonname { get; set; }
        public int dgonsn { get; set; }
        public List<betcountonly> donly { get; set; }



    }

    public class betcountonly
    {
        public betcountonly(betcountonly by)
        {
            if (by != null)
            {
                this.dgonnumber = by.dgonnumber;
                this.dgonRate = by.dgonRate;
            }
            else {
                this.dgonnumber = 0;
                this.dgonRate = 0;
            }
            
        }

        public int? dgonnumber { get; set; }
        public int? dgonRate { get; set; }
    }



    public class ChoiceViewModel
    {
        public ChoiceViewModel(choiceDto cd,string encryptedKey)
        {
            this.choiceSn = cd.sn;
            this.choiceStr = cd.choiceStr;
            this.comment = cd.comment;
            this.betMoney = cd.betMoneygti;
            this.betball = Math.Round(this.betMoney / 10000, 2);
            this.Odds = cd.Odds;
            this.betModel = cd.betModel;
            this.dragonshort = cd.dragonshort;          
            this.encryptedKey = encryptedKey;
            var teams = new TeamsRepository().getImg(cd.choiceStr);
            if(teams != null)
            {
                this.teamimg = teams.imageURL;
            }
            
            if (cd.betMoney != null)
            {
                this.betMoneycount = cd.betMoney.Count();
            }
            else {
                this.betMoneycount = 0;
            }
            
        }

        public string EchoiceSn
        {
            get
            {
                if (!string.IsNullOrEmpty(_encryptedKey))
                {
                    Encryption oEncrypt = new Encryption();
                    _encryptedChoiceSn = oEncrypt.EncryptString(_encryptedKey, _choiceSn.ToString());
                }
                return _encryptedChoiceSn;
            }
            set
            {
                _encryptedChoiceSn = value;
            }
        }

        private string _encryptedChoiceSn { get; set; }
        public string encryptedKey
        {
            set
            {
                _encryptedKey = value;
            }
        }
        private string _encryptedKey { get; set; }

        public int choiceSn
        {
            set
            {
                _choiceSn = value;
            }
        }
        private int _choiceSn { get; set; }
        public string md5ChoiceSn
        {
            get
            {
                if (string.IsNullOrEmpty(_md5ChoiceSn))
                {
                    MD5 md5 = MD5.Create();
                    byte[] source = Encoding.Default.GetBytes(_choiceSn.ToString());
                    byte[] crypto = md5.ComputeHash(source);
                    string smd5 = Convert.ToBase64String(crypto);
                    string pattern = @"([^0-9a-zA-Z])+";
                    Regex rgx = new Regex(pattern);
                    smd5 = rgx.Replace(smd5, "");
                    return smd5;
                }
                else
                {
                    return _md5ChoiceSn;
                }
            }
            set
            {
                _md5ChoiceSn = value;
            }
        }

        private string _md5ChoiceSn { get; set; }

        public string choiceStr { get; set; }
        public string comment { get; set; }
        public Nullable<double> Odds { get; set; }

        public double betMoney { get; set; }
        public double betball { get; set; }
        public int betModel { get; set; }
        public double betMoneyRate { get; set; }
        public string dragonshort { get; set; }
        public string teamimg { get; set; }
        public double betMoneycount { get; set; }

    }

    public class betViewModel
    {
        public List<betDetail> betList { get; set; }
    }

    public class betDetail
    {
        public string EchoiceSn
        {
            set
            {
                _encryptedChoiceSn = value;
            }
            get
            {
                return _encryptedChoiceSn;
            }
        }
        private string _encryptedChoiceSn { get; set; }

        public string encryptedKey
        {
            set
            {
                _encryptedKey = value;
            }
        }
        private string _encryptedKey { get; set; }
        private int _choiceSn { get; set; }
        public int choiceSn { get; set; }

        /*

        public int choiceSn
        {
            get
            {
                Encryption oEncrypt = new Encryption();
                if (!string.IsNullOrEmpty(_encryptedKey))
                {
                    string schoiceSn = oEncrypt.DecryptString(EncryptionProvider.Rijndael, _encryptedKey, _encryptedChoiceSn);
                    int ichoiceSn = 0;
                    if (int.TryParse(schoiceSn, out ichoiceSn))
                        _choiceSn = ichoiceSn;
                }
                return _choiceSn;
            }
        }
        */
        /*^\+?[1-9]*0*0$|^$|^0$*/
        [RegularExpression(@"^\+?[1-9][0-9]*0$|^$|^0$", ErrorMessage ="預測金額請以10為單位(例：880)")]
        /*[RegularExpression(@"^\+?[0-9]{1,10}[0][0]$|^$|^0$", ErrorMessage = "下注金額請以100為單位(例：800)")]*/
        public Nullable<double> money { get; set; }

        public Nullable<int> count { get; set; }
        public string strsn { get; set; }

        public int getChoiceSn(string encryptedKey)
        {
            this.encryptedKey = encryptedKey;
            Encryption oEncrypt = new Encryption();
            if (!string.IsNullOrEmpty(_encryptedKey))
            {
                string schoiceSn = oEncrypt.DecryptString(_encryptedKey, _encryptedChoiceSn);
                int ichoiceSn = 0;
                if (int.TryParse(schoiceSn, out ichoiceSn))
                    _choiceSn = ichoiceSn;
            }
            return _choiceSn;


        }

    }

}