// WARNING: Auto generated code. Modifications will be lost!
using System;

namespace Unity.Services.Deployment.Editor.Shared.DependencyInversion
{
    class TypeAlreadyRegisteredException : Exception
    {
        public TypeAlreadyRegisteredException(Type type)
            : base($"A factory for type {type.Name} has already been registered")
        {
        }
    }
}
