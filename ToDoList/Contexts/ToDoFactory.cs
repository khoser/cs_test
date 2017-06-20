using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace ToDoList.Contexts
{
    /// <summary>
    /// Factory class for EmployeesContext
    /// </summary>
    public static class ToDoFactory
    {
        public static ToDoContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ToDoContext>();
            optionsBuilder.UseMySQL(connectionString);

            //Ensure database creation
            var context = new ToDoContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
