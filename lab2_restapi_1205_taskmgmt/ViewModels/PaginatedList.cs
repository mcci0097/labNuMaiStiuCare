using System;
using System.Collections.Generic;
using System.Linq;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class PaginatedList<T>
    {
        public const int EntriesPerPage = 5;
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int TotalEntries { get; set; }
        public List<T> Entries { get; set; }
    }
}
