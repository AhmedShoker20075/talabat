
namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ApiResponse(int statuscode , string? message = null)
        {
			StatusCode = statuscode;
            Message = message ?? GetDefaultMessageForStatusCode(statuscode);
        }

		private string? GetDefaultMessageForStatusCode(int statuscode)
		{
			return statuscode switch
			{
				400 => "a bad Request , you have made",
				401 => "Authorized, you are not",
				404 => "Resource is not found",
				500 => "Server Error",
				_ => null
			};
		}
	}
}
