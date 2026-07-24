using System;
using System.Collections.Generic;

namespace TechnicalTest.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages
        {
            get { return PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize); }
        }
    }
}
