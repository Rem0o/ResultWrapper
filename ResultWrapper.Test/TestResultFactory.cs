namespace ResultWrapper.Test
{
    public static class TestResultFactory
    {
        public static TestResult<T> From<T>(T value)
        {
            return new TestResult<T>(value);
        }

        public static TestResult<T> From<T>(IResult<T, string> result)
        {
            return result is TestResult<T> TestResult ?
               TestResult :
               new TestResult<T>(result.Value, result.Messages);
        }

        public static TestResult<T> ToTestResult<T>(this T value)
        {
            return From(value);
        }

        public static TestResult<T> ToTestResult<T>(this IResult<T, string> result)
        {
            return From(result);
        }
    }
}
