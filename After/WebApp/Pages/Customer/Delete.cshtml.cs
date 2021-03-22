using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Model = Data.Model;

namespace WebApp.Pages.Customer
{
    [Authorize(Roles = "CustomerService")]
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration config;

        [BindProperty]
        public Model.Customer Customer { get; set; }
        public IHttpClientFactory HttpClientFactory { get; }

        public DeleteModel(IConfiguration config,
                            IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            HttpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGet(int? customerId)
        {
            if (customerId.HasValue)
            {
                HttpClient client = HttpClientFactory.CreateClient("API");
                HttpResponseMessage response = await client.GetAsync(config["APIurl"] + "Customer/" + customerId.Value);
                if (response.IsSuccessStatusCode)
                    this.Customer = await response.Content.ReadAsAsync<Model.Customer>();
            }
            else
                return RedirectToPage("../Error");
            return Page();
        }

        public async Task<IActionResult> OnPost(int customerId)
        {
            HttpClient client = HttpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.DeleteAsync(
                                                        config["APIurl"] + "Customer/" + customerId)
                                                            .ContinueWith(i => i.Result.EnsureSuccessStatusCode());
            return RedirectToPage("./List");
        }
    }
}
