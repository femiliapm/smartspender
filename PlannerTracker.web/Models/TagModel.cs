using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using System.Net;
using System.Net.Http.Headers;

namespace PlannerTracker.web.Models
{
    public class TagModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        public TagModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<List<string>>?> FetchName(string token)
        {
            VMResponse<List<string>>? response = new();

            try
            {
                string url = apiUrl + "Tag/Name";
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<string>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<string>>>(responseString);

                if (response == null)
                {
                    throw new Exception("Tag API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at TagModel.FetchName: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
