using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ResultWrapper.Test
{
    public class TestResultTest
    {
        [Fact]
        public void ResultConstructor_WithMessages_MessagesInResult()
        {
            var value = 100.0;
            var messages = GetTestMessages();
            var result = new TestResult<double>(value, messages);

            Assert.True(messages.SequenceEqual(result.Messages));
        }

        [Fact]
        public void Result_WithMessages_SuccessAffected()
        {
            var someDictionary = new Dictionary<string, int>
            {
                { "John", 100 },
                { "Alex", 50 }
            };

            var result = someDictionary
                .ToTestResult(GetTestMessages())
                .AssertIsFailure();
        }

        [Fact]
        public void Result_WithMessages_MessagesAdded()
        {
            var messages = GetTestMessages();
            var result = 1.ToTestResult(messages);

            Assert.True(messages.SequenceEqual(result.Messages));
        }

        [Fact]
        public void FailedResults_Combine_CombinedFailingResult()
        {
            var messageA = "Salution";
            var messageB = "Sir!";

            var valueA = "Hi";
            var valueB = "Mark";

            var resultA = valueA.ToTestResult(messageA);
            var resultB = valueB.ToTestResult(messageB);

            string CombinationDelegate(string a, string b) => a + b;

            var combinedResult = resultA
                .Combine(resultB, CombinationDelegate)
                .AssertIsFailure();

            Assert.True(combinedResult.Messages.SequenceEqual(new[] { messageA, messageB }));
        }


        [Fact]
        public void SuccessResults_Combine_CombinedSuccessResult()
        {
            var valueA = "Hi ";
            var valueB = "Mark";

            var resultA = valueA.ToTestResult();
            var resultB = valueB.ToTestResult();

            string CombinationDelegate(string a, string b) => a + b;

            var combinedResult = resultA
                .Combine(resultB, CombinationDelegate)
                .AssertIsSuccess();

            Assert.Equal(CombinationDelegate(valueA, valueB), combinedResult.Value);
        }

        [Fact]
        public void SuccessResult_Map_NewSuccessResult()
        {
            var value = 100;
            var result = value.ToTestResult();

            int Double(int v) => v * 2;

            Assert.True(result.IsSuccess());

            var mappedResult = result
                .Map(Double)
                .AssertIsSuccess();

            Assert.Equal(Double(value), mappedResult.Value);
        }

        [Fact]
        public void FailedResult_Map_NewSuccessResult()
        {
            var value = 100;
            var message = "Some error";
            var result = value.ToTestResult(message);
            int Double(int v) => v * 2;

            Assert.False(result.IsSuccess());

            var mappedResult = result
                .Map(Double)
                .AssertIsFailure();

            Assert.Contains(message, mappedResult.Messages);
        }

        [Fact]
        public void SuccessResult_MapSuccesResult_NewSuccessResult()
        {
            var value = new[] { 1, 2, 4 };
            var result = value.ToTestResult();

            var mappedResult = result
                .MapResult(array => array.Sum()
                .ToTestResult())
                .AssertIsSuccess();

            Assert.Equal(value.Sum(), mappedResult.Value);
        }

        [Fact]
        public void SuccessResult_MapFailedResult_NewFailedResult()
        {
            var value = new[] { 1, 2, 4 };
            var result = value.ToTestResult();

            var mappedResult = result
              .MapResult(array => array.Sum()
              .ToTestResult("With some error"))
              .AssertIsFailure();
        }

        [Fact]
        public void SuccessResult_ValidateReturnsError_NewFailedResult()
        {
            var value = "some value";
            var result = value.ToTestResult();

            var validatedResult = result
                .Validate(v => new[] { "Some error" })
                .AssertIsFailure();
        }

        [Fact]
        public void SuccessResult_ValidateReturnsNoError_NewFailedResult()
        {
            var value = "some value";
            var result = value.ToTestResult();

            var validatedResult = result
                .Validate(v => new string[] {})
                .AssertIsSuccess(v => v == value);
        }

        [Fact]
        public void SuccessResult_DoSomething_DidSomething()
        {
            bool didSomething = false;
            void DoSomething()
            {
                didSomething = true;
            }

            "Do something!".ToTestResult()
                .AssertIsSuccess()
                .Do(x => DoSomething());

            Assert.True(didSomething);
        }

        [Fact]
        public void FailedResult_DoSomething_DidNotDoSomething()
        {
            bool didSomething = false;
            void DoSomething()
            {
                didSomething = true;
            }

            "Do something!".ToTestResult("DENIED!")
                .AssertIsFailure()
                .Do(x => DoSomething());

            Assert.False(didSomething);
        }

        [Fact]
        public void FailedResult_Catch_SuccessResult()
        {
            var catchedFailedResult = 1.ToTestResult("Failed!")
                .AssertIsFailure()
                .Catch( messages => {
                    return 2;
                });

            catchedFailedResult.AssertIsSuccess();
        }

        [Fact]
        public void FailedResult_Catch_NewFailedResult()
        {
            var catchedFailedResult = 1.ToTestResult("Failed!")
                .AssertIsFailure()
                .Catch( messages => {
                    return 2.ToTestResult("DENIED");
                });

            catchedFailedResult.AssertIsFailure();
        }

        private static string[] GetTestMessages()
        {
            return new[] { "Some message", "And some other message" };
        }
    }
}
