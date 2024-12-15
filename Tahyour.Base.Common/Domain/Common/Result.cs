
namespace Tahyour.Base.Common.Domain.Common;

public class Result<T>
{
    public T Content { get; set; }
    public bool HasError => ErrorMessage != "";
    public string ErrorMessage { get; set; } = "";
    public string Message { get; set; } = "";
    public string RequestId { get; set; } = "";
    public int DataCount { get; set; }
    public bool IsSuccess { get; set; } = true;
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

    public Result()
    {

    }

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public void SetError(string errorMessage, string messsage)
    {
        ErrorMessage = errorMessage;
        Message = messsage;
        IsSuccess = false;
    }

    public void SetSuccess(T content, string messsage)
    {
        Content = content;
        IsSuccess = true;
        Message = messsage;

        if (content is IList list)
        {
            DataCount = list.Count;
        }
        else DataCount = 1;
    }
}
