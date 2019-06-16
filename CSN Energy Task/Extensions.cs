using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSN_Energy_Task
{
    public static class Extensions
    {
        public static void WriteToConsole<T>(this Result<T> result)
        {
            if (result.IsFailure)
            {
                Console.WriteLine(result.Error);
                return;
            }
                
            Console.WriteLine(result.Value);
        }

        public static bool HasFailure<T>(this IEnumerable<Result<T>> results)
        {
            if (results.Any(r => r.IsFailure))
                return true;

            return false;
        }

        public static Result GetFailure<T>(this IEnumerable<Result<T>> results)
        {
            return results.First(x => x.IsFailure);
        }

        public static Result<T> ToSingleResult<T>(this Result<Result<T>> result)
        {
            if (result.IsFailure)
                return Result.Fail<T>(result.Error);

            return result.Value;
        }
    }
}
