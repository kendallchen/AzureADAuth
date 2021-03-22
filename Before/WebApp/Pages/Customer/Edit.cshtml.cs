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
    public class EditModel : PageModel
    {
        private readonly IConfiguration config;

        [BindProperty]
        public Model.Customer Customer { get; set; }
        public string Action { get; set; }   //determines if this is "Add" or "Edit"

        public EditModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<IActionResult> OnGet(int? customerId)
        {
            if (customerId.HasValue)  
            {
                if (customerId == 0)  //if adding customer
                {
                    this.Action = "Add";
                    this.Customer = new Model.Customer() { CustomerId = (int)customerId, FirstName = "", LastName = "" };
                }
                else  //editing customer
                {
                    this.Action = "Edit";
                    HttpResponseMessage response = await new HttpClient().GetAsync(config["APIurl"] + "Customer/" + customerId.Value);
                    if (response.IsSuccessStatusCode)
                        this.Customer = await response.Content.ReadAsAsync<Model.Customer>();
                }
               
            }
            else
                return RedirectToPage("../Error");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            if (this.Customer.CustomerId == 0)  //if adding customer
            {
                HttpResponseMessage response = await new HttpClient().PostAsJsonAsync(
                                                        config["APIurl"] + "Customer/",
                                                        this.Customer)
                                                            .ContinueWith(i => i.Result.EnsureSuccessStatusCode());
            }
            else  //if edit customer
            {
                HttpResponseMessage response = await new HttpClient().PutAsJsonAsync(
                                                        config["APIurl"] + "Customer/" + this.Customer.CustomerId,
                                                        this.Customer)
                                                            .ContinueWith(i => i.Result.EnsureSuccessStatusCode());
            }
            return RedirectToPage("./List");
        }
    }
}