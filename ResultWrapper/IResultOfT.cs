using System;
using System.Collections.Generic;
using System.Text;

namespace ResultWrapper
{
    public interface IResult<T, MessageType> : IResult<MessageType>
    {
        IResult<V, MessageType> Combine<U, V>(IResult<U, MessageType> result, Func<T, U, V> combineDelegate);
        IResult<U, MessageType> Map<U>(Func<T, U> mapperDelegate);
        IResult<U, MessageType> MapResult<U>(Func<T, IResult<U, MessageType>> mapperDelegate);
        IResult<T, MessageType> Validate(Func<T, IEnumerable<MessageType>> validationDelegate);
        IResult<T, MessageType> OnSuccess(Action<T> action);
        IResult<T, MessageType> OnError(Func<IEnumerable<MessageType>, T> mapperDelegate);
        IResult<T, MessageType> OnError(Func<IEnumerable<MessageType>, IResult<T, MessageType>> mapperDelegate);
        T Value { get; }
    }
}