using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOListDemo.Models
{
    public class TODOlistViewModel
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }
        public SectionInfo SectionInfo { get; set; }
    }
}