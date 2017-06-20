using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Models
{
    public interface IToDoRepository
    {
        void Add(ToDoItem item);
        IEnumerable<ToDoItem> GetAll();
        ToDoItem Find(String key);
        ToDoItem Remove(String key);
        void Update(ToDoItem item);
    }
}
