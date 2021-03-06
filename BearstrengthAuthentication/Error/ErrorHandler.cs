﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BearstrengthAuthentication.Error
{
    public static class ErrorHandler
    {
        public static async Task HandleHttpExceptions(HttpContext context)
        {
            var exception = 
                context.Features.Get<IExceptionHandlerPathFeature>().Error;

            var response = getErrorResponse(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.Status;
            await context.Response.WriteAsync(response.ToJson());
        }

        private static ErrorResponse getErrorResponse(Exception exception)
        {
            var errorResponse = new ErrorResponse
            {
                Status = 503,
                Message = ErrorConstants.ServiceUnavailable,
                ErrorDetail = null
            };

            if (!(exception is AuthenticationAppException))
            {
                return errorResponse;
            }

            var bearstrengthException = (AuthenticationAppException)exception;
            errorResponse.ErrorDetail = bearstrengthException.Messages;

            if (bearstrengthException is ConflictException)
            {
                errorResponse.Message = ErrorConstants.Conflict;
                errorResponse.Status = 409;
            }
            return errorResponse;
        }
    }
}
