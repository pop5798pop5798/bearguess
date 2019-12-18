using AutoMapper;
using Dapper;
using SITDto;
using SITDto.ViewModel;
using SITW.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SITAPI.Models.Repository
{
    public class GameRepository
    {
        private sitdbEntities db = new sitdbEntities();

        public List<gameDto> GetGame()
        {
            List<gameDto> gamedtoList = new List<gameDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT g.*,u.userID
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
where g.valid=1
";
                using (var multi = cn.QueryMultiple(query, new { }))
                {
                    IEnumerable<gameDto> gtolist = multi.Read<gameDto>();
                    if (gtolist.Count() > 0)
                    {
                        gamedtoList = gtolist.ToList();
                        
                    }
                }
            }
            return gamedtoList;

        }

        public List<gameDto> GetGameList()
        {
            List<gameDto> gamedtoList = new List<gameDto>();
            

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
DECLARE @SUM Float
DECLARE @PoolSUM Float
DECLARE @OutlaySUM float
SELECT @SUM=
isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=4)
,0)
SELECT @PoolSUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=3)
,0)

SELECT @OutlaySUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=2)
,0)

SELECT g.*,u.userID,isnull((
select (sum(ppr.assets)-isnull(@SUM,0)) as betMoneyall
	from  prizepoolRecord ppr
	where ppr.type=2

),null)as betmoneyall
into #games
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
where g.valid=1

select *
from #games

SELECT t.*,isnull((
CASE WHEN @OutlaySUM < t.promote THEN
(select (sum(ppr.assets)-isnull(@PoolSUM,0)) as poolall
	from  prizepoolRecord ppr
	where ppr.type=1
)
ELSE
(
select @OutlaySUM -isnull(@SUM,0) as poolall
where g.betModel = 5
)
END),null)as poolall
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
and t.valid=1 or t.valid=3

SELECT c.sn,c.comSn,c.topicSn,
isnull((
	select gd.name
	from gamedragon gd
	where g.betModel = 5 and cs.choiceStr1 = gd.sn
),c.choiceStr) as choiceStr
,c.comment,c.Odds,c.isTrue,c.totalMoney,c.createDate,c.modiDate,c.bearSn
,isnull((
	select sum(b.money) as betMoneygti
	from bets b
	where b.choiceSn=c.sn
),0) as betMoneygti,g.betModel as betModel,
isnull((
select gd.en_short
from gamedragon gd
where g.betModel = 5 and cs.choiceStr1 = gd.sn),null
) as dragonshort,
isnull(cs.trueCount,null) as trueCount,isnull(cs.choiceStr1,null) as chstr,
c.cNumberType,c.eChoiceStr


FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
JOIN dbo.choices c ON
	c.topicSn=t.sn
FULL OUTER JOIN dbo.choiceStrs cs ON
	c.sn=cs.choiceSn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
select c.sn as choiceSn,bu.value as unitSn, isnull(sum(b.money),0) as betMoney
,isnull(c.Odds,0) as Odds
from #games g
CROSS APPLY string_split(betUnit,',') bu
left join topics t on t.gameSn=g.sn
left join choices c on c.topicSn=t.sn
left join bets b on b.choiceSn=c.sn and b.unitSn=bu.value
left join choiceOdds co on co.choiceSn=c.sn and b.unitSn=bu.value
group by c.sn,bu.value,c.Odds 
order by c.sn,unitSn

select bs.*
from #games g
join bonus bs on bs.gamesn = g.sn

select cstr.*
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join choiceStrs cstr on cstr.choiceSn=c.sn
order by cstr.sn

select bcs.betSn,t.sn as topicSn,bcs.unitSn,gdn.name as choiceStr,bcs.choiceCount,isnull(count(choiceCount),0) as allcount
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join bets bt on bt.choiceSn=c.sn
join betCounts bcs on bcs.betSn=bt.sn
join gamedragon gdn on gdn.sn = bcs.choiceStr
group by bcs.unitSn,bcs.betSn,t.sn,gdn.name,bcs.unitSn,bcs.choiceCount
order by t.sn
";
                using (var multi = cn.QueryMultiple(query))
                {
                    gamedtoList = multi.Read<gameDto>().ToList();
                    var topic = multi.Read<topicDto>();
                    var choice = multi.Read<choiceDto>();
                    var choiceBet = multi.Read<choiceBetMoneyDto>();
                    var bonus = multi.Read<bonusDto>();
                    var choicestr = multi.Read<choiceStrDto>();
                    var betcount = multi.Read<betCountDto>();
                    foreach (gameDto gamedto in gamedtoList)
                    {
                        gamedto.topicList = topic.Where(p => p.gameSn == gamedto.sn).ToList();
                        gamedto.bonusList = bonus.Where(x => x.gamesn == gamedto.sn).OrderBy(x=> x.sn).ToList();
                        foreach (var t in gamedto.topicList)
                        {
                            if (!gamedto.canbet)
                                t.canbet = false;
                            t.choiceList = choice.Where(p => p.topicSn == t.sn).ToList();
                            t.betcountList = betcount.Where(p => p.topicSn == t.sn).ToList();
                            foreach (var c in t.choiceList)
                            {
                                c.betMoney = choiceBet.Where(p => p.choiceSn == c.sn).ToList();
                                if (gamedto.betModel == 2)
                                {
                                    c.Odds = null;
                                }
                                if(gamedto.betModel == 5)
                                {
                                    c.choiceString = choicestr.Where(x => x.choiceSn == c.sn).ToList();
                                }
                                c.valid = 1;
                            }
                        }
                    }
                }
            }

            return gamedtoList;

        }

        public List<gameDto> GetLiveGameList(int id)
        {
            List<gameDto> gamedtoList = new List<gameDto>();


            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
DECLARE @SUM Float
DECLARE @PoolSUM Float
DECLARE @OutlaySUM float
SELECT @SUM=
isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=4)
,0)
SELECT @PoolSUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=3)
,0)

SELECT @OutlaySUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=2)
,0)

SELECT g.*,u.userID,isnull((
select (sum(ppr.assets)-isnull(@SUM,0)) as betMoneyall
	from  prizepoolRecord ppr
	where ppr.type=2

),null)as betmoneyall
into #games
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
where g.valid=1 and g.sn=@gameSn

select *
from #games

SELECT t.*,isnull((
CASE WHEN @OutlaySUM < t.promote THEN
(select (sum(ppr.assets)-isnull(@PoolSUM,0)) as poolall
	from  prizepoolRecord ppr
	where ppr.type=1
)
ELSE
(
select @OutlaySUM -isnull(@SUM,0) as poolall
where g.betModel = 5
)
END),null)as poolall
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
and t.valid=1 or t.valid=3

SELECT c.sn,c.comSn,c.topicSn,
isnull((
	select gd.name
	from gamedragon gd
	where g.betModel = 5 and cs.choiceStr1 = gd.sn
),c.choiceStr) as choiceStr
,c.comment,c.Odds,c.isTrue,c.totalMoney,c.createDate,c.modiDate,c.bearSn
,isnull((
	select sum(b.money) as betMoneygti
	from bets b
	where b.choiceSn=c.sn
),0) as betMoneygti,g.betModel as betModel,
isnull((
select gd.en_short
from gamedragon gd
where g.betModel = 5 and cs.choiceStr1 = gd.sn),null
) as dragonshort,
isnull(cs.trueCount,null) as trueCount,isnull(cs.choiceStr1,null) as chstr


FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
JOIN dbo.choices c ON
	c.topicSn=t.sn
FULL OUTER JOIN dbo.choiceStrs cs ON
	c.sn=cs.choiceSn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
select c.sn as choiceSn,bu.value as unitSn, isnull(sum(b.money),0) as betMoney
,isnull(c.Odds,0) as Odds
from #games g
CROSS APPLY string_split(betUnit,',') bu
left join topics t on t.gameSn=g.sn
left join choices c on c.topicSn=t.sn
left join bets b on b.choiceSn=c.sn and b.unitSn=bu.value
left join choiceOdds co on co.choiceSn=c.sn and b.unitSn=bu.value
group by c.sn,bu.value,c.Odds 
order by c.sn,unitSn

select bs.*
from #games g
join bonus bs on bs.gamesn = g.sn

select cstr.*
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join choiceStrs cstr on cstr.choiceSn=c.sn
order by cstr.sn

