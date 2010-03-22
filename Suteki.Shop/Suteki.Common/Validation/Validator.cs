using System;
using System.Collections.Generic;
using System.Linq;

namespace Suteki.Common.Validation
{
    public class Validator : List<Action>
    {
        public void Validate()
        {
			var errors = new List<ValidationException>();

            foreach (var validation in this)
            {
                try
                {
                    validation();
                }
                catch (ValidationException validationException)
                {
					errors.Add(validationException);
                }
            }

            if (errors.Count > 0)
            {
				//backwards compatibility
				var error = string.Join("", errors.Select(x => x.Message + "<br />").ToArray());
                throw new ValidationException(error, errors);
            }
        }
    }
}