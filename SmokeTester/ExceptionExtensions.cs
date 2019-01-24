using System;
using System.Text;

namespace Forte.SmokeTester
{
    public static class ExceptionExtensions
    {
        public static string FlattenInnerMessages(this Exception exception, string separator = " ==> ")
        {
            if (exception == null)
            {
                return null;
            }

            var result = new StringBuilder(exception.Message);
            while (exception.InnerException != null)
            {
                result.Append(separator);
                result.Append(exception.InnerException.Message);

                exception = exception.InnerException;
            }

            return result.ToString();
        }
    }
}