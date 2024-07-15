namespace CarRentalSystem.Application.Base;

public class BaseResponseDto<TD>
{
    public TD Data { get; set; }
    public List<TD> DataList { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

    public BaseResponseDto(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = isSuccess ? 200 : 400;
    }

    public BaseResponseDto(List<TD> items)
    {
        IsSuccess = true;
        DataList = items;
        TotalPages = 1;
        CurrentPage = 1;
        PageSize = items.Count;
        TotalCount = items.Count;
        HasPrevious = false;
        HasNext = false;
        StatusCode = 200;
    }

    public BaseResponseDto(TD data)
    {
        IsSuccess = true;
        Data = data;
        StatusCode = 200;
    }
    
    public BaseResponseDto(TD data, string message)
    {
        IsSuccess = true;
        Data = data;
        StatusCode = 200;
        Message = message;
    }
}