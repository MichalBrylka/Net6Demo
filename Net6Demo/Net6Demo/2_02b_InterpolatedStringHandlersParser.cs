using System.Runtime.CompilerServices;

using static Net6Demo.ParsingExtensions;


namespace Net6Demo;

internal static class InterpolatedStringHandlersParser
{
    public static void Test()
    {
        string input = "Name: Mike; Age: 37; Children: 1; Favorite Number: 3.14;";

        string? name = null;
        int age = 0;
        byte children = 0;
        float number = 0;

        Console.WriteLine(
            input.TryParse(
                $"Name: {Placeholder(ref name)}; Age: {Placeholder(ref age)}; Children: {Placeholder(ref children)}; Favorite Number: {Placeholder(ref number)};")
                ? $"{name} {age} {children} {number}"
                : "Does not match :(");
    }
}


public static class ParsingExtensions
{
    public static PlaceholderCell<T?> Placeholder<T>(ref T? arg) => new(ref arg);

    public static bool TryParse(this string input, [InterpolatedStringHandlerArgument("input")] ref TryParseHandler handler)
        => handler.IsSuccessful;
}

public readonly unsafe ref struct PlaceholderCell<T>
{
    private readonly void* _ptr;
    public PlaceholderCell(ref T? arg) => _ptr = Unsafe.AsPointer(ref arg);

    public bool IsNull => _ptr is null;

    public ref T Get() => ref Unsafe.AsRef<T>(_ptr);
    public void Set(T arg) => Unsafe.Write(_ptr, arg);
}

[InterpolatedStringHandler]
public ref struct TryParseHandler
{
    private ReadOnlySpan<char> _input;
    private PlaceholderCell<string?> _substringPlaceholder;

    public TryParseHandler(int literalLength, int formattedCount, ReadOnlySpan<char> input)
    {
        _input = input;
        _substringPlaceholder = default;
        IsSuccessful = true;
    }

    public bool IsSuccessful { get; private set; }

    private bool Failed()
    {
        IsSuccessful = false;
        return false;
    }

    public bool AppendLiteral(string literal)
    {
        if (!_substringPlaceholder.IsNull)
        {
            var index = _input.IndexOf(literal, StringComparison.Ordinal);
            if (index < 1)
                return Failed();

            _substringPlaceholder.Set(_input[..index].ToString());
            _substringPlaceholder = default;

            _input = _input[(index + literal.Length)..];

            return true;
        }

        if (_input.StartsWith(literal, StringComparison.Ordinal))
        {
            _input = _input[literal.Length..];
            return true;
        }

        return Failed();
    }

    public bool AppendFormatted(PlaceholderCell<string?> placeholder)
    {
        if (_input.Length == 0)
            return Failed();

        if (!_substringPlaceholder.IsNull)
            return Failed();

        _substringPlaceholder = placeholder;

        return true;
    }

    public bool AppendFormatted(PlaceholderCell<int> placeholder)
    {
        if (_input.Length == 0)
            return Failed();

        var startPos = 0;
        while (startPos < _input.Length && !char.IsDigit(_input[startPos]))
            startPos++;

        if (startPos >= _input.Length)
            return Failed();

        var endPos = startPos;
        while (endPos < _input.Length - 1 && char.IsDigit(_input[endPos + 1]))
            endPos++;

        var numberSlice = _input.Slice(startPos, endPos - startPos + 1);
        if (!int.TryParse(numberSlice, out var value))
            return Failed();

        placeholder.Set(value);

        if (!_substringPlaceholder.IsNull)
        {
            _substringPlaceholder.Set(_input[..startPos].ToString());
            _substringPlaceholder = default;
        }

        _input = _input[(endPos + 1)..];

        return true;
    }

    public bool AppendFormatted<T>(PlaceholderCell<T> placeholder)
        where T : ISpanParseable<T>
    {
        if (_input.Length == 0)
            return Failed();

        var startPos = 0;
        var endPos = startPos;

        while (endPos < _input.Length - 1 && T.TryParse(_input[startPos..(endPos + 1)], CultureInfo.InvariantCulture, out _))
            endPos++;

        var numberSlice = _input[startPos..endPos];
        if (!T.TryParse(numberSlice, CultureInfo.InvariantCulture, out var value))
            return Failed();

        placeholder.Set(value);

        if (!_substringPlaceholder.IsNull)
        {
            _substringPlaceholder.Set(_input[..startPos].ToString());
            _substringPlaceholder = default;
        }

        _input = _input[endPos..];

        return true;
    }
}

