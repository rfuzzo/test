using RedDatabase.Annotations;
using RedDatabase.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace RedDatabase.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //public List<RedFileViewModel> ItemsSource { get; set; } = new();

        public RedFile SelectedItem { get; set; }
        //public object SelectedItem { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            // load db
            //using var db = new RedDBContext();
            
        }

        //public void Reload()
        //{
        //    // Load DataBase
        //    var filename = Path.GetFullPath("dependencies.json");
        //    if (!File.Exists(filename))
        //    {
        //        throw new FileNotFoundException();
        //    }
        //    var options = new JsonSerializerOptions { WriteIndented = true };
        //    var json = File.ReadAllText(filename);
        //    var data = JsonSerializer.Deserialize<Dictionary<ulong, RedDataset>>(json, options);
        //    var list = new Dictionary<ulong, RedFile>();
        //    foreach (var (key, redDataset) in data)
        //    {
        //        list.Add(key, new RedFile()
        //        {
        //            HashId = key,
        //            Name = redDataset.FileName
        //        });
        //    }

        //    foreach (var (key, redDataset) in data)
        //    {
        //        var item = list[key];
        //        var deps = redDataset.Dependencies.Keys
        //            .Select(_ => list.ContainsKey(_) ? list[_] : null)
        //            .Where(_ => _ is not null);
        //        item.Dependencies = new List<RedFile>(deps);
        //    }

        //    var final = list.Where(_ => _.Value.Dependencies.Any());

        //    using var db = new RedDBContext();
        //    Console.WriteLine($"Database path: {db.DbPath}.");

        //    db.Files.RemoveRange(db.Files);
        //    db.SaveChanges();

        //    // Create
        //    Console.WriteLine("Inserting a new dataset");
        //    foreach (var (key, redFile) in final)
        //    {
        //        db.Add(redFile);
        //    }
        //    db.SaveChanges();

        //    // Read
        //    Console.WriteLine($"Files: {db.Files.Count()}");
        //}

        public void Query(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                using var db = new RedDBContext();
                var result = db.Files
                    .FromSqlRaw(text);
                SetItemSource(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        public void QueryUsingFiles(ulong hashId)
        {
            //try
            //{
            //    using var db = new RedDBContext();
            //    var result = db.Files
            //        .Where(x => x.Uses.Contains(SelectedItem.GetModel()));
            //    SetItemSource(result);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    //throw;
            //}
        }

        private void SetItemSource(IQueryable<RedFile> redFiles, bool enqueue = true)
        {
            //var list = redFiles.ToList();

            //ItemsSource = list;
            //OnPropertyChanged(nameof(ItemsSource));
            //if (enqueue)
            //{
            //    var enumerable = list.Select(x => x.HashId).ToList();
            //    if (enumerable.Any())
            //    {
            //        _backwardStack.Push(enumerable);
            //    }
                
            //    _forwardStack.Clear();
            //}
        }

        private readonly Stack<IEnumerable<ulong>> _backwardStack = new();
        private readonly Stack<IEnumerable<ulong>> _forwardStack = new();

        public void Back()
        {
            if (_backwardStack.Count == 0)
            {
                return;
            }

            var hashset = _backwardStack.Pop();
            _forwardStack.Push(hashset);

            using var db = new RedDBContext();
            var items = db.Files.Where(_ => hashset.Contains(_.RedFileId));
            SetItemSource(items, false);
        }

        public void Forward()
        {
            if (_forwardStack.Count == 0)
            {
                return;
            }

            var hashset = _forwardStack.Pop();
            _backwardStack.Push(hashset);

            using var db = new RedDBContext();
            var items = db.Files.Where(_ => hashset.Contains(_.RedFileId));
            SetItemSource(items, false);
        }
    }
}
