using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using System.Net;
using System.Text;

namespace PlannerTracker.web.Models
{
    public class AuthModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        private HttpContent? content;
        private string? jsonData;

        public AuthModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<VMAuth>?> Login(VMAuth vAuth)
        {
            VMResponse<VMAuth>? response = new();

            try
            {
                jsonData = JsonConvert.SerializeObject(vAuth);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = apiUrl + "Auth";
                Console.WriteLine(url);

                //response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(
                //        await httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync()
                //    );

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(errorContent);

                    if (response != null) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(responseString);

                if (response == null)
                {
                    throw new Exception("Auth API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at AuthModel.Login: {ex.Message}");
            }

            return response;
        }

        public async Task<VMResponse<VMAuth>?> Register(VMAuth vAuth)
        {
            VMResponse<VMAuth>? response = new();

            try
            {
                jsonData = JsonConvert.SerializeObject(vAuth);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = apiUrl + "Auth/Regist";
                Console.WriteLine(url);

                //response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(
                //        await httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync()
                //    );

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(errorContent);

                    if (response != null) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMAuth>>(responseString);

                if (response == null)
                {
                    throw new Exception("Auth API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at AuthModel.Register: {ex.Message}");
            }

            return response;
        }
    }
}
