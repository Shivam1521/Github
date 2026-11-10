namespace Entities
{
    public class Response
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
      
    }

    public class MappingResponse
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public int MasterMappedQty { get; set; }

    }
    public class ScratchResponse
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;

        // public object Data { get; set; }
        public string FileUrl { get; set; } = string.Empty;   // single zip link


    }


    public class ResponseWithData
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public string ResponseData { get; set; } = string.Empty;
    }

    public class HttpResponse<T> where T : class
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public IEnumerable<T>? ResponseData { get; set; }
    }

    public class SingleResponse<T>
    {
        public string ResponseCode { get; set; } = "200";
        public string ResponseMessage { get; set; } = "Success";
        public T? ResponseData { get; set; }
    }

    public static class ResponseCodes
    {
        public const string Success = "200";
        public const string Unauthorized = "401";
        public const string BadRequest = "400";
        public const string InternalServerError = "500";
    }

    public static class ResponseHelper
    {
        public static SingleResponse<T> Success<T>(T data, string message = "Success") =>
            new() { ResponseCode = ResponseCodes.Success, ResponseMessage = message, ResponseData = data };

        public static SingleResponse<T> Error<T>(string code, string message) =>
            new() { ResponseCode = code, ResponseMessage = message, ResponseData = default };
    }


    public class ResponseSerial
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int DispatchedQty { get; set; } 
        public string LineNumber { get; set; } = string.Empty;

    }
}
