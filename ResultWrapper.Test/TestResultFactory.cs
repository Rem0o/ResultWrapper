using System;
using System.Linq;
using Xunit;

namespace ResultWrapper.Test
{
    public static class TestResultFactory
    {
        public static TestResult<T> From<T>(T value, params string[] messages)
        {
            return new TestResult<T>(value, messages);
        }

        public static TestResult<T> From<T>(IResult<T, string> result, params string[] messages)
        {
            return (result is TestResult<T> TestResult ?
                   TestResult.Validate(v => messages) :
                   new TestResult<T>(result.Value, result.Messages.Concat(messages))
               )
               .ToTestResult();
        }

        public static TestResult<T> ToTestResult<T>(this T value, params string[] messages)
        {
            return From(value, messages);
        }

        public static TestResult<T> ToTestResult<T>(this IResult<T, string> result, params string[] messages)
        {
            return From(result, messages);
        }

        public static IResult<T, string> AssertIsSuccess<T>(this IResult<T, string> result, Func<T, Boolean> testFunc = null )
        {
            Assert.True(result.IsSuccess());
            Assert.True(testFunc?.Invoke(result.Value) ?? true);
            return result;
        }

        public static IResult<T, string> AssertIsFailure<T>(this IResult<T, string> result)
        {
            Assert.True(result.HasError());
            return result;
        }
    }
}
