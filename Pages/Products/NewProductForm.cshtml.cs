using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StockManagementRazorPage.Models.Product;
using System.Net.Http.Headers;
using System.Text;

namespace StockManagementRazorPage.Pages
{
    public class NewProductModel : PageModel
    {
        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public List<ProductCategory> ProductsCategories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public object PrevPage { get; private set; }
        public object NextPage { get; private set; }
        public int TotalPages { get; set; }
        public string RoleId { get; set; }

        private readonly IConfiguration _configuration;

        public NewProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task OnGet()
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");

                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{APIurl}/ProductCategory");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ProductCategoryResponse>();

                    if (apiResponse != null)
                    {
                        ProductsCategories = apiResponse.ProductsCategories;
                    }
                    else
                    {
                        ProductsCategories = new List<ProductCategory>();
                    }
                }
                else
                {
                    ProductsCategories = new List<ProductCategory>();
                }
            }
        }

        public async Task<IActionResult> OnPostCreateProduct(Product product)
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");
                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (product.Price <= 0 || product.IdProductCategory_Id <= 0)
                {
                    return Redirect("/Products/NewProduct");
                }

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{APIurl}/Product", content);
                var productsResponse = await httpClient.GetAsync($"{APIurl}/Product");

                if (productsResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await productsResponse.Content.ReadFromJsonAsync<ProductResponse>();

                    if (apiResponse != null)
                    {
                        Products = apiResponse.Products;
                        PrevPage = apiResponse.PrevPage;
                        NextPage = apiResponse.NextPage;
                    }
                    else
                    {
                        Products = new List<Product>();
                    }
                    return Redirect("./");
                }
            }
            return Page();
        }

        public string GetApiUrl()
        {
            var apiUrl = _configuration.GetValue<string>("ConnectionStrings:APIurl");
            return apiUrl;
        }
    }
}
