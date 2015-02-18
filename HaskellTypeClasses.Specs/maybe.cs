using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace HaskellTypeClasses.Specs
{
    [TestClass]
    public class MaybeSpecs
    {

        [TestMethod]
        public void Nothing_isJust_should_be_false()
        {
            Maybe.Nothing<int>().isJust.ShouldBe(false);
        }

        [TestMethod]
        public void A_Just_isJust_should_be_true()
        {
            Maybe.Just(1).isJust.ShouldBe(true);
        }

        [TestMethod]
        public void Nothing_isNothing_should_be_true()
        {
            Maybe.Nothing<int>().isNothing.ShouldBe(true);
        }

        [TestMethod]
        public void A_Just_isNothing_should_be_false()
        {
            Maybe.Just(1).isNothing.ShouldBe(false);
        }

        [TestMethod]
        public void This_is_the_regular_way_you_would_use_a_maybe()
        {
            var result = Maybe.Just("25")
                .Bind(
                    s =>
                    {
                        // return int if parsable
                        int i = 0;
                        return int.TryParse(s, out i) ? Maybe.Just(i) : Maybe.Nothing<int>();
                    },
                    i =>
                        // return sqrt if > 0
                        i < 0 ? Maybe.Nothing<double>() : Maybe.Just(Math.Sqrt((double)i))
                ).Do(
                    d =>
                        // print
                        Console.WriteLine(d)
                ).Bind(
                    d =>
                        // subtract 2
                        Maybe.Just(d - 2.0)
                );

            // one should never actually use this, but use do & bind IRL
            result.isJust.ShouldBe(true);
            result.fromJust.ShouldBe(3.0);
        }


        [TestMethod]
        public void A_nothing_should_not_invoke_any_functions()
        {
            var methods = new Methods();

            var result = Maybe.Nothing<string>()
                .Bind(
                    methods.Parse_string_as_int,
                    methods.Squareroot_int
                ).Do(
                    methods.Print_double
                ).Bind(
                    methods.Subtract_2
                );

            result.ShouldBe(Maybe.Nothing<double>());

            methods.Parse_string_as_int_was_called.ShouldBe(false);
            methods.Squareroot_int_was_called.ShouldBe(false);
            methods.Print_double_was_called.ShouldBe(false);
            methods.Subtract_2_was_called.ShouldBe(false);
        }

        [TestMethod]
        public void A_non_parsable_string_should_return_a_nothing()
        {
            var methods = new Methods();

            var result = Maybe.Just("@ToJans")
                .Bind(
                    methods.Parse_string_as_int,
                    methods.Squareroot_int
                ).Do(
                    methods.Print_double
                ).Bind(
                    methods.Subtract_2
                );

            result.ShouldBe(Maybe.Nothing<double>());

            methods.Parse_string_as_int_was_called.ShouldBe(true);
            methods.Squareroot_int_was_called.ShouldBe(false);
            methods.Print_double_was_called.ShouldBe(false);
            methods.Subtract_2_was_called.ShouldBe(false);
        }

        [TestMethod]
        public void A_parsable_negative_int_should_return_a_nothing()
        {
            var methods = new Methods();

            var result = Maybe.Just("-5")
                .Bind(
                    methods.Parse_string_as_int,
                    methods.Squareroot_int
                ).Do(
                    methods.Print_double
                ).Bind(
                    methods.Subtract_2
                );

            result.ShouldBe(Maybe.Nothing<double>());

            methods.Parse_string_as_int_was_called.ShouldBe(true);
            methods.Squareroot_int_was_called.ShouldBe(true);
            methods.Print_double_was_called.ShouldBe(false);
            methods.Subtract_2_was_called.ShouldBe(false);
        }


        [TestMethod]
        public void A_parsable_positive_int_should_return_it_s_squareroot()
        {
            var methods = new Methods();

            var result = Maybe.Just("25")
                .Bind(
                    methods.Parse_string_as_int,
                    methods.Squareroot_int
                ).Do(
                    methods.Print_double
                ).Bind(
                    methods.Subtract_2
                );


            result.ShouldBe(Maybe.Just(3.0));

            methods.Parse_string_as_int_was_called.ShouldBe(true);
            methods.Squareroot_int_was_called.ShouldBe(true);
            methods.Print_double_was_called.ShouldBe(true);
            methods.Subtract_2_was_called.ShouldBe(true);
        }

        class Methods
        {
            public Methods() { }

            public bool Parse_string_as_int_was_called = false;
            public bool Squareroot_int_was_called = false;
            public bool Print_double_was_called = false;
            public bool Subtract_2_was_called = false;

            public Maybe.Monad<int> Parse_string_as_int(string s)
            {
                Parse_string_as_int_was_called = true;
                var i = 0;
                return int.TryParse(s, out i) ? Maybe.Just(i) : Maybe.Nothing<int>();
            }

            public Maybe.Monad<double> Squareroot_int(int i)
            {
                Squareroot_int_was_called = true;
                return i < 0 ? Maybe.Nothing<double>() : Maybe.Just(Math.Sqrt((double)i));
            }

            public void Print_double(double d)
            {
                Print_double_was_called = true;
                Console.WriteLine("output: %d", d);
            }

            public Maybe.Monad<double> Subtract_2(double d)
            {
                Subtract_2_was_called = true;
                return Maybe.Just(d - 2.0);
            }


        }




    }
}
