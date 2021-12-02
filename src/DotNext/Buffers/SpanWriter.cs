using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNext.Buffers;

/// <summary>
/// Represents simple memory writer backed by <see cref="Span{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
[StructLayout(LayoutKind.Auto)]
public ref struct SpanWriter<T>
{
    private readonly Span<T> span;
    private int position;

    /// <summary>
    /// Initializes a new memory writer.
    /// </summary>
    /// <param name="span">The span used to write elements.</param>
    public SpanWriter(Span<T> span)
    {
        this.span = span;
        position = 0;
    }

    /// <summary>
    /// Initializes a new memory writer.
    /// </summary>
    /// <param name="reference">Managed pointer to the memory block.</param>
    /// <param name="length">The length of the elements referenced by the pointer.</param>
    public SpanWriter(ref T reference, int length)
    {
        if (Unsafe.IsNullRef(ref reference))
            throw new ArgumentNullException(nameof(reference));

        span = MemoryMarshal.CreateSpan(ref reference, length);
        position = 0;
    }

    /// <summary>
    /// Gets the element at the current position in the
    /// underlying memory block.
    /// </summary>
    /// <exception cref="InvalidOperationException">The position of this writer is out of range.</exception>
    public readonly ref T Current
    {
        get
        {
            if (position >= span.Length)
                throw new InvalidOperationException();

            return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), position);
        }
    }

    /// <summary>
    /// Gets the available space in the underlying span.
    /// </summary>
    public readonly int FreeCapacity => span.Length - position;

    /// <summary>
    /// Gets the number of occupied elements in the underlying span.
    /// </summary>
    public readonly int WrittenCount => position;

    /// <summary>
    /// Gets the remaining part of the span.
    /// </summary>
    public readonly Span<T> RemainingSpan => span.Slice(position);

    /// <summary>
    /// Advances the position of this writer.
    /// </summary>
    /// <param name="count">The number of written elements.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is greater than the available space in the rest of the memory block.</exception>
    public void Advance(int count)
    {
        var newPosition = position + count;
        if ((uint)newPosition > (uint)span.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        position = newPosition;
    }

    /// <summary>
    /// Moves the writer back the specified number of items.
    /// </summary>
    /// <param name="count">The number of items.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero or greater than <see cref="WrittenCount"/>.</exception>
    public void Rewind(int count)
    {
        if ((uint)count > (uint)position)
            throw new ArgumentOutOfRangeException(nameof(count));

        position -= count;
    }

    /// <summary>
    /// Sets writer position to the first element.
    /// </summary>
    public void Reset() => position = 0;

    /// <summary>
    /// Gets the span over written elements.
    /// </summary>
    /// <value>The segment of underlying span containing written elements.</value>
    public readonly Span<T> WrittenSpan => span.Slice(0, position);

    /// <summary>
    /// Gets underlying span.
    /// </summary>
    public readonly Span<T> Span => span;

    /// <summary>
    /// Copies the elements to the underlying span.
    /// </summary>
    /// <param name="input">The span to copy from.</param>
    /// <returns>
    /// <see langword="true"/> if all elements are copied successfully;
    /// <see langword="false"/> if remaining space in the underlying span is not enough to place all elements from <paramref name="input"/>.
    /// </returns>
    public bool TryWrite(ReadOnlySpan<T> input)
    {
        if (!input.TryCopyTo(span.Slice(position)))
            return false;

        position += input.Length;
        return true;
    }

    /// <summary>
    /// Copies the elements to the underlying span.
    /// </summary>
    /// <param name="input">The span of elements to copy from.</param>
    /// <returns>The number of written elements.</returns>
    public int Write(ReadOnlySpan<T> input)
    {
        input.CopyTo(RemainingSpan, out var writtenCount);
        position += writtenCount;
        return writtenCount;
    }

    /// <summary>
    /// Puts single element into the underlying span.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <returns>
    /// <see langword="true"/> if item has beem placed successfully;
    /// <see langword="false"/> if remaining space in the underlying span is not enough to place the item.
    /// </returns>
    public bool TryAdd(T item)
    {
        var newLength = position + 1;
        if ((uint)newLength > (uint)span.Length)
            return false;

        Unsafe.Add(ref MemoryMarshal.GetReference(span), position) = item;
        position = newLength;
        return true;
    }

    /// <summary>
    /// Puts single element into the underlying span.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <exception cref="InternalBufferOverflowException">Remaining space in the underlying span is not enough to place the item.</exception>
    public void Add(T item)
    {
        if (!TryAdd(item))
            throw new InternalBufferOverflowException(ExceptionMessages.NotEnoughMemory);
    }

    /// <summary>
    /// Obtains the portion of underlying span and marks it as written.
    /// </summary>
    /// <param name="count">The size of the segment.</param>
    /// <param name="segment">The portion of the underlying span.</param>
    /// <returns>
    /// <see langword="true"/> if segment is obtained successfully;
    /// <see langword="false"/> if remaining space in the underlying span is not enough to place <paramref name="count"/> elements.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public bool TrySlide(int count, out Span<T> segment)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var newLength = position + count;
        if ((uint)newLength <= (uint)span.Length)
        {
            segment = span.Slice(position, count);
            position = newLength;
            return true;
        }

        segment = default;
        return false;
    }

    /// <summary>
    /// Obtains the portion of underlying span and marks it as written.
    /// </summary>
    /// <param name="count">The size of the segment.</param>
    /// <returns>The portion of the underlying span.</returns>
    /// <exception cref="InternalBufferOverflowException">Remaining space in the underlying span is not enough to place <paramref name="count"/> elements.</exception>
    public Span<T> Slide(int count)
        => TrySlide(count, out var result) ? result : throw new InternalBufferOverflowException(ExceptionMessages.NotEnoughMemory);

    /// <summary>
    /// Writes a portion of data.
    /// </summary>
    /// <param name="action">The action responsible for writing elements.</param>
    /// <param name="arg">The state to be passed to the action.</param>
    /// <param name="count">The number of the elements to be written.</param>
    /// <typeparam name="TArg">The type of the argument to be passed to the callback.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is zero.</exception>
    /// <exception cref="InternalBufferOverflowException">Remaining space in the underlying span is not enough to place <paramref name="count"/> elements.</exception>
    [CLSCompliant(false)]
    public unsafe void Write<TArg>(delegate*<Span<T>, TArg, void> action, TArg arg, int count)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (!TrySlide(count, out var buffer))
            throw new InternalBufferOverflowException(ExceptionMessages.NotEnoughMemory);
        action(buffer, arg);
    }

    /// <summary>
    /// Attempts to write a portion of data.
    /// </summary>
    /// <param name="action">The action responsible for writing elements.</param>
    /// <param name="arg">The state to be passed to the action.</param>
    /// <typeparam name="TArg">The type of the argument to be passed to the callback.</typeparam>
    /// <returns><see langword="true"/> if all elements are written successfully; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is zero.</exception>
    [CLSCompliant(false)]
    public unsafe bool TryWrite<TArg>(delegate*<TArg, Span<T>, out int, bool> action, TArg arg)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        bool result;

        if (result = action(arg, RemainingSpan, out var writtenCount))
            position += writtenCount;

        return result;
    }

    /// <summary>
    /// Gets the textual representation of the written content.
    /// </summary>
    /// <returns>The textual representation of the written content.</returns>
    public readonly override string ToString() => WrittenSpan.ToString();
}