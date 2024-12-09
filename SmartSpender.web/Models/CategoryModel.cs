using Newtonsoft.Json;
using SmartSpender.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartSpender.web.Models
{
    public class CategoryModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        private HttpContent? content;
        private string? jsonData;

        public CategoryModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<VMCategory>?> Create(VMCategoryReq req, string token)
        {
            VMResponse<VMCategory>? response = new();

            try
            {
                jsonData = JsonConvert.SerializeObject(req);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = apiUrl + "Category/user/" + req.ModifiedBy;
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMCategory>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMCategory>>(responseString);

                if (response == null)
                {
                    throw new Exception("Category API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at CategoryModel.Create: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<List<VMCategory>>?> Fetch(string token, Guid? userId)
        {
            VMResponse<List<VMCategory>>? response = new();

            try
            {
                string url = apiUrl + "Category/user/" + userId;
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<VMCategory>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<VMCategory>>>(responseString);

                if (response == null)
                {
                    throw new Exception("Category API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at CategoryModel.Fetch: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
