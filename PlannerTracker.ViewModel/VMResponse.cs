using System.Net;

namespace PlannerTracker.ViewModel
{
    public class VMResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public VMResponse()
        {
            StatusCode = HttpStatusCode.InternalServerError;
            Message = string.Empty;
            //Data = default(T);
            Data = default;
        }
    }
}
