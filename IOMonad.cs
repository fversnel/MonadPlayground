using System;

namespace MonadPlayground.IO {
    public delegate A IO<A>();

    public static class IOMonad {
        public static IO<A> AsIO<A>(this Func<A> work) {
            return () => work();
        }

        public static Func<A, IO<Unit>> AsIO<A>(this Action<A> work) {
            return value => () => {
                work(value);
                return Unit.Default;
            };
        }

        public static IO<B> Select<A, B>(
            this IO<A> input,
            Func<A, B> transform) {
            return () => transform(input());
        }

        public static IO<C> SelectMany<A, B, C>(
            this IO<A> input,
            Func<A, IO<B>> select,
            Func<A, B, C> combine) {
            return () => {
                var a = input();
                var b = select(a)();
                return combine(a, b);
            };
        }
    }

    public static class IOFns {
        public delegate int RandomGen();

        public static readonly Func<string, IO<Unit>> Write = IOMonad.AsIO<string>(line => Console.Write(line));
        public static readonly Func<string, IO<Unit>> WriteLine = line => Write(line + Environment.NewLine);
        public static readonly IO<string> ReadLine = () => Console.ReadLine();

        public static readonly Func<string, IO<string>> ReadLinePrefix = text => {
            return from _1 in Write(text)
                from output in ReadLine
                select output;
        };

        public static readonly Func<Random, RandomGen> FromRandom = random => () => random.Next();
        public static readonly Func<RandomGen, IO<int>> RandomInt = randGen => () => randGen();

        public static readonly Func<RandomGen, int, int, IO<int>> RandomRange = (randGen, start, end) => () => {
            var range = end - start + 1;
            return randGen() % range + start;
        };
    }
}