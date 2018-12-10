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
        public void TestMethod1()
        {
            SearchQuery  _sq= new SearchQuery() ;
            SearchCondition _sc = new SearchCondition( new FilterByOptions { FilterFor= "Files", FilterOperator=FilterByOptions.FilterOperatorOptions.Contains, MemberName= "Name" });
            _sq.Conditions.Add(_sc);  
            var _data =_context.UseDms().Repositories.Search(_sq);

            var _count = _data.Count();  
        }


        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
