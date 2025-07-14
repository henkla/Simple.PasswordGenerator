using System;

namespace Simple.PasswordGenerator
{
    /// <summary>
    /// Represents errors that occur during password generation or validation.
    /// </summary>
    [Serializable]
    public class PasswordGeneratorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGeneratorException"/> class.
        /// </summary>
        public PasswordGeneratorException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGeneratorException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PasswordGeneratorException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGeneratorException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public PasswordGeneratorException(string message, Exception inner)
            : base(message, inner) { }
    }
}