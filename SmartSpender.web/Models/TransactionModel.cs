using Newtonsoft.Json;
using SmartSpender.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartSpender.web.Models
{
    public class TransactionModel
    {
        private readonly HttpClient httpClient = new();
        private readonly string? apiUrl;

        private HttpContent? content;
        private string? jsonData;

        public TransactionModel(IConfiguration _config)
        {
            apiUrl = _config["ApiUrl"];
        }

        public async Task<VMResponse<VMTransaction>?> SaveTransaction(VMTransactionReq req, string token)
        {
            VMResponse<VMTransaction>? response = new();

            try
            {
                string url = apiUrl + "Transaction";
                Console.WriteLine(url);

                jsonData = JsonConvert.SerializeObject(req);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.PostAsync(url, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<VMTransaction>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<VMTransaction>>(responseString);

                if (response == null)
                {
                    throw new Exception("Transaction API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at TransactionModel.SaveTransaction: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<List<VMTransaction>>?> FetchAll(string token, string? filter, Guid? userId)
        {
            VMResponse<List<VMTransaction>>? response = new();

            try
            {
                string url = apiUrl + "Transaction" + (userId != null ? "/user/" + userId : string.Empty) + (!string.IsNullOrEmpty(filter) ? "?filter=" + filter : string.Empty);
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<VMTransaction>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<VMTransaction>>>(responseString);

                if (response == null)
                {
                    throw new Exception("Transaction API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at TransactionModel.FetchAll: {ex.Message}");
                throw;
            }

            return response;
        }

        public async Task<VMResponse<List<VMTransactionCategory>>?> FetchByCategory(string token)
        {
            VMResponse<List<VMTransactionCategory>>? response = new();

            try
            {
                string url = apiUrl + "Transaction/Category?by=expense";
                Console.WriteLine(url);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await responseMessage.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<VMResponse<List<VMTransactionCategory>>>(errorContent);

                    if (response != null && !string.IsNullOrEmpty(response.Message)) return response;

                    Console.WriteLine($"Error: {responseMessage.StatusCode}, Content: {errorContent}");
                    throw new Exception($"{errorContent}");
                }
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<VMResponse<List<VMTransactionCategory>>>(responseString);

                if (response == null)
                {
                    throw new Exception("Transaction API cannot be reached!");
                }

                if (response != null && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at TransactionModel.FetchByCategory: {ex.Message}");
                throw;
            }

            return response;
        }
    }
}
