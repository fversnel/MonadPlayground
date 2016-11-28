using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonadPlayground
{
    public struct Maybe<T> {
        private Maybe(T value, bool isJust) {
            IsJust = isJust;
            Value = value;
        }

        public bool IsJust { get; }
        public bool IsNothing { get { return !IsJust; } }
        public T Value { get; }

        public override string ToString() {
            if (IsJust) {
                return "Just(" + Value + ")";
            }
            return "Nothing";
        }

        public static Maybe<T> Just(T value) {
            return new Maybe<T>(value, isJust: true);
        }

        public static Maybe<T> Nothing {
            get { return new Maybe<T>(); }
        }
    }

    public static class MaybeMonad {

        public static Maybe<A> ToMaybe<A>(A value) where A : class {
            if (value == null) {
                return Maybe<A>.Nothing;
            }
            return Maybe<A>.Just(value);
        }

        public static Maybe<A> ToMaybe<A>(A? nullable) where A : struct {
            if (!nullable.HasValue) {
                return Maybe<A>.Nothing;
            }
            return Maybe<A>.Just(nullable.Value);
        }
        
        public static Maybe<B> Select<A, B>(
		    this Maybe<A> id,
		    Func<A, B> transform) {

            if (id.IsJust) {
                return Maybe<B>.Just(transform(id.Value));
            }
            return Maybe<B>.Nothing;
	    }

        public static Maybe<A> Where<A>(
		    this Maybe<A> id,
		    Func<A, bool> predicate) {

            if (id.IsJust && predicate(id.Value)) {
                return id;
            }
            return Maybe<A>.Nothing;
	    }

        public static Maybe<C> SelectMany<A, B, C>(
		    this Maybe<A> id,
		    Func<A, Maybe<B>> transform,
		    Func<A, B, C> combine) {

            if (id.IsJust) {
                var transformedId = transform(id.Value);
                if (transformedId.IsJust) {
                    return Maybe<C>.Just(combine(id.Value, transformedId.Value));
                }
            }
            return Maybe<C>.Nothing;
	    }
    }
    
}
