using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PlannerTracker.web.Models
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

        public async Task<VMResponse<List<VMBudgetPlan>>?> Fetch(string token, string? filter)
        {
            VMResponse<List<VMBudgetPlan>>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan" + (!string.IsNullOrEmpty(filter) ? "?filter=" + filter : string.Empty);
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

        public async Task<VMResponse<VMBudgetPlan>?> FetchById(string token, string id)
        {
            VMResponse<VMBudgetPlan>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan/" + id;
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
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

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at BudgetPlanModel.FetchById: {ex.Message}");
                throw;
            }

            return response;
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

        public async Task<VMResponse<VMBudgetPlan>?> UpdateBudgetPlan(string token, VMBudgetPlanReq req, string id)
        {
            VMResponse<VMBudgetPlan>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan/" + id;
                Console.WriteLine(url);

                jsonData = JsonConvert.SerializeObject(req);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.PutAsync(url, content);
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

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at BudgetPlanModel.UpdateBudgetPlan: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<VMBudgetPlan>?> DeleteBudgetPlan(string token, string id, string userId)
        {
            VMResponse<VMBudgetPlan>? response = new();

            try
            {
                string url = apiUrl + "BudgetPlan?id=" + Uri.EscapeDataString(id) + "&userId=" + userId;
                //string url = apiUrl + "BudgetPlan/DeleteMultiple?id=" + id + "&userId=" + userId;
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(url);
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

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at BudgetPlanModel.DeleteBudgetPlan: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
