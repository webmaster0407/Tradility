using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tradility.Common.Utils
{
    public class TEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> comparerDelegate;

        public TEqualityComparer(Func<T, T, bool> comparerDelegate)
        {
            if (comparerDelegate == null)
                throw new ArgumentNullException(nameof(comparerDelegate));

            this.comparerDelegate = comparerDelegate;
        }

        public override bool Equals(T? x, T? y) => comparerDelegate(x!, y!);

        public override int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
    }
}
