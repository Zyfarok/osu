﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Input.Bindings;
using osu.Game.IO;
using osu.Game.Rulesets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace osu.Game.Database
{
    public class OsuDbContext : DbContext
    {
        public DbSet<BeatmapInfo> BeatmapInfo { get; set; }
        public DbSet<BeatmapSetInfo> BeatmapSetInfo { get; set; }
        public DbSet<DatabasedKeyBinding> DatabasedKeyBinding { get; set; }
        public DbSet<FileInfo> FileInfo { get; set; }
        public DbSet<RulesetInfo> RulesetInfo { get; set; }
        private readonly string connectionString;

        public OsuDbContext()
        {
            connectionString = "DataSource=:memory:";
        }

        public OsuDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.UseLoggerFactory(new OsuDbLoggerFactory());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BeatmapInfo>().HasIndex(b => b.MD5Hash);
            modelBuilder.Entity<BeatmapSetInfo>().HasIndex(b => b.DeletePending);
            modelBuilder.Entity<DatabasedKeyBinding>().HasIndex(b => b.Variant);
            modelBuilder.Entity<DatabasedKeyBinding>().HasIndex(b => b.IntAction);
            modelBuilder.Entity<FileInfo>().HasIndex(b => b.Hash).IsUnique();
            modelBuilder.Entity<FileInfo>().HasIndex(b => b.ReferenceCount);
            modelBuilder.Entity<RulesetInfo>().HasIndex(b => b.Name).IsUnique();
            modelBuilder.Entity<RulesetInfo>().HasIndex(b => b.InstantiationInfo).IsUnique();
            modelBuilder.Entity<RulesetInfo>().HasIndex(b => b.Available);
        }

        private class OsuDbLoggerFactory : ILoggerFactory
        {
            #region Disposal

            public void Dispose()
            {
            }

            #endregion

            public ILogger CreateLogger(string categoryName) => new OsuDbLogger();

            public void AddProvider(ILoggerProvider provider) => new OsuDbLoggerProvider();

            private class OsuDbLoggerProvider : ILoggerProvider
            {
                #region Disposal

                public void Dispose()
                {
                }

                #endregion

                public ILogger CreateLogger(string categoryName) => new OsuDbLogger();
            }

            private class OsuDbLogger : ILogger
            {
                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                    => Logger.Log(formatter(state, exception), LoggingTarget.Database, Framework.Logging.LogLevel.Debug);

                public bool IsEnabled(LogLevel logLevel) => true;

                public IDisposable BeginScope<TState>(TState state) => null;
            }
        }
    }
}
