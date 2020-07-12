using System;

namespace MonadPlayground.StringArithmetic  {

    public static class StringArithmeticMonad {

        public static int ToInt(this string s) {
            return int.Parse(s);
        }

        public static string Select(
            this string input,
            Func<int, int> transform) {

            return transform(input.ToInt()).ToString();
        }

        public static string SelectMany(
            this string input,
            Func<int, int> select,
            Func<int, int, int> combine) {

            var a = input.ToInt();
            var b = select(a);
            return combine(a, b).ToString();
        }
    }
}
