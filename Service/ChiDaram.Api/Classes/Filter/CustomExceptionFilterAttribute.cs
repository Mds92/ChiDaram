using System;
using ChiDaram.Common.Classes;
using ChiDaram.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace ChiDaram.Api.Classes.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var exceptionMessage = exception.GetExceptionMessages();
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(exception.Message);
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.Message))
                    context.Result = new BadRequestObjectResult(new
                    {
                        Data = apiResponse.Message,
                        CustomException = true,
                        apiResponse.Message
                    });
            }
            catch
            {
                // ignored
            }
            context.Result = new BadRequestObjectResult(new
            {
                CustomException = true,
                Message = exceptionMessage,
            });
        }
    }
}
