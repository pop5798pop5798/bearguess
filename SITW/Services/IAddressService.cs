using SITW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SITW.Services
{
    public interface IAddressService : IDisposable
    {
        bool IsExists(Expression<Func<AddressRecord, bool>> predicate);
        int TotalCount(Expression<Func<AddressRecord, bool>> predicate);

        AddressRecord FindOne(int id);
        AddressRecord FindOneByPostalCode(int postalCode);

        IQueryable<AddressRecord> FindAll();
        IQueryable<AddressRecord> Find(Expression<Func<AddressRecord, bool>> predicate);

        Dictionary<string, string> GetAllCities();
        Dictionary<string, string> GetAllCityDictinoary();
        Dictionary<string, string> GetCountyByCityName(string cityName);
    }
}
