using System;

namespace HaskellTypeClasses
{
    public class Maybe
    {
        public static Monad<A> Nothing<A>() { return Monad<A>.Nothing; }
        public static Monad<A> Just<A>(A a) { return Monad<A>.Just(a); }

        public class Monad<A>
        {
            public static readonly Monad<A> Nothing = new Monad<A>(default(A));

            private A a;

            private Monad(A a)
            {
                this.a = a;
            }

            public static Monad<A> Just(A a)
            {
                return new Monad<A>(a);
            }

            public bool isNothing { get { return this == Nothing; } }

            public bool isJust { get { return !isNothing; } }

            public A fromJust { get { if (isJust) return a; else throw new InvalidOperationException(); } }

            public A fromMaybe(A @default)
            {
                return isJust ? fromJust : @default;
            }

            public override int GetHashCode()
            {
                return (a==null?98654132:a.GetHashCode()) ^ isJust.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (this == obj) return true;
                if (obj == Monad<A>.Nothing && this != Monad<A>.Nothing) return false;
                var c = obj as Monad<A>;
                if (c == null) return false;
                if (c.a == null && this.a != null) return false;
                return c.a.Equals(this.a);
            }

            public Monad<B> Bind<B>(
                Func<A, Monad<B>> ab
                )
            {
                return this.isNothing ? Monad<B>.Nothing : ab(this.a);
            }

            public Monad<C> Bind<B, C>(
                Func<A, Monad<B>> ab,
                Func<B, Monad<C>> bc)
            {
                return this.Bind(ab).Bind(bc);
            }

            public Monad<D> Bind<B, C, D>(
                Func<A, Monad<B>> ab,
                Func<B, Monad<C>> bc,
                Func<C, Monad<D>> cd)
            {
                return this.Bind(ab, bc).Bind(cd);
            }

            public Monad<E> Bind<B, C, D, E>(
                Func<A, Monad<B>> ab,
                Func<B, Monad<C>> bc,
                Func<C, Monad<D>> cd,
                Func<D, Monad<E>> de)
            {
                return this.Bind(ab, bc, cd).Bind(de);
            }

            public Monad<A> Do(Action<A> action)
            {
                if (this.isJust)
                {
                    action(this.a);
                }
                return this;
            }

        }
    }
}
