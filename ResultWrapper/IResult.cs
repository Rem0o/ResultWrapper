using System;
using System.Collections.Generic;

namespace ResultWrapper
{
    public interface IResult<MessageType>
    {
        bool IsSuccess();
        IEnumerable<MessageType> Messages { get; }
    }
}
