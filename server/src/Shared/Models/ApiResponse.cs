namespace Server.Shared.Models;
/// <summary>
/// return new ApiResponse.Single(new { lesenList=list });
/// return new ApiResponse.Paginated(list, totalCount: 100, pageNumber: 1, pageSize: 10);
/// return new ApiResponse.Error(list, statusCode: 404, message: "Not Found");
/// </summary>
public class ApiResponse
{
    public object? Data { get; set; }
    public PageMeta? PageMeta { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    // Private constructor
    private ApiResponse(object? data, PageMeta? pageMeta, int statusCode, string? message)
    {
        Data = data;
        PageMeta = pageMeta;
        StatusCode = statusCode;
        Message = message;
    }

    // Factory method for single object responses (no pagination)
    public static ApiResponse Single(object? data, int statusCode = 200, string? message = "Success")
    {
        return new ApiResponse(data, null, statusCode, message);
    }

    // Factory method for paginated responses
    public static ApiResponse Paginated(object? data, int totalCount, int pageNumber = 1, int pageSize = 0, int statusCode = 200, string? message = "Success")
    {
        int totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 1;
        pageNumber = pageNumber > 0 ? pageNumber : 1;

        var pageMeta = new PageMeta
        {
            PageNumber = pageNumber,
            TotalPages = totalPages,
            TotalCount = totalCount,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };

        return new ApiResponse(data, pageMeta, statusCode, message);
    }

    // Factory method for error responses
    public static ApiResponse Error(object? data = null, int statusCode = 400, string? message = "Error")
    {
        return new ApiResponse(data, null, statusCode, message);
    }
}
