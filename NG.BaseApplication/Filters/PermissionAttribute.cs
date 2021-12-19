using System.Net;
using System.Text.RegularExpressions;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NG.AccountCommands.Commands;
using NG.BaseApplication.Implements;
using NG.BaseApplication.Interfaces;
using VNN.AccountManager.Shared;
using ILogger = DnsClient.Internal.ILogger;

namespace NG.BaseApplication.Filters;

public class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(Type type) : base(type)
    {
    }
}

public class PermissionFilter : IAsyncActionFilter
{
    private readonly ILogger<PermissionFilter> _logger;
    public static HashSet<string> KeysExist = new HashSet<string>();
    
    private string Group { get; }
    private string Name { get; }
    private bool IsRoot { get; }
    private string Key { get; }

    public PermissionFilter(string group, string name, bool isRoot, string key, ILogger<PermissionFilter> logger)
    {
        Group = group;
        Name = name;
        IsRoot = isRoot;
        Key = key;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var actionName = descriptor?.ActionName;
        var controllerName = descriptor?.ControllerName;
        string key = (!string.IsNullOrEmpty(Key) ? Key : $"{controllerName}/{actionName}").ToLower();
        if (!KeysExist.Contains(key))
        {
            IRoleService roleService =
                (IRoleService) context.HttpContext.RequestServices.GetRequiredService(typeof(IRoleService));
            await roleService.ActionDefineAdd(new ActionDefineAddCommand()
            {
                Group = Group,
                Id = key,
                Name = Name,
                IsRoot = IsRoot,
            });
            KeysExist.Add(key);
        }

        IList<object> actionDescriptor = context.ActionDescriptor.EndpointMetadata;
        bool isCheckToken = true;
        foreach (var filterDescriptors in actionDescriptor)
        {
            if (filterDescriptors.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
            {
                await next();
                isCheckToken = false;
                break;
            }
        }

        if (isCheckToken)
        {
            IAuthenService authenService =
                (IAuthenService)context.HttpContext.RequestServices.GetRequiredService(typeof(IAuthenService));
            string sessionKey = context.HttpContext?.User?.FindFirst(ContextService.SessionCode)?.Value;
            var user = await authenService.GetLoginInfo(sessionKey);
            if (user != null)
                {
                    if (user.OtpVerify)
                    {
                        if (user.IsAdministrator)
                        {
                            await next();
                            return;
                        }

                        if (user.GroupAdmins != null && user.GroupAdmins.Contains(Group.ToLower()))
                        {
                            await next();
                            return;
                        }

                        if (user.Permissions == null || user.Permissions.Count <= 0)
                        {
                            context.Result = new ContentResult()
                            {
                                Content = "Permission denied action",
                                StatusCode = (int) HttpStatusCode.Forbidden
                            };
                            return;
                        }

                        if (user.Permissions.Contains(key))
                        {
                            await next(); // the actual action
                        }
                        else
                        {
                            context.Result = new ContentResult()
                            {
                                Content = "Permission denied action",
                                StatusCode = (int) HttpStatusCode.Forbidden
                            };
                        }
                    }
                    else
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Permission denied",
                            StatusCode = (int) HttpStatusCode.Unauthorized
                        };
                    }
                }
                else
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Token expired",
                        StatusCode = (int) HttpStatusCode.Unauthorized
                    };
                }
        }
    }
    
    
}