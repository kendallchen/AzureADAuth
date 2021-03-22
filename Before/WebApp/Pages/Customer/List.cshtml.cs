using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dto = Data.Model;
using Core;
using Newtonsoft.Json;

namespace WebApp.Pages.Customer
{
    public class ListModel : PageModel
    {
        public IEnumerable<Dto.Customer> CustomerList { get; set; }
        private readonly IConfiguration config;

        public IEnumerable<SelectListItem> PageSizeList { get; set; } = new SelectList(new List<int> { 5, 10, 25, 50 });

        public PageSortParam PageSortParam { get; set; } = new PageSortParam();
        public PageSortResult PageSortResult { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PageSize { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }  

        [BindProperty(SupportsGet = true)]
        public SortDirection SortDir { get; set; }

        //for the next sort direction when the user clicks on the header
        [BindProperty(SupportsGet = true)]
        public SortDirection? SortDirNext { get; set; }

        public ListModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task OnGet()
        {
            if (PageSize.HasValue)
                PageSortParam.PageSize = (int)PageSize;

            PageSortParam.CurrentPage = PageNumber;

            //if never sorted
            if (SortField == null)
                SortDir = new SortDirection();
            else if (SortDirNext != null)  //if requested new sort direction
                SortDir = (SortDirection)SortDirNext;

            //SortDirNext will be the reverse of SortDir
            SortDirNext = SortDir == SortDirection.Ascending ? SortDirection.Decending : SortDirection.Ascending;
            
            PageSortParam.SortField = SortField;
            PageSortParam.SortDir = SortDir;
            
            HttpResponseMessage response = await new HttpClient().GetAsync(this.config["APIurl"] + "Customer?PageSize=" + PageSortParam.PageSize
                                                                                                    + "&CurrentPage=" + PageSortParam.CurrentPage
                                                                                                    + "&SortField=" + PageSortParam.SortField
                                                                                                    + "&SortDir=" + PageSortParam.SortDir);
            //display the list of customers
            if (response.IsSuccessStatusCode)
                CustomerList = await response.Content.ReadAsAsync<IEnumerable<Dto.Customer>>();
            //get the paging meta data from the header
            IEnumerable<string> headerValue;
            if (response.Headers.TryGetValues("X-PageSortResult", out headerValue))
            {
                PageSortResult = JsonConvert.DeserializeObject<PageSortResult>(headerValue.First());
            }
        }

    }
}