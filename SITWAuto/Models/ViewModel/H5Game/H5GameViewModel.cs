using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SITW.Models.ViewModel;

namespace SITW.Models.ViewModel
{
    public class H5GameViewModel
    {

        public GameNumberRecord gamenumberRecords { get; set; }
        public GameBets gameBets { get; set; }

    }
    public class H5Bets
    {

        public PlayerNumber playnumber { get; set; }
        public GameBets gameBets { get; set; }

    }

    public class H5LottoBets
    {
        public List<PlayerNumber> playnumber { get; set; }
        public GameBets gameBets { get; set; }

    }
    public class LottoGameVewModel
    {

        public List<GameNumberRecord> gamenumberRecords { get; set; }
        public List<Lottobets> Bets { get; set; }
        public H5Games h5game { get; set; }

        public List<double> accumulation { get; set; }

        



    }

    public class SlotVewModel
    {

        public double usermoney { get; set; }
        public int loginUser { get; set; }
        public cfgSlotCash slotm { get; set; }

    }




    public class AkGameVewModel
    {

        public List<GameNumberRecord> gamenumberRecords { get; set; }
        public List<bets> Bets { get; set; }
        public H5Games h5game { get; set; }
        public string Brand
        {
            get
            {
                if (gamenumberRecords != null)
                {
                    switch (gamenumberRecords.FirstOrDefault().number)
                    {
                        case 1:
                            return "A";
                        case 11:
                            return "J";
                        case 12:
                            return "Q";
                        case 13:
                            return "K";
                        default:
                            return gamenumberRecords.FirstOrDefault().number.ToString();
                    }
                }
                else {
                    return null;
                }
                
            }
        }

    }
    public class bets
    {
        public GameBets gameBets { get; set; }
        public Nullable<int> bn { get; set; }
        public string betBrand
        {
            get
            {
                if (bn != null)
                {
                    switch (bn)
                    {
                        case 0:
                            return "暫無";
                        case 1:
                            return "A";
                        case 11:
                            return "J";
                        case 12:
                            return "Q";
                        case 13:
                            return "K";
                        default:
                            return bn.ToString();
                    }
                }
                else
                {
                    return null;
                }

            }
        }
        public Nullable<int> istrue { get; set; }
        public Nullable<int> tureNuber { get; set; }
        public Nullable<double> readMoney {get;set;}
        public string Brand
        {
            get
            {
                if (tureNuber != null)
                {
                    switch (tureNuber)
                    {
                        case 0:
                            return "暫無";
                        case 1:
                            return "SquareA.png";
                        case 11:
                            return "SquareJ.png";
                        case 12:
                            return "SquareQ.png";
                        case 13:
                            return "SquareK.png";
                        default:
                            return "Square"+tureNuber.ToString() + ".png";
                    }
                }
                else
                {
                    return null;
                }

            }
        }

    }


    public class Lottobets
    {
        public GameBets gameBets { get; set; }
        public List<int> bn { get; set; }

        public Nullable<int> istrue { get; set; }
        public List<int> tureNuber { get; set; }
        public string trueLotto
        {
            get
            {
                if (tureNuber != null)
                {
                    return tureNuber.ToString();                                            
                     
                }
                else
                {
                    return "暫無";
                }

            }
        }
        public Nullable<double> readMoney { get; set; }       

    }

    public class BallGameModel
    {
        public GameBets gamebets { get; set; }
        public int count { get; set; }
    }
    public class GNRList
    {
        public int id { get; set; }
        public Nullable<int> gameSn { get; set; }
        public Nullable<int> number { get; set; }
        public Nullable<int> gameGroup { get; set; }
        public Nullable<System.DateTime> inpdate { get; set; }

    }

}