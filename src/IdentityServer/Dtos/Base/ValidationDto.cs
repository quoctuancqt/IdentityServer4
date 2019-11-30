namespace IdentityServer.Dtos.Base
{
    using FluentValidation.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    public class ValidationDto
    {
        public bool IsValid { get; private set; }

        public object Errors { get; private set; }

        public ValidationDto() : this(true)
        {
        }

        public ValidationDto(bool isValid)
        {
            IsValid = isValid;
        }

        public ValidationDto(ValidationResult validationResult)
        {
            IsValid = validationResult.IsValid;

            if (!IsValid)
            {
                Errors = GetErrors(validationResult.Errors);
            }
        }

        private object GetErrors(IList<ValidationFailure> Errors)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            int length = Errors.Count();

            foreach (var error in Errors.Select((item, i) => new
            {
                item = item,
                i = i
            }))
            {
                if (error.i > 0 && error.i < length) sb.Append(',');

                sb.AppendFormat("\"{0}\":\"{1}\"", FirstCharToLower(error.item.PropertyName), error.item.ErrorMessage.Replace("'", ""));

            }

            sb.Append('}');
            
            return JsonConvert.DeserializeObject(sb.ToString(), typeof(object));
        }

        private static string FirstCharToLower(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.FirstOrDefault().ToString().ToLower() + input.Substring(1);
            }
        }
    }
}
