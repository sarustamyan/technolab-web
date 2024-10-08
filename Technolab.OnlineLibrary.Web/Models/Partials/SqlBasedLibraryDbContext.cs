﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Technolab.OnlineLibrary.Web.Models;

public partial class SqlBasedLibraryDbContext : ILibraryDbContext
{
    public SqlBasedLibraryDbContext(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConnectionString);
        optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }
}