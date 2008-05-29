using System;
using System.Collections.Generic;
using System.Text;
using Suteki.Common.Validation;

namespace Suteki.Common.Validation
{
    public class Validator : List<Action>
    {
        public void Validate()
        {
            StringBuilder message = new StringBuilder();

            foreach (Action validation in this)
            {
                try
                {
                    validation();
                }
                catch (ValidationException validationException)
                {
                    message.AppendFormat("{0}<br />", validationException.Message);
                }
            }

            if (message.Length > 0)
            {
                throw new ValidationException(message.ToString());
            }
        }
    }
}