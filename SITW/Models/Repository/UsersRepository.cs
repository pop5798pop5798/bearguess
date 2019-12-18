using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SITW.Models.Repository
{
    public class UserRepository
    {
        private sitwEntities Db = new sitwEntities();
        public UserRepository()
        {

        }
        public void addlive(string user)
        {
            Live l = new Live();
            l.username = user;
            l.valid = 1;

            Db.Live.Add(l);
            Db.SaveChanges();

        }

        public void addbug(ProblemViewModel pb)
        {
            Problem b = new Problem();
            b.userId = pb.userId;
            b.valid = pb.valid;
            b.title = pb.title;
            b.inpdate = pb.inpdate;
            b.comment = pb.comment;
            b.image_1 = pb.image_1;
            b.image_2 = pb.image_2;
            b.image_3 = pb.image_3;
            Db.Problem.Add(b);
            Db.SaveChanges();

        }


        public void applylive(LiveApply la)
        {
            Db.LiveApply.Add(la);
            Db.SaveChanges();

        }

        public List<Live> getlive()
        {

            return Db.Live.ToList();

        }

        public List<Recommend> getRecommend()
        {

            return Db.Recommend.ToList();

        }
        public Recommend getRecommendValid(string user)
        {

            return Db.Recommend.Where(x=>x.userId == user).FirstOrDefault();

        }


        public RecommendStart getRecommendStart(int r)
        {

            return Db.RecommendStart.Where(x=>x.id == r).FirstOrDefault();

        }

        public RecommendStart getRecommendStartV(int r)
        {

            return Db.RecommendStart.Where(x => x.id == r && x.edate > DateTime.Now).FirstOrDefault();

        }

        public void CreateRecommend(string user,string code)
        {
            if (user == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Recommend instance = new Recommend();
                instance.code = code;
                instance.userId = user;
                instance.ReId = 1;
                Db.Recommend.Add(instance);
                Db.SaveChanges();

            }


        }

        public void CreateIP(string user, string ip)
        {
            if (user == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                IPRecord instance = new IPRecord();
                instance.userId = user;
                instance.IP = ip;
                instance.inpdate = DateTime.Now;
                Db.IPRecord.Add(instance);
                Db.SaveChanges();

            }


        }

        public List<Users> GetAll()
        {
            return Db.Users.ToList();


        }




        public bool checkPhoneNumber(string phoneNum)
        {
            bool isTrue = false;
            string query = @"
            select count(*) as sendCount
            from Users u
            where u.PhoneNumber=@phoneNum
			and u.PhoneNumberConfirmed=1
            ";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {

                cn.Open();
                using (SqlCommand command = new SqlCommand(query, cn))
                {


                    try
                    {
                        command.Parameters.AddWithValue("@phoneNum", phoneNum);
                        int _count = (int)command.ExecuteScalar();
                        if (_count >= 1)
                            isTrue = true;
                    }
                    catch (Exception e)
                    {
                        //抓錯誤訊息
                        return false;
                    }
                    finally
                    {
                    }
                }

                return isTrue;

            }

        }

        public bool checkUserHavePhone(string userId)
        {
            bool isTrue = false;
            string query = @"
            select count(*) as phoneCount
            from Users u
            where Id=@userId
            and u.PhoneNumber is not null
            and u.PhoneNumberConfirmed=1
            ";
            using (SqlConnection cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SQLData"].ConnectionString.ToString()))
            {

                cn.Open();
                using (SqlCommand command = new SqlCommand(query, cn))
                {


                    try
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int _count = (int)command.ExecuteScalar();
                        if (_count >= 1)
                            isTrue = true;
                    }
                    catch (Exception e)
                    {
                        //抓錯誤訊息
                        return false;
                    }
                    finally
                    {
                    }
                }

                return isTrue;

            }
        }
    }
}