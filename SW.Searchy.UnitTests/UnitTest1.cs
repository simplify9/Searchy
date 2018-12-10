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
    public class UnitTest1 : IDisposable
    {
        private DmsContext _context;
        private SqliteConnection _connection;

        private IConfigurationRoot _config;
        public UnitTest1()
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
            SearchQuery  _sq= new SearchQuery() ;
            SearchCondition _sc = new SearchCondition( new FilterByOptions { FilterFor= "Files", FilterOperator=FilterByOptions.FilterOperatorOptions.Contains, MemberName= "Name" });
            _sq.Conditions.Add(_sc);  
            var _data =_context.UseDms().Repositories.Search(_sq);

            var _count = _data.Count();  
        }

        [TestMethod]
        public void SearchyWithOrderBy()
        {
            SearchQuery _sq = new SearchQuery();
            SearchCondition _sc = new SearchCondition(new FilterByOptions { FilterFor = "Files", FilterOperator = FilterByOptions.FilterOperatorOptions.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            List<OrderByOptions> _ob = new List<OrderByOptions> { new OrderByOptions("Name", OrderByOptions.Order.ASC) };

            var _data = _context.UseDms().Repositories.Search(_sq, _ob, 0, 0).ToList();

        }

        [TestMethod]
        public void SearchyWithPaging()
        {
            SearchQuery _sq = new SearchQuery();
            SearchCondition _sc = new SearchCondition(new FilterByOptions { FilterFor = "Files", FilterOperator = FilterByOptions.FilterOperatorOptions.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            List<OrderByOptions> _ob = new List<OrderByOptions> { new OrderByOptions("Name", OrderByOptions.Order.ASC) };

            var _data0 = _context.UseDms().Repositories.Search(_sq, _ob, 2, 0).ToList();

            var _data1 = _context.UseDms().Repositories.Search(_sq, _ob,2, 1).ToList();


            var _data2 = _context.UseDms().Repositories.Search(_sq, _ob, 3, 2).ToList();

        }

        [TestMethod]
        public void SearchyWithAndFilter()
        {
            SearchQuery _sq = new SearchQuery();
            List<FilterByOptions> _fol = new List<FilterByOptions>();
            _fol.Add(new FilterByOptions { FilterFor = "Files", FilterOperator = FilterByOptions.FilterOperatorOptions.Contains, MemberName = "Name" });
            _fol.Add(new FilterByOptions { FilterFor = 2000, FilterOperator = FilterByOptions.FilterOperatorOptions.EqualsTo, MemberName = "MaxDocSize" });

            SearchCondition _sc = new SearchCondition(_fol);
            _sq.Conditions.Add(_sc);

            var _data = _context.UseDms().Repositories.Search(_sq).ToList();


        }

        [TestMethod]
        public void SearchyWithOrFilter()
        {
            SearchQuery _sq = new SearchQuery();
            SearchCondition _sc = new SearchCondition(new FilterByOptions { FilterFor = "Files", FilterOperator = FilterByOptions.FilterOperatorOptions.Contains, MemberName = "Name" });
            _sq.Conditions.Add(_sc);
            SearchCondition _sc2 = new SearchCondition(new FilterByOptions { FilterFor = 2000, FilterOperator = FilterByOptions.FilterOperatorOptions.EqualsTo, MemberName = "MaxDocSize" });

            var _data = _context.UseDms().Repositories.Search(_sq).ToList();

        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
