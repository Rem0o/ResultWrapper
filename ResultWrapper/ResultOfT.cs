using System;
using System.Collections.Generic;
using System.Linq;

namespace ResultWrapper
{
    public abstract class Result<T, MessageType> : Result<MessageType>, IResult<T, MessageType>
    {
        public abstract IResult<U, MessageType> ResultFactory<U>(U value, IEnumerable<MessageType> messages = null);

        public Result(T value, IEnumerable<MessageType> messages = null) : base(messages)
        {
            Value = value;
        }

        public Result(IResult<T, MessageType> value, IEnumerable<MessageType> messages = null) : base(messages.Concat(value.Messages))
        {
            Value = value.Value;
        }

        public override bool IsSuccess()
        {
            return Value != null && base.IsSuccess();
        }

        public IResult<T, MessageType> WithMessages(params MessageType[] messages)
        {
            if (messages != null)
                _messages.AddRange(messages);

            return this;
        }

        public IResult<T, MessageType> WithMessages(params IEnumerable<MessageType>[] messages)
        {
            if (messages != null)
                _messages.AddRange(messages.SelectMany(x => x));

            return this;
        }

        public IResult<V, MessageType> Combine<U, V>(IResult<U, MessageType> result, Func<T, U, V> combineDelegate)
        {
            return IsSuccess() && result.IsSuccess() ?
                ResultFactory(combineDelegate(Value, result.Value)) :
                ResultFactory(default(V))
                    .WithMessages(Messages)
                    .WithMessages(result.Messages);
        }

        public IResult<U, MessageType> Map<U>(Func<T, U> mapperDelegate)
        {
            IResult<U, MessageType> resultMapper(T val) => ResultFactory(mapperDelegate(Value));
            return Map(resultMapper);
        }

        public IResult<U, MessageType> Map<U>(Func<T, IResult<U, MessageType>> mapperDelegate)
        {
            var result = IsSuccess() ? mapperDelegate(Value) : ResultFactory(default(U));
            return result.WithMessages(Messages);
        }

        public IResult<T, MessageType> Validate(Func<T, IEnumerable<MessageType>> validationDelegate)
        {
            var result = Map(validationDelegate);
            return result.Combine(this, (a, b) => b)
                .WithMessages(result.Value);
        }

        public IResult<T, MessageType> Validate(Func<T, MessageType> validationDelegate)
        {
            Func<T, IEnumerable<MessageType>> validationDelegateMany = val => new[] { validationDelegate(val) };
            return Validate(validationDelegateMany);
        }

        public IResult<T, MessageType> Do(Action<T> action)
        {
            if (IsSuccess())
                action(Value);

            return this;
        }

        public T Value { get; protected set; }
    }
}
