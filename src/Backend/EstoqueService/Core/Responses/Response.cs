
using Newtonsoft.Json;
using EstoqueService.Configurations;

namespace EstoqueService.Core.Responses;

public class Response<TData>
{
    private readonly int _code;

    [JsonIgnore]
    public int Code => _code;

    [JsonConstructor]
    public Response()
        => _code = ApiConstants.DefaultStatusCode;

    public Response(
        TData? data, 
        int code = ApiConstants.DefaultStatusCode,
        string? message = null)
    {
        Data = data;
        Message = message;
        _code = code;
    }
    public TData? Data { get; set; }
    public string? Message { get; set; }


    [JsonIgnore]    
    public bool IsSuccess => _code is >= 200 and <= 299;
}