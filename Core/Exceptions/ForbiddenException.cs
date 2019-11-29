using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Exceptions
{
    public class ForbiddenException : Exception
    {
        public override string Message
        {
            get
            {
                if (Errors != null && Errors.Count() > 0)
                {
                    return string.Join(",", Errors);
                }
                else
                {
                    return base.Message;
                }
            }
        }

        public ForbiddenException() { }

        public ForbiddenException(string message) : base(message)
        {

        }

        public ForbiddenException(IDictionary<string, string> errors)
        {
            Errors = errors;
        }

        public IDictionary<string, string> Errors { get; set; }
    }
}
