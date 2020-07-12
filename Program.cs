using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MonadPlayground.State;
using MonadPlayground.IO;
using MonadPlayground.Maybe;
using MonadPlayground.Reader;
using MonadPlayground.Tuples;
using MonadPlayground.StringArithmetic;

namespace MonadPlayground {

    public static class Program {

        public interface IMonoid<A> {
            A Zero { get; }
            Func<A, A, A> Add { get; }
        }

        public class Monoid<A> : IMonoid<A> {
            public Monoid(A zero, Func<A, A, A> add) {
                Zero = zero;
                Add = add;
            }

            public A Zero { get; }
            public Func<A, A, A> Add { get; }
        }

        public static readonly IMonoid<int> ClockMonoid = new Monoid<int>(12, (valueA, valueB) => (valueA + valueB) % 12);

        public class StateValuePair<TState, TValue> {
            public readonly TState State;
            public readonly TValue Value;

            public StateValuePair(TState state, TValue value) {
                State = state;
                Value = value;
            }
        }

        static void Main(string[] args) {
            var numbers = new [] {1, 3, 4, 12};
            var sum = numbers.Sum(ClockMonoid);
//            Console.WriteLine(sum);
//            Console.ReadLine();
//
//            var aapState = StateMonad<int, int>.Construct(42)
//                .Bind(StateMonad<int, int>.Converter(aap1 => aap1 + 42))
//                .Lookup(5)
//                .Increment()
//                .Increment();
//            Console.WriteLine("aap " + aapState.Value + ", state " + aapState.State);
//            Console.ReadLine();


            var aNumber = from n in Maybe<int>.Just(30)
                          where n < 50
                          where n > 20
                          select n;

            var state = from s in 42.WithState<int, int>()
                        from s1 in UpdateState(s, 5)
                        from s2 in UpdateState(s1, 4)
                        from s3 in UpdateState(s2, 3)
                        from s4 in UpdateState(s3, 2)
                        select s3;

            var runReader = from s in 42.WithReader<int, ReaderEnvironment>()
                            from s1 in ReadAndCalculate(s)
                            from s2 in ReadAndCalculate(s1)
                            select s2;

            var readerResult = runReader(new ReaderEnvironment(5, "reader"));

            var console = new IOFns.ConsoleFns(DotNetConsole.Default);
            var random = IOFns.FromRandom(new Random());
            var runProgram = from input in console.ReadLinePrefix("input: ")
                             from r in IOFns.RandomRange(random, 5, 10)
                             from _1 in console.WriteLine("State: " + state(0) + ", random number " + r)
                             from _2 in console.WriteLine("input was: " + input + ", reader produced: " + readerResult)
                             from _3 in console.ReadLinePrefix("press any key to exit")
                             select Unit.Default;


            //runProgram();

            var stringNumber = from a in "5"
                               from a1 in a * 2
                               select a1;

            console.WriteLine("string number " + stringNumber)();
            console.ReadLine()();
        }

        public static T Sum<T>(this IEnumerable<T> collection, IMonoid<T> monoid) {
            return collection.Aggregate(monoid.Add);
        }

        public static WithState<int, int> UpdateState(int value, int toAdd) {
            return state => (value + toAdd).With(state + 1);
        }

        public static Reader<int, ReaderEnvironment> ReadAndCalculate(int input) {
            return env => input + env.ContextA;
        }

        public static Maybe<int> Under42(int value) {
            if (value < 42) {
                return Maybe<int>.Just(value);
            }
            return Maybe<int>.Nothing;
        }

        public static Maybe<int> Under50(int value) {
            if (value < 50) {
                return Maybe<int>.Just(value);
            }
            return Maybe<int>.Nothing;
        }

        public static Maybe<int> Above60(int value) {
            if (value > 60) {
                return Maybe<int>.Just(value);
            }
            return Maybe<int>.Nothing;
        }

        public class ReaderEnvironment {
            public readonly int ContextA;
            public readonly string ContextB;

            public ReaderEnvironment(int contextA, string contextB) {
                ContextA = contextA;
                ContextB = contextB;
            }
        }
    }
}
