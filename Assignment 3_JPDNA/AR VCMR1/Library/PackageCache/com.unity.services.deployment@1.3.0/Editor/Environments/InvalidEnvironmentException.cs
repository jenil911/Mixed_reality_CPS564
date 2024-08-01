using System;
using System.Runtime.Serialization;
using Unity.Services.Core.Editor.Environments;

namespace Unity.Services.Deployment.Editor.Environments
{
    /// <summary>
    /// Type of exception thrown when using an invalid environment.
    /// </summary>
    [Serializable]
    class InvalidEnvironmentException : Exception
    {
        /// <summary>
        /// InvalidEnvironmentException constructor.
        /// </summary>
        /// <param name="validationResult"><see cref="ValidationResult"/> that caused the exception.</param>
        public InvalidEnvironmentException(ValidationResult validationResult)
            : base(validationResult.ErrorMessage)
        {
        }

        /// <summary>
        /// InvalidEnvironmentException constructor.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="SerializationInfo"/>.</param>
        /// <param name="streamingContext">The <see cref="StreamingContext"/>.</param>
        protected InvalidEnvironmentException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
