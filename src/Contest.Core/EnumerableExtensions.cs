namespace Contest.Core {
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions {
        public static void Each<T>(this IEnumerable<T> target, Action<T> callback) {
            foreach (var item in target)
                callback(item);
        }
    }
}