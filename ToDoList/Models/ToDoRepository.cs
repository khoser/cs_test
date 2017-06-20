using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Contexts;

namespace ToDoList.Models
{
    public class ToDoRepository : IToDoRepository
    {
        private static ConcurrentDictionary<string, ToDoItem> _todos =
              new ConcurrentDictionary<string, ToDoItem>();
        
        private ToDoContext context;

        public ToDoRepository()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("SampleConnection");

            context = ToDoFactory.Create(connectionString);

            Add(new ToDoItem { Name = "Item1" });
        }

        public IEnumerable<ToDoItem> GetAll()
        {
            //return _todos.Values;
            return context.ToDoItems.ToList();
            
        }

        public void Add(ToDoItem item)
        {

            // Create an ToDo instance and save the entity to the database
            item.Key = Guid.NewGuid().ToString();

            context.Add(item);
            context.SaveChanges();
            
            //Console.WriteLine($"Employee was saved in the database with id: {entry.Key}");

            //_todos[item.Key] = item;
        }

        public ToDoItem Find(String key)
        {
            //ToDoItem item;
            return context.Find<ToDoItem>(key);
            //_todos.TryGetValue(key, out item);
            //return item;
        }

        public ToDoItem Remove(String key)
        {
            ToDoItem item = context.Find<ToDoItem>(key);
            if (item != null)
            {
                context.Remove(item);
                context.SaveChanges();
            }
            return item;
            //_todos.TryRemove(key, out item);
            //return item;
        }

        public void Update(ToDoItem item)
        {
            //_todos[item.Key] = item;
            context.Update(item);
            context.SaveChanges();
        }
    }
}