select bcs.betSn,t.sn as topicSn,bcs.unitSn,gdn.name as choiceStr,bcs.choiceCount,isnull(count(choiceCount),0) as allcount
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join bets bt on bt.choiceSn=c.sn
join betCounts bcs on bcs.betSn=bt.sn
join gamedragon gdn on gdn.sn = bcs.choiceStr
group by bcs.unitSn,bcs.betSn,t.sn,gdn.name,bcs.unitSn,bcs.choiceCount
order by t.sn
";
                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    gamedtoList = multi.Read<gameDto>().ToList();
                    var topic = multi.Read<topicDto>();
                    var choice = multi.Read<choiceDto>();
                    var choiceBet = multi.Read<choiceBetMoneyDto>();
                    var bonus = multi.Read<bonusDto>();
                    var choicestr = multi.Read<choiceStrDto>();
                    var betcount = multi.Read<betCountDto>();
                    foreach (gameDto gamedto in gamedtoList)
                    {
                        gamedto.topicList = topic.Where(p => p.gameSn == gamedto.sn).ToList();
                        gamedto.bonusList = bonus.Where(x => x.gamesn == gamedto.sn).OrderBy(x => x.sn).ToList();
                        foreach (var t in gamedto.topicList)
                        {
                            if (!gamedto.canbet)
                                t.canbet = false;
                            t.choiceList = choice.Where(p => p.topicSn == t.sn).ToList();
                            t.betcountList = betcount.Where(p => p.topicSn == t.sn).ToList();
                            foreach (var c in t.choiceList)
                            {
                                c.betMoney = choiceBet.Where(p => p.choiceSn == c.sn).ToList();
                                if (gamedto.betModel == 2)
                                {
                                    c.Odds = null;
                                }
                                if (gamedto.betModel == 5)
                                {
                                    c.choiceString = choicestr.Where(x => x.choiceSn == c.sn).ToList();
                                }
                                c.valid = 1;
                            }
                        }
                    }
                }
            }

            return gamedtoList;

        }


        public List<gameDto> GetPayGameList(int id)
        {
            List<gameDto> gamedtoList = new List<gameDto>();


            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
DECLARE @SUM Float
DECLARE @PoolSUM Float
DECLARE @OutlaySUM float
SELECT @SUM=
isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=4)
,0)
SELECT @PoolSUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=3)
,0)

SELECT @OutlaySUM=isnull
((SELECT SUM(ppr.assets) 
FROM prizepoolRecord ppr
where ppr.type=2)
,0)

SELECT g.*,u.userID,isnull((
select (sum(ppr.assets)-isnull(@SUM,0)) as betMoneyall
	from  prizepoolRecord ppr
	where ppr.type=2

),null)as betmoneyall
into #games
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
where g.valid=1 and g.sn=@gameSn

select *
from #games

SELECT t.*,isnull((
CASE WHEN @OutlaySUM < t.promote THEN
(select (sum(ppr.assets)-isnull(@PoolSUM,0)) as poolall
	from  prizepoolRecord ppr
	where ppr.type=1
)
ELSE
(
select @OutlaySUM -isnull(@SUM,0) as poolall
where g.betModel = 5
)
END),null)as poolall
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
and t.valid=0

SELECT c.sn,c.comSn,c.topicSn,
isnull((
	select gd.name
	from gamedragon gd
	where g.betModel = 5 and cs.choiceStr1 = gd.sn
),c.choiceStr) as choiceStr
,c.comment,c.Odds,c.isTrue,c.totalMoney,c.createDate,c.modiDate,c.bearSn
,isnull((
	select sum(b.money) as betMoneygti
	from bets b
	where b.choiceSn=c.sn
),0) as betMoneygti,g.betModel as betModel,
isnull((
select gd.en_short
from gamedragon gd
where g.betModel = 5 and cs.choiceStr1 = gd.sn),null
) as dragonshort,
isnull(cs.trueCount,null) as trueCount,isnull(cs.choiceStr1,null) as chstr


FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
JOIN dbo.choices c ON
	c.topicSn=t.sn
FULL OUTER JOIN dbo.choiceStrs cs ON
	c.sn=cs.choiceSn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
select c.sn as choiceSn,bu.value as unitSn, isnull(sum(b.money),0) as betMoney
,isnull(c.Odds,0) as Odds
from #games g
CROSS APPLY string_split(betUnit,',') bu
left join topics t on t.gameSn=g.sn
left join choices c on c.topicSn=t.sn
left join bets b on b.choiceSn=c.sn and b.unitSn=bu.value
left join choiceOdds co on co.choiceSn=c.sn and b.unitSn=bu.value
group by c.sn,bu.value,c.Odds 
order by c.sn,unitSn

select bs.*
from #games g
join bonus bs on bs.gamesn = g.sn

select cstr.*
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join choiceStrs cstr on cstr.choiceSn=c.sn
order by cstr.sn

