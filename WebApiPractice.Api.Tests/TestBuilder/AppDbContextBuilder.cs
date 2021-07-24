using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests.TestBuilder
{
    public class AppDbContextBuilder
    {
        private DbConnection _sqlConnection;
        private readonly List<Customer> _customers;
        private readonly List<Note> _customerNotes;

        public AppDbContextBuilder()
        {
            this._customers = new List<Customer>();
            this._customerNotes = new List<Note>();
        }

        public AppDbContextBuilder UseInMemorySqlite()
        {
            this._sqlConnection = new SqliteConnection("DataSource=:memory:");
            return this;
        }

        public AppDbContextBuilder WithCustomers(List<Customer> customers)
        {
            this._customers.AddRange(customers);
            return this;
        }

        public AppDbContextBuilder WithNotes(List<Note> notes)
        {
            this._customerNotes.AddRange(notes);
            return this;
        }

        public AppDbContext Build()
        {
            if (_sqlConnection is null)
                throw new Exception("SqlConnections has to be initialized");

            if (_sqlConnection.State == System.Data.ConnectionState.Closed)
                _sqlConnection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_sqlConnection).Options;
            var dbContext = new AppDbContext(options);
            dbContext.Database.EnsureCreated();

            dbContext.Customers.AddRange(this._customers);
            dbContext.Notes.AddRange(this._customerNotes);
            dbContext.SaveChanges();
            return dbContext;
        }
    }
}

