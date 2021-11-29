using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Net6Demo
{
    internal class InterpolatedStringHandlers
    {
        public class Person
        {
            public string Name { get; }
            public int Age { get; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Age)}: {Age,4}";

            /*NET5
             public override string ToString()
            {
                return string.Format("{0}: {1}, {2}: {3,4}", new object[]
                {
                    "Name",
                    this.Name,
                    "Age",
                    this.Age
                });
            }*/

            /*NET6:
             public override string ToString()
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 4);
                defaultInterpolatedStringHandler.AppendFormatted("Name");
                defaultInterpolatedStringHandler.AppendLiteral(": ");
                defaultInterpolatedStringHandler.AppendFormatted(this.Name);
                defaultInterpolatedStringHandler.AppendLiteral(", ");
                defaultInterpolatedStringHandler.AppendFormatted("Age");
                defaultInterpolatedStringHandler.AppendLiteral(": ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(this.Age, 4);
                return defaultInterpolatedStringHandler.ToStringAndClear();
            }*/

            //even faster!!!
            public string ToStringFast() =>
                string.Create(CultureInfo.InvariantCulture, stackalloc char[64], $"{nameof(Name)}: {Name}, {nameof(Age)}: {Age}");


            //Interpolate to StringBuilder 
            public void AppendPerson(StringBuilder builder) =>
                builder.Append($"{nameof(Name)}: {Name}, {nameof(Age)}: {Age}");
            /*public void AppendPerson(StringBuilder builder)
            {
                StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(6, 4, builder);
                appendInterpolatedStringHandler.AppendFormatted("Name");
                appendInterpolatedStringHandler.AppendLiteral(": ");
                appendInterpolatedStringHandler.AppendFormatted(this.Name);
                appendInterpolatedStringHandler.AppendLiteral(", ");
                appendInterpolatedStringHandler.AppendFormatted("Age");
                appendInterpolatedStringHandler.AppendLiteral(": ");
                appendInterpolatedStringHandler.AppendFormatted<int>(this.Age);
                builder.Append(ref appendInterpolatedStringHandler);
            }*/
        }

        public record struct Point(int X, int Y) : ISpanFormattable
        {
            /*public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            {
                charsWritten = 0;

                if (!X.TryFormat(destination, out int tmpCharsWritten, format, provider))
                {
                    return false;
                }
                destination = destination.Slice(tmpCharsWritten);

                if (destination.Length < 2)
                {
                    return false;
                }
                ", ".AsSpan().CopyTo(destination);
                tmpCharsWritten += 2;
                destination = destination.Slice(2);

                if (!Y.TryFormat(destination, out int tmp, format, provider))
                {
                    return false;
                }
                charsWritten = tmp + tmpCharsWritten;
                return true;
            }*/

            public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
                destination.TryWrite(provider, $"{X}, {Y}", out charsWritten);

            public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();

        }

        /*public static void Assert(
        [DoesNotReturnIf(false)] bool condition,
        [InterpolatedStringHandlerArgument("condition")] Debug.AssertInterpolatedStringHandler message)*/

        class ValidationResult
        {
            public bool Success => false;

            public string[] FailedMembers() => new[] { "Name", "Id", "Value" };
        }

        public static void NewAssert()
        {
            var result = new ValidationResult();
            Debug.Assert(result.Success, $"Problem during validation of following members: {result.FailedMembers()}");
        }

      /*public static void NewAssert()
        {
            var result = new InterpolatedStringHandlers.ValidationResult();
            bool success = result.Success;
            bool condition = success;
            bool flag;
            var assertInterpolatedStringHandler = new Debug.AssertInterpolatedStringHandler(48, 1, success, ref flag);
            if (flag)
            {
                assertInterpolatedStringHandler.AppendLiteral("Problem during validation of following members: ");
                assertInterpolatedStringHandler.AppendFormatted<string[]>(result.FailedMembers());
            }
            Debug.Assert(condition, ref assertInterpolatedStringHandler);
        }*/
    }
}