select bcs.betSn,t.sn as topicSn,bcs.unitSn,gdn.name as choiceStr,bcs.choiceCount,isnull(count(choiceCount),0) as allcount
from #games g
join topics t on t.gameSn=g.sn
join choices c on c.topicSn=t.sn
join bets bt on bt.choiceSn=c.sn
join betCounts bcs on bcs.betSn=bt.sn
join gamedragon gdn on gdn.sn = bcs.choiceStr
group by bcs.unitSn,bcs.betSn,t.sn,gdn.name,bcs.unitSn,bcs.choiceCount
order by t.sn
";
                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    gamedtoList = multi.Read<gameDto>().ToList();
                    var topic = multi.Read<topicDto>();
                    var choice = multi.Read<choiceDto>();
                    var choiceBet = multi.Read<choiceBetMoneyDto>();
                    var bonus = multi.Read<bonusDto>();
                    var choicestr = multi.Read<choiceStrDto>();
                    var betcount = multi.Read<betCountDto>();
                    foreach (gameDto gamedto in gamedtoList)
                    {
                        gamedto.topicList = topic.Where(p => p.gameSn == gamedto.sn).ToList();
                        gamedto.bonusList = bonus.Where(x => x.gamesn == gamedto.sn).OrderBy(x => x.sn).ToList();
                        foreach (var t in gamedto.topicList)
                        {
                            if (!gamedto.canbet)
                                t.canbet = false;
                            t.choiceList = choice.Where(p => p.topicSn == t.sn).ToList();
                            t.betcountList = betcount.Where(p => p.topicSn == t.sn).ToList();
                            foreach (var c in t.choiceList)
                            {
                                c.betMoney = choiceBet.Where(p => p.choiceSn == c.sn).ToList();
                                if (gamedto.betModel == 2)
                                {
                                    c.Odds = null;
                                }
                                if (gamedto.betModel == 5)
                                {
                                    c.choiceString = choicestr.Where(x => x.choiceSn == c.sn).ToList();
                                }
                                c.valid = 1;
                            }
                        }
                    }
                }
            }

            return gamedtoList;

        }
        public SettingListDto GetBonusGameList()
        {
            SettingListDto gamedtoList = new SettingListDto();


            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT *
FROM dbo.gameSettings

SELECT *
FROM dbo.bonusSettings

";
                using (var multi = cn.QueryMultiple(query))
                {
                    var game = multi.Read<gameSettingDto>().ToList();
                    var bonus = multi.Read<bonusSettingDto>().ToList();
                    
                    gamedtoList.bonussettingList = bonus;
                    gamedtoList.gamesettingList = game;


                }
            }
            return gamedtoList;

        }





        public SettingListDto GettopicGameList()
        {
            SettingListDto gamedtoList = new SettingListDto();


            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT *
FROM dbo.topicSettings

SELECT *
FROM dbo.choiceSettings

";
                using (var multi = cn.QueryMultiple(query))
                {
                    var topic = multi.Read<topicSettingDto>().ToList();
                    var choice = multi.Read<choiceSettingDto>().ToList();

                    gamedtoList.topicsettingList = topic;
                    foreach(var topiclist in topic)
                    {
                        List<choiceSettingDto> choicelist = choice.Where(x => x.topiceSetting == topiclist.id).ToList();
                        if(choicelist != null)
                        {
                            topiclist.choicsettingList = choicelist;
                        }
                    }
                    


                }
            }
            return gamedtoList;

        }




        public List<gameDto> GetGameAdminList()
        {
            List<gameDto> gamedtoList = new List<gameDto>();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT g.*,u.userID
into #games
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
where g.valid=1

select *
from #games

SELECT t.*
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
and t.valid=1

SELECT c.*
,isnull((
	select sum(b.money) as betMoney
	from bets b
	where b.choiceSn=c.sn
),0) as betMoney
,g.betModel as betModel,
isnull((
select gd.en_short
from gamedragon gd
where g.betModel = 5 and cs.choiceStr1 = gd.sn),null
) as dragonshort,
isnull(cs.trueCount,null) as trueCount,isnull(cs.choiceStr1,null) as chstr


FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
JOIN dbo.choices c ON
	c.topicSn=t.sn
FULL OUTER JOIN  dbo.choiceStrs cs ON
	c.sn=cs.choiceSn
WHERE exists(
	select *
	from #games
	where sn=g.sn
)
";
                using (var multi = cn.QueryMultiple(query))
                {
                    gamedtoList = multi.Read<gameDto>().ToList();
                    var topic = multi.Read<topicDto>();
                    var choice = multi.Read<choiceDto>();
                    foreach (gameDto gamedto in gamedtoList)
                    {
                        gamedto.topicList = topic.Where(p => p.gameSn == gamedto.sn).ToList();

                        foreach (var t in gamedto.topicList)
                        {
                            if (!gamedto.canbet)
                                t.canbet = false;
                            t.choiceList = choice.Where(p => p.topicSn == t.sn).ToList();
                            foreach (var c in t.choiceList)
                            {
                                if (gamedto.betModel == 2)
                                {
                                    c.Odds = null;
                                }
                                c.valid = 1;
                            }
                        }
                    }
                }
            }
            return gamedtoList;

        }



        public List<choiceBetMoneyDto> getdgonbet()
        {
            List<choiceBetMoneyDto> choiceBet = new List<choiceBetMoneyDto>();
            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
            select c.sn as choiceSn,bu.value as unitSn, isnull(sum(b.money), 0) as betMoney
,isnull(c.Odds, 0) as Odds
from games g
CROSS APPLY string_split(betUnit,',') bu
left join topics t on t.gameSn = g.sn
left join choices c on c.topicSn = t.sn
left join bets b on b.choiceSn = c.sn and b.unitSn = bu.value
left join choiceOdds co on co.choiceSn = c.sn and b.unitSn = bu.value
group by c.sn,bu.value,c.Odds
order by c.sn,unitSn";
                using (var multi = cn.QueryMultiple(query))
                {
                    choiceBet = multi.Read<choiceBetMoneyDto>().ToList();

                }

                
            }
            return choiceBet;
        }

        public gameDto GetGame(int id)
        {
            gameDto gamedto = new gameDto();

            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT g.*,u.userID
FROM dbo.games g
JOIN dbo.users u ON u.sn=g.userSn
WHERE g.sn=@gameSn

SELECT t.*
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
WHERE g.sn=@gameSn


SELECT c.sn,c.comSn,c.topicSn,isnull(cs.choiceStr1,c.choiceStr)as choiceStr,c.comment,c.Odds,c.isTrue,c.totalMoney,c.createDate,c.modiDate,c.bearSn
FROM dbo.games g
JOIN dbo.topics t ON 
	g.comSn = t.comSn
	AND t.gameSn=g.sn
JOIN dbo.choices c ON
	c.topicSn=t.sn
FULL OUTER JOIN dbo.choiceStrs cs ON
	c.sn=cs.choiceSn
WHERE g.sn=@gameSn
";
                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    IEnumerable<gameDto> gtolist = multi.Read<gameDto>();
                    if (gtolist.Count() > 0)
                    {
                        gamedto = gtolist.Single();
                        var topic = multi.Read<topicDto>().ToList();
                        gamedto.topicList = topic;
                        var choice = multi.Read<choiceDto>().ToList();

                        foreach (var t in gamedto.topicList)
                        {
                            if (!gamedto.canbet)
                                t.canbet = false;
                            t.choiceList = choice.Where(p => p.topicSn == t.sn).ToList();
                        }
                    }
                }
            }
            return gamedto;

        }

        public game setData(game g)
        {
            if (g.sn == 0)
                return AddNew(g);
            else
                return Edit(g);
        }

        private game AddNew(game g)
        {
            db.games.Add(g);
            db.SaveChanges();
            return g;
        }

        private game Edit(game g)
        {
            g.modiDate = DateTime.Now;
            game gdb = db.games.Where(p => p.sn == g.sn).FirstOrDefault();
            gdb.sdate = g.sdate;
            gdb.edate = g.edate;
            gdb.betModel = g.betModel;
            gdb.modiDate = g.modiDate;
            gdb.title = g.title;
            gdb.comment = g.comment;
            gdb.rake = g.rake;
            gdb.gamedate = g.gamedate;
            gdb.gameplace = g.gameplace;
            gdb.gamedate = g.gamedate;
            gdb.gameStatus = g.gameStatus;
            db.SaveChanges();
            return g;
        }

        public gameSetting DgonsettingEdit(gameSettingDto setting)
        {
            gameSetting gstting = new gameSetting();
            gstting.sn = (int)setting.sn;
            gstting.title = setting.title;
            gstting.betnumber = setting.betnumber;
            gstting.outlay = setting.outlay;
            gstting.promote = setting.promote;
            gstting.allocation = setting.allocation;
            db.Entry(gstting).State = EntityState.Modified;
            db.SaveChanges();
            return gstting;
        }

        public bonusSetting bonussettingEdit(bonusSettingDto setting)
        {
            bonusSetting bonus = new bonusSetting();
            bonus.sn = (int)setting.sn;
            bonus.bonus = setting.bonus;
            bonus.settingsn = 1;
            bonus.BonusRatio = setting.BonusRatio;
            bonus.pool = setting.pool;
            bonus.quantity = setting.Quantity;
            db.Entry(bonus).State = EntityState.Modified;
            db.SaveChanges();
             return bonus;
        }

        public void topicsettingEdit(topicSettingDto setting)
        {
            topicSetting topic = new topicSetting();
            topic.id = (int)setting.id;
            topic.topicsName = setting.topicsName;
            topic.Model = setting.Model;
            topic.valid = setting.valid;
            topic.enName = setting.enName;
            topic.gametype = setting.gametype;
            topic.choiceType = setting.choiceType;
            topic.image = setting.image;
            topic.hoverImage = setting.hoverImage;
            topic.autotype = setting.autotype;
            topic.numberType = setting.numberType;

            db.Entry(topic).State = EntityState.Modified;
            db.SaveChanges();
            if(setting.choiceType == true)
            {
                foreach (var c in setting.choicsettingList)
                {
                    choiceSetting choice = new choiceSetting();
                    choice.id = c.id;
                    choice.topiceSetting = c.topiceSetting;
                    choice.eName = c.eName;
                    choice.valid = c.valid;
                    choice.choiceName = c.choiceName;
                    choice.cNumberType = c.cNumberType;

                    db.Entry(choice).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            
            


        }

        public void topicsettingCreate(topicSettingDto setting)
        {
            topicSetting topic = new topicSetting();
            topic.topicsName = setting.topicsName;
            topic.valid = 1;
            topic.gametype = setting.gametype;
            topic.choiceType = setting.choiceType;
            topic.image = setting.image;
            topic.hoverImage = setting.hoverImage;
            topic.Model = setting.Model;
            db.topicSettings.Add(topic);
            db.SaveChanges();
            foreach (var c in setting.choicsettingList)
            {
                choiceSetting choice = new choiceSetting();
                choice.id = c.id;
                choice.topiceSetting = topic.id;
                choice.valid = c.valid;
                choice.choiceName = c.choiceName;

                db.choiceSettings.Add(choice);
                db.SaveChanges();

            }



        }



        public bool SetGame(int id, gameDto gamed)
        {
            try
            {
                gameDto gameDB = GetGame(gamed.sn);
                byte gameStatus = (gameDB.gameStatus.HasValue ? gameDB.gameStatus.Value : (byte)1);
                //if (!new byte[] { 1 }.Contains(gameStatus))
                //    return false;
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<gameDto, game>();
                    cfg.CreateMap<topicDto, topic>();
                    cfg.CreateMap<choiceDto, choice>();
                });
                game game = Mapper.Map<game>(gamed);
                game = setData(game);


                foreach (topicDto topicd in gamed.topicList)
                {
                    if (topicd.valid.HasValue && topicd.valid.Value == 1)
                    {
                        topic topic = Mapper.Map<topic>(topicd);
                        topic.comSn = game.comSn;
                        topic.gameSn = game.sn;
                        topic = new TopicRepository().setData(topic);

                        foreach (choiceDto choiced in topicd.choiceList)
                        {
                            if (choiced.valid.HasValue && choiced.valid.Value == 1)
                            {
                                choice choice = Mapper.Map<choice>(choiced);
                                choice.comSn = game.comSn;
                                choice.topicSn = topic.sn;
                                choice = new ChoiceRepository().setData(choice);
                               /* foreach (choiceStrDto cb in choiced.choiceString)
                                {
                                    //樂透模式才會新增
                                    if (game.betModel == 5)
                                    {
                                        choiceStr cs = Mapper.Map<choiceStr>(cb);
                                        //只有在可下注單位的資料要存
                                        //if (game.betUnit.Contains(cs.unitSn.ToString()))
                                        //{
                                            cs.choiceSn = choice.sn;
                                            cs = new choiceStrRepository().setData(cs);
                                        //}
                                    }
                                }*/

                            }
                        }
                    }
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!gameExists(id))
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        public bool SetGameLive(int id, gameDto gamed)
        {
            game Game = new game();
            try
            {
                gameDto gameDB = GetGame(gamed.sn);
                gameDB.gameStatus = gamed.gameStatus;
                //byte gameStatus = (gameDB.gameStatus.HasValue ? gameDB.gameStatus.Value : (byte)1);
                //if (!new byte[] { 1 }.Contains(gameStatus))
                //    return false;
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<gameDto, game>();
                    cfg.CreateMap<topicDto, topic>();
                    cfg.CreateMap<choiceDto, choice>();
                });
                Game = Mapper.Map<game>(gamed);
                Game = setData(Game);


                foreach (topicDto topicd in gamed.topicList)
                {
                    if (topicd.valid.HasValue && topicd.valid.Value == 1)
                    {
                        topic topic = Mapper.Map<topic>(topicd);
                        topic.comSn = Game.comSn;
                        topic.gameSn = Game.sn;
                        topic = new TopicRepository().setData(topic);

                        foreach (choiceDto choiced in topicd.choiceList)
                        {
                            if (choiced.valid.HasValue && choiced.valid.Value == 1)
                            {
                                choice choice = Mapper.Map<choice>(choiced);
                                choice.comSn = Game.comSn;
                                choice.topicSn = topic.sn;
                                choice = new ChoiceRepository().setData(choice);
                                /* foreach (choiceStrDto cb in choiced.choiceString)
                                 {
                                     //樂透模式才會新增
                                     if (game.betModel == 5)
                                     {
                                         choiceStr cs = Mapper.Map<choiceStr>(cb);
                                         //只有在可下注單位的資料要存
                                         //if (game.betUnit.Contains(cs.unitSn.ToString()))
                                         //{
                                             cs.choiceSn = choice.sn;
                                             cs = new choiceStrRepository().setData(cs);
                                         //}
                                     }
                                 }*/

                            }
                        }
                    }
                }
                

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!gameExists(id))
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            return true;


        }

        public byte? getGameCloseStatus(int id)
        {
            if (gameExists(id))
            {
                game g = db.games.Where(p => p.sn == id).FirstOrDefault();
                return g.gameStatus;
            }
            else
                return null;
        }

        public void ChangeGameValid(int id)
        {

                topic t = db.topics.Where(p => p.sn == id).FirstOrDefault();
                t.valid = 0;
                db.SaveChanges();

        }

        public bool setGameCloseStatus(int id,byte gameStatus)
        {
            if (gameExists(id))
            {
                game g = db.games.Where(p => p.sn == id).FirstOrDefault();
                byte gs = (g.gameStatus.HasValue ? g.gameStatus.Value : (byte)0);
                if (gameStatus == 2 && !new byte[] { 0, 3 , 4 }.Contains(gs))
                    return false;
                g.gameStatus = gameStatus;
				if (gameStatus == 3)
                    g.payDate = System.DateTime.Now;
                //g.modiDate = System.DateTime.Now;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool setTopicCloseStatus(int id, byte gameStatus,string back)
        {
            string b = (back != null) ? "(" + back + ")" : "";
                topic t = db.topics.Where(p => p.sn == id).FirstOrDefault();
                t.title = t.title +  b;
                t.valid = gameStatus;
                db.SaveChanges();
                return true;
        }

        public bool setWinner(List<choiceDto> choiceList)
        {
            foreach(choiceDto choiced in choiceList)
            {
                choice choice = db.choices.Where(p => p.sn == choiced.sn).FirstOrDefault();
                choice.isTrue = (choiced.isTrue.HasValue ? choiced.isTrue : 0);
                //龍的傳人才新增
                if(choiced.betModel == 5)
                {
                    choiceStr choicstr = db.choiceStrs.Where(x => x.choiceSn == choice.sn && x.choiceStr1 == choiced.chstr).FirstOrDefault();
                    choicstr.trueCount = (choiced.trueCount.HasValue ? choiced.trueCount : 0);
                }
                db.SaveChanges();
            }
            return true;
        }

        public bool setLiveWinner(List<choiceDto> choiceList)
        {
            foreach (choiceDto choiced in choiceList)
            {
                choice choice = db.choices.Where(p => p.sn == choiced.sn).FirstOrDefault();
                choice.isTrue = (choiced.isTrue.HasValue ? choiced.isTrue : null);
                db.SaveChanges();
            }
            return true;
        }


        public List<payoutDto> livepays(int id)
        {
            Runpays(id);
            List<payoutDto> payoutList = new List<payoutDto>();

            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string sql = @"
                BEGIN tran
                BEGIN TRY

                    update t set t.totalmoney=(
	                    select sum(b.money) from bets b 
	                    where b.topicSn=t.sn and b.valid in (1,2)
	                    and not exists(select * from choices c where c.isTrue in (2) and b.choiceSn=c.sn)
                    )
                    from topics t
                    where t.gameSn=@gameSn
	                and not exists(
		                select *
		                from payouts p
		                where p.topicSn=t.sn
	                )

                    update c set c.totalMoney=(select sum(b.money) from bets b where b.choiceSn=c.sn and b.valid in (1,2))
                    from choices c
                    join topics t on t.sn=c.topicSn
                    where t.gameSn=@gameSn
	                and not exists(
		                select *
		                from payouts p
		                where p.topicSn=t.sn
	                )
                    
                    --賠率模式
	                INSERT INTO [dbo].[payouts]
			            ([betSn]
			            ,[comSn]
			            ,[gameSn]
			            ,[topicSn]
						,[choiceSn]
			            ,[userSn]
			            ,[Odds]
			            ,[money]
			            ,[realMoney]
                        ,[unitSn]
                        ,[isTrue])
	                SELECT b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
	                ,b.Odds,b.[money],b.Odds*b.[money] as [realMoney]
					,2 as unitSn,c.isTrue
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn and t.walk IS NULL
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND c.isTrue=1
	                AND b.valid=1
	                AND g.gameStatus=4
					AND g.betModel=1
					AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )

                    --總彩池模式
                    INSERT INTO [dbo].[payouts]
	                    ([betSn]
	                    ,[comSn]
	                    ,[gameSn]
	                    ,[topicSn]
						,[choiceSn]
	                    ,[userSn]
	                    ,[Odds]
	                    ,[money]
	                    ,[realMoney]
                        ,[unitSn]
                        ,[topicTotalMoney]
                        ,[correctTotalMoney]
                        ,[isTrue]
	                    ,[rake])
                    select *
                    from(
	                    SELECT b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn
						,c.sn as choiceSn,b.userSn,b.Odds,b.[money]
	                    ,round(sum(b.money) over(PARTITION BY c.topicSn)*b.money/sum(b.money) over(PARTITION BY c.topicSn,c.isTrue)
		                    * (CASE WHEN c.isTrue =1 THEN 1 ELSE 0 END)
		                    * (100-isnull(g.rake,0))/100
		                    ,0) as [realMoney]
	                    ,1 as unitSn
	                    ,sum(b.money) over(PARTITION BY c.topicSn) as topicTotalMoney
	                    ,sum(b.money) over(PARTITION BY c.topicSn,c.isTrue) as correctTotalMoney
	                    ,c.isTrue,isnull(g.rake,0) as rake
	                    FROM dbo.games g
	                    JOIN dbo.topics t ON t.gameSn=g.sn and t.walk IS NULL
	                    JOIN dbo.choices c ON c.topicSn=t.sn
	                    JOIN dbo.bets b ON b.choiceSn=c.sn
	                    WHERE g.sn=@gameSn
	                    AND b.valid in (1,2)
	                    --AND g.gameStatus=4
	                    AND g.betModel=2
	                    AND b.money>0
	                    and c.isTrue not in (2)
						and not exists(
							select *
							from payouts p
							where p.betSn=b.sn
						)
                    ) as payt
                    where isTrue=1

                    --返還
                    INSERT INTO [dbo].[payouts]
	                    ([betSn]
	                    ,[comSn]
	                    ,[gameSn]
	                    ,[topicSn]
						,[choiceSn]
	                    ,[userSn]
	                    ,[Odds]
	                    ,[money]
	                    ,[realMoney]
                        ,[unitSn]
                        ,[isTrue]
	                    ,[rake])
                    SELECT b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
                    ,b.Odds,b.[money],b.[money] as [realMoney]
                    ,b.unitSn,c.isTrue,g.rake
                    FROM dbo.games g
                    JOIN dbo.topics t ON t.gameSn=g.sn
                    JOIN dbo.choices c ON c.topicSn=t.sn
                    JOIN dbo.bets b ON b.choiceSn=c.sn
                    WHERE g.sn=@gameSn
                    AND c.isTrue=2
                    AND b.valid=1
                    AND g.gameStatus=4
                    AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )

	                UPDATE b SET b.valid=3
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND EXISTS(
		                SELECT * FROM dbo.payouts pp
		                WHERE pp.gameSn=g.sn
	                )
	                AND b.valid=1

	                COMMIT
                END TRY
                BEGIN CATCH
                    ROLLBACK
                END CATCH;
                ";
                cn.Open();
                SqlCommand myCommand = new SqlCommand(sql, cn);
                myCommand.Parameters.Add("@gameSn", SqlDbType.Int);
                myCommand.Parameters["@gameSn"].Value = id;


                myCommand.ExecuteReader();


                string query = @"
	                select p.*,u.userId
	                from payouts p
	                join users u on u.sn=p.userSn
	                join bets b on b.sn=p.betSn
	                where p.gameSn=@gameSn
	                and b.valid=3
";
                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    IEnumerable<payoutDto> pdlist = multi.Read<payoutDto>();
                    if (pdlist.Count() > 0)
                    {
                        payoutList = pdlist.ToList();
                    }
                }


                sql = @"
	                UPDATE b SET b.valid=2
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND EXISTS(
		                SELECT * FROM dbo.payouts pp
		                WHERE pp.gameSn=g.sn
	                )
	                AND b.valid=3
