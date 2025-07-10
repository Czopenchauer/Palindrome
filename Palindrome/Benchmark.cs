using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using Xunit;

namespace Palindrome;

[MemoryDiagnoser] // we need to enable it in explicit way
public class Benchmarks
{
    private const string Input = "abcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcba";

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
                        var firsHalf = Vector128.Create<int>(firstSpan);
                        var secondHalf = Vector128.Create<int>(secondSpan);
                        int length = firstSpan.Length;
                        int remaining = firstSpan.Length % Vector<int>.Count;
                        for (int i = 0; i < length - remaining; i += Vector<int>.Count)
                        {
                            if (firsHalf.Equals(secondHalf))
                            {
                                return true;
                            }
                        }

                        for (int i = length - remaining; i < length; i++)
                        {
                            if (firsHalf[i] != secondHalf[i])
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
    [InlineData("abcdefghijklmnopqrstuvwxyzzyxwvutsrqponmlkjihgfedcba", true)]
    public void Correctly_detect_palindrome(string input, bool result)
    {
        var sut = new Benchmarks();
        var res = sut.IsPalindromeInner(input);
        Assert.Equal(result, res);
    }
}