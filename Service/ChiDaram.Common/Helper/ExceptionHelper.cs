using System;

namespace ChiDaram.Common.Helper
{
    public static class ExceptionHelper
    {
        public static string GetExceptionMessages(this Exception exception)
        {
            var exceptionMessage = exception.Message;
            exception = exception.InnerException;
            while (exception != null)
            {
                exceptionMessage += $"{Environment.NewLine}InnerException: {exception.Message}";
                exception = exception.InnerException;
            }
            return exceptionMessage;
        }
    }
}
