using Newtonsoft.Json;
using SmartSpender.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartSpender.web.Models
{
    public class BudgetPlanModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        private HttpContent? content;
        private string? jsonData;

        public BudgetPlanModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<VMBudgetPlan>?> SaveBudgetPlan(string token, VMBudgetPlanReq req)
        {
            VMResponse<VMBudgetPlan>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan";
                Console.WriteLine(url);

                jsonData = JsonConvert.SerializeObject(req);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMBudgetPlan>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMBudgetPlan>>(responseString);

                if (response == null)
                {
                    throw new Exception("BudgetPlan API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at BudgetPlanModel.SaveBudgetPlan: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<List<VMBudgetPlan>>?> Fetch(string token, string? filter, Guid? userId)
        {
            VMResponse<List<VMBudgetPlan>>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan/User/" + userId + (!string.IsNullOrEmpty(filter) ? "?filter=" + filter : string.Empty);
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<VMBudgetPlan>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<VMBudgetPlan>>>(responseString);

                if (response == null)
                {
                    throw new Exception("BudgetPlan API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at BudgetPlanModel.Fetch: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
