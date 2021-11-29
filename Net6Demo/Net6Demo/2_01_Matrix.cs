using System.Diagnostics.CodeAnalysis;

namespace Net6Demo
{
    /*public interface IParseable<TSelf>
        where TSelf : IParseable<TSelf>
    {
        static abstract TSelf Parse(string s, IFormatProvider? provider);

        static abstract bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TSelf result);
    }*/

    internal class Matrix<TNumber> : IParseable<Matrix<TNumber>>
        where TNumber : struct,
                        IFloatingPoint<TNumber>,
                        IAdditionOperators<TNumber, TNumber, TNumber>,
                        IMultiplyOperators<TNumber, TNumber, TNumber>,
                        IUnaryNegationOperators<TNumber, TNumber>,
                        ISpanParseable<TNumber>
    {
        private readonly TNumber[,] _data;
        public int Rows { get; }
        public int Columns { get; }

        public TNumber this[int iRow, int iCol] => _data[iRow, iCol];

        public Matrix(TNumber[,] data)
        {
            _data = data;
            Rows = data.GetLength(0);
            Columns = data.GetLength(1);
        }

        private static readonly char[] _rowSeparators = new[] { '\n', '\r', ';' };
        private static readonly char[] _colSeparators = new[] { ' ', '\t', ',' };
        public static Matrix<TNumber> Parse(string s, IFormatProvider? provider = null)
        {
            provider ??= CultureInfo.InvariantCulture;

            var rows = s.Split(_rowSeparators, StringSplitOptions.RemoveEmptyEntries);
            var numCols = rows[0].Split(_colSeparators, StringSplitOptions.RemoveEmptyEntries).Length;
            var matrix = new TNumber[rows.Length, numCols];
            for (int i = 0; i < rows.Length; i++)
            {
                var col = rows[i].Split(_colSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < col.Length; j++)
                    matrix[i, j] = TNumber.Parse(col[j], provider);
            }
            return new Matrix<TNumber>(matrix);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Matrix<TNumber>? result)
        {
            provider ??= CultureInfo.InvariantCulture;

            try
            {
                result = s switch
                {
                    null => default,
                    "" => new Matrix<TNumber>(new TNumber[0, 0]),
                    _ => Parse(s, provider)
                };

                return result is not null;
            }
            catch
            {
                result = default;
                return false;
            }
        }


        public Matrix<TNumber> Minor(int iRow, int iCol)
        {
            var minor = new TNumber[Rows - 1, Columns - 1];
            int m = 0;
            for (int i = 0; i < Rows; i++)
            {
                if (i == iRow)
                    continue;
                int n = 0;
                for (int j = 0; j < Columns; j++)
                {
                    if (j == iCol)
                        continue;
                    minor[m, n] = this[i, j];
                    n++;
                }
                m++;
            }
            return new(minor);
        }

        public TNumber Determinant()
        {
            var det = new TNumber();
            if (Rows != Columns) throw new("Determinant of a non-square matrix doesn't exist");
            if (Rows == 1) return this[0, 0];
            for (int j = 0; j < Columns; j++)
            {
                TNumber reduced = this[0, j] * Minor(0, j).Determinant();
                if (j % 2 == 1)
                    reduced = -reduced;
                det += reduced;
            }
            return det;
        }
    }
}
