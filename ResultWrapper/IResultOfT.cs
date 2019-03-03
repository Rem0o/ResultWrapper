using System;
using System.Collections.Generic;
using System.Text;

namespace ResultWrapper
{
    public interface IResult<T, MessageType> : IResult<MessageType>
    {
        IResult<T, MessageType> WithMessages(params MessageType[] messages);
        IResult<T, MessageType> WithMessages(params IEnumerable<MessageType>[] messages);
        IResult<V, MessageType> Combine<U, V>(IResult<U, MessageType> result, Func<T, U, V> combineDelegate);
        IResult<U, MessageType> Map<U>(Func<T, U> mapperDelegate);
        IResult<U, MessageType> Map<U>(Func<T, IResult<U, MessageType>> mapperDelegate);
        IResult<T, MessageType> Validate(Func<T, IEnumerable<MessageType>> validationDelegate);
        IResult<T, MessageType> Validate(Func<T, MessageType> validationDelegate);
        IResult<T, MessageType> Do(Action<T> action);
        T Value { get; }
    }
}
