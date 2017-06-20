using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class ToDoItem
    {
        [Key]
        [Column(TypeName = "varchar(200)")]
        public String Key { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
