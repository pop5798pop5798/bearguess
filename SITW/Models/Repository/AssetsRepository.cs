﻿using Dapper;
using SITDto;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SITW.Models.Repository
{
    public class AssetsRepository
    {
        private sitwEntities db = new sitwEntities();

        /// <summary>
        /// 派彩
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddAssetsByPay(List<payoutDto> pdList)
        {
            if (pdList == null || pdList.Count == 0)
                return false;
            string dataquery = "";
            foreach(payoutDto pd in pdList)
            {
                dataquery += (dataquery == "" ? "" : "union all ");
                dataquery += "select '" + pd.userId + "'," + pd.unitSn + "," + pd.realMoney + "," + pd.gameSn + "," + pd.topicSn + "," + pd.choiceSn + "," + (pd.isTrue.HasValue ? pd.isTrue.Value.ToString() : "1");

                //當鮭魚沒值時
                Assets assDb = db.Assets.Where(p => p.UserId == pd.userId && p.unitSn == pd.unitSn).FirstOrDefault();
                if (assDb == null)
                {
                    assDb = new Assets
                    {
                        UserId = pd.userId,
                        unitSn = pd.unitSn,
                        assets = 0
                    };
                    db.Assets.Add(assDb);
                    db.SaveChanges();
                }
            }

            string query = @"
BEGIN TRY

    BEGIN TRAN
    DECLARE @Assets table
               ([UserId] nvarchar(128)
               ,[unitSn] int
               ,[assets] float
		       ,[gameSn] int
			   ,[topicSn] int
			   ,[choiceSn] int
               ,[isTrue] tinyint)
    INSERT @Assets
    (
        UserId,
        unitSn,
        assets,
	    gameSn,
		topicSn,
		choiceSn,
        isTrue
    )
	    " + dataquery + @"



    UPDATE a2 SET a2.assets=round(a2.assets+a1.assets,2)
    FROM (
	    SELECT a.UserId,a.unitSn,a.gameSn,sum(a.assets) AS assets
	    FROM @Assets a
		where a.assets>0
	    GROUP BY a.UserId,a.unitSn,a.gameSn
    ) a1
    JOIN dbo.Assets a2 ON a2.UserId=a1.UserId AND a2.unitSn=a1.unitSn

    INSERT dbo.Assets
    (
        UserId,
        unitSn,
        assets
    )
    SELECT a.UserId,a.unitSn,sum(a.assets) AS assets
    FROM @Assets a
    WHERE NOT EXISTS(
	    SELECT * FROM dbo.Assets a2 WHERE a2.UserId=a.UserId AND a2.unitSn=a.unitSn
    )
    GROUP BY a.UserId,a.unitSn


    INSERT dbo.AssetsRecord
    (
        UserId,
        unitSn,
        assets,
        type,
	    gameSn,
        topicSn,
        choiceSn
    )
    SELECT a.UserId,a.unitSn,a.assets AS assets
    ,(case a.isTrue when 2 then 4 else 1 end) AS type,a.gameSn
    ,topicSn,choiceSn
    FROM @Assets a


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }


        /// <summary>
        /// 直播派彩
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddLiveAssetsByPay(List<payoutDto> pdList)
        {
            if (pdList == null || pdList.Count == 0)
                return false;
            string dataquery = "";
            foreach (payoutDto pd in pdList)
            {
                dataquery += (dataquery == "" ? "" : "union all ");
                dataquery += "select '" + pd.userId + "'," + pd.unitSn + "," + pd.realMoney + "," + pd.gameSn + "," + pd.topicSn + "," + pd.choiceSn + "," + (pd.isTrue.HasValue ? pd.isTrue.Value.ToString() : "1");
            }

            string query = @"
BEGIN TRY

    BEGIN TRAN
    DECLARE @Assets table
               ([UserId] nvarchar(128)
               ,[unitSn] int
               ,[assets] float
		       ,[gameSn] int
			   ,[topicSn] int
			   ,[choiceSn] int
               ,[isTrue] tinyint)
    INSERT @Assets
    (
        UserId,
        unitSn,
        assets,
	    gameSn,
		topicSn,
		choiceSn,
        isTrue
    )
	    " + dataquery + @"



    UPDATE a2 SET a2.assets=round(a2.assets+a1.assets,2)
    FROM (
	    SELECT a.UserId,a.unitSn,a.gameSn,sum(a.assets) AS assets
	    FROM @Assets a
		where a.assets>0
	    GROUP BY a.UserId,a.unitSn,a.gameSn
    ) a1
    JOIN dbo.Assets a2 ON a2.UserId=a1.UserId AND a2.unitSn=a1.unitSn

    INSERT dbo.Assets
    (
        UserId,
        unitSn,
        assets
    )
    SELECT a.UserId,a.unitSn,sum(a.assets) AS assets
    FROM @Assets a
    WHERE NOT EXISTS(
	    SELECT * FROM dbo.Assets a2 WHERE a2.UserId=a.UserId AND a2.unitSn=a.unitSn
    )
    GROUP BY a.UserId,a.unitSn


    INSERT dbo.AssetsRecord
    (
        UserId,
        unitSn,
        assets,
        type,
	    gameSn,
        topicSn,
        choiceSn
    )
    SELECT a.UserId,a.unitSn,a.assets AS assets
    ,(case a.isTrue when 2 then 4 else 1 end) AS type,a.gameSn
    ,topicSn,choiceSn
    FROM @Assets a


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }


        /// <summary>
        /// 簡單猜猜派彩
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddGuessAssetsByPay(List<payoutDto> pdList)
        {
            if (pdList == null || pdList.Count == 0)
                return false;
            string dataquery = "";
            foreach (payoutDto pd in pdList)
            {
                pd.unitSn = 1;
                dataquery += (dataquery == "" ? "" : "union all ");
                dataquery += "select '" + pd.userId + "'," + pd.unitSn + "," + pd.realMoney + "," + pd.gameSn + "," + pd.topicSn + "," + pd.choiceSn + "," + (pd.isTrue.HasValue ? pd.isTrue.Value.ToString() : "1");
            }

            string query = @"
BEGIN TRY

    BEGIN TRAN
    DECLARE @Assets table
               ([UserId] nvarchar(128)
               ,[unitSn] int
               ,[assets] float
		       ,[gameSn] int
			   ,[topicSn] int
			   ,[choiceSn] int
               ,[isTrue] tinyint)
    INSERT @Assets
    (
        UserId,
        unitSn,
        assets,
	    gameSn,
		topicSn,
		choiceSn,
        isTrue
    )
	    " + dataquery + @"



    UPDATE a2 SET a2.assets=round(a2.assets+a1.assets,2)
    FROM (
	    SELECT a.UserId,a.unitSn,a.gameSn,sum(a.assets) AS assets
	    FROM @Assets a
		where a.assets>0
	    GROUP BY a.UserId,a.unitSn,a.gameSn
    ) a1
    JOIN dbo.Assets a2 ON a2.UserId=a1.UserId AND a2.unitSn=a1.unitSn

    INSERT dbo.Assets
    (
        UserId,
        unitSn,
        assets
    )
    SELECT a.UserId,a.unitSn,sum(a.assets) AS assets
    FROM @Assets a
    WHERE NOT EXISTS(
	    SELECT * FROM dbo.Assets a2 WHERE a2.UserId=a.UserId AND a2.unitSn=a.unitSn
    )
    GROUP BY a.UserId,a.unitSn


    INSERT dbo.AssetsRecord
    (
        UserId,
        unitSn,
        assets,
        type,
	    gameSn,
        topicSn,
        choiceSn
    )
    SELECT a.UserId,a.unitSn,a.assets AS assets
    ,(case a.isTrue when 2 then 4 else 1 end) AS type,a.gameSn
    ,topicSn,choiceSn
    FROM @Assets a


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }

        /// <summary>
        /// 派彩返還
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddAssetsByPayRollback(List<payoutDto> pdList)
        {
            if (pdList == null || pdList.Count == 0)
                return false;
            string dataquery = "";
            foreach (payoutDto pd in pdList)
            {
                dataquery += (dataquery == "" ? "" : "union all ");
                dataquery += "select '" + pd.userId + "'," + pd.unitSn + "," + pd.realMoney*-1 + "," + pd.gameSn + "," + pd.topicSn + "," + pd.choiceSn + "," + (pd.isTrue.HasValue ? pd.isTrue.Value.ToString() : "1");
            }

            string query = @"
BEGIN TRY

    BEGIN TRAN
    DECLARE @Assets table
               ([UserId] nvarchar(128)
               ,[unitSn] int
               ,[assets] float
		       ,[gameSn] int
			   ,[topicSn] int
			   ,[choiceSn] int
               ,[isTrue] tinyint)
    INSERT @Assets
    (
        UserId,
        unitSn,
        assets,
	    gameSn,
		topicSn,
		choiceSn,
        isTrue
    )
	    " + dataquery + @"



    UPDATE a2 SET a2.assets=round(a2.assets+a1.assets,2)
    FROM (
	    SELECT a.UserId,a.unitSn,a.gameSn,sum(a.assets) AS assets
	    FROM @Assets a
	    GROUP BY a.UserId,a.unitSn,a.gameSn
    ) a1
    JOIN dbo.Assets a2 ON a2.UserId=a1.UserId AND a2.unitSn=a1.unitSn
    WHERE EXISTS(
	    SELECT *
	    FROM dbo.AssetsRecord ar
	    WHERE ar.gameSn=a1.gameSn AND ar.UserId=a1.UserId
        and ar.type=1
    )

    INSERT dbo.Assets
    (
        UserId,
        unitSn,
        assets
    )
    SELECT a.UserId,a.unitSn,sum(a.assets) AS assets
    FROM @Assets a
    WHERE NOT EXISTS(
	    SELECT * FROM dbo.Assets a2 WHERE a2.UserId=a.UserId AND a2.unitSn=a.unitSn
    )
    AND EXISTS(
	    SELECT *
	    FROM dbo.AssetsRecord ar
	    WHERE ar.gameSn=a.gameSn AND ar.UserId=a.UserId
    )
    GROUP BY a.UserId,a.unitSn


    INSERT dbo.AssetsRecord
    (
        UserId,
        unitSn,
        assets,
        type,
	    gameSn,
        topicSn,
        choiceSn
    )
    SELECT a.UserId,a.unitSn,a.assets AS assets
    ,-4 AS type,a.gameSn
    ,topicSn,choiceSn
    FROM @Assets a


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }




        /// <summary>
        /// 無派彩返還
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddAssetsByRollback(int gamesn)
        {         

            string query = @"
BEGIN TRY

    BEGIN TRAN
     
    INSERT dbo.AssetsRecord(UserId,unitSn,gameSn,topicSn,choiceSn,assets,type)
SELECT UserId,unitSn,gameSn,topicSn,choiceSn,abs(assets) as assets,(5) as type
FROM AssetsRecord ar
WHERE ar.gameSn ='" + gamesn  + @"' and type = -1


UPDATE a2
SET a2.assets = ROUND(ar1.assets+a2.assets,2)
FROM(SELECT ar.UserId,ar.unitSn,ar.gameSn,sum(ar.assets) as assets
		FROM dbo.AssetsRecord ar
		WHERE ar.gameSn ='" + gamesn + @"' and ar.type = 5
		GROUP BY ar.UserId,ar.unitSn,ar.gameSn) ar1
JOIN dbo.Assets a2 ON a2.UserId = ar1.UserId and a2.unitSn = ar1.unitSn
WHERE EXISTS(
		SELECT *
	    FROM dbo.AssetsRecord ar
	    WHERE ar.gameSn=ar1.gameSn AND ar.UserId=ar1.UserId
        and ar.type=5
)


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }

        /// <summary>
        /// 直播主無派彩返還
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddLiveAssetsByRollback(int topicsn)
        {

            string query = @"
BEGIN TRY

    BEGIN TRAN
     
    INSERT dbo.AssetsRecord(UserId,unitSn,gameSn,topicSn,choiceSn,assets,type)
SELECT UserId,unitSn,gameSn,topicSn,choiceSn,abs(assets) as assets,(5) as type
FROM AssetsRecord ar
WHERE ar.topicSn ='" + topicsn + @"' and type = -1


UPDATE a2
SET a2.assets = ROUND(ar1.assets+a2.assets,2)
FROM(SELECT ar.UserId,ar.unitSn,ar.gameSn,sum(ar.assets) as assets
		FROM dbo.AssetsRecord ar
		WHERE ar.topicSn ='" + topicsn + @"' and ar.type = 5
		GROUP BY ar.UserId,ar.unitSn,ar.gameSn) ar1
JOIN dbo.Assets a2 ON a2.UserId = ar1.UserId and a2.unitSn = ar1.unitSn
WHERE EXISTS(
		SELECT *
	    FROM dbo.AssetsRecord ar
	    WHERE ar.gameSn=ar1.gameSn AND ar.UserId=ar1.UserId
        and ar.type=5
)


	COMMIT
END TRY
BEGIN CATCH
    ROLLBACK
END CATCH;

";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                cn.Open();
                SqlCommand myCommand = new SqlCommand(query, cn);

                myCommand.ExecuteReader();
            }
            return true;
        }

        /// <summary>
        /// 直播主派彩返還
        /// </summary>
        /// <param name="pdList"></param>
        /// <returns></returns>
        public bool AddLiveAssetsPayByRollback(int topicsn)
        {
            List<AssetsRecord> arm = new List<AssetsRecord>();
            var ar = db.AssetsRecord.Where(x => x.topicSn == topicsn).ToList();
            var arwin = ar.Where(x => x.type == 1).ToList();
            foreach(var a in ar)
            {
                var tg = false;
                foreach (var aw in arwin)
                {
                   
                    if(aw.UserId == a.UserId && aw.choiceSn == a.choiceSn)
                    {
                        tg = true;

                    }

                }

                if(!tg)
                {
                    var aa = db.Assets.Where(x => x.UserId == a.UserId && x.unitSn == 1).FirstOrDefault();
                    aa.assets += Math.Abs(a.assets);
                    db.Entry(aa).State = EntityState.Modified;

                    a.type = 5;
                    db.AssetsRecord.Add(a);
                    db.SaveChanges();

                }



            }



            return true;
        }




        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="bet">下注資料</param>
        /// <returns></returns>
        public bool AddAssetsByBet(betDto bet)
        {
            Assets assDb = db.Assets.Where(p => p.UserId == bet.userId && p.unitSn == bet.unitSn).FirstOrDefault();
            double fAssets = (assDb == null ? 0 : assDb.assets);
            if (assDb == null)
            {
                assDb = new Assets
                {
                    UserId = bet.userId,
                    unitSn = bet.unitSn.Value,
                    assets = fAssets + bet.money.Value * -1
                };
                db.Assets.Add(assDb);
            }
            else
            {
                assDb.assets += bet.money.Value * -1;
            }
            db.SaveChanges();

            AssetsRecord assr = new AssetsRecord
            {
                UserId = bet.userId,
                unitSn = bet.unitSn.Value,
                assets = bet.money.Value * -1,
                gameSn = bet.gameSn,
                topicSn = bet.topicSn,
                choiceSn = bet.choiceSn,
                inpdate = DateTime.Now,
                type = -1
            };
            db.AssetsRecord.Add(assr);
            db.SaveChanges();


            return true;
        }

        /// <summary>
        /// 給予金錢，任務達成、管理者增加金幣功能
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public bool AddAssetsByAssets(AssetsRecord ar)
        {
            Assets assDb = db.Assets.Where(p => p.UserId == ar.UserId && p.unitSn == ar.unitSn).FirstOrDefault();
            double fAssets = (assDb == null ? 0 : assDb.assets);
            if (assDb == null)
            {
                assDb = new Assets
                {
                    UserId = ar.UserId,
                    unitSn = ar.unitSn,
                    assets = fAssets + ar.assets
                };
                db.Assets.Add(assDb);
            }
            else
            {
                assDb.assets += ar.assets;
            }
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw;
            }

            AssetsRecord assr = new AssetsRecord
            {
                UserId = ar.UserId,
                unitSn = ar.unitSn,
                assets = ar.assets,
                gameSn = ar.gameSn,
                inpdate = DateTime.Now,
                type = 2
            };
            db.AssetsRecord.Add(assr);
            db.SaveChanges();


            return true;
        }

        /// <summary>
        /// h5遊戲派彩給予金錢&&下注扣錢
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public bool Addh5gameByAssets(AssetsRecord ar)
        {
            Assets assDb = db.Assets.Where(p => p.UserId == ar.UserId && p.unitSn == ar.unitSn).FirstOrDefault();
            double fAssets = (assDb == null ? 0 : assDb.assets);
            if (assDb == null)
            {
                assDb = new Assets
                {
                    UserId = ar.UserId,
                    unitSn = ar.unitSn,
                    assets = fAssets + ar.assets
                };
                db.Assets.Add(assDb);
            }
            else
            {
                assDb.assets += ar.assets;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            AssetsRecord assr = new AssetsRecord
            {
                UserId = ar.UserId,
                unitSn = ar.unitSn,
                assets = ar.assets,
                gameSn = ar.gameSn,
                inpdate = DateTime.Now,
                type = ar.type,
                h5forValue = ar.h5forValue,
                h5score = ar.h5score
            };
            db.AssetsRecord.Add(assr);
            db.SaveChanges();


            return true;
        }

        //商城給予魚骨、兌換購買等功能
        public bool AddBearByAssets(ProductRecord ar)
        {
            Assets assDb = db.Assets.Where(p => p.UserId == ar.UserID && p.unitSn == ar.unitSn).FirstOrDefault();
            double fAssets = (assDb == null ? 0 : assDb.assets);
            if (assDb == null)
            {
                assDb = new Assets
                {
                    UserId = ar.UserID,
                    unitSn = (int)ar.unitSn,
                    assets = fAssets + (double)ar.assets
                };
                db.Assets.Add(assDb);
            }
            else
            {
                assDb.assets += (double)ar.assets;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            ProductRecord assr = new ProductRecord
            {
                UserID = ar.UserID,
                unitSn = ar.unitSn,
                assets = ar.assets,
                inpdate = DateTime.Now,
                type = ar.type,
                ProductId = ar.ProductId,
                recipient = ar.recipient,
                address = ar.address,
                phone = ar.phone
            };
            db.ProductRecord.Add(assr);
            db.SaveChanges();


            return true;
        }


        


        //轉帳系統功能
        public bool AddAssetsByTransfer(TransferRecords tr)
        {
            //轉出方增加
            Assets assDb = db.Assets.Where(p => p.UserId == tr.Transfer && p.unitSn == tr.unitSn).FirstOrDefault();
            double fAssets = (assDb == null ? 0 : assDb.assets);
            if (assDb == null)
            {
                assDb = new Assets
                {
                    UserId = tr.Transfer,
                    unitSn = tr.unitSn,
                    assets = fAssets + tr.credit * (100 - (double)tr.rake) / 100
                };
                db.Assets.Add(assDb);
            }
            else
            {
                assDb.assets += tr.credit * (100 - (double)tr.rake) / 100;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            //支出方扣除
            assDb = db.Assets.Where(p => p.UserId == tr.UserId && p.unitSn == tr.unitSn).FirstOrDefault();
            fAssets = (assDb == null ? 0 : assDb.assets);
            assDb.assets -= tr.credit;           
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            TransferRecords assr = new TransferRecords
            {
                UserId = tr.UserId,
                Transfer = tr.Transfer,
                unitSn = tr.unitSn,
                credit = tr.credit,
                type = tr.type,
                createDate = DateTime.Now,
                rake = tr.rake
            };         


            db.TransferRecords.Add(assr);
            db.SaveChanges();

            AssetsRecord assru = new AssetsRecord
            {
                UserId = tr.UserId,
                unitSn = tr.unitSn,
                assets = tr.credit,
                inpdate = DateTime.Now,
                type = -12
            };
            db.AssetsRecord.Add(assru);
            db.SaveChanges();
            AssetsRecord assrt = new AssetsRecord
            {
                UserId = tr.Transfer,
                unitSn = tr.unitSn,
                assets = tr.credit,
                inpdate = DateTime.Now,
                type = 12
            };
            db.AssetsRecord.Add(assrt);
            db.SaveChanges();


            return true;
        }


        public double getAssetsByUserID(string UserID,int unitSn)
        {
            Assets ass = db.Assets.Where(p => p.UserId == UserID && p.unitSn == unitSn).FirstOrDefault();
            return (ass == null ? 0 : ass.assets);
        }

        public List<AssetsViewModel> getAssetsListByUserID(string UserID, int? allUnit = 0)
        {
            List<AssetsViewModel> assetsList = new List<AssetsViewModel>();
            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
select isnull(a.assets,0) as Asset,cu.showStr as unitName,cu.sn as unitSn,cu.iconURL
from cfgUnit cu
left join Assets a on cu.sn=a.unitSn
and a.UserId=@userId
where (@allUnit=1 or a.UserId is not null)
";
                using (var multi = cn.QueryMultiple(query, new { userId = UserID , allUnit = allUnit }))
                {
                    assetsList = multi.Read<AssetsViewModel>().ToList();
                }
            }
            return assetsList;
        }

        public levelExpViewModel getExpByUserID(string UserID)
        {
            levelExpViewModel levm = new levelExpViewModel();
            using (IDbConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {
                string query = @"
SELECT * FROM getExpByUserID(@userId)
";
                using (var multi = cn.QueryMultiple(query, new { userId = UserID }))
                {
                    IEnumerable<levelExpViewModel> lelist = multi.Read<levelExpViewModel>();
                    levm = lelist.Single();
                }
            }
            return levm;

        }

    }
}