";
                myCommand = new SqlCommand(sql, cn);
                myCommand.Parameters.Add("@gameSn", SqlDbType.Int);
                myCommand.Parameters["@gameSn"].Value = id;
                myCommand.ExecuteReader();



            }
            return payoutList;
        }




        public List<payoutDto> pays(int id)
        {
            Runpays(id);
            List<payoutDto> payoutList = new List<payoutDto>();

            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string sql = @"
                BEGIN tran
                BEGIN TRY

                    update t set t.totalmoney=(
	                    select sum(b.money) from bets b 
	                    where b.topicSn=t.sn and b.valid in (1,2)
	                    and not exists(select * from choices c where c.isTrue in (2) and b.choiceSn=c.sn)
                    )
                    from topics t
                    where t.gameSn=@gameSn
	                and not exists(
		                select *
		                from payouts p
		                where p.topicSn=t.sn
	                )

                    update c set c.totalMoney=(select sum(b.money) from bets b where b.choiceSn=c.sn and b.valid in (1,2))
                    from choices c
                    join topics t on t.sn=c.topicSn
                    where t.gameSn=@gameSn
	                and not exists(
		                select *
		                from payouts p
		                where p.topicSn=t.sn
	                )
                    
                    --賠率模式
	                INSERT INTO [dbo].[payouts]
			            ([betSn]
			            ,[comSn]
			            ,[gameSn]
			            ,[topicSn]
						,[choiceSn]
			            ,[userSn]
			            ,[Odds]
			            ,[money]
			            ,[realMoney]
                        ,[unitSn]
                        ,[isTrue])
	                SELECT b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
	                ,b.Odds,b.[money],b.Odds*b.[money] as [realMoney]
					,2 as unitSn,c.isTrue
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn and t.walk IS NULL
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND c.isTrue=1
	                AND b.valid=1
	                AND g.gameStatus=4
					AND g.betModel=1
					AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )

                    --簡單猜猜
	                INSERT INTO [dbo].[payouts]
			            ([betSn]
			            ,[comSn]
			            ,[gameSn]
			            ,[topicSn]
						,[choiceSn]
			            ,[userSn]
			            ,[Odds]
			            ,[money]
			            ,[realMoney]
                        ,[unitSn]
                        ,[isTrue])
	                SELECT b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
	                ,b.Odds,b.[money],b.Odds*b.[money] as [realMoney]
					,2 as unitSn,c.isTrue
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn and t.walk IS NULL
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND c.isTrue=1
	                AND b.valid=1
	                AND g.gameStatus=4
					AND g.betModel=10
					AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )

                    --總彩池模式
                    INSERT INTO [dbo].[payouts]
	                    ([betSn]
	                    ,[comSn]
	                    ,[gameSn]
	                    ,[topicSn]
						,[choiceSn]
	                    ,[userSn]
	                    ,[Odds]
	                    ,[money]
	                    ,[realMoney]
                        ,[unitSn]
                        ,[topicTotalMoney]
                        ,[correctTotalMoney]
                        ,[isTrue]
	                    ,[rake])
                    select *
                    from(
	                    SELECT b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn
						,c.sn as choiceSn,b.userSn,b.Odds,b.[money]
	                    ,round(sum(b.money) over(PARTITION BY c.topicSn)*b.money/sum(b.money) over(PARTITION BY c.topicSn,c.isTrue)
		                    * (CASE WHEN c.isTrue =1 THEN 1 ELSE 0 END)
		                    * (100-isnull(g.rake,0))/100
		                    ,0) as [realMoney]
	                    ,2 as unitSn
	                    ,sum(b.money) over(PARTITION BY c.topicSn) as topicTotalMoney
	                    ,sum(b.money) over(PARTITION BY c.topicSn,c.isTrue) as correctTotalMoney
	                    ,c.isTrue,isnull(g.rake,0) as rake
	                    FROM dbo.games g
	                    JOIN dbo.topics t ON t.gameSn=g.sn and t.walk IS NULL
	                    JOIN dbo.choices c ON c.topicSn=t.sn
	                    JOIN dbo.bets b ON b.choiceSn=c.sn
	                    WHERE g.sn=@gameSn
	                    AND b.valid in (1,2)
	                    --AND g.gameStatus=4
	                    AND g.betModel=2
	                    AND b.money>0
	                    and c.isTrue not in (2)
						and not exists(
							select *
							from payouts p
							where p.betSn=b.sn
						)
                    ) as payt
                    where isTrue=1

                    --返還
                    INSERT INTO [dbo].[payouts]
	                    ([betSn]
	                    ,[comSn]
	                    ,[gameSn]
	                    ,[topicSn]
						,[choiceSn]
	                    ,[userSn]
	                    ,[Odds]
	                    ,[money]
	                    ,[realMoney]
                        ,[unitSn]
                        ,[isTrue]
	                    ,[rake])
                    SELECT b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
                    ,b.Odds,b.[money],b.[money] as [realMoney]
                    ,b.unitSn,c.isTrue,g.rake
                    FROM dbo.games g
                    JOIN dbo.topics t ON t.gameSn=g.sn
                    JOIN dbo.choices c ON c.topicSn=t.sn
                    JOIN dbo.bets b ON b.choiceSn=c.sn
                    WHERE g.sn=@gameSn
                    AND c.isTrue=2
                    AND b.valid=1
                    AND g.gameStatus=4
                    AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )

	                UPDATE b SET b.valid=3
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND EXISTS(
		                SELECT * FROM dbo.payouts pp
		                WHERE pp.gameSn=g.sn
	                )
	                AND b.valid=1

	                COMMIT
                END TRY
                BEGIN CATCH
                    ROLLBACK
                END CATCH;
                ";
                cn.Open();
                SqlCommand myCommand = new SqlCommand(sql, cn);
                myCommand.Parameters.Add("@gameSn", SqlDbType.Int);
                myCommand.Parameters["@gameSn"].Value = id;


                myCommand.ExecuteReader();


                string query = @"
	                select p.*,u.userId
	                from payouts p
	                join users u on u.sn=p.userSn
	                join bets b on b.sn=p.betSn
	                where p.gameSn=@gameSn
	                and b.valid=3
