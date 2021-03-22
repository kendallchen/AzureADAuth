using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// For paging functionality
    /// </summary>
    public class PageList<T> : List<T>
    {
        public PageSortParam Param { get; }
        public PageSortResult Result { get; }

        public PageList(PageSortParam param)
        {
            Param = param;
            Result = new PageSortResult();
        }

        public async Task GetData(IQueryable<T> query)
        {
            //get the total count
            Result.TotalCount = await query.CountAsync();
            //find the number of pages
            Result.TotalPages = (int)Math.Ceiling(Result.TotalCount / (double)Param.PageSize);
            //find previous and next page number
            if (Param.CurrentPage - 1 > 0)
                Result.PreviousPage = Param.CurrentPage - 1;
            if (Param.CurrentPage + 1 <= Result.TotalPages)
                Result.NextPage = Param.CurrentPage + 1;
            //find first row and last row on the page
            if (Result.TotalCount == 0)  //if no record found
                Result.FirstRowOnPage = Result.LastRowOnPage = 0;
            else
            {
                Result.FirstRowOnPage = (Param.CurrentPage - 1) * Param.PageSize + 1;
                Result.LastRowOnPage = Math.Min(Param.CurrentPage * Param.PageSize, Result.TotalCount);
            }

            //if has sorting criteria
            if (Param.SortField != null)
                query = query.OrderBy(Param.SortField + (Param.SortDir == SortDirection.Ascending ? " ascending" : " descending"));

            List<T> list = await query.Skip((Param.CurrentPage - 1) * Param.PageSize).Take(Param.PageSize).ToListAsync();
            AddRange(list);  //add the list of items
        }
    }
}
