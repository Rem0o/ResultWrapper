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
                .ToTestResult()
                .WithMessages(GetTestMessages());

            Assert.False(result.IsSuccess());
        }

        [Fact]
        public void Result_WithMessages_MessagesAdded()
        {
            var messages = GetTestMessages();
            var result = 1.ToTestResult().WithMessages(messages);

            Assert.True(messages.SequenceEqual(result.Messages));
        }

        [Fact]
        public void FailedResults_Combine_CombinedFailingResult()
        {
            var messageA = "Salution";

            var valueA = "Hi ";
            var valueB = "Mark";

            var resultA = valueA.ToTestResult().WithMessages(messageA);
            var resultB = valueB.ToTestResult();

            string CombinationDelegate(string a, string b) => a + b;

            var combinedResult = resultA.Combine(resultB, CombinationDelegate);

            Assert.False(combinedResult.IsSuccess());
            Assert.True(combinedResult.Messages.SequenceEqual(new[] { messageA }));
        }


        [Fact]
        public void SuccessResults_Combine_CombinedSuccessResult()
        {
            var valueA = "Hi ";
            var valueB = "Mark";

            var resultA = valueA.ToTestResult();
            var resultB = valueB.ToTestResult();

            string CombinationDelegate(string a, string b) => a + b;

            var combinedResult = resultA.Combine(resultB, CombinationDelegate);

            Assert.True(combinedResult.IsSuccess());
            Assert.Equal(CombinationDelegate(valueA, valueB), combinedResult.Value);
        }

        [Fact]
        public void SuccessResult_Map_NewSuccessResult()
        {
            var value = 100;
            var result = TestResultFactory.From(value);

            int Double(int v) => v * 2;

            Assert.True(result.IsSuccess());

            var mappedResult = result.Map(Double);

            Assert.True(mappedResult.IsSuccess());
            Assert.Equal(Double(value), mappedResult.Value);
        }

        [Fact]
        public void FailedResult_Map_NewSuccessResult()
        {
            var value = 100;
            var message = "Some error";
            var result = TestResultFactory.From(value)
                .WithMessages(message);

            int Double(int v) => v * 2;

            Assert.False(result.IsSuccess());

            var mappedResult = result.Map(Double);

            Assert.False(mappedResult.IsSuccess());
            Assert.Contains(message, mappedResult.Messages);
        }

        private static string[] GetTestMessages()
        {
            return new[] { "Some message", "And some other message" };
        }
    }
}
