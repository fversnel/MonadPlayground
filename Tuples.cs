namespace MonadPlayground.Tuples {
    public static class Tuples {
        public static T2<A, B> With<A, B>(this A first, B second) {
            return new T2<A, B>(first, second);
        }

        public static T2<A, B> Tuple<A, B>(A first, B second) {
            return new T2<A, B>(first, second);
        }
    }

    public struct T2<A, B> {
        public readonly A First;
        public readonly B Second;

        public T2(A first, B second) {
            First = first;
            Second = second;
        }

        public override string ToString() {
            return "(" + First + ", " + Second + ")";
        }
    }
}