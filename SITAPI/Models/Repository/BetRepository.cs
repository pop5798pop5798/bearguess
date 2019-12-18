using SITDto.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Dapper;
using SITDto.Request;
using SITDto;
using AutoMapper;

namespace SITAPI.Models.Repository
{
    public class BetRepository
    {
        public List<NabobBetListDto> getNabobBetsByUserID(string userID)
        {
            List<NabobBetListDto> BetNewList = new List<NabobBetListDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
                 SELECT g.sn AS gameSn, g.title AS game
                ,g.gameStatus
                ,g.betModel
                ,t.sn AS topicSn,t.title AS topic
                ,c.sn AS choiceSn,c.choiceStr AS choice
                ,b.Odds ,b.money,b.sn as betSn
                ,(CASE pa.isTrue WHEN 1 THEN Round(pa.realMoney,2) when 2 then 0 ELSE b.money*-1 END) realmoney
                ,t.totalmoney as topicMoney,c.totalMoney as choiceMoney
                ,isnull(g.rake,0) as rake
                ,cu.name AS unit
                ,(
				case g.betModel
				WHEN 5
				THEN
				case pa.isTrue 
				WHEN 1 THEN '正解' ELSE '未定' END
				ELSE
				case c.isTrue WHEN 1 THEN '正確' WHEN 0 THEN '錯誤' when 2 then '無效' ELSE '未定' END
					END			
				) AS isTrue
                ,(case c.isTrue WHEN 1 THEN 'true' WHEN 0 THEN 'false' when 2 then 'invalid' ELSE 'unknow' END) AS isTrueValue
                ,u.userID,b.comSn
                ,b.createDate as betDatetime,b.equalchoice
                FROM dbo.bets b
                JOIN dbo.choices c ON c.sn=b.choiceSn
                JOIN dbo.topics t ON t.sn=b.topicSn
                JOIN dbo.games g ON g.sn=t.gameSn
                JOIN dbo.users u ON u.sn=b.userSn
                JOIN dbo.cfgUnit cu ON cu.sn=b.unitSn
                left join dbo.payouts pa on pa.betSn=b.sn
                WHERE u.userID=@userID
                and b.valid in (1,2) and g.betModel = 6
                order by betDatetime desc
";
                using (var multi = cn.QueryMultiple(query, new { userID = userID }))
                {
                    List<BetListDto> BetList = multi.Read<BetListDto>().ToList();   
                    
                    foreach(var blist in BetList)
                    {
                        NabobBetListDto bnls = new NabobBetListDto();
                        bnls.sn = blist.betSn;
                        bnls.betModel = blist.betModel;
                        bnls.comSn = blist.comSn;
                        bnls.game = blist.game;
                        bnls.gameSn = blist.gameSn;
                        bnls.gamepostsn = blist.gamepostsn;
                        bnls.gameStatus = blist.gameStatus;
                        bnls.money = blist.money;
                        bnls.realmoney = blist.realmoney;
                        bnls.betDatetime = blist.betDatetime;
                        var topiclist = BetList.Where(x => x.equalchoice == blist.betSn).ToList();
                        List<NabobtopicDto> nabobList = new List<NabobtopicDto>();

                        foreach(var topic in topiclist)
                        {
                            NabobtopicDto topdto = new NabobtopicDto();
                            topdto.topic = topic.topic;
                            topdto.choiceSn = topic.choiceSn;
                            topdto.choice = topic.choice;
                            topdto.isTrue = topic.isTrue;
                            topdto.isTrueValue = topic.isTrueValue;
                            topdto.unit = topic.unit;
                            nabobList.Add(topdto);

                        }

                            
                        if(nabobList.Count != 0)
                        {
                            bnls.topic = nabobList;
                            BetNewList.Add(bnls);
                        }


                    }

                }
            }


            return BetNewList;
        }


