using System.Collections.Generic;

namespace ResultWrapper.Test
{
    public class TestResult<T> : Result<T, string>
    {
        public TestResult(T value, IEnumerable<string> messages = null) : base(value, messages) { }

        public override bool IsSuccess() => base.IsSuccess();

        public override IResult<U, string> ResultFactory<U>(U value, IEnumerable<string> messages = null)
        {
            return new TestResult<U>(value, messages);
        }
    }
}
