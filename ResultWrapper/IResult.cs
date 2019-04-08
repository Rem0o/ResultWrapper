using System;
using System.Collections.Generic;

namespace ResultWrapper
{
    public interface IResult<MessageType>
    {
        bool IsSuccess();
        bool HasError();
        IEnumerable<MessageType> Messages { get; }
    }
}