            public List<BetListDto> getBetsByUserID(string userID)
        {

            List<BetListDto> BetList = new List<BetListDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
                 SELECT g.sn AS gameSn, g.title AS game
                ,g.gameStatus
                ,g.betModel
                ,t.sn AS topicSn,t.title AS topic
                ,c.sn AS choiceSn,c.choiceStr AS choice
                ,b.Odds ,b.money,b.sn as betSn
                ,(CASE pa.isTrue WHEN 1 THEN Round(pa.realMoney,2) when 2 then 0 ELSE b.money*-1 END) realmoney
                ,t.totalmoney as topicMoney,c.totalMoney as choiceMoney
                ,isnull(g.rake,0) as rake
                ,cu.name AS unit
                ,(
				case g.betModel
				WHEN 5
				THEN
				case pa.isTrue 
				WHEN 1 THEN '正解' ELSE '未定' END
				ELSE
				case c.isTrue WHEN 1 THEN '勝' WHEN 0 THEN '負' when 2 then '無效' ELSE '未定' END
					END			
				) AS isTrue
                ,(case c.isTrue WHEN 1 THEN 'true' WHEN 0 THEN 'false' when 2 then 'invalid' ELSE 'unknow' END) AS isTrueValue
                ,u.userID,b.comSn,g.allocation
                ,b.createDate as betDatetime,t.walk,g.live
                FROM dbo.bets b
                JOIN dbo.choices c ON c.sn=b.choiceSn
                JOIN dbo.topics t ON t.sn=b.topicSn
                JOIN dbo.games g ON g.sn=t.gameSn
                JOIN dbo.users u ON u.sn=b.userSn
                JOIN dbo.cfgUnit cu ON cu.sn=b.unitSn
                left join dbo.payouts pa on pa.betSn=b.sn
                WHERE u.userID=@userID
                and b.valid in (1,2)
                order by betDatetime desc

SELECT * 
FROM dbo.betCounts

SELECT * 
FROM dbo.bets
where bets.totalmoney IS NOT NULL 


";
                using (var multi = cn.QueryMultiple(query, new { userID = userID }))
                {
                    BetList = multi.Read<BetListDto>().ToList();
                    var betcount = multi.Read<betCountDto>().ToList();
                    var BetAll = multi.Read<betDto>().ToList();
                    foreach (var bet in BetList)
                    {
                        List<betCountDto> bto = new List<betCountDto>();
                        foreach (betCountDto count in betcount)
                        {
                            
                            if (bet.betModel == 5 && count.betSn == bet.betSn && count.choiceCount != 0)
                            {
                                // betCountDto betdto = new betCountDto();
                                betCountDto betmodel = new betCountDto();
                                //betCount betcountdto = Mapper.Map<betCount>(count);
                                betmodel.betSn = count.betSn;
                                betmodel.choiceCount = count.choiceCount;
                                betmodel.choiceStr = new ChoiceRepository().getdragon(count.choiceStr); 
                                betmodel.unitSn = count.unitSn;

                                bto.Add(betmodel);

                            }
                            

                        }

                        if(bet.walk == 1)
                        {
                            bet.totalmoney = 0;
                            foreach (var bl in BetAll)
                            {
                                if(bet.betSn <= bl.sn && bet.topicSn == bl.topicSn)
                                {
                                    
                                    bet.totalmoney += ((float)bl.money * (100 - bet.allocation) / 100) / (float)bl.totalmoney * (float)bet.money;
                                }
                            }
                            bet.totalmoney = (float)Math.Round((Decimal)bet.totalmoney, 2, MidpointRounding.AwayFromZero);


                        }

                        bet.betcountlist = bto;
                    }
                }
            }
           return BetList;


        }

        public List<BetListDto> getLiveBetsByUserID(string userID)
        {

            List<BetListDto> BetList = new List<BetListDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
                 SELECT g.sn AS gameSn, g.title AS game
                ,g.gameStatus
                ,g.betModel
                ,t.sn AS topicSn,t.title AS topic
                ,c.sn AS choiceSn,c.choiceStr AS choice
                ,b.Odds ,b.money,b.sn as betSn
                ,(CASE pa.isTrue WHEN 1 THEN Round(pa.realMoney,2) when 2 then 0 ELSE b.money*-1 END) realmoney
                ,t.totalmoney as topicMoney,c.totalMoney as choiceMoney
                ,isnull(g.rake,0) as rake
                ,cu.name AS unit
                ,(
				case g.betModel
				WHEN 5
				THEN
				case pa.isTrue 
				WHEN 1 THEN '正解' ELSE '未定' END
				ELSE
				case c.isTrue WHEN 1 THEN '勝' WHEN 0 THEN '負' when 2 then '無效' ELSE '未定' END
					END			
				) AS isTrue
                ,(case c.isTrue WHEN 1 THEN 'true' WHEN 0 THEN 'false' when 2 then 'invalid' ELSE 'unknow' END) AS isTrueValue
                ,u.name as userID,b.comSn,g.allocation
                ,b.createDate as betDatetime,t.walk
                FROM dbo.bets b
                JOIN dbo.choices c ON c.sn=b.choiceSn
                JOIN dbo.topics t ON t.sn=b.topicSn
                JOIN dbo.games g ON g.sn=t.gameSn
                left join users u on u.sn = b.userSn
				right join users ug on ug.sn = g.userSn
                JOIN dbo.cfgUnit cu ON cu.sn=b.unitSn
                left join dbo.payouts pa on pa.betSn=b.sn
                WHERE ug.userID=@userID
                and b.valid in (1,2)
                order by betDatetime desc

SELECT * 
FROM dbo.betCounts

SELECT * 
FROM dbo.bets
where bets.totalmoney IS NOT NULL 


";
                using (var multi = cn.QueryMultiple(query, new { userID = userID }))
                {
                    BetList = multi.Read<BetListDto>().ToList();
                    var betcount = multi.Read<betCountDto>().ToList();
                    var BetAll = multi.Read<betDto>().ToList();
                    foreach (var bet in BetList)
                    {
                        List<betCountDto> bto = new List<betCountDto>();
                        foreach (betCountDto count in betcount)
                        {

                            if (bet.betModel == 5 && count.betSn == bet.betSn && count.choiceCount != 0)
                            {
                                // betCountDto betdto = new betCountDto();
                                betCountDto betmodel = new betCountDto();
                                //betCount betcountdto = Mapper.Map<betCount>(count);
                                betmodel.betSn = count.betSn;
                                betmodel.choiceCount = count.choiceCount;
                                betmodel.choiceStr = new ChoiceRepository().getdragon(count.choiceStr);
                                betmodel.unitSn = count.unitSn;

                                bto.Add(betmodel);

                            }


                        }

                        if (bet.walk == 1)
                        {
                            bet.totalmoney = 0;
                            foreach (var bl in BetAll)
                            {
                                if (bet.betSn <= bl.sn && bet.topicSn == bl.topicSn)
                                {

                                    bet.totalmoney += ((float)bl.money * (100 - bet.allocation) / 100) / (float)bl.totalmoney * (float)bet.money;
                                }
                            }
                            bet.totalmoney = (float)Math.Round((Decimal)bet.totalmoney, 2, MidpointRounding.AwayFromZero);


                        }

                        bet.betcountlist = bto;
                    }
                }
            }
            return BetList;


        }


        internal List<BetListDto> getBetsByGame(BetsByGameReq bbgr)
        {
            List<BetListDto> BetList = new List<BetListDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
                SELECT g.sn AS gameSn, g.title AS game
                ,g.betModel
                ,t.sn AS topicSn,t.title AS topic
                ,c.sn AS choiceSn,c.choiceStr AS choice
                ,b.Odds ,b.money,b.sn as betSn
                ,(CASE c.isTrue WHEN 1 THEN Round(pa.realMoney,2) when 2 then 0 ELSE b.money*-1 END) realmoney
                ,t.totalmoney as topicMoney,c.totalMoney as choiceMoney
                ,isnull(g.rake,0) as rake
                ,cu.name AS unit
                ,(case c.isTrue WHEN 1 THEN '勝' WHEN 0 THEN '負' when 2 then '無效' ELSE '未定' END) AS isTrue
                ,(case c.isTrue WHEN 1 THEN 'true' WHEN 0 THEN 'false' when 2 then 'invalid' ELSE 'unknow' END) AS isTrueValue
                ,u.userID,b.comSn
                ,b.createDate as betDatetime,b.equalchoice
                FROM dbo.bets b
                JOIN dbo.choices c ON c.sn=b.choiceSn
                JOIN dbo.topics t ON t.sn=b.topicSn
                JOIN dbo.games g ON g.sn=t.gameSn
                JOIN dbo.users u ON u.sn=b.userSn
                JOIN dbo.cfgUnit cu ON cu.sn=b.unitSn
                JOIN dbo.users uAdmin ON uAdmin.sn=g.userSn
                left join dbo.payouts pa on pa.betSn=b.sn
                WHERE g.sn=@GameSn
                and b.valid in (1,2)
";
                using (var multi = cn.QueryMultiple(query, new { GameSn= bbgr.GameSn }))
                {
                    BetList = multi.Read<BetListDto>().ToList();                    
                }
            }
            return BetList;
        }


     
    }
}