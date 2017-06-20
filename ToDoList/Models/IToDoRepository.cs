using System;
using System.Collections.Generic;

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
