using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StockManagementRazorPage.Models.Product;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

namespace StockManagementRazorPage.Pages.Products
{
    public class EditProductModel : PageModel
    {
        [BindProperty]
        public List<ProductCategory> ProductsCategories { get; set; }
        public Product product { get; set; }

        public List<Product> Products { get; set; }
        public ProductCategory productCategory { get; set; }
        public string RoleId { get; set; }

        private readonly IConfiguration _configuration;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public object PrevPage { get; private set; }
        public object NextPage { get; private set; }

        public int TotalPages { get; set; }

        public EditProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");

                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{APIurl}/Product/id/{id}");

                var productCategories = await httpClient.GetAsync($"{APIurl}/ProductCategory");

                if (response.IsSuccessStatusCode && productCategories.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadFromJsonAsync<Product>();

                    var productCategoriesResponse = await productCategories.Content.ReadFromJsonAsync<ProductCategoryResponse>();
                    ProductsCategories = productCategoriesResponse.ProductsCategories;

                    var productCategoryResponse = await httpClient.GetAsync($"{APIurl}/ProductCategory/{product.IdProductCategory_Id}");
                    productCategory = await productCategoryResponse.Content.ReadFromJsonAsync<ProductCategory>();
                }
                else
                {
                    product = new Product();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEditProduct(Product product)
        {
            using (var httpClient = new HttpClient())
            {
                string APIurl = GetApiUrl();
                RoleId = HttpContext.Session.GetString("Role");

                string token = HttpContext.Session.GetString("BearerToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsJsonAsync($"{APIurl}/Product/{product.Id}", product);


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
                    return Redirect("/Products");
                }

            }
            return Page();
        }


        private string GetApiUrl()
        {
            return _configuration.GetValue<string>("ConnectionStrings:APIurl");
        }
    }
}