";
                using (var multi = cn.QueryMultiple(query, new { gameSn= id }))
                {
                    IEnumerable<payoutDto> pdlist = multi.Read<payoutDto>();
                    if (pdlist.Count() > 0)
                    {
                        payoutList = pdlist.ToList();
                    }
                }


                sql = @"
	                UPDATE b SET b.valid=2
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND EXISTS(
		                SELECT * FROM dbo.payouts pp
		                WHERE pp.gameSn=g.sn
	                )
	                AND b.valid=3
";
                myCommand = new SqlCommand(sql, cn);
                myCommand.Parameters.Add("@gameSn", SqlDbType.Int);
                myCommand.Parameters["@gameSn"].Value = id;
                myCommand.ExecuteReader();



            }
            return payoutList;
        }
        //走地模式
        public void Runpays(int id)
        {
            List<payoutDto> payoutList = new List<payoutDto>();
            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"  
SELECT b.*
FROM dbo.games g
	 JOIN dbo.topics t ON t.gameSn=g.sn
	 JOIN dbo.choices c ON c.topicSn=t.sn
	 JOIN dbo.bets b ON b.choiceSn=c.sn
where t.gameSn = @gameSn and t.walk = 1


     
	                   SELECT b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn
						,c.sn as choiceSn,b.userSn,b.Odds,b.[money]
	                    ,round(sum(b.money) over(PARTITION BY c.topicSn)*b.money/sum(b.money) over(PARTITION BY c.topicSn,c.isTrue)
		                    * (CASE WHEN c.isTrue =1 THEN 1 ELSE 0 END)
		                    * g.allocation / 100
		                    ,0) as [realMoney]
	                    ,2 as unitSn
	                    ,sum(b.money) over(PARTITION BY c.topicSn) as topicTotalMoney
	                    ,sum(b.money) over(PARTITION BY c.topicSn,c.isTrue) as correctTotalMoney
	                    ,c.isTrue,isnull(g.rake,0) as rake,g.allocation,u.userId
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
                    join users u on u.sn=b.userSn
	                WHERE g.sn=@gameSn
	                AND b.valid in (1,2)
	                    --AND g.gameStatus=4
	                    AND t.walk = 1
	                    AND b.money>0
	                    and c.isTrue not in (2)
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
						)
						";

                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    var bet = multi.Read<bet>().ToList();
                    var pay = multi.Read<payoutDto>().Where(x=>x.isTrue == 1).ToList();
                    // List<nabobpayoutDto> npaydto = new List<nabobpayoutDto>();
                    foreach (var p in pay)
                    {

                           // List<nabobpayoutDto> npaydto = nabobpay.Where(x => x.equalchoice == pay.betSn).ToList();
                            //var paytrue = 0;
                            

                                var config = new MapperConfiguration(cfg => cfg.CreateMap<payoutDto, payout>()
                                .IgnoreAllNonExisting().
                                ForMember(up => up.comSn, opt => opt.MapFrom(u => "1"))
                                );
                                config.AssertConfigurationIsValid();
                                var mapper = config.CreateMapper();
                                
                             
                                payout payouta = mapper.Map<payout>(p);
                                //進入計算
                                foreach (var b in bet)
                                {
                                    if (payouta.betSn <= b.sn)
                                    {
                                        payouta.realMoney += (float)b.money * (float)(100 - p.allocation) /100 / (float)b.totalmoney * (float)payouta.money;
                                    }

                                }
                                //p.realMoney = payouta.realMoney * (float)(100 - payouta.rake) / 100;
                                payouta.realMoney = (float)Math.Round((Decimal)(payouta.realMoney * (float)(100 - payouta.rake) / 100), 2, MidpointRounding.AwayFromZero);
                                
                                
                                db.payouts.Add(payouta);
                                db.SaveChanges();

                    
                        //更新下注valid為3
                        bet bets = db.bets.Where(x => x.sn == p.betSn && x.valid == 1).FirstOrDefault();
                        if (bets != null)
                        {
                            bets.valid = 3;
                            db.SaveChanges();
                        }
                    }

                    string query2 = @"
	                select p.*,u.userId
	                from payouts p
	                join users u on u.sn=p.userSn
	                join bets b on b.sn=p.betSn
	                where p.gameSn=@gameSn
	                and b.valid=3
