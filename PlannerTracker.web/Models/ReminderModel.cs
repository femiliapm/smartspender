using Newtonsoft.Json;
using PlannerTracker.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PlannerTracker.web.Models
{
    public class ReminderModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        private HttpContent? content;
        private string? jsonData;

        public ReminderModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<List<VMReminder>>?> FetchAll(string token, VMReminderFilter? filter)
        {
            VMResponse<List<VMReminder>>? response = new();

            try
            {
                string url = apiUrl + "Reminder?IsCompleted=" + filter?.IsCompleted + "&DateFrom=" + filter?.DateFrom?.ToString("yyyy-MM-dd") + "&DateTo=" + filter?.DateTo?.ToString("yyyy-MM-dd");
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<VMReminder>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<VMReminder>>>(responseString);

                if (response == null)
                {
                    throw new Exception("Reminder API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at ReminderModel.FetchAll: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<VMReminder>?> SaveReminder(VMReminderReq req, string token)
        {
            VMResponse<VMReminder>? response = new();

            try
            {
                string url = apiUrl + "Reminder";
                Console.WriteLine(url);

                jsonData = JsonConvert.SerializeObject(req);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMReminder>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMReminder>>(responseString);

                if (response == null)
                {
                    throw new Exception("Reminder API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at ReminderModel.SaveReminder: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
