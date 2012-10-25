using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOListDemo.Models
{
    public class SectionInfo
    {
        public int TotalItems { get; set; }
        public int ItemsLoaded { get; set; }
        public int CurrentSection { get; set; }
    }
}