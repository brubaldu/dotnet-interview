using System.Net;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoApi.Middleware;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        HttpStatusCode code;
        switch (context.Exception)
        {
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }

        var error = new ErrorModel
        (
            (int)code,
            context.Exception.Message,
            context.Exception.StackTrace?.ToString()
        );

        context.Result = new JsonResult( error );
    }
}