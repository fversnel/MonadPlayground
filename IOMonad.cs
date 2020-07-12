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

        public class ConsoleFns {
            public readonly IConsole Console;

            public ConsoleFns(IConsole console) {
                Console = console;
            }

            public IO<Unit> Write(char c) {
                return () => {
                    Console.Write(c);
                    return Unit.Default;
                };
            }

            public IO<Unit> Write(string s) {
                return () => {
                    for (int i = 0; i < s.Length; i++) {
                        Console.Write(s[i]);
                    }
                    return Unit.Default;
                };
            }
            public IO<Unit> WriteLine(string line) {
                return Write(line + Environment.NewLine);
            }

            public IO<char> Read() {
                return () => Console.Read();
            }

            public IO<string> ReadUntil(Func<string, bool> predicate) {
                return () => {
                    var result = "";
                    while (!predicate(result)) {
                        result += (from c in Read()
                                   from _1 in Write(c)
                                   select c)();
                    }
                    return result;
                };
            }

            public IO<string> ReadLine() {
                return from line in ReadUntil(r => r.EndsWith(Environment.NewLine))
                       select line.Remove(line.LastIndexOf(Environment.NewLine));
            }

            public IO<string> ReadLinePrefix(string prefix) {
                return from _1 in Write(prefix)
                       from output in ReadLine()
                       select output;
            }
        }

        public delegate int RandomGen();
        public static readonly Func<Random, RandomGen> FromRandom = random => () => random.Next();
        public static readonly Func<RandomGen, IO<int>> RandomInt = randGen => () => randGen();
        public static readonly Func<RandomGen, int, int, IO<int>> RandomRange = (randGen, start, end) => () => {
            var range = end - start + 1;
            return randGen() % range + start;
        };
    }
}