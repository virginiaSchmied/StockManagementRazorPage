using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockManagementRazorPage.Models.Product;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StockManagementRazorPage.Pages.Products
{
    public class ProductsModel : PageModel
    {
        [BindProperty]
        public List<Product> Products { get; set; }
        public List<Product> ProductListByAmount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public object PrevPage { get; private set; }
        public object NextPage { get; private set; }
        public int TotalPages { get; set; }
        public string RoleId { get; set; }

        private readonly IConfiguration _configuration;

        public ProductsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");

                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (TempData.TryGetValue("ProductListByAmount", out var productListJson))
                {
                    ProductListByAmount = JsonSerializer.Deserialize<List<Product>>(productListJson.ToString());
                }
                else
                {
                    ProductListByAmount = new List<Product>();
                }

                var response = await httpClient.GetAsync($"{APIurl}/Product?pageNumber={PageNumber}&pageSize={PageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ProductResponse>();

                    if (apiResponse != null)
                    {
                        Products = apiResponse.Products;
                        PrevPage = apiResponse.PrevPage;
                        NextPage = apiResponse.NextPage;
                        TotalPages = apiResponse.TotalPages;
                    }
                    else
                    {
                        Products = new List<Product>();
                    }
                }
                else
                {
                    Products = new List<Product>();
                }
            }
        }

        public async Task<IActionResult> OnPostCreateProductListAsync(double amount)
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");

                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{APIurl}/Product/amount/{amount}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<List<Product>>();

                    if (apiResponse != null)
                    {
                        TempData["ProductListByAmount"] = JsonSerializer.Serialize(apiResponse);
                        return RedirectToPage();
                    }
                    else
                    {
                        ProductListByAmount = new List<Product>();
                    }
                }
                else
                {
                    ProductListByAmount = new List<Product>();
                }
            }
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var responseUser = await httpClient.DeleteAsync($"{APIurl}/Product/{id}");

                if (responseUser.IsSuccessStatusCode)
                {
                    return Redirect("/Products");
                }
                else
                {
                    return Page();
                }
            }
        }

        public string GetApiUrl()
        {
            var apiUrl = _configuration.GetValue<string>("ConnectionStrings:APIurl");
            return apiUrl;
        }
    }
}
