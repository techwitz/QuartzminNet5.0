﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Quartzmin.Helpers;

public class JsonErrorResponseAttribute : ActionFilterAttribute
{
    private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
    {
        ContractResolver = new DefaultContractResolver(), // PascalCase as default
    };

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            context.Result = new JsonResult(new { ExceptionMessage = context.Exception.Message }, _serializerSettings) { StatusCode = 400 };
            context.ExceptionHandled = true;
        }
    }
}