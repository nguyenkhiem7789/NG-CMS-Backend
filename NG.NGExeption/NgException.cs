using System.Net;

namespace NG.NGException;

public class NgException : Exception
{
    public List<(string, string)> Messages { get; set; }
    
    public HttpStatusCode HttpCode { get; set; }
    
    private NgException()
    {
        Messages = new List<(string, string)>();
    }

    public NgException(string? message) : this()
    {
        Messages.Add((string.Empty, message));
    }

    public NgException(HttpStatusCode httpCode, string message) : this()
    {
        HttpCode = httpCode;
        Messages.Add((httpCode.ToString(), message));
    }

    public NgException(HttpStatusCode httpCode) : this()
    {
        HttpCode = httpCode;
        Messages.Add((httpCode.ToString(), httpCode.ToString()));
    }

    public void Add(string message)
    {
        Messages.Add((string.Empty, message));
    }

    public new string Message
    {
        get
        {
            string message = string.Empty;
            foreach (var item in Messages)
            {
                if (string.IsNullOrEmpty(item.Item1))
                {
                    if (item.Item2?.Length > 0)
                    {
                        message += $"{item.Item2}, ";
                    }
                }
                else
                {
                    if (item.Item2?.Length > 0)
                    {
                        message += $"{item.Item1}: {item.Item2}, ";
                    }
                    else
                    {
                        message += $"{item.Item1}, ";
                    }
                }
            }

            return message.Trim().TrimEnd(',');
        }
    }
}