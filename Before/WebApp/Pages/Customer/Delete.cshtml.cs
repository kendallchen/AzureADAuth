using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Model = Data.Model;

namespace WebApp.Pages.Customer
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration config;

        [BindProperty]
        public Model.Customer Customer { get; set; }

        public DeleteModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<IActionResult> OnGet(int? customerId)
        {
            if (customerId.HasValue)
            {
                HttpResponseMessage response = await new HttpClient().GetAsync(config["APIurl"] + "Customer/" + customerId.Value);
                if (response.IsSuccessStatusCode)
                    this.Customer = await response.Content.ReadAsAsync<Model.Customer>();
            }
            else
                return RedirectToPage("../Error");
            return Page();
        }

        public async Task<IActionResult> OnPost(int customerId)
        {
            HttpResponseMessage response = await new HttpClient().DeleteAsync(
                                                        config["APIurl"] + "Customer/" + customerId)
                                                            .ContinueWith(i => i.Result.EnsureSuccessStatusCode());
            return RedirectToPage("./List");
        }
    }
}
