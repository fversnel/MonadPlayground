using System;

namespace MonadPlayground {

    public delegate A Reader<A, Environment>(Environment environment);

    public static class ReaderMonad {

        public static Reader<A, Environment> Construct<A, Environment>(A value) {
            return env => value;
        }

        public static Reader<B, Environment> Select<A, B, Environment>(
            this Reader<A, Environment> reader,
            Func<A, B> transform) {

            return env => transform(reader(env));
        }

        public static Reader<C, Environment> SelectMany<A, B, C, Environment>(
            this Reader<A, Environment> reader,
            Func<A, Reader<B, Environment>> transform,
            Func<A, B, C> combine) {

            return env => {
                var firstValue = reader(env);
                var secondValue = transform(firstValue)(env);
                return combine(firstValue, secondValue);
            };
        }
    }
}
