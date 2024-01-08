namespace UsersAPI.Models;

public record BaseResponse
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}