﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using TryMLearning.Model.Exceptions;

namespace TryMLearning.WebAPI.App_Infrastructure
{
    public class ApplicationExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
            {
                return;
            }

            if (actionExecutedContext.Exception.GetType() == typeof(ValidationException))
            {
                var validationException = (ValidationException) actionExecutedContext.Exception;
                var data = new
                {
                    validationException.Message,
                    ValidationErrors = validationException.Errors
                };

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.NotAcceptable, data);
            }
            else if (actionExecutedContext.Exception.GetType() == typeof(UnauthorizedAccessException))
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Forbidden, "Unauthorized Access");
            }
        }
    }
}