";
                    using (var multi2 = cn.QueryMultiple(query2, new { gameSn = id }))
                    {
                        IEnumerable<payoutDto> pdlist = multi2.Read<payoutDto>();
                        if (pdlist.Count() > 0)
                        {
                            payoutList = pdlist.ToList();
                        }


                    }

                    //更新下注valid為2
                    /*foreach (var gpaylist in pay)
                    {
                        bet bets = db.bets.Where(p => p.sn == gpaylist.betSn && p.valid == 3).FirstOrDefault();
                        //
                        if (bets != null)
                        {
                            bets.valid = 2;
                            db.SaveChanges();
                        }
                    }*/


                }





            }
            //return payoutList;
        }


        //百倍大串燒
        public List<payoutDto> nabobpays(int id)
        {
            List<payoutDto> payoutList = new List<payoutDto>();
            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"  
SELECT *
FROM dbo.topics t
where t.gameSn = @gameSn


     
	                  SELECT b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn,c.sn as choiceSn,b.userSn
	                ,b.Odds,b.[money],b.[money] as [realMoney]
					,b.unitSn,c.isTrue,b.equalchoice
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND c.isTrue=1
	                AND b.valid=1
	                AND g.gameStatus=4
					AND g.betModel=6
					AND b.money>0
	                and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                )
						";

                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    var topic = multi.Read<topic>().ToList();
                    var nabobpay = multi.Read<nabobpayoutDto>().ToList();
                   // List<nabobpayoutDto> npaydto = new List<nabobpayoutDto>();
                   //寫入派彩
                    foreach (var pay in nabobpay)
                    {
                        
                        if(pay.equalchoice == pay.betSn)
                        {
                            List<nabobpayoutDto> npaydto = nabobpay.Where(x => x.equalchoice == pay.betSn).ToList();
                            //var paytrue = 0;
                            if(npaydto.Count == topic.Count)
                            {
                                var config = new MapperConfiguration(cfg => cfg.CreateMap<nabobpayoutDto, payout>()
                                .IgnoreAllNonExisting().
                                ForMember(up => up.comSn, opt => opt.MapFrom(u => "1"))
                                );
                                config.AssertConfigurationIsValid();
                                var mapper = config.CreateMapper();
                                payout payouta = mapper.Map<payout>(pay);                                                              
                                //進入計算
                                payouta.realMoney = pay.money * 100;                               
                                payouta.unitSn = 2;
                                payouta.isTrue = 1;
                                db.payouts.Add(payouta);
                                db.SaveChanges();                               

                            }                          

                        }
                        //更新下注valid為3
                        bet bets = db.bets.Where(p => p.sn == pay.betSn && p.valid == 1).FirstOrDefault();
                        if (bets != null)
                        {
                            bets.valid = 3;
                            db.SaveChanges();
                        }
                    }

                    string query2 = @"
	                select p.*,u.userId
	                from payouts p
	                join users u on u.sn=p.userSn
	                join bets b on b.sn=p.betSn
	                where p.gameSn=@gameSn
	                and b.valid=3
