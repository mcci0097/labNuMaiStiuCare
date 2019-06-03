using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lab2_restapi_1205_taskmgmt.Models
{
    public enum Importance
    {
        Low,
        Medium,
        High
    }

    public enum State
    {
        Open,
        InProgress,
        Closed
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? ClosedAt { get; set; }
        public User Owner { get; set; }

        [EnumDataType(typeof(Importance))]
        public Importance Importance { get; set; }

        [EnumDataType(typeof(State))]
        public State State { get; set; }

        public List<Comment> Comments { get; set; }


    }
}