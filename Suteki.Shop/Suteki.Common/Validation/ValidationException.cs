using System;
using System.Collections.Generic;
using System.Linq;

namespace Suteki.Common.Validation
{
    [global::System.Serializable]
    public class ValidationException : ApplicationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

		[Obsolete("This constructor is only here for backwards compatibility")]
		public ValidationException(string error, IEnumerable<ValidationException> errors) : base(error)
		{
			this.errors = errors.ToArray();
		}

    	public ValidationException(IEnumerable<ValidationException> errors)
    	{
			this.errors = errors.ToArray();
    	}

    	private ValidationException[] errors = new ValidationException[0];

    	public ValidationException[] Errors
    	{
			get { return errors;  }
    	}
    }
}