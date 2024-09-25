using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace StockManagementRazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string APIurl = GetApiUrl();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"{APIurl}/Authentication/userAuthentication");
            request.Content = new StringContent(JsonConvert.SerializeObject(Input), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
                Console.WriteLine(responseObject);
                var token = responseObject["token"];
                var roleId = responseObject["roleId"];

                if (roleId.Equals("1"))
                {
                    HttpContext.Session.SetString("Role", "Admin");
                }
                else
                {
                    HttpContext.Session.SetString("Role", "Regular");
                }

                HttpContext.Session.SetString("BearerToken", token);
                HttpContext.Session.SetString("UserId", roleId);

                TempData["roleId"] = roleId;
                TempData.Keep("roleId");
                TempData["Token"] = token;
                TempData.Keep("Token");

                return LocalRedirect(Url.Content("/Products"));
            }
            else
            {
                ModelState.AddModelError("Error", "Verifique su email y/o contraseña.");
                return Page();
            }
        }

        public string GetApiUrl()
        {
            var apiUrl = _configuration.GetValue<string>("ConnectionStrings:APIurl");
            return apiUrl;
        }
    }
}
