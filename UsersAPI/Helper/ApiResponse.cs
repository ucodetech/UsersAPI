namespace UsersAPI.Models;

public record ApiResponse<T> : BaseResponse
{
    public T? Data { get; set; }
}