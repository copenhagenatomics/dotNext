using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InlineIL.IL;
using static InlineIL.IL.Emit;
using static InlineIL.MethodRef;
using static InlineIL.TypeRef;

namespace DotNext.Runtime;

/// <summary>
/// Represents highly optimized runtime intrinsic methods.
/// </summary>
public static class Intrinsics
{
    [StructLayout(LayoutKind.Auto)]
    private struct FNV1a32 : IConsumer<int>
    {
        private const int Offset = unchecked((int)2166136261);
        private const int Prime = 16777619;

        private int result = Offset;

        internal readonly int Result => result;

        public void Invoke(int data) => result = (result ^ data) * Prime;
    }

    [StructLayout(LayoutKind.Auto)]
    private struct FNV1a64 : IConsumer<long>
    {
        private const long Offset = unchecked((long)14695981039346656037);
        private const long Prime = 1099511628211;

        private long result = Offset;

        internal readonly long Result => result;

        public void Invoke(long data) => result = (result ^ data) * Prime;
    }

    /// <summary>
    /// Provides the fast way to check whether the specified type accepts  <see langword="null"/> value as valid value.
    /// </summary>
    /// <remarks>
    /// This method always returns <see langword="true"/> for all reference types and <see cref="Nullable{T}"/>.
    /// On mainstream implementations of .NET CLR, this method is replaced by constant value by JIT compiler with zero runtime overhead.
    /// </remarks>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns><see langword="true"/> if <typeparamref name="T"/> is nullable type; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable<T>()
    {
        Unsafe.SkipInit(out T value);
        return value is null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TTo InToRef<TFrom, TTo>(in TFrom source)
    {
        PushInRef(in source);
        return ref ReturnRef<TTo>();
    }

    /// <summary>
    /// Returns default value of the given type.
    /// </summary>
    /// <remarks>
    /// This method helps to avoid generation of temporary variables
    /// necessary for <c>default</c> keyword implementation.
    /// </remarks>
    /// <typeparam name="T">The type for which default value should be obtained.</typeparam>
    /// <returns>The default value of type <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? DefaultOf<T>() => default;

    /// <summary>
    /// Obtain a value of type <typeparamref name="TResult"/> by
    /// reinterpreting the object representation of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Every bit in the value representation of the returned <typeparamref name="TResult"/> object
    /// is equal to the corresponding bit in the object representation of <typeparamref name="T"/>.
    /// The values of padding bits in the returned <typeparamref name="TResult"/> object are unspecified.
    /// The method takes into account size of <typeparamref name="T"/> and <typeparamref name="TResult"/> types
    /// and able to provide conversion between types of different size. However, the result may vary between
    /// CPU architectures if size of types is different.
    /// </remarks>
    /// <param name="input">A value to convert.</param>
    /// <param name="output">Conversion result.</param>
    /// <typeparam name="T">The value type to be converted.</typeparam>
    /// <typeparam name="TResult">The type of output struct.</typeparam>
    public static void Bitcast<T, TResult>(in T input, out TResult output)
        where T : unmanaged
        where TResult : unmanaged
    {
        // ldobj/stobj pair is used instead of cpobj because this instruction
        // has unspecified behavior if src is not assignable to dst, ECMA-335 III.4.4
        const string slowPath = "slow";
        PushOutRef(out output);
        Sizeof<T>();
        Sizeof<TResult>();
        Blt_Un(slowPath);

        // copy from input into output as-is
        PushInRef(in input);
        Ldobj<TResult>();
        Stobj<TResult>();
        Ret();

        MarkLabel(slowPath);
        PushInRef(in input);
        Ldobj<T>();
        Stobj<T>();
        Ret();
    }

    /// <summary>
    /// Indicates that specified value type is the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">Value to check.</param>
    /// <returns><see langword="true"/>, if value is default value; otherwise, <see langword="false"/>.</returns>
    public static bool IsDefault<T>(in T value) => Unsafe.SizeOf<T>() switch
    {
        0 => true,
        sizeof(byte) => InToRef<T, byte>(value) == 0,
        sizeof(ushort) => InToRef<T, ushort>(value) == 0,
        sizeof(uint) => InToRef<T, uint>(value) == 0,
        sizeof(ulong) => InToRef<T, ulong>(value) == 0UL,
        _ => IsZero(ref InToRef<T, byte>(in value), Unsafe.SizeOf<T>()),
    };

    /// <summary>
    /// Returns the runtime handle associated with type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type which runtime handle should be obtained.</typeparam>
    /// <returns>The runtime handle representing type <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RuntimeTypeHandle TypeOf<T>()
    {
        Ldtoken(Type<T>());
        return Return<RuntimeTypeHandle>();
    }

    /// <summary>
    /// Determines whether one or more bit fields are set in the given value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="flag">An enumeration value.</param>
    /// <returns><see langword="true"/> if the bit field or bit fields that are set in <paramref name="flag"/> are also set in <paramref name="value"/>; otherwise, <see langword="false"/>.</returns>
    public static bool HasFlag<T>(T value, T flag)
        where T : struct, Enum => Unsafe.SizeOf<T>() switch
        {
            0 => true,
            sizeof(byte) => (InToRef<T, byte>(value) & InToRef<T, byte>(flag)) != 0,
            sizeof(ushort) => (InToRef<T, ushort>(value) & InToRef<T, ushort>(flag)) != 0,
            sizeof(uint) => (InToRef<T, uint>(value) & InToRef<T, uint>(flag)) != 0,
            sizeof(long) => (InToRef<T, ulong>(value) & InToRef<T, ulong>(flag)) != 0UL,
            _ => value.HasFlag(flag),
        };

    /// <summary>
    /// Provides unified behavior of type cast for reference and value types.
    /// </summary>
    /// <remarks>
    /// This method never returns <see langword="null"/> because it treats <see langword="null"/>
    /// value passed to <paramref name="obj"/> as invalid object of type <typeparamref name="T"/>.
    /// </remarks>
    /// <param name="obj">The object to cast.</param>
    /// <typeparam name="T">Conversion result.</typeparam>
    /// <returns>The result of conversion.</returns>
    /// <exception cref="InvalidCastException"><paramref name="obj"/> is <see langword="null"/> or not of type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cast<T>(object? obj)
        where T : notnull
        => obj is null ? throw new InvalidCastException() : (T)obj;

    internal static T? NullAwareCast<T>(object? obj)
    {
        if (IsNullable<T>())
            goto success;
        if (obj is not T)
            throw new InvalidCastException();

        success:
        return (T?)obj;
    }

    /// <summary>
    /// Computes transient hash code of the specified pointer.
    /// </summary>
    /// <param name="pointer">The pointer value.</param>
    /// <returns>The hash code of the pointer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public static unsafe int PointerHashCode([In] void* pointer)
    {
        Ldarga(nameof(pointer));
        Call(Method(Type<UIntPtr>(), nameof(UIntPtr.GetHashCode)));
        return Return<int>();
    }

    /// <summary>
    /// Returns an address of the given by-ref parameter.
    /// </summary>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <param name="value">The object whose address is obtained.</param>
    /// <returns>An address of the given object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint AddressOf<T>(in T value)
    {
        PushInRef(in value);
        Conv_I();
        return Return<IntPtr>();
    }

    /// <summary>
    /// Converts typed reference into managed pointer.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="reference">The typed reference.</param>
    /// <returns>A managed pointer to the value represented by reference.</returns>
    /// <exception cref="InvalidCastException"><typeparamref name="T"/> is not identical to the type stored in the typed reference.</exception>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(this TypedReference reference)
    {
        Ldarg(nameof(reference));
        Refanyval<T>();
        return ref ReturnRef<T>();
    }

    internal static int Compare(ref byte first, ref byte second, nint length)
    {
        var comparison = 0;
        for (int count; length > 0L && comparison == 0; length -= count, first = ref Unsafe.Add(ref first, count), second = ref Unsafe.Add(ref second, count))
        {
            count = length > int.MaxValue ? int.MaxValue : (int)length;
            comparison = MemoryMarshal.CreateSpan(ref first, count).SequenceCompareTo(MemoryMarshal.CreateSpan(ref second, count));
        }

        return comparison;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Read<T>(this ref byte address)
        where T : unmanaged
        => Unsafe.As<byte, T>(ref address);

    /// <summary>
    /// Bitwise comparison of two memory blocks.
    /// </summary>
    /// <param name="first">The pointer to the first memory block.</param>
    /// <param name="second">The pointer to the second memory block.</param>
    /// <param name="length">The length of the first and second memory blocks.</param>
    /// <returns>Comparison result which has the semantics as return type of <see cref="IComparable.CompareTo(object)"/>.</returns>
    [CLSCompliant(false)]
    public static unsafe int Compare([In] void* first, [In] void* second, nint length)
        => Compare(ref Unsafe.AsRef<byte>(first), ref Unsafe.AsRef<byte>(second), length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static unsafe bool EqualsAligned(ref byte first, ref byte second, nint length)
    {
        var result = false;
        if (Vector.IsHardwareAccelerated)
        {
            for (; length >= sizeof(Vector<byte>); first = ref first.Advance<Vector<byte>>(), second = ref second.Advance<Vector<byte>>())
            {
                if (first.Read<Vector<byte>>() == second.Read<Vector<byte>>())
                    length -= Vector<byte>.Count;
                else
                    goto exit;
            }
        }

        for (; length >= sizeof(nuint); first = ref first.Advance<nuint>(), second = ref second.Advance<nuint>())
        {
            if (first.Read<nuint>() == second.Read<nuint>())
                length -= sizeof(nuint);
            else
                goto exit;
        }

        for (; length > 0; first = ref first.Advance<byte>(), second = ref second.Advance<byte>())
        {
            if (first == second)
                length -= sizeof(byte);
            else
                goto exit;
        }

        // TODO: Workaround for https://github.com/dotnet/coreclr/issues/13549
        result = true;
    exit:
        return result;
    }

    /// <summary>
    /// Computes equality between two blocks of memory.
    /// </summary>
    /// <param name="first">A pointer to the first memory block.</param>
    /// <param name="second">A pointer to the second memory block.</param>
    /// <param name="length">Length of first and second memory blocks, in bytes.</param>
    /// <returns><see langword="true"/>, if both memory blocks have the same data; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static unsafe bool Equals([In] void* first, [In] void* second, nint length)
    {
        var result = true;
        for (int count; length > 0 && result; length -= count, first = Unsafe.Add<byte>(first, count), second = Unsafe.Add<byte>(first, count))
        {
            count = length > int.MaxValue ? int.MaxValue : (int)length;
            result = new ReadOnlySpan<byte>(first, count).SequenceEqual(new ReadOnlySpan<byte>(second, count));
        }

        return result;
    }

    /// <summary>
    /// Allows to reinterpret managed pointer to array element.
    /// </summary>
    /// <typeparam name="T">The type of array elements.</typeparam>
    /// <typeparam name="TBase">The requested type.</typeparam>
    /// <param name="array">The array object.</param>
    /// <param name="index">The index of the array element.</param>
    /// <returns>The reference to the array element with restricted mutability.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TBase GetReadonlyRef<T, TBase>(this T[] array, nint index)
        where T : class, TBase
    {
        Push(array);
        Push(index);
        Readonly();
        Ldelema<TBase>();
        return ref ReturnRef<TBase>();
    }

    /// <summary>
    /// Throws <see cref="NullReferenceException"/> if given managed pointer is <see langword="null"/>.
    /// </summary>
    /// <param name="value">The managed pointer to check.</param>
    /// <typeparam name="T">The type of the managed pointer.</typeparam>
    /// <exception cref="NullReferenceException"><paramref name="value"/> pointer is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull<T>(in T value)
    {
        PushInRef(value);
        Ldobj<T>();
        Pop();
        Ret();
    }

    /// <summary>
    /// Copies one value into another.
    /// </summary>
    /// <typeparam name="T">The value type to copy.</typeparam>
    /// <param name="input">The reference to the source location.</param>
    /// <param name="output">The reference to the destination location.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(in T input, out T output)
        where T : struct
    {
        PushOutRef(out output);
        PushInRef(in input);
        Cpobj<T>();
        Ret();
    }

    /// <summary>
    /// Copies one value into another assuming unaligned memory access.
    /// </summary>
    /// <typeparam name="T">The value type to copy.</typeparam>
    /// <param name="input">The reference to the source location.</param>
    /// <param name="output">The reference to the destination location.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107", Justification = "Unaligned is a prefix instruction")]
    public static unsafe void CopyUnaligned<T>([In] T* input, [Out] T* output)
        where T : unmanaged
    {
        Push(output);
        Push(input);
        Unaligned(1); Ldobj<T>();
        Unaligned(1); Stobj<T>();
    }

    /// <summary>
    /// Copies one value into another.
    /// </summary>
    /// <typeparam name="T">The value type to copy.</typeparam>
    /// <param name="input">The reference to the source location.</param>
    /// <param name="output">The reference to the destination location.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [CLSCompliant(false)]
    public static unsafe void Copy<T>([In] T* input, [Out] T* output)
        where T : unmanaged
        => Copy(in input[0], out output[0]);

    private static void Copy([In] ref byte source, [In] ref byte destination, nint length)
    {
        for (int count; length > 0; length -= count, source = ref Unsafe.Add(ref source, count), destination = ref Unsafe.Add(ref destination, count))
        {
            count = length > int.MaxValue ? int.MaxValue : (int)length;
            Unsafe.CopyBlock(ref destination, ref source, (uint)count);
        }
    }

    /// <summary>
    /// Copies the specified number of elements from source address to the destination address.
    /// </summary>
    /// <param name="source">The address of the bytes to copy.</param>
    /// <param name="destination">The target address.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <typeparam name="T">The type of the element.</typeparam>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Copy<T>(in T source, out T destination, long count)
        where T : unmanaged
    {
        Unsafe.SkipInit(out destination);
        Copy(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)), ref Unsafe.As<T, byte>(ref destination), checked((nint)count * sizeof(T)));
    }

    /// <summary>
    /// Swaps two values.
    /// </summary>
    /// <param name="first">The first value to be replaced with <paramref name="second"/>.</param>
    /// <param name="second">The second value to be replaced with <paramref name="first"/>.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static void Swap<T>(ref T first, ref T second)
    {
        var tmp = first;
        first = second;
        second = tmp;
    }

    /// <summary>
    /// Swaps two values.
    /// </summary>
    /// <param name="first">The first value to be replaced with <paramref name="second"/>.</param>
    /// <param name="second">The second value to be replaced with <paramref name="first"/>.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap<T>(T* first, T* second)
        where T : unmanaged
        => Swap(ref first[0], ref second[0]);

    /// <summary>
    /// Indicates that two managed pointers are equal.
    /// </summary>
    /// <typeparam name="T">Type of managed pointer.</typeparam>
    /// <param name="first">The first managed pointer.</param>
    /// <param name="second">The second managed pointer.</param>
    /// <returns><see langword="true"/>, if both managed pointers are equal; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSame<T>(in T first, in T second)
    {
        PushInRef(in first);
        PushInRef(in second);
        Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ref byte Advance<T>(this ref byte ptr)
        where T : unmanaged
        => ref Unsafe.Add(ref ptr, sizeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ref byte Advance<T>([In] this ref byte address, [In, Out] nint* length)
        where T : unmanaged
    {
        *length -= sizeof(T);
        return ref address.Advance<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static unsafe bool IsZero([In] ref byte address, nint length)
    {
        var result = false;
        if (Vector.IsHardwareAccelerated)
        {
            while (length >= Vector<byte>.Count)
            {
                if (address.Read<Vector<byte>>() == Vector<byte>.Zero)
                    address = ref address.Advance<Vector<byte>>(&length);
                else
                    goto exit;
            }
        }

        while (length >= sizeof(nuint))
        {
            if (address.Read<nuint>() == 0U)
                address = ref address.Advance<nuint>(&length);
            else
                goto exit;
        }

        while (length > 0)
        {
            if (address == 0)
                address = ref address.Advance<byte>(&length);
            else
                goto exit;
        }

        result = true;
    exit:
        return result;
    }

    /// <summary>
    /// Sets all bits of allocated memory to zero.
    /// </summary>
    /// <param name="address">The pointer to the memory to be cleared.</param>
    /// <param name="length">The length of the memory to be cleared, in bytes.</param>
    [CLSCompliant(false)]
    public static unsafe void ClearBits([In, Out] void* address, nint length)
    {
        for (int count; length > 0; length -= count, address = Unsafe.Add<byte>(address, count))
        {
            count = length > int.MaxValue ? int.MaxValue : (int)length;

            Unsafe.InitBlock(address, 0, (uint)count);
        }
    }

    #region Bitwise Hash Code

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static unsafe void GetHashCode64<THashFunction>(ref THashFunction hash, [In] ref byte source, nint length)
        where THashFunction : struct, IConsumer<long>
    {
        switch (length)
        {
            default:
                for (; length >= sizeof(long); source = ref source.Advance<long>(&length))
                    hash.Invoke(Unsafe.ReadUnaligned<long>(ref source));
                for (; length > 0; source = ref source.Advance<byte>(&length))
                    hash.Invoke(source);
                break;
            case 0:
                break;
            case sizeof(byte):
                hash.Invoke(source);
                break;
            case sizeof(ushort):
                hash.Invoke(Unsafe.ReadUnaligned<ushort>(ref source));
                break;
            case sizeof(uint):
                hash.Invoke(Unsafe.ReadUnaligned<uint>(ref source));
                break;
        }
    }

    internal static unsafe long GetHashCode64([In] ref byte source, nint length, bool salted)
    {
        var hash = new FNV1a64();
        GetHashCode64(ref hash, ref source, length);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Result;
    }

    private static THashFunction GetHashCode<T, TInput, THashFunction>(Func<T, int, TInput> getter, int count, T arg)
        where THashFunction : IConsumer<TInput>, new()
    {
        var hash = new THashFunction();

        for (var i = 0; i < count; i++)
            hash.Invoke(getter(arg, i));

        return hash;
    }

    /// <summary>
    /// Computes 64-bit hash code for the vector.
    /// </summary>
    /// <param name="getter">The pointer to the function responsible for providing data from the vector.</param>
    /// <param name="count">The number of elements in the vector.</param>
    /// <param name="arg">The argument to be passed to the data getter.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <typeparam name="T">The type of the argument to be passed to the vector accessor.</typeparam>
    /// <returns>The computed hash.</returns>
    public static long GetHashCode64<T>(Func<T, int, long> getter, int count, T arg, bool salted = true)
    {
        ArgumentNullException.ThrowIfNull(getter);

        var hash = GetHashCode<T, long, FNV1a64>(getter, count, arg);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Result;
    }

    /// <summary>
    /// Computes 32-bit hash code for the vector.
    /// </summary>
    /// <param name="getter">The pointer to the function responsible for providing data from the vector.</param>
    /// <param name="count">The number of elements in the vector.</param>
    /// <param name="arg">The argument to be passed to the data getter.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <typeparam name="T">The type of the argument to be passed to the vector accessor.</typeparam>
    /// <returns>The computed hash.</returns>
    public static int GetHashCode32<T>(Func<T, int, int> getter, int count, T arg, bool salted = true)
    {
        ArgumentNullException.ThrowIfNull(getter);

        var hash = GetHashCode<T, int, FNV1a32>(getter, count, arg);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Result;
    }

    /// <summary>
    /// Computes 64-bit hash code for the block of memory, 64-bit version.
    /// </summary>
    /// <remarks>
    /// This method may give different value each time you run the program for
    /// the same data. To disable this behavior, pass false to <paramref name="salted"/>.
    /// </remarks>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="hash">Initial value of the hash.</param>
    /// <param name="hashFunction">Hashing function.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Hash code of the memory block.</returns>
    [CLSCompliant(false)]
    public static unsafe long GetHashCode64([In] void* source, nint length, long hash, Func<long, long, long> hashFunction, bool salted = true)
    {
        var fn = new Accumulator<long, long>(hashFunction, hash);
        GetHashCode64(ref fn, ref ((byte*)source)[0], length);

        if (salted)
            fn.Invoke(RandomExtensions.BitwiseHashSalt);

        return fn.Invoke();
    }

    /// <summary>
    /// Computes 64-bit hash code for the block of memory, 64-bit version.
    /// </summary>
    /// <remarks>
    /// This method may give different value each time you run the program for
    /// the same data. To disable this behavior, pass false to <paramref name="salted"/>.
    /// </remarks>
    /// <typeparam name="THashFunction">The type providing implementation of the hash function.</typeparam>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Hash code of the memory block.</returns>
    [CLSCompliant(false)]
    public static unsafe long GetHashCode64<THashFunction>([In] void* source, nint length, bool salted = true)
        where THashFunction : struct, IConsumer<long>, ISupplier<long>
    {
        var hash = new THashFunction();
        GetHashCode64(ref hash, ref ((byte*)source)[0], length);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Invoke();
    }

    /// <summary>
    /// Computes 64-bit hash code for the block of memory.
    /// </summary>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <remarks>
    /// This method uses FNV-1a hash algorithm.
    /// </remarks>
    /// <returns>Content hash code.</returns>
    /// <seealso href="http://www.isthe.com/chongo/tech/comp/fnv/#FNV-1a">FNV-1a</seealso>
    [CLSCompliant(false)]
    public static unsafe long GetHashCode64([In] void* source, nint length, bool salted = true)
        => GetHashCode64(ref ((byte*)source)[0], length, salted);

    /// <summary>
    /// Computes 32-bit hash code for the block of memory.
    /// </summary>
    /// <remarks>
    /// This method may give different value each time you run the program for
    /// the same data. To disable this behavior, pass false to <paramref name="salted"/>.
    /// </remarks>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="hash">Initial value of the hash.</param>
    /// <param name="hashFunction">Hashing function.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Hash code of the memory block.</returns>
    [CLSCompliant(false)]
    public static unsafe int GetHashCode32([In] void* source, nint length, int hash, Func<int, int, int> hashFunction, bool salted = true)
    {
        var fn = new Accumulator<int, int>(hashFunction, hash);
        GetHashCode32(ref fn, ref ((byte*)source)[0], length);

        if (salted)
            fn.Invoke(RandomExtensions.BitwiseHashSalt);

        return fn.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static unsafe void GetHashCode32<THashFunction>(ref THashFunction hash, [In] ref byte source, nint length)
        where THashFunction : struct, IConsumer<int>
    {
        switch (length)
        {
            default:
                for (; length >= sizeof(int); source = ref source.Advance<int>(&length))
                    hash.Invoke(Unsafe.ReadUnaligned<int>(ref source));
                for (; length > 0; source = ref source.Advance<byte>(&length))
                    hash.Invoke(source);
                break;
            case 0:
                break;
            case sizeof(byte):
                hash.Invoke(source);
                break;
            case sizeof(ushort):
                hash.Invoke(Unsafe.ReadUnaligned<ushort>(ref source));
                break;
        }
    }

    internal static unsafe int GetHashCode32([In] ref byte source, nint length, bool salted)
    {
        var hash = new FNV1a32();
        GetHashCode32(ref hash, ref source, length);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Result;
    }

    /// <summary>
    /// Computes 32-bit hash code for the block of memory.
    /// </summary>
    /// <remarks>
    /// This method may give different value each time you run the program for
    /// the same data. To disable this behavior, pass false to <paramref name="salted"/>.
    /// </remarks>
    /// <typeparam name="THashFunction">The type providing implementation of the hash function.</typeparam>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <returns>Hash code of the memory block.</returns>
    [CLSCompliant(false)]
    public static unsafe int GetHashCode32<THashFunction>([In] void* source, nint length, bool salted = true)
        where THashFunction : struct, IConsumer<int>, ISupplier<int>
    {
        var hash = new THashFunction();
        GetHashCode32(ref hash, ref ((byte*)source)[0], length);

        if (salted)
            hash.Invoke(RandomExtensions.BitwiseHashSalt);

        return hash.Invoke();
    }

    /// <summary>
    /// Computes 32-bit hash code for the block of memory.
    /// </summary>
    /// <param name="source">A pointer to the block of memory.</param>
    /// <param name="length">Length of memory block to be hashed, in bytes.</param>
    /// <param name="salted"><see langword="true"/> to include randomized salt data into hashing; <see langword="false"/> to use data from memory only.</param>
    /// <remarks>
    /// This method uses FNV-1a hash algorithm.
    /// </remarks>
    /// <returns>Content hash code.</returns>
    /// <seealso href="http://www.isthe.com/chongo/tech/comp/fnv/#FNV-1a">FNV-1a</seealso>
    [CLSCompliant(false)]
    public static unsafe int GetHashCode32([In] void* source, nint length, bool salted = true)
        => GetHashCode32(ref ((byte*)source)[0], length, salted);
    #endregion

    /// <summary>
    /// Reverse bytes in the specified value of blittable type.
    /// </summary>
    /// <typeparam name="T">Blittable type.</typeparam>
    /// <param name="value">The value which bytes should be reversed.</param>
    public static void Reverse<T>(ref T value)
        where T : unmanaged
        => Span.AsBytes(ref value).Reverse();

    /// <summary>
    /// Checks whether the specified object is exactly of the specified type.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <typeparam name="T">The expected type of object.</typeparam>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is not <see langword="null"/> and of type <typeparamref name="T"/>; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsExactTypeOf<T>(object? obj) => obj?.GetType() == typeof(T);

    /// <summary>
    /// Throws arbitrary object as exception.
    /// </summary>
    /// <remarks>
    /// This method never returns successfully.
    /// </remarks>
    /// <param name="obj">The object to be thrown.</param>
    /// <exception cref="RuntimeWrappedException">The exception containing wrapped <paramref name="obj"/>.</exception>
    [DoesNotReturn]
    [StackTraceHidden]
    public static void Throw(object obj)
    {
        Push(obj);
        Emit.Throw();
        throw Unreachable();
    }

    /// <summary>
    /// Throws arbitrary object as exception.
    /// </summary>
    /// <remarks>
    /// This method never returns successfully but returned value helpful for constructing terminated statement
    /// such as <c>throw Error("Error");</c>.
    /// </remarks>
    /// <param name="obj">The object to be thrown.</param>
    /// <returns>The value is never returned from the method.</returns>
    /// <exception cref="RuntimeWrappedException">The exception containing wrapped <paramref name="obj"/>.</exception>
    /// <seealso cref="Throw(object)"/>
    [DoesNotReturn]
    [StackTraceHidden]
    public static Exception Error(object obj)
    {
        Push(obj);
        Emit.Throw();
        throw Unreachable();
    }

    /// <summary>
    /// Creates shallow copy of the given object.
    /// </summary>
    /// <param name="obj">The object to clone.</param>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <returns>The clone of <paramref name="obj"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ShallowCopy<T>(T obj)
        where T : class
    {
        Push(obj);
        Call(Method(Type<object>(), nameof(MemberwiseClone)));
        return Return<T>();
    }

    /// <summary>
    /// Gets length of the array.
    /// </summary>
    /// <remarks>
    /// This method supports one-dimensional as well as multi-dimensional arrays.
    /// </remarks>
    /// <param name="array">The array object.</param>
    /// <returns>The length of the array as native unsigned integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint GetLength(Array array)
    {
        Push(array);
        Ldlen();
        Conv_I();
        return Return<nint>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static TTo ReinterpretCast<TFrom, TTo>(TFrom input)
        => Unsafe.As<TFrom, TTo>(ref input);
}