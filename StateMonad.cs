using System;
using MonadPlayground.Tuples;

namespace MonadPlayground.State {

    public delegate T2<A, State> WithState<A, State>(State state);

    public static class StateMonad {

        public static A ExtractValue<A, State>(this WithState<A, State> withState, State state) {
            return withState(state).First;
        }

        public static State ExtractState<A, State>(this WithState<A, State> withState, State state) {
            return withState(state).Second;
        }

        public static WithState<A, State> WithState<A, State>(this A value) {
            return state => value.With(state);
        }

        public static WithState<A, State> Construct<A, State>(A value) {
            return state => value.With(state);
        }

        public static WithState<B, State> Select<A, B, State>(
            this WithState<A, State> withState,
            Func<A, B> transform) {

            return state => {
                var initialValue = withState.ExtractValue(state);
                return transform(initialValue).With(state);
            };
        }

        public static WithState<C, State> SelectMany<A, B, C, State>(
            this WithState<A, State> initialStateToPair,
            Func<A, WithState<B, State>> select,
            Func<A, B, C> combine) {

            return state => {
                var pair1 = initialStateToPair(state);
                var pair2 = select(pair1.First)(pair1.Second);
                return combine(pair1.First, pair2.First).With(pair2.Second);
            };
        }
    }
}
