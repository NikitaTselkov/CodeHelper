using System.Linq.Expressions;

namespace CodeHelper.Core
{
    public static class PredicateExtemtions
    {
        public static Func<T, bool>? AndAlso<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 == null && predicate2 == null) return null;
            if (predicate1 == null) return arg => predicate2(arg);
            if (predicate2 == null) return arg => predicate1(arg);

            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> OrElse<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 == null && predicate2 == null) return null;
            if (predicate1 == null) return arg => predicate2(arg);
            if (predicate2 == null) return arg => predicate1(arg);

            return arg => predicate1(arg) || predicate2(arg);
        }

        public static Expression<Func<T, bool>>? FuncToExpression<T>(Func<T, bool>? predicate)
        {
            if (predicate == null) return null;

            return arg => predicate(arg);
        }
    }
}
