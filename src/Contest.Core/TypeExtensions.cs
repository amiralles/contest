namespace Contest.Core {
    using System;
    using System.Linq;

    public static class TypeExtensions {
        public static bool HasDefaultCtor(this Type type) {
            return type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);
        }
    }
}