using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Contexts
{
    /// <summary>
    /// The entity framework context with a ToDoItems DbSet
    /// </summary>
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        { }

        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}