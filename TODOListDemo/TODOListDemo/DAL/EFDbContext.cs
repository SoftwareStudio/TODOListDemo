using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TODOListDemo.Models;

namespace TODOListDemo.DAL
{
    public class EFDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}