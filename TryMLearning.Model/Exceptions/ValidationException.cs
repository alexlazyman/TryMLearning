﻿using System;
using System.Collections.Generic;
using System.Linq;
using TryMLearning.Model.Validation;

namespace TryMLearning.Model.Exceptions
{
    public class ValidationException : Exception
    {
        private const string ValidationErrorsKey = "Errors";

        public ValidationException()
        {
            Errors = null;
        }

        public ValidationException(string message, IEnumerable<ValidationError> errors = null)
            : base(message)
        {
            Errors = errors?.ToArray();
        }

        public ValidationError[] Errors
        {
            get => Data[ValidationErrorsKey] as ValidationError[];
            protected set => Data[ValidationErrorsKey] = value;
        }
    }
}