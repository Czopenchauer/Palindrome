using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using Xunit;

namespace Palindrome;

[MemoryDiagnoser] // we need to enable it in explicit way
public class Benchmarks
{
    private const string Input = "abcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcba";

    [Benchmark]
    public bool IsPalindrome_Normal()
    {
        var span = Input.AsSpan();
        if (span.IsEmpty)
        {
            return false;
        }

        for (var i = 0; i < span.Length / 2; i++)
        {
            if (span[i] != span[span.Length - 1 - i])
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool IsPalindrome_XOR()
    {
        var span = Input.AsSpan();
        if (span.IsEmpty)
        {
            return false;
        }

        for (var i = 0; i < span.Length / 2; i++)
        {
            if ((span[i] ^ span[span.Length - 1 - i]) != 0)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool IsPalindrome_PureSpan()
    {
        var span = Input.AsSpan();
        if (span.IsEmpty)
        {
            return false;
        }

        var halfLength = span.Length / 2;

        unsafe
        {
            fixed (char* ptx = &span[halfLength])
            {
                var pinned = ptx;
                var offset = span.Length % 2 == 0 ? 0 : 1;
                Span<char> secondSpan = new(pinned + offset, halfLength);
                secondSpan.Reverse();
                return secondSpan.SequenceCompareTo(span[.. halfLength]) == 0;
            }
        }
    }

    [Benchmark]
    public bool IsPalindrome_Smth()
    {
        var span = Input.AsSpan();
        if (span.IsEmpty)
        {
            return false;
        }

        var halfLength = span.Length / 2;

        if (halfLength / 2 < 8)
        {
            unsafe
            {
                fixed (char* ptx = &span[halfLength])
                {
                    var pinned = ptx;
                    var offset = span.Length % 2 == 0 ? 0 : 1;
                    Span<char> secondSpan = new(pinned + offset, halfLength);
                    secondSpan.Reverse();
                    return secondSpan.SequenceCompareTo(span[.. halfLength]) == 0;
                }
            }
        }

        unsafe
        {
            fixed (char* ptx = &span[halfLength])
            {
                var pinned = ptx;
                fixed (char* ptxFirst = &span[0])
                {
                    var pinnedFirst = ptxFirst;
                    var numberOfFullIntegers = halfLength / 2;
                    if (halfLength % 2 != 0)
                    {
                        if ((*pinned ^ *pinnedFirst) != 0)
                        {
                            return false;
                        }

                        pinnedFirst += 1;
                        *pinnedFirst = (char)0;
                        halfLength -= 1;
                    }

                    Span<int> firstSpan = new(pinnedFirst, numberOfFullIntegers);
                    Span<char> reversable = new(pinned, halfLength);
                    reversable.Reverse();
                    fixed (char* reversablePtr = &reversable[0])
                    {
                        Span<int> secondSpan = new(reversablePtr, numberOfFullIntegers);
                        for (var i = 0; i < firstSpan.Length; i++)
                        {
                            if (firstSpan[i] != secondSpan[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }
        }
    }

    [Benchmark]
    public bool IsPalindrome_Simd()
    {
        var span = Input.AsSpan();
        if (span.IsEmpty)
        {
            return false;
        }

        var halfLength = span.Length / 2;
        unsafe
        {
            fixed (char* ptx = &span[halfLength])
            {
                var pinned = ptx;

                fixed (char* ptxFirstHalf = &span[0])
                {
                    var pinnedFirst = ptxFirstHalf;
                    var numberOfFullIntegers = halfLength / 2;
                    if (halfLength % 2 != 0)
                    {
                        if ((*pinned ^ *pinnedFirst) != 0)
                        {
                            return false;
                        }

                        pinnedFirst += 1;
                        *pinnedFirst = (char)0;
                        halfLength -= 1;
                    }

                    var off = span.Length % 2 == 0 ? 0 : 1;
                    Span<int> firstSpan = new(pinnedFirst, numberOfFullIntegers);
                    Span<char> reversable = new(pinned + off, halfLength);
                    reversable.Reverse();
                    fixed (char* reversablePtr = &reversable[0])
                    {
                        Span<int> secondSpan = new(reversablePtr, numberOfFullIntegers);
                        int length = secondSpan.Length;
                        int remaining = length % Vector256<int>.Count;
                        if (length - remaining > 0)
                        {
                            for (int i = 0; i < length - remaining; i += Vector256<int>.Count)
                            {
                                var offset = i + Vector<int>.Count;
                                var v1 = Vector256.Create<int>(firstSpan[i.. offset]);
                                var v2 = Vector256.Create<int>(secondSpan[i..offset]);
                                if (!v1.Equals(v2))
                                {
                                    return false;
                                }
                            }
                        }

                        for (var i = 0; i < firstSpan.Length; i++)
                        {
                            if (firstSpan[i] != secondSpan[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }
        }
    }

    public bool IsPalindromeInner(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
        {
            return false;
        }

        var halfLength = span.Length / 2;
        unsafe
        {
            fixed (char* ptx = &span[halfLength])
            {
                var pinned = ptx;

                fixed (char* ptxFirstHalf = &span[0])
                {
                    var pinnedFirst = ptxFirstHalf;
                    var numberOfFullIntegers = halfLength / 2;
                    if (halfLength % 2 != 0)
                    {
                        if ((*pinned ^ *pinnedFirst) != 0)
                        {
                            return false;
                        }

                        pinnedFirst += 1;
                        *pinnedFirst = (char)0;
                        halfLength -= 1;
                    }

                    var off = span.Length % 2 == 0 ? 0 : 1;
                    Span<int> firstSpan = new(pinnedFirst, numberOfFullIntegers);
                    Span<char> reversable = new(pinned + off, halfLength);
                    reversable.Reverse();
                    fixed (char* reversablePtr = &reversable[0])
                    {
                        Span<int> secondSpan = new(reversablePtr, numberOfFullIntegers);
                        int length = secondSpan.Length;
                        int remaining = length % Vector256<int>.Count;
                        if (length - remaining > 0)
                        {
                            for (int i = 0; i < length - remaining; i += Vector256<int>.Count)
                            {
                                var offset = i + Vector<int>.Count;
                                var v1 = Vector256.Create<int>(firstSpan[i.. offset]);
                                var v2 = Vector256.Create<int>(secondSpan[i..offset]);
                                if (!v1.Equals(v2))
                                {
                                    return false;
                                }
                            }
                        }

                        for (var i = 0; i < firstSpan.Length; i++)
                        {
                            if (firstSpan[i] != secondSpan[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }
        }
    }
}

public class Tests
{
    [Theory]
    [InlineData("abba", true)]
    [InlineData("kajak", true)]
    [InlineData("madam", true)]
    [InlineData("mddam", false)]
    [InlineData("", false)]
    [InlineData("fhgfda32465", false)]
    [InlineData("123456", false)]
    [InlineData("abcd", false)]
    [InlineData("abcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcbaabcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcba", true)]
    public void Correctly_detect_palindrome(string input, bool result)
    {
        var sut = new Benchmarks();
        var res = sut.IsPalindromeInner(input);
        Assert.Equal(result, res);
    }
}