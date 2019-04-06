using System;
using System.Collections.Generic;
using System.Linq;

namespace ResultWrapper
{
    public abstract class Result<T, MessageType> : Result<MessageType>, IResult<T, MessageType>
    {
        protected abstract IResult<U, MessageType> ResultFactory<U>(U value, IEnumerable<MessageType> messages = null);

        public Result(T value, IEnumerable<MessageType> messages = null) : base(messages)
        {
            Value = value;
        }

        public Result(IResult<T, MessageType> result, IEnumerable<MessageType> messages = null) : base(messages.Concat(result.Messages))
        {
            Value = result.Value;
        }

        public override bool IsSuccess()
        {
            return Value != null && base.IsSuccess();
        }

        public IResult<V, MessageType> Combine<U, V>(IResult<U, MessageType> result, Func<T, U, V> combineDelegate)
        {
            return result
                .MapResult<V>( u => this.Map( t => combineDelegate(t, u)))
                .Catch(m => ResultFactory(default(V), this.Messages.Concat(m)));
        }

        public IResult<U, MessageType> Map<U>(Func<T, U> mapperDelegate)
        {
            IResult<U, MessageType> resultMapper(T val) => ResultFactory(mapperDelegate(Value));
            return MapResult(resultMapper);
        }

        public IResult<U, MessageType> MapResult<U>(Func<T, IResult<U, MessageType>> mapperDelegate)
        {
            return IsSuccess() ? mapperDelegate(Value) : ResultFactory(default(U), Messages);
        }

        public IResult<T, MessageType> Validate(Func<T, IEnumerable<MessageType>> validationDelegate)
        {
            return Map(validationDelegate)
                .MapResult( m => ResultFactory(Value, m));
        }

        public IResult<T, MessageType> Do(Action<T> action)
        {
            if (IsSuccess())
                action(Value);

            return this;
        }

        public IResult<T, MessageType> Catch(Func<IEnumerable<MessageType>, T> mapperDelegate)
        {
            if (!IsSuccess())
                return ResultFactory(mapperDelegate(this.Messages));

            return ResultFactory(this.Value, this.Messages);
        }

        public IResult<T, MessageType> Catch(Func<IEnumerable<MessageType>, IResult<T, MessageType>> mapperDelegate)
        {
             if (!IsSuccess())
                return mapperDelegate(this.Messages);

            return ResultFactory(this.Value, this.Messages);
        }

        public T Value { get; protected set; }
    }
}
