using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SITAPI.Models.Repository
{
    public class choiceStrRepository
    {
        private sitdbEntities db = new sitdbEntities();

        public choiceStr setData(choiceStr c)
        {
            if (db.choiceStrs.Where(p=>p.choiceSn==c.choiceSn).Count()== 0)
                return AddNew(c);
            else
                return Edit(c);
        }

        private choiceStr AddNew(choiceStr c)
        {
            db.choiceStrs.Add(c);
            db.SaveChanges();
            return c;
        }

        private choiceStr Edit(choiceStr c)
        {
            choiceOdd co = db.choiceOdds.Where(p => p.choiceSn == c.choiceSn).FirstOrDefault();
            //co.Odds = c.Odds;
            db.SaveChanges();
            return c;
        }
    }
}