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
        private DbCtxt _context;
        private SqliteConnection _connection;

        private IConfigurationRoot _config;
        public UnitTest11()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<DbCtxt>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging(true)
                .Options;

            // Create the schema in the database
            _context = new DbCtxt(options);
            _context.Database.EnsureCreated();


        }

        [TestMethod]
        public void Searchy()
        {
           // var _data = new List<string>() { "Files", "Fil" };
            
            SearchyQuery  _sq= new SearchyQuery() ;
            SearchyCondition _sc = new SearchyCondition( new SearchyFilter { Value = new List<string>() { "Files", "Fil" }, Rule=SearchyRule.EqualsToList, Field= "Name" });
            _sq.Conditions.Add(_sc);  
            var _data =_context.UseDms().Repositories.Search(_sq).ToList();

            Assert.AreNotEqual(7, _data.Count);
        }

        [TestMethod]
        public void SearchyWithOrderBy()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyCondition _sc = new SearchyCondition(new SearchyFilter { Value = "Files", Rule = SearchyRule.Contains, Field = "Name" });
            _sq.Conditions.Add(_sc);
            List<SearchyOrder> _ob = new List<SearchyOrder> { new SearchyOrder("Name", SearchyOrder.Order.ASC) };

            var _data = _context.UseDms().Repositories.Search(_sq, _ob, 0, 0).ToList();
            Assert.AreEqual(7, _data.Count);

            

        }

        [TestMethod]
        public void SearchyWithPaging()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyCondition _sc = new SearchyCondition(new SearchyFilter { Value = "Files", Rule = SearchyRule.Contains, Field = "Name" });
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
            _fol.Add(new SearchyFilter { Value = "Files", Rule = SearchyRule.Contains, Field = "Name" });
            _fol.Add(new SearchyFilter { Value = 2000, Rule = SearchyRule.EqualsTo, Field = "MaxDocSize" });

            SearchyCondition _sc = new SearchyCondition(_fol);
            _sq.Conditions.Add(_sc);

            var _data = _context.UseDms().Repositories.Search(_sq).ToList();
            Assert.AreEqual(2, _data.Count);

        }

        [TestMethod]
        public void SearchyWithOrFilter()
        {
            SearchyQuery _sq = new SearchyQuery();
            SearchyCondition _sc = new SearchyCondition(new SearchyFilter { Value = "Files", Rule = SearchyRule.Contains, Field = "Name" });
            _sq.Conditions.Add(_sc);
            SearchyCondition _sc2 = new SearchyCondition(new SearchyFilter { Value = 2000, Rule = SearchyRule.EqualsTo, Field = "MaxDocSize" });

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
