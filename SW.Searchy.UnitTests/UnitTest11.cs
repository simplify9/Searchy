using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
//using SW.Dms.Services;
using SW.Common;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using SW.Dms;
using System.Collections.Generic;

namespace SW.Searchy.UnitTests
{
    [TestClass]
    public class UnitTest11 : IDisposable
    {
        private DmsContext _context;
        private SqliteConnection _connection;

        private IConfigurationRoot _config;
        public UnitTest11()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<DmsContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging(true)
                .Options;

            // Create the schema in the database
            _context = new DmsContext(options);
            _context.Database.EnsureCreated();


        }

        [TestMethod]
        public void Searchy()
        {
           // var _data = new List<string>() { "Files", "Fil" };
            
            SearchyQuery  _sq= new SearchyQuery() ;
            SearchyConditon _sc = new SearchyConditon( new SearchyFilter { FilterFor = new List<string>() { "Files", "Fil" }, Rule=SearchyRule.EqualsToList, MemberName= "Name" });
            _sq.Conditions.Add(_sc);  
            var _data =_context.UseDms().Repositories.Search(_sq).ToList();

            Assert.AreNotEqual(7, _data.Count);
        }

        [TestMethod]
        public void SearchyWithOrderBy()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyConditon _sc = new SearchyConditon(new SearchyFilter { FilterFor = "Files", Rule = SearchyRule.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            List<SearchyOrder> _ob = new List<SearchyOrder> { new SearchyOrder("Name", SearchyOrder.Order.ASC) };

            var _data = _context.UseDms().Repositories.Search(_sq, _ob, 0, 0).ToList();
            Assert.AreEqual(7, _data.Count);

            

        }

        [TestMethod]
        public void SearchyWithPaging()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyConditon _sc = new SearchyConditon(new SearchyFilter { FilterFor = "Files", Rule = SearchyRule.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            List<SearchyOrder> _ob = new List<SearchyOrder> { new SearchyOrder("Name", SearchyOrder.Order.ASC) };

            var _data0 = _context.UseDms().Repositories.Search(_sq, _ob, 2, 0).ToList();
            Assert.AreEqual(2, _data0.Count);

            var _data1 = _context.UseDms().Repositories.Search(_sq, _ob,2, 1).ToList();
            Assert.AreEqual(2, _data1.Count);

            var _data2 = _context.UseDms().Repositories.Search(_sq, _ob, 3, 2).ToList();
            Assert.AreEqual(1, _data2.Count);
        }

        [TestMethod]
        public void SearchyWithAndFilter()
        {
            SearchyQuery _sq = new SearchyQuery();
            List<SearchyFilter> _fol = new List<SearchyFilter>();
            _fol.Add(new SearchyFilter { FilterFor = "Files", Rule = SearchyRule.Contains, MemberName = "Name" });
            _fol.Add(new SearchyFilter { FilterFor = 2000, Rule = SearchyRule.EqualsTo, MemberName = "MaxDocSize" });

            SearchyConditon _sc = new SearchyConditon(_fol);
            _sq.Conditions.Add(_sc);

            var _data = _context.UseDms().Repositories.Search(_sq).ToList();
            Assert.AreEqual(2, _data.Count);

        }

        [TestMethod]
        public void SearchyWithOrFilter()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyConditon _sc = new SearchyConditon(new SearchyFilter { FilterFor = "Files", Rule = SearchyRule.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            SearchyConditon _sc2 = new SearchyConditon(new SearchyFilter { FilterFor = 2000, Rule = SearchyRule.EqualsTo, MemberName = "MaxDocSize" });

            var _data = _context.UseDms().Repositories.Search(_sq).ToList();
            Assert.AreEqual(7, _data.Count);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
