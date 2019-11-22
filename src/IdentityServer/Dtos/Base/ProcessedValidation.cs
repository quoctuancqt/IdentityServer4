namespace IdentityServer.Dtos.Base
{
    using FluentValidation;
    using System;

    public static class ProcessedValidation
    {
        public static ValidationDto CheckValidation<TDto>(TDto model)
        {
            if (model == null) return new ValidationDto(false);

            string assemblyQualifiedName = $"{typeof(TDto).Namespace}.{typeof(TDto).Name}Validator, {typeof(TDto).Assembly}";

            Type type = Type.GetType(assemblyQualifiedName);

            IValidator validator = (IValidator)Activator.CreateInstance(type);

            return new ValidationDto(validator.Validate(model));
        }

        public static ValidationDto Validate<TDto>(this TDto model)
        {
            return CheckValidation(model);
        }
    }
}
