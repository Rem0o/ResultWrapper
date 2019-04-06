using System.Collections.Generic;
using System.Linq;

namespace ResultWrapper
{
    public abstract class Result<MessageType> : IResult<MessageType>
    {
        protected Result(IEnumerable<MessageType> messages = null)
        {
            if (messages != null)
            {
                _messages.AddRange(messages);
            }
        }

        public virtual bool IsSuccess()
        {
            return Messages.Any() == false;
        }

        protected List<MessageType> _messages = new List<MessageType>();

        public IEnumerable<MessageType> Messages => _messages;
    }
}
