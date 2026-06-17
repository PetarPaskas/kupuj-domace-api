namespace KupujDomace.Models;

public class HttpError : Exception
{
    public int StatusCode { get; }
    public string Detail { get; }

    public HttpError(int statusCode, string detail) : base(detail)
    {
        StatusCode = statusCode;
        Detail = detail;
    }
}

