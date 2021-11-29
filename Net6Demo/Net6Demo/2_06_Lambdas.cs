using System.Globalization;
using System.Linq.Expressions;

namespace Net6Demo
{
    internal class Lambdas
    {
        public static void TestLambdas()
        {
            Func<string, int> parseOld = (string s) => int.Parse(s);
            var parseNew1 = (string s) => int.Parse(s);
            //var parseNew2 = (ReadOnlySpan<char> s, NumberStyles style, IFormatProvider formatter) => int.Parse(s, style, formatter);
            #region Hidden side
            //Func<ReadOnlySpan<char>, NumberStyles, IFormatProvider, int> parseNew3 = (ReadOnlySpan<char> s, NumberStyles style, IFormatProvider formatter) => int.Parse(s, style, formatter);  
            FuncWithRos parseNew4 = (ReadOnlySpan<char> s, NumberStyles style, IFormatProvider formatter) => int.Parse(s, style, formatter);

            #endregion
        }

        public static void TestMulti()
        {
            var multi16 = (int i01, int i02, int i03, int i04, int i05, int i06, int i07, int i08, int i09, int i10,
                           int i11, int i12, int i13, int i14, int i15, int i16)
                         => i01 + i02 + i03 + i04 + i05 + i06 + i07 + i08 + i09 + i10
                          + i11 + i12 + i13 + i14 + i15 + i16;


            var multi20 = (int i01, int i02, int i03, int i04, int i05, int i06, int i07, int i08, int i09, int i10,
                           int i11, int i12, int i13, int i14, int i15, int i16, int i17, int i18, int i19, int i20)
                         => i01 + i02 + i03 + i04 + i05 + i06 + i07 + i08 + i09 + i10
                          + i11 + i12 + i13 + i14 + i15 + i16 + i17 + i18 + i19 + i20;
        }

        public static void TestOutRef()
        {
            Func<string, int, bool>? tryParseNewFunc = (string s, int i) => int.TryParse(s, out i);

            #region Hidden side
            Func2Out<string, int, bool> tryParseOld = (string s, out int i) => int.TryParse(s, out i);



            var tryParseNew1 = (string s, out int i) => int.TryParse(s, out i);
            /*
[CompilerGenerated]
internal delegate TResult <>F{00000008}<T1, T2, TResult>(T1, out T2);*/
            #endregion
        }

        public static void TestTrees()
        {
            // Expression<Func<string, int>>
            LambdaExpression parseExpr1 = (string s) => int.Parse(s);
            Expression parseExpr2 = (string s) => int.Parse(s);
            //MethodCallExpression parseExpr3 = (string s) => int.Parse(s);
            //InvocationExpression parseExpr3 = (string s) => int.Parse(s);
        }

        public static void TestMethodGroup()
        {
            Func<int> read = Console.Read;
            Action<string> write = Console.Write;

            var read2 = Console.Read;
            //var write2 = Console.Write;

            var write3 = (string s) => Console.Write(s);
        }

        public static void TestReturnType()
        {
            //var choose1 = (bool b) => b ? 1 : "two";
            var choose2 = object (bool b) => b ? 1 : "two";

            var choose3 = (bool b) => b ? (int?)1 : null;
        }

        public static void TestAttributes()
        {
            Func<string, int> parse =[return: My(1)] (s) => int.Parse(s);
            var choose =[My(2)][My(3)] object ([My(4)] bool b) => b ? 1 : "two";


        }

        public static void TestConvert()
        {
            //Func<string, int> func = int.Parse;
            //MyParser mp = func;
        }

        delegate int MyParser(string arg);


        [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
        public class MyAttribute : Attribute
        {
            readonly int _id;
            public MyAttribute(int id) => _id = id;
        }

        delegate TResult Func2Out<T1, T2, out TResult>(T1 arg1, out T2 arg2);

        delegate int FuncWithRos(ReadOnlySpan<char> source, NumberStyles ns, IFormatProvider formatProvider);
    }
}