";
                    using (var multi2 = cn.QueryMultiple(query2, new { gameSn = id }))
                    {
                        IEnumerable<payoutDto> pdlist = multi2.Read<payoutDto>();
                        if (pdlist.Count() > 0)
                        {
                            payoutList = pdlist.ToList();
                        }


                    }

                    //更新下注valid為2
                    foreach (var gpaylist in nabobpay)
                    {
                        bet bets = db.bets.Where(p => p.sn == gpaylist.betSn && p.valid == 3).FirstOrDefault();
                        //
                        if (bets != null)
                        {
                            bets.valid = 2;
                            db.SaveChanges();
                        }
                    }


                }

                



            }
            return payoutList;
        }



        public List<payoutDto> dgnpays(int id)
        {
            
                List<payoutDto> payoutList = new List<payoutDto>();
                using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
                {
                    string query = @"  
DECLARE @COUNT int
DECLARE @SUM int
SELECT @COUNT = COUNT(csts.trueCount)
FROM choiceStrs csts
JOIN dbo.topics t ON t.gameSn=@gameSn
JOIN dbo.choices c On csts.choiceSn = c.sn and c.topicSn = t.sn
where csts.trueCount != 0 AND csts.choiceStr1 != '5' 

SELECT @SUM = SUM(csts.trueCount)
FROM choiceStrs csts
JOIN dbo.topics t ON t.gameSn=@gameSn
JOIN dbo.choices c On csts.choiceSn = c.sn and c.topicSn = t.sn
where csts.trueCount != 0 AND csts.choiceStr1 != '5' 
               
					select b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn,c.sn as choiceSn,b.userSn
	                ,b.Odds,b.[money],
					b.unitSn,c.isTrue,t.outlay,t.promote,t.betnumber
						from dbo.games g
						 JOIN dbo.topics t ON t.gameSn=g.sn
						JOIN dbo.choices c ON c.topicSn=t.sn
						JOIN dbo.choiceStrs cst On cst.choiceSn = c.sn
						JOIN dbo.bets b ON b.choiceSn=c.sn
						JOIN dbo.betCounts bc ON bc.betSn = b.sn
						Where  g.sn=@gameSn AND g.betModel=5 And cst.trueCount != 0
						AND b.money>0  and bc.choiceStr != '5'
					and not exists(
		                select *
		                from payouts p
		                where p.betSn=b.sn
	                ) and exists(
						select　betcs.betSn,sum(betcs.choiceCount),count(betcs.choiceCount)
						from dbo.betCounts betcs
						JOIN dbo.bets bts ON bts.sn = betcs.betSn
						JOIN dbo.choiceStrs coistr ON coistr.choiceSn = c.sn 
						where 
						bts.sn = b.sn and betcs.choiceStr != '5' and betcs.choiceCount != 0 AND					
					betcs.choiceCount = coistr.trueCount AND
					betcs.choiceStr = coistr.choiceStr1
						group by betcs.betSn
						having sum(betcs.choiceCount) = @SUM and count(betcs.choiceCount) = @COUNT
					)	
                    Group by b.sn,b.comSn,g.sn,t.sn,c.sn,b.userSn
	               ,b.Odds,b.[money]
					,b.unitSn,c.isTrue,b.sn,t.outlay,t.promote,t.betnumber
                    
                        select cst.choiceStr1 as choiceStr,cst.trueCount as trueCount
						from dbo.games g
						 JOIN dbo.topics t ON t.gameSn=g.sn
						JOIN dbo.choices c ON c.topicSn=t.sn
						JOIN dbo.choiceStrs cst On cst.choiceSn = c.sn
						Where g.sn = @gameSn

						select ppr.*
						from dbo.prizepoolRecord ppr
						";

                    using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                    {
                        var gpayoutList = multi.Read<gdnpayoutDto>().ToList();
                        List<choiceStrDto> gdcount = multi.Read<choiceStrDto>().ToList();
                        List<PrizeDto> gdnppr = multi.Read<PrizeDto>().ToList();
                        float sumppr = 0;//平台抽稅：隱藏彩池
                        float allpiz = 0;//彩池

                        if (gpayoutList.Count() != 0)
                        {
                            foreach (var ppr in gdnppr)
                            {
                                //平台抽稅總合：隱藏彩池
                                sumppr += (ppr.type == 2) ? ppr.assets : (ppr.type == 4) ? -(ppr.assets):0;
                                //彩池總合
                                allpiz += (ppr.type == 1) ? ppr.assets : (ppr.type == 3) ? -ppr.assets : 0;                   
                            }
                            //判斷手續費是否大於預設支出額 是 為累加至彩池
                            allpiz = (sumppr > gpayoutList.Where(x => x.comSn == 1).FirstOrDefault().promote) ? sumppr : allpiz;
                           /* if (sumppr > gpayoutList.Where(x => x.comSn == 1).FirstOrDefault().promote)
                            {
                                allpiz = sumppr;
                            }*/

                            //正解數量計算 不包含大龍
                            int truecount = 0;
                            foreach (var gdc in gdcount)
                            {
                                truecount += (gdc.choiceStr != "5") ? gdc.truecount : 0;                              

                            }
                          

                            //寫入派彩欄位
                            foreach (var gpaylist in gpayoutList)
                            {
                                var config = new MapperConfiguration(cfg => cfg.CreateMap<gdnpayoutDto, payout>()
                                .IgnoreAllNonExisting().
                                ForMember(up => up.comSn, opt => opt.MapFrom(u => "1"))
                                );
                                config.AssertConfigurationIsValid();
                                var mapper = config.CreateMapper();
                                payout payouta = mapper.Map<payout>(gpaylist);
                          
                                    //進入計算 sumppr 隱藏彩金
                                    payouta.realMoney = gpaymoney(id, gpaylist.betSn, allpiz, truecount, sumppr, gpaylist.betnumber,gpaylist.outlay);
                                    
                                    //判斷彩金是否為0
                                    if (payouta.realMoney != 0)
                                    {
                                        //重新讀取是否有隱藏金
                                        if (truecount > 3)
                                            sumppr += ((sumppr - (float)payouta.realMoney) > 0) ? -(float)payouta.realMoney : 0;  
                                        
                                        payouta.unitSn = 2;
                                        payouta.isTrue = 1;
                                        db.payouts.Add(payouta);
                                        db.SaveChanges();

                                        //更新下注valid為3
                                        bet bets = db.bets.Where(p => p.sn == payouta.betSn && p.valid == 1).FirstOrDefault();
                                  
                                        if (bets != null)
                                        {
                                            bets.valid = 3;
                                            db.SaveChanges();
                                        }

                                    }                              


                            }

                            string query2 = @"
	                select p.*,u.userId
	                from payouts p
	                join users u on u.sn=p.userSn
	                join bets b on b.sn=p.betSn
	                where p.gameSn=@gameSn
	                and b.valid=3
";
                            using (var multi2 = cn.QueryMultiple(query2, new { gameSn = id }))
                            {
                                IEnumerable<payoutDto> pdlist = multi2.Read<payoutDto>();
                                if (pdlist.Count() > 0)
                                    payoutList = pdlist.ToList();

                        }


                            foreach (var gpaylist in gpayoutList)
                            {
                                //更新下注valid為3
                                bet bets = db.bets.Where(p => p.sn == gpaylist.betSn && p.valid == 3).FirstOrDefault();
                                
                                if (bets != null)
                                {
                                    bets.valid = 2;
                                    db.SaveChanges();
                                }
                            }

                            //存入不足彩金記錄
                            string query3 = @"
                            select ppr.unitSn,ppr.gameSn,ppr.topicSn,ppr.choiceSn,SUM(ppr.assets) as assets
						    from dbo.prizepoolRecord ppr
                            where  ppr.type = 3 and ppr.gameSn = @gameSn
						    group by ppr.unitSn,ppr.gameSn,ppr.topicSn,ppr.choiceSn";
                            using (var multi3 = cn.QueryMultiple(query3, new { gameSn = id }))
                            {                            
                                    PrizeDto piz = multi3.Read<PrizeDto>().FirstOrDefault();                                                                   
                                    //不足50萬自動補彩金
                                    bool judge = (piz != null)? true : false;
                                    if (judge)
                                    {
                                        var config = new MapperConfiguration(cfg => cfg.CreateMap<PrizeDto, prizepoolRecord>()
                                           .IgnoreAllNonExisting().
                                           ForMember(up => up.userSn, opt => opt.MapFrom(u => 0)).
                                           ForMember(up => up.inpdate, opt => opt.MapFrom(u => DateTime.Now)).
                                           ForMember(up => up.type, opt => opt.MapFrom(u => 1))
                                       );
                                        config.AssertConfigurationIsValid();
                                        var mapper = config.CreateMapper();
                                        prizepoolRecord pizeuser = mapper.Map<prizepoolRecord>(piz);                                      
                                        db.prizepoolRecords.Add(pizeuser);
                                        db.SaveChanges();

                                    }

                            }
                               

                        }



                    }
                }
                return payoutList;
           



                
        }

        //龍的傳人計算
        public float gpaymoney(int id, int betid, float allpize, int truecount,float notlookpize,int betnumber,int outlay)
        {
            try
            { 
                float payoutmoney = 0;
                using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
                {
                    string query = @"  
                    
                    
					    select b.sn as betSn,b.comSn,g.sn as gameSn,t.sn as topicSn,c.sn as choiceSn,b.userSn
	                    ,b.Odds,b.[money],
					    b.unitSn,c.isTrue,bc.choiceCount,cst.trueCount,cst.choiceStr1 as choiceStr,t.outlay,t.promote
						    from dbo.games g
						     JOIN dbo.topics t ON t.gameSn=g.sn
						    JOIN dbo.choices c ON c.topicSn=t.sn
						    JOIN dbo.choiceStrs cst On cst.choiceSn = c.sn
						    JOIN dbo.bets b ON b.choiceSn=c.sn
						    JOIN dbo.betCounts bc ON bc.betSn = b.sn
						    Where  g.betModel=5 And cst.trueCount != 0 and b.sn = @betsn
						    AND b.money>0  AND  bc.choiceCount LIKE
                              CASE WHEN cst.choiceStr1 = '5' and cst.trueCount >= 4 and cst.trueCount != 100 THEN 
                               4
                              ELSE
                                cst.trueCount
                              END				
                              AND
					    bc.choiceStr = cst.choiceStr1 
					    and not exists(
		                    select *
		                    from payouts p
		                    where p.betSn=b.sn
	                    ) and exists(
						    select *
						    from dbo.games g
						    JOIN dbo.choiceStrs cst On cst.choiceSn = c.sn
						    JOIN dbo.betCounts bc ON bc.betSn = b.sn
						    where cst.choiceStr1 != '5' and bc.choiceStr != '5' 
						    having sum(cst.trueCount) = sum(bc.choiceCount)
					    )
                             select *
						    from dbo.bonus bs					 
						    Where bs.gamesn=@gameSn";
                   
						


                    using (var multi = cn.QueryMultiple(query, new { betsn = betid,gameSn = id }))
                    {
                        var gpayoutList = multi.Read<gdnpayoutDto>().ToList();
                        List<bonusDto> gbonus = multi.Read<bonusDto>().ToList();

                        if (truecount == 100)
                           truecount = 0;


                        //判斷獎勵類型及數值
                        var rate = gbonus.Where(x => x.Quantity == truecount && x.pool == false).FirstOrDefault(); //倍率獎勵
                        var rakeTrue = gbonus.Where(x => x.Quantity == truecount && x.pool == true && x.bonus ==true).FirstOrDefault(); // 有大龍彩池獎勵
                        var rakeFalse = gbonus.Where(x => x.Quantity == truecount && x.pool == true).FirstOrDefault(); //無大龍彩池獎勵
                        var valid = 0;
                   
                        foreach (var gplist in gpayoutList)
                        {
                            int moneyrate = (int)gplist.money / betnumber;//下注倍數
                            int gpjudge = (gplist.choiceStr == "5" && rakeTrue != null) ? 1 : (rakeFalse != null) ? 0 : 2; // 判斷計算類型

                            // 1:有大龍彩池獎勵計算 0:無大龍彩池獎勵計算
                            switch(gpjudge)
                            {
                                case 0:
                                    //無大龍：目前總彩 * 獎勵 / 100 
                                    payoutmoney = allpize * (float)rakeFalse.BonusRatio / 100;
                                    valid = 2;
                                    break;
                                case 1:
                                    //有大龍：目前總彩 * 有大龍獎勵 / 100                         
                                    payoutmoney = allpize * (float)rakeTrue.BonusRatio / 100;
                                    valid = 1;
                                    break;
                                default:
                                    // 皆不是都以倍率獎勵：下注額 * 倍率
                                    payoutmoney = gplist.money * (float)rate.BonusRatio; // 不扣手續費
                                                                             //* ((float)outlay / 100) //扣手續費
                                    break;

                            }
                           
                        }

                        var config = new MapperConfiguration(cfg => cfg.CreateMap<gdnpayoutDto, prizepoolRecord>()
                            .IgnoreAllNonExisting().
                            ForMember(up => up.assets, opt => opt.MapFrom(u => payoutmoney)).
                            ForMember(up => up.inpdate, opt => opt.MapFrom(u => DateTime.Now))
                            );
                        config.AssertConfigurationIsValid();
                        var mapper = config.CreateMapper();
                        prizepoolRecord pizeuser = mapper.Map<prizepoolRecord>(gpayoutList.FirstOrDefault());

                        int judge = (valid != 0) ? 1 : 0;//判斷 1:彩池計算 0:倍率計算

                        switch (judge)
                        {
                            case 1: //彩池模式寫入
                            //判斷計算式 notlookpize= 隱藏彩金 pizeuser.assets = 要派彩金額
                            judge = ((notlookpize - (float)pizeuser.assets) < 0) ? (int)(Math.Abs(notlookpize -(float)pizeuser.assets) / Math.Abs(notlookpize -(float)pizeuser.assets)) : 0;
                            
                            //分記錄類型type 3為彩池支出 4為隱彩支出 1為彩池補充
                            switch(judge)
                            {
                                case 0:
                                    pizeuser.type = 4;
                                    db.prizepoolRecords.Add(pizeuser);
                                    db.SaveChanges();
                                    break;
                                case -1:
                                    pizeuser.type = 3;
                                    db.prizepoolRecords.Add(pizeuser);
                                    db.SaveChanges();
                                    break;
                                case 1:
                                    
                                    float pool = 0;

                                    if (notlookpize > 0)
                                    {
                                            //隱彩記錄
                                            pizeuser.assets = notlookpize;
                                            pizeuser.type = 4;
                                            db.prizepoolRecords.Add(pizeuser);
                                            db.SaveChanges();
                                            //支出記錄
                                            pizeuser.assets = notlookpize / 9;
                                            pizeuser.type = 5;
                                            db.prizepoolRecords.Add(pizeuser);
                                            db.SaveChanges();
                                            pool = notlookpize + (notlookpize / 9);

                                    }
                                    
                                    //重新寫入彩池記錄                                
                                    pizeuser.type = 3;
                                    pizeuser.assets = payoutmoney - pool;                               
                                    db.prizepoolRecords.Add(pizeuser);
                                    db.SaveChanges();
                                    break;                                 
                            }
                                
                                break;
                            default:      //皆不是為倍率寫入記錄                         
                                pizeuser.type = 4;
                                db.prizepoolRecords.Add(pizeuser);
                                db.SaveChanges();
                                break;



                        }                    




                    }
                }
                return payoutmoney;
            }
            catch
            {
                return 0;
            }

        }

        public bool DeleteLtopic(int t)
        {
            var instance = db.topics.Where(x=>x.sn == t).FirstOrDefault();
            if (instance == null)
            {
                return false;
                throw new ArgumentNullException("instance");
                
            }
            else
            {
                db.Entry(instance).State = EntityState.Deleted;
                db.SaveChanges();
                return true;
            }
        }



        public List<payoutDto> paysRollback(int id)
        {
            List<payoutDto> payoutList = new List<payoutDto>();
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {

                string query = @"
					select p.*,u.userId
					from payouts p
					join users u on u.sn=p.userSn
					where p.gameSn=@gameSn
";
                using (var multi = cn.QueryMultiple(query, new { gameSn = id }))
                {
                    IEnumerable<payoutDto> pdlist = multi.Read<payoutDto>();
                    if (pdlist.Count() > 0)
                    {
                        payoutList = pdlist.ToList();
                    }
                }


                string sql = @"
	                UPDATE b SET b.valid=1
	                FROM dbo.games g
	                JOIN dbo.topics t ON t.gameSn=g.sn
	                JOIN dbo.choices c ON c.topicSn=t.sn
	                JOIN dbo.bets b ON b.choiceSn=c.sn
	                WHERE g.sn=@gameSn
	                AND EXISTS(
		                SELECT * FROM dbo.payouts pp
		                WHERE pp.gameSn=g.sn
	                )
	                AND b.valid=2

                    delete p
                    from payouts p
                    where p.gameSn=@gameSn
                ";
                cn.Open();
                SqlCommand myCommand = new SqlCommand(sql, cn);
                myCommand.Parameters.Add("@gameSn", SqlDbType.Int);
                myCommand.Parameters["@gameSn"].Value = id;


                myCommand.ExecuteReader();
            }
            return payoutList;
        }


            private bool gameExists(int id)
        {
            return db.games.Count(e => e.sn == id) > 0;
        }


    }
}