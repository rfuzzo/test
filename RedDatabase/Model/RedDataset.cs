using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RedDatabase.Model
{
    //public class RedFile
    //{
    //    [Key]
    //    public ulong RedFileId { get; set; }
    //    public string Name { get; set; }
    //    public ICollection<RedFile> Dependencies { get; set; }
    //}

    public class RedFile
    {
        [Key]
        public ulong RedFileId { get; set; }

        public string Name { get; set; }

        public string Archive { get; set; }

        public virtual ICollection<RedFile> Uses { get; set; }
        public virtual ICollection<RedFile> UsedBy { get; set; }
    }

    public class RedDBContext : DbContext
    {
        public RedDBContext()
        {
            const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Path.Combine(Environment.GetFolderPath(folder), "reddb");
            var path = Environment.GetFolderPath(folder);
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}red.db";
        }

        public DbSet<RedFile> Files { get; set; }

        public string DbPath { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blog>()
        //        .Property(b => b.Url)
        //        .IsRequired();
        //}
    }

    //public sealed class RedFileViewModel
    //{
    //    private readonly RedFile _model;

    //    public ulong HashId { get; }
    //    public string Name { get; }
    //    public string Description { get; }

    //    public RedFileViewModel(RedFile model)
    //    {
    //        _model = model;
    //        HashId = model.RedFileId;
    //        Name = model.Name;

    //        Description = "";
    //        if (model.Uses != null)
    //        {
    //            var max = Math.Min(model.Uses.Count, 5);
    //            for (int i = 0; i < max; i++)
    //            {
    //                Description += model.Uses.ToList()[i];
    //                if (i < max - 1) Description += ";";
    //            }
    //        }
    //    }

    //    public RedFile GetModel() => _model;
    //}
}
