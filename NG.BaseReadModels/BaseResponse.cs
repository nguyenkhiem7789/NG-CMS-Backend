using System.Globalization;
using NG.EnumDefine;
using ProtoBuf;

namespace NG.BaseReadModels;

[ProtoContract]
public class BaseResponse
{
    [ProtoMember(1)] public bool Status { get; set; }
    [ProtoMember(2)] public ErrorCodeEnum ErrorCode { get; set; }
    [ProtoMember(3)] public List<string> Messages { get; set; }
    [ProtoMember(4)] public int Version { get; set; }
    [ProtoMember(5)] public string ServerTime { get; set; }
    
    public BaseResponse()
    {
        Messages = new List<string>();
        ServerTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }

    public void SetSuccess()
    {
        Status = true;
        ErrorCode = ErrorCodeEnum.NoErrorCode;
    }

    public void SetPermissionDeny()
    {
        Status = false;
        ErrorCode = ErrorCodeEnum.PermissionDeny;
        string message = ErrorCode.ToString();
        Messages.Add(message);
    }

    public void SetFail(ErrorCodeEnum code)
    {
        Status = false;
        ErrorCode = code;
        string message = code.ToString();
        Messages.Add(message);
    }

    public void SetFail(string message, ErrorCodeEnum code = ErrorCodeEnum.NoErrorCode)
    {
        Status = false;
        ErrorCode = code;
        Messages.Add(message);
    }
    
    public void SetFail(IEnumerable<string> messages, ErrorCodeEnum code = ErrorCodeEnum.NoErrorCode)
    {
        Status = false;
        ErrorCode = code;
        foreach (var message in messages)
        {
            Messages.Add(message);
        }
    }
}

[ProtoContract]
public class BaseResponse<T>
{
    public BaseResponse()
    {
        Messages = new List<string>();
        ServerTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }

    [ProtoMember(1)] public bool Status { get; set; }
    [ProtoMember(2)] public ErrorCodeEnum ErrorCode { get; set; }
    [ProtoMember(3)] public List<string> Messages { get; set; }
    [ProtoMember(4)] public int Version { get; set; }
    [ProtoMember(5)] public string ServerTime { get; set; }

    [ProtoMember(6)] public T? Data { get; set; }

    public void SetSuccess()
    {
        Status = true;
        ErrorCode = ErrorCodeEnum.NoErrorCode;
    }

    public void SetSuccess(string message)
    {
        Status = true;
        Messages.Add(message);
        ErrorCode = ErrorCodeEnum.NoErrorCode;
    }

    public void SetPermissionDeny()
    {
        Status = false;
        ErrorCode = ErrorCodeEnum.PermissionDeny;
        string message = ErrorCode.ToString();
        Messages.Add(message);
    }

    public void SetFail(ErrorCodeEnum code)
    {
        Status = false;
        ErrorCode = code;
        string message = code.ToString();
        Messages.Add(message);
    }

    public void SetFail(string message, ErrorCodeEnum code = ErrorCodeEnum.NoErrorCode)
    {
        Status = false;
        ErrorCode = code;
        Messages.Add(message);
    }

    public void SetFail(Exception ex, ErrorCodeEnum code = ErrorCodeEnum.NoErrorCode)
    {
        Status = false;
        ErrorCode = code;
        string message = $"Message: {ex.Message}";
        Messages.Add(message);
    }

    public void SetFail(IEnumerable<string> messages, ErrorCodeEnum code = ErrorCodeEnum.NoErrorCode)
    {
        Status = false;
        ErrorCode = code;
        foreach (var message in messages)
        {
            Messages.Add(message);
        }
    }
}