using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Net6Demo
{
    internal static class CallerArgumentExpression
    {
        /*T Single<T>(T[] array)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Length == 1);

            return array[0];
        }*/
        private static void Assert(bool condition, [CallerArgumentExpression("condition")] string message = null) =>
            Debug.Assert(condition, message);

        public static void Test1()
        {
            var array = new[] { 15, 16 };
            Assert(array is { Length: 1 });
        }

        public static void Test2()
        {
            var array = new[] { 15, 16 };
            ElementAt(array, 5);
        }
        
        public static void Test3()
        {
            var person = new Person("Mike");
            person.Name.ShouldBe("Peter");
        }

        public static void ShouldBe<T>(this T @this, T expected, [CallerArgumentExpression("this")] string thisExpression = null)
        {
            if (!Equals(@this, expected))
                throw new ArgumentException(
                    $"{thisExpression} equal to '{@this}' is expected to be equal to '{expected}'");
        }

        static T Single<T>(T[] array)
        {
            Verify.NotNull(array);
            Verify.Argument(array.Length == 1, "Array must contain a single element.");

            return array[0];
        }

        static T ElementAt<T>(T[] array, int index)
        {
            Verify.NotNull(array);
            Verify.InRange(index, 0, array.Length - 1);

            return array[index];
        }

        public static class Verify
        {
            public static void Argument(bool condition, string message, [CallerArgumentExpression("condition")] string conditionExpression = null)
            {
                if (!condition) throw new ArgumentException(message: message, paramName: conditionExpression);
            }

            public static void InRange(int argument, int low, int high,
                [CallerArgumentExpression("argument")] string argumentExpression = null,
                [CallerArgumentExpression("low")] string lowExpression = null,
                [CallerArgumentExpression("high")] string highExpression = null)
            {
                if (argument < low)
                    throw new ArgumentOutOfRangeException(paramName: argumentExpression,
                        message: $"{argumentExpression} ({argument}) cannot be less than {lowExpression} ({low}).");

                if (argument > high)
                    throw new ArgumentOutOfRangeException(paramName: argumentExpression,
                        message: $"{argumentExpression} ({argument}) cannot be greater than {highExpression} ({high}).");
            }

            public static void NotNull<T>(T argument, [CallerArgumentExpression("argument")] string argumentExpression = null)
                where T : class
            {
                if (argument == null) throw new ArgumentNullException(paramName: argumentExpression);
            }
        }

        record Person(string Name);
    }
}
