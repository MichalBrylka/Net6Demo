using System.Text;

namespace Net6Demo
{
    record class Rectangle(int Width, int Height)
    {
        //01. sealed 
        public sealed override string ToString() => $"{GetType().Name} {Width}x{Height}";
    }

    record class Square(int side) : Rectangle(side, side)
    {
        //public override string ToString() => $"{GetType().Name} {Width}x{Height}";
    }

    //02. (readonly) record struct 
    readonly record struct Rect(int X, int Y, int Width, int Height);

    readonly struct RectangleStruct
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }

        //03. ctor
        public RectangleStruct()
        {
            X = Y = 0;
            Width = Height = 1;
        }

        public RectangleStruct(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    class RectDemo
    {
        public static void Test()
        {
            var r1 = new Rect(1, 2, 3, 4);
            var r2 = r1 with { X = 15 };

            //04. with
            var r3 = new RectangleStruct(1, 2, 3, 4);
            var r4 = r3 with { X = 15 };
        }
    }

    //free stuff: structural equality+IEquatable+operators, positional deconstruction, printing/formatting
    internal readonly struct RectDecompiled : IEquatable<RectDecompiled>
    {
        public RectDecompiled(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Rect");
            stringBuilder.Append(" { ");
            if (PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        private bool PrintMembers(StringBuilder builder)
        {
            builder.Append("X = ");
            builder.Append(X.ToString());
            builder.Append(", Y = ");
            builder.Append(Y.ToString());
            builder.Append(", Width = ");
            builder.Append(Width.ToString());
            builder.Append(", Height = ");
            builder.Append(Height.ToString());
            return true;
        }

        public static bool operator !=(RectDecompiled left, RectDecompiled right) => !(left == right);

        public static bool operator ==(RectDecompiled left, RectDecompiled right) => left.Equals(right);

        public override int GetHashCode() =>
            (EqualityComparer<int>.Default.GetHashCode(X) * -1521134295 +
             EqualityComparer<int>.Default.GetHashCode(Y)) * -1521134295 +
            EqualityComparer<int>.Default.GetHashCode(Width) * -1521134295 +
            EqualityComparer<int>.Default.GetHashCode(Height);

        public override bool Equals(object? obj) => obj is RectDecompiled rect && Equals(rect);

        public bool Equals(RectDecompiled other) =>
            EqualityComparer<int>.Default.Equals(X, other.X) &&
            EqualityComparer<int>.Default.Equals(Y, other.Y) &&
            EqualityComparer<int>.Default.Equals(Width, other.Width) &&
            EqualityComparer<int>.Default.Equals(Height, other.Height);

        public void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }
    }
}

