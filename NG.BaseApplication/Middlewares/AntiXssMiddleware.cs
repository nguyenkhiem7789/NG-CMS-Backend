
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using NG.BaseReadModels;
using NG.EnumDefine;

namespace NG.BaseApplication.Middlewares;
public class AntiXssMiddleware
{
    private readonly RequestDelegate _next;
    private BaseResponse _error;
    private readonly int _statusCode = (int)HttpStatusCode.BadRequest;

    public AntiXssMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
        {
            var url = context.Request.Path.Value;

            if (CrossSiteScriptingValidation.IsDangerousString(url))
            {
                await RespondWithAnError(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in query string
        if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
        {
            var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

            if (CrossSiteScriptingValidation.IsDangerousString(queryString))
            {
                await RespondWithAnError(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in request content
        var originalBody = context.Request.Body;
        try
        {
            var content = await ReadRequestBody(context);

            if (CrossSiteScriptingValidation.IsDangerousString(content))
            {
                await RespondWithAnError(context).ConfigureAwait(false);
                return;
            }

            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            context.Request.Body = originalBody;
        }
    }

    private static async Task<string> ReadRequestBody(HttpContext context)
    {
        var buffer = new MemoryStream();
        await context.Request.Body.CopyToAsync(buffer);
        context.Request.Body = buffer;
        buffer.Position = 0;

        var encoding = Encoding.UTF8;

        var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
        context.Request.Body.Position = 0;

        return requestContent;
    }

    private async Task RespondWithAnError(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.AddHeaders();
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = _statusCode;

        _error ??= new BaseResponse()
        {
            Status = false,
            ErrorCode = ErrorCodeEnum.AntiXss,
            Messages = new List<string>() { "ANTIXSS" },
            Version = 1,
            // Description = "Error from AntiXssMiddleware",
            // ErrorCode = 500
        };

        await context.Response.WriteAsync(_error.ToJSON());
    }
}

public static class AntiXssMiddlewareExtension
{
    public static IApplicationBuilder UseAntiXssMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AntiXssMiddleware>();
    }
}


/// <summary>
/// Imported from System.Web.CrossSiteScriptingValidation Class
/// </summary>
public static class CrossSiteScriptingValidation
{
    private static readonly char[] StartingChars = { '<', '&' };
    private static readonly string script = "<[script|style|a](.|\n)*?>(.|\n)*?</[script|style]>";

    private static readonly string scriptSttr =
        "(<script|<style|<a|</script|</style|</a|href|onafterprint|onbeforeprint|onbeforeunload|onerror|onhashchange|onload|onmessage|onoffline|ononline|onpagehide|onpageshow|onpopstate|onresize|onstorage|onunload|onblur|onchange|oncontextmenu|onfocus|oninput|oninvalid|onreset|onsearch|onselect|onsubmit|onkeydown|onkeypress|onkeyup|onclick|ondblclick|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|onmousewheel|onwheel|ondrag|ondragend|ondragenter|ondragleave|ondragover|ondragstart|ondrop|onscroll|oncopy|oncut|onpaste|onabort|oncanplay|oncanplaythrough|oncuechange|ondurationchange|onemptied|onended|onerror|onloadeddata|onloadedmetadata|onloadstart|onpause|onplay|onplaying|onprogress|onratechange|onseeked|onseeking|onstalled|onsuspend|ontimeupdate|onvolumechange|onwaiting|ontoggle)";

    #region Public methods

    // public static bool IsDangerousString(string s, out int matchIndex)
    // {
    //     //bool inComment = false;
    //     matchIndex = 0;
    //
    //     for (var i = 0;;)
    //     {
    //         // Look for the start of one of our patterns 
    //         var n = s.IndexOfAny(StartingChars, i);
    //
    //         // If not found, the string is safe
    //         if (n < 0) return false;
    //
    //         // If it's the last char, it's safe 
    //         if (n == s.Length - 1) return false;
    //
    //         matchIndex = n;
    //
    //         switch (s[n])
    //         {
    //             case '<':
    //                 // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
    //                 if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
    //                 break;
    //             case '&':
    //                 // If the & is followed by a #, it's unsafe (e.g. S) 
    //                 if (s[n + 1] == '#') return true;
    //                 break;
    //         }
    //
    //         // Continue searching
    //         i = n + 1;
    //     }
    // }

    public static bool IsDangerousString(string s)
    {
        string content = HttpUtility.HtmlDecode(s);
        Regex rx = new Regex(scriptSttr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        bool isMatch = rx.IsMatch(content);
        return isMatch;
    }

    #endregion

    #region Private methods

    private static bool IsAtoZ(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    #endregion

    public static void AddHeaders(this IHeaderDictionary headers)
    {
        if (headers["P3P"].IsNullOrEmpty())
        {
            headers.Add("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        }
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    public static string ToJSON(this object value)
    {
        return Common.Serialize.JsonSerializeObject(value);
    }
}
