using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class PageSortParam
    {
        public int PageSize { get; set; } = 10;  //default page size
        public int CurrentPage { get; set; } = 1;

        public string SortField { get; set; } = null;
        public SortDirection SortDir { get; set; } 
    }

    public enum SortDirection
    {
        Ascending = 0,   //default as ascending
        Decending
    }

}
