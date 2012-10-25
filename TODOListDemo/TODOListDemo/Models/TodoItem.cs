using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TODOListDemo.Models
{
    public class TodoItem
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? TimeFinished { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool Done { get; set; }

        public int SortNum { get; set; }

        public int PriorityId { get; set; }

        public string Priority
        {
            get
            {
                Priority enumDisplayStatus = ((Priority)PriorityId);
                return enumDisplayStatus.ToString();
            }
        }

        public bool Expired
        {
            get
            {
                return StartTime > DateTime.Now;
            }
        }
    }
}