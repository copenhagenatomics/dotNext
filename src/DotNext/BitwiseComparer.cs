using static System.Runtime.CompilerServices.Unsafe;

namespace DotNext;

using static Runtime.Intrinsics;

/// <summary>
/// Represents bitwise comparer for the arbitrary value type.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class BitwiseComparer<T> : IEqualityComparer<T>, IComparer<T>
    where T : struct
{
    private BitwiseComparer()
    {
    }

    /// <summary>
    /// Gets instance of this comparer.
    /// </summary>
    /// <remarks>
    /// Use this property only if you need object implementing <see cref="IEqualityComparer{T}"/>
    /// or <see cref="IComparer{T}"/> interface. Otherwise, use static methods.
    /// </remarks>
    /// <returns>The instance of this comparer.</returns>
    public static BitwiseComparer<T> Instance { get; } = new();

    /// <summary>
    /// Checks bitwise equality between two values of different value types.
    /// </summary>
    /// <remarks>
    /// This method doesn't use <see cref="object.Equals(object)"/>
    /// even if it is overridden by value type.
    /// </remarks>
    /// <typeparam name="TOther">Type of second value.</typeparam>
    /// <param name="first">The first value to check.</param>
    /// <param name="second">The second value to check.</param>
    /// <returns><see langword="true"/>, if both values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals<TOther>(in T first, in TOther second)
        where TOther : struct
        => SizeOf<T>() == SizeOf<TOther>() && SizeOf<T>() switch
        {
            0 => true,
            sizeof(byte) => InToRef<T, byte>(first) == InToRef<TOther, byte>(second),
            sizeof(ushort) => InToRef<T, ushort>(first) == InToRef<TOther, ushort>(second),
            sizeof(uint) => InToRef<T, uint>(first) == InToRef<TOther, uint>(second),
            sizeof(ulong) => InToRef<T, ulong>(first) == InToRef<TOther, ulong>(second),
            _ => EqualsAligned(ref InToRef<T, byte>(first), ref InToRef<TOther, byte>(second), SizeOf<T>()),
        };

    /// <summary>
    /// Compares bits of two values of the different type.
    /// </summary>
    /// <typeparam name="TOther">Type of the second value.</typeparam>
    /// <param name="first">The first value to compare.</param>
    /// <param name="second">The second value to compare.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public static int Compare<TOther>(in T first, in TOther second)
        where TOther : struct
        => SizeOf<T>() != SizeOf<TOther>() ? SizeOf<T>() - SizeOf<TOther>() : SizeOf<T>() switch
        {
            0 => 0,
            sizeof(byte) => InToRef<T, byte>(first).CompareTo(InToRef<TOther, byte>(second)),
            sizeof(ushort) => InToRef<T, ushort>(first).CompareTo(InToRef<TOther, ushort>(second)),
            sizeof(uint) => InToRef<T, uint>(first).CompareTo(InToRef<TOther, uint>(second)),
            sizeof(ulong) => InToRef<T, ulong>(first).CompareTo(InToRef<TOther, ulong>(second)),
            _ => Runtime.Intrinsics.Compare(ref InToRef<T, byte>(first), ref InToRef<TOther, byte>(second), SizeOf<T>()),
        };

    /// <summary>
    /// Computes hash code for the structure content.
    /// </summary>
    /// <param name="value">Value to be hashed.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Content hash code.</returns>
    public static int GetHashCode(in T value, bool salted = true)
    {
        int hash;
        switch (SizeOf<T>())
        {
            default:
                return GetHashCode32(ref InToRef<T, byte>(value), SizeOf<T>(), salted);
            case 0:
                hash = 0;
                break;
            case sizeof(byte):
                hash = InToRef<T, byte>(value);
                break;
            case sizeof(ushort):
                hash = InToRef<T, ushort>(value);
                break;
            case sizeof(int):
                hash = InToRef<T, int>(value);
                break;
            case sizeof(ulong):
                hash = InToRef<T, ulong>(value).GetHashCode();
                break;
        }

        if (salted)
            hash ^= RandomExtensions.BitwiseHashSalt;

        return hash;
    }

    private static void GetHashCode<THashFunction>(in T value, ref THashFunction hashFunction, bool salted)
        where THashFunction : struct, IConsumer<int>
    {
        switch (SizeOf<T>())
        {
            default:
                GetHashCode32(ref hashFunction, ref InToRef<T, byte>(value), SizeOf<T>());
                break;
            case 0:
                break;
            case sizeof(byte):
                hashFunction.Invoke(InToRef<T, byte>(in value));
                break;
            case sizeof(ushort):
                hashFunction.Invoke(InToRef<T, ushort>(in value));
                break;
            case sizeof(int):
                hashFunction.Invoke(InToRef<T, int>(in value));
                break;
        }

        if (salted)
            hashFunction.Invoke(RandomExtensions.BitwiseHashSalt);
    }

    /// <summary>
    /// Computes bitwise hash code for the specified value.
    /// </summary>
    /// <remarks>
    /// This method doesn't use <see cref="object.GetHashCode"/>
    /// even if it is overridden by value type.
    /// </remarks>
    /// <param name="value">A value to be hashed.</param>
    /// <param name="hash">Initial value of the hash.</param>
    /// <param name="hashFunction">Hashing function.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Bitwise hash code.</returns>
    public static int GetHashCode(in T value, int hash, Func<int, int, int> hashFunction, bool salted = true)
    {
        var fn = new Accumulator<int, int>(hashFunction, hash);
        GetHashCode(in value, ref fn, salted);
        return fn.Invoke();
    }

    /// <summary>
    /// Computes bitwise hash code for the specified value.
    /// </summary>
    /// <remarks>
    /// This method doesn't use <see cref="object.GetHashCode"/>
    /// even if it is overridden by value type.
    /// </remarks>
    /// <typeparam name="THashFunction">The type of the hash algorithm.</typeparam>
    /// <param name="value">A value to be hashed.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Bitwise hash code.</returns>
    [CLSCompliant(false)]
    public static int GetHashCode<THashFunction>(in T value, bool salted = true)
        where THashFunction : struct, IConsumer<int>, ISupplier<int>
    {
        var hash = new THashFunction();
        GetHashCode(in value, ref hash, salted);
        return hash.Invoke();
    }

    /// <inheritdoc/>
    bool IEqualityComparer<T>.Equals(T x, T y) => Equals(in x, in y);

    /// <inheritdoc/>
    int IEqualityComparer<T>.GetHashCode(T obj) => GetHashCode(in obj, true);

    /// <inheritdoc/>
    int IComparer<T>.Compare(T x, T y) => Compare(in x, in y);
}