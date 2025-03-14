Release Notes
====

# 02-01-2022
<a href="https://www.nuget.org/packages/dotnext/4.2.0">DotNext 4.2.0</a>
* Improved scalability of mechanism that allows to attach custom data to arbitrary objects using `UserDataStorage` and `UserDataSlot<T>` types. The improvement works better in high workloads without the risk of lock contention but requires a bit more CPU cycles to obtain the data attached to the object
* Added ability to enumerate values stored in `TypeMap<T>` or `ConcurrentTypeMap<T>`
* Improved debugging experience of `UserDataStorage` type
* Added `Dictionary.Empty` static method that allows to obtain a singleton of empty [IReadOnlyDictionary&lt;TKey, TValue&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)
* Fixed decoding buffer oveflow in `Base64Decoder` type
* Added `Base64Encoder` type for fast encoding of large binary data
* Deprecation of `Sequence.FirstOrEmpty` extension methods in favor of `Sequence.FirstOrNone`
* Fixed [#91](https://github.com/dotnet/dotNext/pull/91)
* Public constructors of `PooledBufferWriter` and `PooledArrayBufferWriter` with parameters are obsolete in favor of init-only properties
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members
* Optimized performance of `Timeout`, `Optional<T>`, `Result<T>` and `Result<T, TError>` types
* Introduced `DotNext.Runtime.SoftReference` data type in addition to [WeakReference](https://docs.microsoft.com/en-us/dotnet/api/system.weakreference) from .NET

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.2.0">DotNext.Metaprogramming 4.2.0</a>
* Improved overall performance of some scenarios where `UserDataStorage` is used
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members

<a href="https://www.nuget.org/packages/dotnext.reflection/4.2.0">DotNext.Reflection 4.2.0</a>
* Improved overall performance of some scenarios where `UserDataStorage` is used
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.2.0">DotNext.Unsafe 4.2.0</a>
* Updated dependencies
* Reduced size of the compiled assembly: omit private and internal member's nullability attributes

<a href="https://www.nuget.org/packages/dotnext.threading/4.2.0">DotNext.Threading 4.2.0</a>
* Reduced execution time of `CreateTask` overloads declared in `ValueTaskCompletionSource` and `ValueTaskCompletionSource<T>` classes
* Added overflow check to `AsyncCounter` class
* Improved debugging experience of all asynchronous locks
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members
* Reduced lock contention that can be caused by asynchronous locks in concurrent scenarios
* Added `Reset()` method to `TaskCompletionPipe<T>` that allows to reuse the pipe

<a href="https://www.nuget.org/packages/dotnext.io/4.2.0">DotNext.IO 4.2.0</a>
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members
* `FileWriter` now implements [IBufferWriter&lt;byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1)

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.2.0">DotNext.Net.Cluster 4.2.0</a>
* Improved compatibility with IL trimming
* Reduced size of the compiled assembly: omit private and internal member's nullability attributes
* Completely rewritten implementation of TCP transport: better buffering and less network overhead. This version of protocol is not binary compatible with any version prior to 4.2.0
* Increased overall stability of the cluster
* Fixed bug with incorrect calculation of the offset within partition file when using persistent WAL. The bug could prevent the node to start correctly with non-empty WAL
* Added Reflection-free support of JSON log entries powered by JSON Source Generator from .NET

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.2.0">DotNext.AspNetCore.Cluster 4.2.0</a>
* Improved compatibility with IL trimming
* Reduced size of the compiled assembly: omit nullability attributes for private and internal members

# 12-20-2021
<a href="https://www.nuget.org/packages/dotnext/4.1.3">DotNext 4.1.3</a>
* Smallish performance improvements

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.1.3">DotNext.Metaprogramming 4.1.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/4.1.3">DotNext.Reflection 4.1.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.1.3">DotNext.Unsafe 4.1.3</a>
* Performance improvements of `Pointer<T>` public methods

<a href="https://www.nuget.org/packages/dotnext.threading/4.1.3">DotNext.Threading 4.1.3</a>
* Fixed potential concurrency issue than can be caused by `AsyncBridge` public methods when cancellation token or wait handle is about to be canceled or signaled

<a href="https://www.nuget.org/packages/dotnext.io/4.1.3">DotNext.IO 4.1.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.1.3">DotNext.Net.Cluster 4.1.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.1.3">DotNext.AspNetCore.Cluster 4.1.3</a>
* Updated dependencies

# 12-12-2021
<a href="https://www.nuget.org/packages/dotnext/4.1.2">DotNext 4.1.2</a>
* Minor performance improvements

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.1.2">DotNext.Metaprogramming 4.1.2</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/4.1.2">DotNext.Reflection 4.1.2</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.1.2">DotNext.Unsafe 4.1.2</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/4.1.2">DotNext.Threading 4.1.2</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/4.1.2">DotNext.IO 4.1.2</a>
* Minor performance improvements of `FileBufferingWriter` class

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.1.2">DotNext.Net.Cluster 4.1.2</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.1.2">DotNext.AspNetCore.Cluster 4.1.2</a>
* Updated dependencies

# 12-09-2021
<a href="https://www.nuget.org/packages/dotnext/4.1.1">DotNext 4.1.1</a>
* Minor performance improvements

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.1.1">DotNext.Metaprogramming 4.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/4.1.1">DotNext.Reflection 4.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.1.1">DotNext.Unsafe 4.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/4.1.1">DotNext.Threading 4.1.1</a>
* Minor performance improvements

<a href="https://www.nuget.org/packages/dotnext.io/4.1.1">DotNext.IO 4.1.1</a>
* Minor performance improvements

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.1.1">DotNext.Net.Cluster 4.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.1.1">DotNext.AspNetCore.Cluster 4.1.1</a>
* Updated dependencies

# 12-05-2021
<a href="https://www.nuget.org/packages/dotnext/4.1.0">DotNext 4.1.0</a>
* Optimized bounds check in growable buffers
* Changed behavior of exceptions capturing by `DotNext.Threading.Tasks.Synchronization.GetResult` overloaded methods
* Added `DotNext.Threading.Tasks.Synchronization.TryGetResult` method
* Added `DotNext.Buffers.ReadOnlySequencePartitioner` static class with methods for [ReadOnlySequence&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) partitioning in [parallel](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel) processing scenarios
* Enabled support of IL trimming

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.1.0">DotNext.Metaprogramming 4.1.0</a>
* IL trimming is explicitly disabled because the library highly relied on Reflection API

<a href="https://www.nuget.org/packages/dotnext.reflection/4.1.0">DotNext.Reflection 4.1.0</a>
 IL trimming is explicitly disabled because the library highly relied on Reflection API

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.1.0">DotNext.Unsafe 4.1.0</a>
* Enabled support of IL trimming

<a href="https://www.nuget.org/packages/dotnext.threading/4.1.0">DotNext.Threading 4.1.0</a>
* Reduced memory allocation by async locks
* Added cancellation support to `AsyncLazy<T>` class
* Introduced `TaskCompletionPipe<T>` class that allows to consume tasks as they complete
* Removed _Microsoft.Extensions.ObjectPool_ dependency
* Enabled support of IL trimming

<a href="https://www.nuget.org/packages/dotnext.io/4.1.0">DotNext.IO 4.1.0</a>
* Enabled support of IL trimming

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.1.0">DotNext.Net.Cluster 4.1.0</a>
* Enabled support of IL trimming

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.1.0">DotNext.AspNetCore.Cluster 4.1.0</a>
* Enabled support of IL trimming

# 11-25-2021
.NEXT 4.0.0 major release is out! Its primary focus is .NET 6 support as well as some other key features:
* Native support of [C# 10 Interpolated Strings](https://devblogs.microsoft.com/dotnet/string-interpolation-in-c-10-and-net-6/) across various buffer types, streams and other I/O enhancements. String building and string encoding/decoding with zero allocation overhead is now a reality
* All asynchronous locks do not allocate [tasks](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) anymore in case of lock contention. Instead, they are moved to [ValueTask](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) pooling
* `ValueTaskCompletionSource` and `ValueTaskCompletionSource<T>` classes are stabilized and used as a core of [ValueTask](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) pooling
* Introduced Raft-native cluster membership management as proposed in Diego's original paper instead of external discovery mechanism
* Introduced Gossip-based messaging framework

Use [this](https://dotnet.github.io/dotNext/migration/index.html) guide to migrate from 3.x.

<a href="https://www.nuget.org/packages/dotnext/4.0.0">DotNext 4.0.0</a>
* Added `DotNext.Span.Shuffle` and `DotNext.Collections.Generic.List.Shuffle` extension methods that allow to randomize position of elements within span/collection
* Added `DotNext.Collections.Generic.Sequence.Copy` extension method for making copy of the original enumerable collection. The memory for the copy is always rented from the pool
* Added `DotNext.Collections.Generic.Collection.PeekRandom` extension method that allows to select random element from the collection
* Improved performance of `DotNext.Span.TrimLength` and `StringExtensions.TrimLength` extension methods
* Introduced `DotNext.Buffers.BufferHelpers.TrimLength` extension methods for [ReadOnlyMemory&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlymemory-1) and [Memory&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1) data types
* Improved performance of `DotNext.Buffers.BufferWriter<T>.AddAll` method
* Reduced memory allocations by `ElementAt`, `FirstOrEmpty`, `FirstOrNull`, `ForEach` extension methods in `DotNext.Collections.Generic.Sequence` class
* Added `DotNext.Numerics.BitVector` that allows to convert **bool** vectors into integral types
* Added ability to write interpolated strings to `IBufferWriter<char>` without temporary allocations
* Added ability to write interpolated strings to `BufferWriterSlim<char>`. This makes `BufferWriterSlim<char>` type as allocation-free alternative to [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)
* Introduced a concept of binary-formattable types. See `DotNext.Buffers.IBinaryFormattable<TSelf>` interface for more information
* Introduced `Reference<T>` type as a way to pass the reference to the memory location in asynchronous scenarios
* `Box<T>` is replaced with `Reference<T>` value type
* `ITypeMap<T>` interface and implementing classes allow to associate an arbitrary value with the type
* Added overloaded `Result<T, TError>` value type for C-style error handling

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/4.0.0">DotNext.Metaprogramming 4.0.0</a>
* Added support of interpolated string expression as described in [this article](https://devblogs.microsoft.com/dotnet/string-interpolation-in-c-10-and-net-6/) using `InterpolationExpression.Create` static method
* Added support of task pooling to async lambda expressions
* Migration to C# 10 and .NET 6

<a href="https://www.nuget.org/packages/dotnext.reflection/4.0.0">DotNext.Reflection 4.0.0</a>
* Migration to C# 10 and .NET 6

<a href="https://www.nuget.org/packages/dotnext.unsafe/4.0.0">DotNext.Unsafe 4.0.0</a>
* Unmanaged memory pool has moved to [NativeMemory](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativememory) class instead of [Marshal.AllocHGlobal](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.allochglobal) method

<a href="https://www.nuget.org/packages/dotnext.threading/4.0.0">DotNext.Threading 4.0.0</a>
* Polished `ValueTaskCompletionSource` and `ValueTaskCompletionSource<T>` data types. Also these types become a foundation for all synchronization primitives within the library
* Return types of all methods of asynchronous locks now moved to [ValueTask](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) and [ValueTask&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1) types
* Together with previous change, all asynchronous locks are written on top of `ValueTaskCompletionSource` and `ValueTaskCompletionSource<T>` data types. It means that these asynchronous locks use task pooling that leads to zero allocation on the heap and low GC latency
* Added `AsyncEventHub` synchronization primitive for asynchronous code
* Introduced diagnostics and debugging tools for all synchronization primitives: lock contentions, information about suspended callers, et. al.

<a href="https://www.nuget.org/packages/dotnext.io/4.0.0">DotNext.IO 4.0.0</a>
* Added `DotNext.IO.SequenceBinaryReader.Position` property that allows to obtain the current position of the reader in the underlying sequence
* Added `DotNext.IO.SequenceBinaryReader.Read(Span<byte>)` method
* Optimized performance of some `ReadXXX` methods of `DotNext.IO.SequenceReader` type
* All `WriteXXXAsync` methods of `IAsyncBinaryWriter` are replaced with a single `WriteFormattableAsync` method supporting [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable) interface. Now you can encode efficiently any type that implements this interface
* Added `FileWriter` and `FileReader` classes that are tuned for fast file I/O with the ability to access the buffer explicitly
* Introduced a concept of a serializable Data Transfer Objects represented by `ISerializable<TSelf>` interface. The interface allows to control the serialization/deserialization behavior on top of `IAsyncBinaryWriter` and `IAsyncBinaryReader` interfaces. Thanks to static abstract interface methods, the value of the type can be easily reconstructed from its serialized state
* Added support of binary-formattable types to `IAsyncBinaryWriter` and `IAsyncBinaryReader` interfaces
* Improved performance of `FileBufferingWriter` I/O operations with preallocated file size feature introduced in .NET 6
* `StreamExtensions.Combine` allows to represent multiple streams as a single stream

<a href="https://www.nuget.org/packages/dotnext.net.cluster/4.0.0">DotNext.Net.Cluster 4.0.0</a>
* Optimized memory allocation for each hearbeat message emitted by Raft node in leader state
* Fixed compatibility of WAL Interpreter Framework with TCP/UDP transports
* Added support of Raft-native cluster configuration management that allows to use Raft features for managing cluster members instead of external discovery protocol
* Persistent WAL has moved to new implementation of asynchronous locks to reduce the memory allocation
* Added various snapshot building strategies: incremental and inline
* Optimized file I/O performance of persistent WAL
* Reduced the number of opened file descriptors required by persistent WAL
* Improved performance of partitions allocation in persistent WAL with preallocated file size feature introduced in .NET 6
* Fixed packet loss for TCP/UDP transports
* Added read barrier for linearizable reads on Raft follower nodes
* Added transport-agnostic implementation of [HyParView](https://asc.di.fct.unl.pt/~jleitao/pdf/dsn07-leitao.pdf) membership protocol suitable for Gossip-based messaging

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/4.0.0">DotNext.AspNetCore.Cluster 4.0.0</a>
* Added configurable HTTP protocol version selection policy
* Added support of leader lease in Raft implementation for optimized read operations
* Added `IRaftCluster.LeadershipToken` property that allows to track leadership transfer
* Introduced `IRaftCluster.Readiness` property that represents the readiness probe. The probe indicates whether the cluster member is ready to serve client requests

# 08-12-2021
<a href="https://www.nuget.org/packages/dotnext/3.3.1">DotNext 3.3.1</a>
* `DotNext.Threading.Tasks.Synchronization.WaitAsync` doesn't suspend the exception associated with faulty input task anymore

<a href="https://www.nuget.org/packages/dotnext.threading/3.3.1">DotNext.Threading 3.3.1</a>
* Fixed [73](https://github.com/dotnet/dotNext/issues/73)

# 07-28-2021
<a href="https://www.nuget.org/packages/dotnext/3.3.0">DotNext 3.3.0</a>
* Added `ValueTypeExtensions.Normalize` extension methods that allow to normalize numbers of different types
* Improved overall performance of extension methods declaring in `RandomExtensions` class
* Added `Func.IsTypeOf<T>()` and `Predicate.IsTypeOf<T>()` cached predicates
* Deprecation of `CallerMustBeSynchronizedAttribute`
* Fixed backward compatibility issues when _DotNext 3.2.x_ or later used in combination with _DotNext.IO 3.1.x_
* Fixed LGTM warnings

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.3.0">DotNext.Metaprogramming 3.3.0</a>
* Added `CodeGenerator.Statement` static method to simplify migration from pure Expression Trees
* Fixed LGTM warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/3.3.0">DotNext.Reflection 3.3.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.3.0">DotNext.Unsafe 3.3.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/3.3.0">DotNext.Threading 3.3.0</a>
* Introduced a new asynchronous primitive `AsyncCorrelationSource` for synchronization
* Added `ValueTaskCompletionSource<T>` as reusable source of tasks suitable for pooling

<a href="https://www.nuget.org/packages/dotnext.io/3.3.0">DotNext.IO 3.3.0</a>
* `FileBufferingWriter.GetWrittenContentAsync` overload returning `ReadOnlySequence<T>` now ensures that the buffer tail is flushed to the disk
* `FileBufferingWriter.Flush` and `FileBufferingWriter.FlushAsync` methods ensure that the buffer tail is flushed to the disk

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.3.0">DotNext.Net.Cluster 3.3.0</a>
* Added implementation of [Jump](https://arxiv.org/pdf/1406.2294.pdf) consistent hash
* Added support of typed message handlers. See `MessagingClient` and `MessageHandler` classes for more information

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.3.0">DotNext.AspNetCore.Cluster 3.3.0</a>
* Added ETW counter for response time of nodes in the cluster

# 06-09-2021
<a href="https://www.nuget.org/packages/dotnext/3.2.1">DotNext 3.2.1</a>
* Fixed implementation of `Optional<T>.GetHashCode` to distinguish hash code of undefined and null values

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.2.1">DotNext.Metaprogramming 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/3.2.1">DotNext.Reflection 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.2.1">DotNext.Unsafe 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/3.2.1">DotNext.Threading 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/3.2.1">DotNext.IO 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.2.1">DotNext.Net.Cluster 3.2.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.2.1">DotNext.AspNetCore.Cluster 3.2.1</a>
* Updated dependencies

# 06-07-2021
<a href="https://www.nuget.org/packages/dotnext/3.2.0">DotNext 3.2.0</a>
* Added `TryDetachBuffer` method to `BufferWriterSlim<T>` type that allows to flow buffer in async scenarios
* Added `TryGetWrittenContent` method to `SparseBufferWriter<T>` that allows to obtain the written buffer if it is represented by contiguous memory block
* Added `OptionalConverterFactory` class that allows to use `Optional<T>` data type in JSON serialization. This type allows to hide data from JSON if the property of field has undefined value. Useful for designing DTOs for REST API with partial resource updates via PATCH method. Available only when target is .NET 5.
* Added `TryResize` and `Resize` methods to `MemoryOwner<T>` value type
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.2.0">DotNext.Metaprogramming 3.2.0</a>
* Call site optimization for `AsDynamic()` extension method that allows to construct LINQ expression tree on-the-fly using C# expressions
* Fixed [70](https://github.com/dotnet/dotNext/issues/70)

<a href="https://www.nuget.org/packages/dotnext.reflection/3.2.0">DotNext.Reflection 3.2.0</a>
* Respect volatile modifier when reading/writing field

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.2.0">DotNext.Unsafe 3.2.0</a>
* Added additional overloads to `Pointer<T>` value type with **nuint** parameter

<a href="https://www.nuget.org/packages/dotnext.threading/3.2.0">DotNext.Threading 3.2.0</a>
* Added `EnsureState` to `AsyncTrigger` class as synchronous alternative with fail-fast behavior

<a href="https://www.nuget.org/packages/dotnext.io/3.2.0">DotNext.IO 3.2.0</a>
* Improved performance of all `IAsyncBinaryReader` interface implementations
* Added `TryReadBlock` extension method that allows to read the block of memory from pipe synchronously
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.2.0">DotNext.Net.Cluster 3.2.0</a>
* Smallish improvements of I/O operations related to log entries
* Improved performance of background compaction algorithm
* Persistent WAL now supports concurrent read/write. Appending of new log entries to the log tail doesn't suspend readers anymore
* Added event id and event name to all log messages

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.2.0">DotNext.AspNetCore.Cluster 3.2.0</a>
* Improved performance of log entries decoding on receiver side
* Added event id and event name to all log messages

# 05-14-2021
<a href="https://www.nuget.org/packages/dotnext/3.1.1">DotNext 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.1.1">DotNext.Metaprogramming 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/3.1.1">DotNext.Reflection 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.1.1">DotNext.Unsafe 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/3.1.1">DotNext.Threading 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/3.1.1">DotNext.IO 3.1.1</a>
* `FileBufferingWriter.Options` is refactored as value type to avoid heap allocation
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.1.1">DotNext.Net.Cluster 3.1.1</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.1.1">DotNext.AspNetCore.Cluster 3.1.1</a>
* Updated dependencies

# 05-11-2021
This release is primarily focused on improvements of stuff related to cluster programming and Raft: persistent WAL, transferring over the wire, buffering and reducing I/O overhead. Many ideas for this release were proposed by [potrusil-osi](https://github.com/potrusil-osi) in the issue [57](https://github.com/dotnet/dotNext/issues/57).

<a href="https://www.nuget.org/packages/dotnext/3.1.0">DotNext 3.1.0</a>
* Added async support to `IGrowableBuffer<T>` interface
* Added indexer to `MemoryOwner<T>` supporting **nint** data type
* Added more members to `SpanReader<T>` and `SpanWriter<T>` types

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.1.0">DotNext.Metaprogramming 3.1.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/3.1.0">DotNext.Reflection 3.1.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.1.0">DotNext.Unsafe 3.1.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/3.1.0">DotNext.Threading 3.1.0</a>
* `AsyncTigger` now supports fairness policy when resuming suspended callers
* Added support of diagnostics counters

<a href="https://www.nuget.org/packages/dotnext.io/3.1.0">DotNext.IO 3.1.0</a>
* Added `SkipAsync` method to `IAsyncBinaryReader` interface
* Added `TryGetBufferWriter` to `IAsyncBinaryWriter` interface that allows to avoid async overhead when writing to in-memory buffer
* Added more performance optimization options to `FileBufferingWriter` class
* Fixed bug in `StreamSegment.Position` property setter causes invalid position in the underlying stream

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.1.0">DotNext.Net.Cluster 3.1.0</a>
* Added support of three log compaction modes to `PersistentState` class:
   * _Sequential_ which is the default compaction mode in 3.0.x and earlier versions. Provides best optimization of disk space by the cost of the performance of adding new log entries
   * _Background_ which allows to run log compaction in parallel with write operations
   * _Foreground_ which runs log compaction in parallel with commit operation
* Small performance improvements when passing log entries over the wire for TCP and UDP protocols
* Added buffering API for log entries
* Added optional buffering of log entries and snapshot when transferring using TCP or UDP protocols
* Introduced _copy-on-read_ behavior to `PersistentState` class to reduce lock contention between writers and the replication process
* Introduced in-memory cache of log entries to `PersistentState` class to eliminate I/O overhead when appending and applying new log entries
* Reduced number of reads from Raft audit trail during replication
* Interpreter Framework: removed overhead caused by deserialization of command identifier from the log entry. Now the identifier is a part of log entry metadata which is usually pre-cached by underlying WAL implementation

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.1.0">DotNext.AspNetCore.Cluster 3.1.0</a>
* Added ability to override cluster members discovery service. See `IMembersDiscoveryService` interface
* Small performance improvements when passing log entries over the wire for HTTP/1, HTTP/2 and HTTP/3 protocols
* Added optional buffering of log entries and snapshot when transferring over the wire. Buffering allows to reduce lock contention of persistent WAL
* Introduced incremental compaction of committed log entries which is running by special background worker 

**Breaking Changes**: Binary format of persistent WAL has changed. `PersistentState` class from 3.1.0 release is unable to parse the log that was created by earlier versions.

# 02-28-2021
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.0.2">DotNext.AspNetCore.Cluster 3.0.2</a>
* Fixed IP address filter when white list of allowed networks is in use

# 02-26-2021
<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.0.1">DotNext.Net.Cluster 3.0.1</a>
* Minor performance optimizations of Raft heartbeat processing

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.0.1">DotNext.AspNetCore.Cluster 3.0.1</a>
* Unexpected HTTP response received from Raft RPC call cannot crash the node anymore (see [54](https://github.com/dotnet/dotNext/issues/54))

# 01-30-2021
The next major version is out! Its primary focus is .NET 5 support while keeping compatibility with .NET Standard 2.1. As a result, .NEXT libraries built for multiple target frameworks. Additional changes include performance optimizations, polishing of existing API, dropping support of members that were deprecated in 2.x, expanding usage of nullable reference types.

Migration guide for 2.x users is [here](https://dotnet.github.io/dotNext/migration/2.html). Please consider that this version is not fully backward compatible with 2.x.

<a href="https://www.nuget.org/packages/dotnext/3.0.0">DotNext 3.0.0</a>
* Improved performance of [SparseBufferWriter&lt;T&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/SparseBufferWriter%601), [BufferWriterSlim&lt;T&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/BufferWriterSlim%601), [PooledArrayBufferWriter&lt;T&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/PooledArrayBufferWriter%601), [PooledBufferWriter&lt;T&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/PooledBufferWriter%601)
* Fixed nullability attributes
* `ArrayRental<T>` type is replaced by [MemoryOwner&lt;T&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/MemoryOwner%601) type
* Removed obsolete members and classes
* Removed `UnreachableCodeExecutionException` exception
* Completely rewritten implementation of extension methods provided by [AsyncDelegate](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Threading/AsyncDelegate) class
* Added [Base64Decoder](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Text/Base64Decoder) type for efficient decoding of base64-encoded bytes in streaming scenarios
* Removed `Future&lt;T&gt;` type
* Added `ThreadPoolWorkItemFactory` static class with extension methods for constructing [IThreadPoolWorkItem](https://docs.microsoft.com/en-us/dotnet/api/system.threading.ithreadpoolworkitem) instances from method pointers. Available only for .NET 5 target
* Introduced factory methods for constructing delegate instances from the pointers to the managed methods
* `DOTNEXT_STACK_ALLOC_THRESHOLD` environment variable can be used to override stack allocation threshold for all .NEXT routines
* Dropped support of value delegates. They are replaced by functional interfaces. However, they are hiddent from the library consumer so every public API that was based on value delegates now has at least two overloads: CLS-compliant version using regular delegate type and unsafe version using function pointer syntax.
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/3.0.0">DotNext.IO 3.0.0</a>
* Changed behavior of `FileBufferingWriter.GetWrittenContentAsStream` and `FileBufferingWriter.GetWrittenContentAsStreamAsync` in a way which allows you to use synchronous/asynchronous I/O for writing and reading separately
* Introduced extension methods for [BufferWriterSlim&lt;char&gt;](https://www.fuget.org/packages/DotNext/3.0.0/lib/net5.0/DotNext.dll/DotNext.Buffers/BufferWriterSlim%601) type for encoding of primitive data types
* Fixed nullability attributes
* Added advanced encoding/decoding methods to [IAsyncBinaryWriter](https://www.fuget.org/packages/DotNext.IO/3.0.0/lib/net5.0/DotNext.IO.dll/DotNext.IO/IAsyncBinaryWriter) and [IAsyncBinaryReader](https://www.fuget.org/packages/DotNext.IO/3.0.0/lib/net5.0/DotNext.IO.dll/DotNext.IO/IAsyncBinaryReader) interfaces
* Removed obsolete members and classes
* Simplified signature of `AppendAsync` methods exposed by [IAuditTrail&lt;TEntry&gt;](https://www.fuget.org/packages/DotNext.IO/3.0.0/lib/net5.0/DotNext.IO.dll/DotNext.IO.Log/IAuditTrail%601) interface
* Improved performances of extension methods declared in [PipeExtensions](https://www.fuget.org/packages/DotNext.IO/3.0.0/lib/net5.0/DotNext.IO.dll/DotNext.IO.Pipelines/PipeExtensions) class
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/3.0.0">DotNext.Metaprogramming 3.0.0</a>
* Fixed nullability attributes
* Fixed [issue 23](https://github.com/dotnet/dotNext/issues/23)
* Fixed code generation of **finally** blocks inside of asynchronous lambda expressions
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/3.0.0">DotNext.Reflection 3.0.0</a>
* Improved performance of reflective calls
* [DynamicInvoker](https://www.fuget.org/packages/DotNext.Reflection/3.0.0/lib/net5.0/DotNext.Reflection.dll/DotNext.Reflection/DynamicInvoker) delegate allows to pass arguments for dynamic invocation as [Span&lt;object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1) instead of `object[]`
* Fixed nullability attributes

<a href="https://www.nuget.org/packages/dotnext.threading/3.0.0">DotNext.Threading 3.0.0</a>
* Modified ability to await on [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) and [WaitHandle](https://docs.microsoft.com/en-us/dotnet/api/system.threading.waithandle). [ValueTask](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) is the primary return type of the appropriate methods
* Fixed nullability attributes
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/3.0.0">DotNext.Unsafe 3.0.0</a>
* Removed obsolete members and classes
* Fixed nullability attributes
* Added `PinnedArray<T>` as a wrapper of pinned arrays from .NET 5
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/3.0.0">DotNext.Net.Cluster 3.0.0</a>
* Improved performance of [persistent WAL](https://www.fuget.org/packages/DotNext.Net.Cluster/3.0.0/lib/net5.0/DotNext.Net.Cluster.dll/DotNext.Net.Cluster.Consensus.Raft/PersistentState)
* Added support of active-standby configuration of Raft cluster. Standby node cannot become a leader but can be used for reads
* Introduced [framework](https://www.fuget.org/packages/DotNext.Net.Cluster/3.0.0/lib/net5.0/DotNext.Net.Cluster.dll/DotNext.Net.Cluster.Consensus.Raft.Commands/CommandInterpreter) for writing interpreters of log entries stored in persistent write-ahead log
* Added support of JSON-serializable log entries (available for .NET 5 only)
* Fixed bug causing long shutdown of Raft node which is using TCP transport
* Added support of **PreVote** extension for Raft preventing _term inflation_

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/3.0.0">DotNext.AspNetCore.Cluster 3.0.0</a>
* Added `UsePersistenceEngine` extension method for correct registration of custom persistence engine derived from [PersistentState](https://www.fuget.org/packages/DotNext.Net.Cluster/3.0.0/lib/net5.0/DotNext.Net.Cluster.dll/DotNext.Net.Cluster.Consensus.Raft/PersistentState) class
* Added support of HTTP/3 (available for .NET 5 only)
* Significantly optimized performance and traffic volume of **AppendEntries** Raft RPC call. Now replication performance is comparable to TCP/UDP transports
* Added DNS support. Now cluster member address can be specified using its name instead of IP address

`DotNext.Augmentation` IL weaver add-on for MSBuild is no longer supported.

# 01-07-2021
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.12.2">DotNext.Metaprogramming 2.12.2</a>
* Fixed [46](https://github.com/dotnet/dotNext/issues/46)

# 12-16-2020
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.12.1">DotNext.Metaprogramming 2.12.1</a>
* Fixed invalid detection of the collection item type inside of [CollectionAccessExpression](https://dotnet.github.io/dotNext/api/DotNext.Linq.Expressions.CollectionAccessExpression.html)

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.12.1">DotNext.Net.Cluster 2.12.1</a>
* Fixed issue [24](https://github.com/dotnet/dotNext/issues/24)

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.12.1">DotNext.AspNetCore.Cluster 2.12.1</a>
* Fixed issue [24](https://github.com/dotnet/dotNext/issues/24)

# 12-04-2020
<a href="https://www.nuget.org/packages/dotnext/2.12.0">DotNext 2.12.0</a>
* Added consuming enumerator for [IProducerConsumerCollection&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.iproducerconsumercollection-1)
* Introduced `ServiceProviderFactory` class and its factory methods for producing [Service Providers](https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider)
* Significant performance improvements of `StringExtensions.Reverse` method
* Introduced a new class `SparseBufferWriter<T>` in addition to existing buffer writes which acts as a growable buffer without memory reallocations
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/2.12.0">DotNext.IO 2.12.0</a>
* Introduced `TextBufferReader` class inherited from [TextReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader) that can be used to read the text from [ReadOnlySequence&lt;char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) or [ReadOnlyMemory&lt;char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlymemory-1)
* Added `SequenceBuilder<T>` type for building [ReadOnlySequence&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) instances from the chunk of memory blocks
* Added `GetWrittenContentAsStream` and `GetWrittenContentAsStreamAsync` methods to [FileBufferingWriter](https://dotnet.github.io/dotNext/api/DotNext.IO.FileBufferingWriter.html) class
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.12.0">DotNext.Metaprogramming 2.12.0</a>
* Added support of `await using` statement
* Added support of `await foreach` statement
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.12.0">DotNext.Reflection 2.12.0</a>
* More performance optimizations in code generation mechanism responsible for the method or constructor calls
* Added ability to reflect abstract and interface methods
* Added support of volatile access to the field via reflection

<a href="https://www.nuget.org/packages/dotnext.threading/2.12.0">DotNext.Threading 2.12.0</a>
* Added support of `Count` and `CanCount` properties inherited from [ChannelReader&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels.channelreader-1) by persistent channel reader
* Added support of diagnostics counters for persistent channel
* Fixed resuming of suspended callers in [AsyncTrigger](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncTrigger.html) class
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.12.0">DotNext.Unsafe 2.12.0</a>
* Fixed ignoring of array offset in `ReadFrom` and `WriteTo` methods of [Pointer&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Runtime.InteropServices.Pointer-1.html) type
* Added `ToArray` method to [Pointer&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Runtime.InteropServices.Pointer-1.html) type
* Added indexer property to [IUnmanagedArray&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Runtime.InteropServices.IUnmanagedArray-1.html) interface
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.12.0">DotNext.Net.Cluster 2.12.0</a>
* Updated dependencies shipped with .NET Core 3.1.10

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.12.0">DotNext.AspNetCore.Cluster 2.12.0</a>
* Updated dependencies shipped with .NET Core 3.1.10

# 11-11-2020
<a href="https://www.nuget.org/packages/dotnext.reflection/2.11.2">DotNext.Reflection 2.11.2</a>
* More performance optimizations in code generation mechanism responsible for construction dynamic method or constructor calls

# 11-08-2020
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.11.1">DotNext.Metaprogramming 2.11.1</a>
* Fixed issue [19](https://github.com/dotnet/dotNext/issues/19)

<a href="https://www.nuget.org/packages/dotnext.reflection/2.11.1">DotNext.Reflection 2.11.1</a>
* `Reflector.Unreflect` now can correctly represents **void** method or property setter as [DynamicInvoker](https://dotnet.github.io/dotNext/api/DotNext.Reflection.DynamicInvoker.html) delegate
* Unreflected members via [DynamicInvoker](https://dotnet.github.io/dotNext/api/DotNext.Reflection.DynamicInvoker.html) delegate correctly handles boxed value types
* Improved performance of [DynamicInvoker](https://dotnet.github.io/dotNext/api/DotNext.Reflection.DynamicInvoker.html) for by-ref argument of value type

# 11-01-2020
<a href="https://www.nuget.org/packages/dotnext/2.11.0">DotNext 2.11.0</a>
* Added `Span<T>.CopyTo` and `ReadOnlySpan<T>.CopyTo` extension methods to support cases when the source span can be larger than the destination
* Added `Span.AsSpan` and `Span.AsReadOnlySpan` for value tuples
* Deprecated [EnumerableTuple](https://dotnet.github.io/dotNext/api/DotNext.EnumerableTuple-2.html) data type
* Minor performance improvements
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.11.0">DotNext.Metaprogramming 2.11.0</a>
* Overloaded `CodeGenerator.AsyncLambda` supports _Pascal_-style return (issue [13](https://github.com/dotnet/dotNext/issues/13))
* Fixed suppression of exceptions raised by generated async lambda (issue [14](https://github.com/dotnet/dotNext/issues/14))
* Fixed invalid behavior of async lambda body rewriter (issue [17](https://github.com/dotnet/dotNext/issues/17))
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.11.0">DotNext.Reflection 2.11.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/2.11.0">DotNext.Threading 2.11.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.11.0">DotNext.Unsafe 2.11.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.11.0">DotNext.Net.Cluster 2.11.0</a>
* Added `requestTimeout` configuration property for TCP/UDP transports
* Stabilized shutdown of Raft server for TCP/UDP transports
* Added SSL support for TCP transport
* Updated dependencies shipped with .NET Core 3.1.9

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.11.0">DotNext.AspNetCore.Cluster 2.11.0</a>
* Added `requestTimeout` and `rpcTimeout` configuration properties for precise control over timeouts used for communication between Raft nodes (issue [12](https://github.com/dotnet/dotNext/issues/12))
* Updated dependencies shipped with .NET Core 3.1.9

# 09-28-2020
<a href="https://www.nuget.org/packages/dotnext/2.10.1">DotNext 2.10.1</a>
* Fixed correctness of `Clear(bool)` method overridden by `PooledArrayBufferWriter<T>` and `PooledBufferWriter<T>` classes
* Added `RemoveLast` and `RemoveFirst` methods to `PooledArrayBufferWriter<T>` class
* `Optional<T>` type distinguishes **null** and undefined value
* [DotNext.Sequence](https://dotnet.github.io/dotNext/api/DotNext.Sequence.html) class is now deprecated and replaced with [DotNext.Collections.Generic.Sequence](https://dotnet.github.io/dotNext/api/DotNext.Collections.Generic.Sequence.html) class. It's binary compatible but source incompatible change
* Added [new API](https://dotnet.github.io/dotNext/api/DotNext.Resources.ResourceManagerExtensions.html) for writing resource string readers. It utilizes [Caller Info](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/caller-information) feature in C# to resolve resource entry name using accessor method or property
* Introduced [BufferWriterSlim&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.BufferWriterSlim-1.html) type as lightweight and stackalloc-friendly version of [PooledBufferWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.PooledBufferWriter-1.html) type
* Introduced [SpanReader&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.SpanReader-1.html) and [SpanWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.SpanWriter-1.html) types that can be used for sequential access to the elements in the memory span
* Removed unused resource strings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.10.1">DotNext.Metaprogramming 2.10.1</a>
* Added extension methods of [ExpressionBuilder](https://dotnet.github.io/dotNext/api/DotNext.Linq.Expressions.ExpressionBuilder.html) class for constructing expressions of type [Optional&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Optional-1.html), [Result&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Result-1.html) or [Nullable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)
* Fixed bug with expression building using **dynamic** keyword
* [UniversalExpression](https://dotnet.github.io/dotNext/api/DotNext.Linq.Expressions.UniversalExpression.html) is superseded by _ExpressionBuilder.AsDynamic_ extension method
* Removed unused resource strings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.10.1">DotNext.Reflection 2.10.1</a>
* Removed unused resource strings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/2.10.1">DotNext.Threading 2.10.1</a>
* [AsyncExchanger&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncExchanger-1.html) class now has a method for fast synchronous exchange
* [AsyncTimer](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncTimer.html) implements [IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable) for graceful shutdown
* Removed unused resource strings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.10.1">DotNext.Unsafe 2.10.1</a>
* [Pointer&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Runtime.InteropServices.Pointer-1.html) value type now implements [IPinnable](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ipinnable) interface
* Added interop between [Pointer&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Runtime.InteropServices.Pointer-1.html) and [System.Reflection.Pointer](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.pointer)
* Removed unused resource strings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.10.1">DotNext.Net.Cluster 2.10.1</a>
* Removed unused resource strings
* Updated dependencies shipped with .NET Core 3.1.8

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.10.1">DotNext.AspNetCore.Cluster 2.10.1</a>
* Removed unused resource strings
* Updated dependencies shipped with .NET Core 3.1.8

# 08-16-2020
<a href="https://www.nuget.org/packages/dotnext/2.9.6">DotNext 2.9.6</a>
* Improved performance of [Enum Member API](https://dotnet.github.io/dotNext/features/core/enum.html)

<a href="https://www.nuget.org/packages/dotnext.io/2.7.6">DotNext.IO 2.7.6</a>
* Fixed compiler warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.6.6">DotNext.Metaprogramming 2.6.6</a>
* Fixed compiler warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.6.6">DotNext.Reflection 2.6.6</a>
* Fixed compiler warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/2.9.6">DotNext.Threading 2.9.6</a>
* Fixed compiler warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.7.6">DotNext.Unsafe 2.7.6</a>
* Fixed compiler warnings
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.6.6">DotNext.Net.Cluster 2.6.6</a>
* Fixed unstable behavior of Raft TCP transport on Windows. See issue [#10](https://github.com/dotnet/dotNext/issues/10) for more info.
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.6.6">DotNext.AspNetCore.Cluster 2.6.6</a>
* Updated dependencies

# 08-08-2020
<a href="https://www.nuget.org/packages/dotnext/2.9.5">DotNext 2.9.5</a>
* Added support of custom attributes to [Enum Member API](https://dotnet.github.io/dotNext/features/core/enum.html)

# 08-06-2020
<a href="https://www.nuget.org/packages/dotnext/2.9.1">DotNext 2.9.1</a>
* Added `Continuation.ContinueWithTimeout<T>` extension method that allows to produce the task from the given task with attached timeout and, optionally, token

<a href="https://www.nuget.org/packages/dotnext.threading/2.9.0">DotNext.Threading 2.9.0</a>
* Fixed graceful shutdown for async locks if they are not in locked state
* Added  [AsyncExchanger&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncExchanger-1.html) synchronization primitive that allows to organize pipelines
* [AsyncTrigger](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncTrigger.html) now has additional `SignalAndWaitAsync` overloads

# 07-30-2020
<a href="https://www.nuget.org/packages/dotnext/2.9.0">DotNext 2.9.0</a>
* Added `Sequence.ToAsyncEnumerable()` extension method that allows to convert arbitrary [IEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) to [IAsyncEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)
* Added extension methods to `Sequence` class for working with [async streams][IAsyncEnumerable&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)

<a href="https://www.nuget.org/packages/dotnext.io/2.7.3">DotNext.IO 2.7.3</a>
* Fixed behavior of `GetObjectDataAsync` method in [StreamTransferObject](https://dotnet.github.io/dotNext/api/DotNext.IO.StreamTransferObject.html). Now it respects the value of `IsReusable` property.

# 07-27-2020
<a href="https://www.nuget.org/packages/dotnext/2.8.0">DotNext 2.8.0</a>
* Added `MemoryTemplate<T>` value type that represents pre-compiled template with placeholders used for fast creation of `Memory<T>` and **string** objects

# 07-24-2020
<a href="https://www.nuget.org/packages/dotnext.io/2.7.2">DotNext.IO 2.7.2</a>
* Added `BufferWriter.WriteLine` overloaded extension method that allows to specify `ReadOnlySpan<char>` as an input argument

# 07-15-2020
<a href="https://www.nuget.org/packages/dotnext.io/2.7.1">DotNext.IO 2.7.1</a>
* Text writer constructed with `TextWriterSource.AsTextWriter` extension method can be converted to string containing all written characters

# 07-13-2020
<a href="https://www.nuget.org/packages/dotnext.unsafe/2.7.1">DotNext.Unsafe 2.7.1</a>
* Optimized `UnmanagedMemoryPool<T>.GetAllocator` method

# 07-11-2020
<a href="https://www.nuget.org/packages/dotnext.unsafe/2.7.0">DotNext.Unsafe 2.7.0</a>
* `UnmanagedMemoryPool<T>.GetAllocator` public static method is added for compatibility with [MemoryAllocator&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.MemoryAllocator-1.html) delegate

# 07-09-2020
This release is mainly focused on `DotNext.IO` library to add new API unifying programming experience across I/O pipelines, streams, [sequences](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) and [buffer writers](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1).

<a href="https://www.nuget.org/packages/dotnext/2.7.0">DotNext 2.7.0</a>
* Introduced extension methods in [Span](https://dotnet.github.io/dotNext/api/DotNext.Span.html) class for concatenation of memory spans
* Removed allocation of [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream) in the extension methods of [StreamSource](https://dotnet.github.io/dotNext/api/DotNext.IO.StreamSource.html) class when passed [ReadOnlySequence&lt;byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) is empty
* [StreamSource](https://dotnet.github.io/dotNext/api/DotNext.IO.StreamSource.html) has additional methods to create streams from various things
* [PooledArrayBufferWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.PooledArrayBufferWriter-1.html) and [PooledBufferWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.PooledBufferWriter-1.html) support reuse of the internal buffer using overloaded `Clear(bool)` method

<a href="https://www.nuget.org/packages/dotnext.io/2.7.0">DotNext.IO 2.7.0</a>
* [BufferWriter](https://dotnet.github.io/dotNext/api/DotNext.Buffers.BufferWriter.html) now contains extension methods that allow to use any object implementing [IBufferWriter&lt;char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1) as pooled string builder
* [IAsyncBinaryReader](https://dotnet.github.io/dotNext/api/DotNext.IO.IAsyncBinaryReader.html), [IAsyncBinaryWriter](https://dotnet.github.io/dotNext/api/DotNext.IO.IAsyncBinaryWriter.html), [PipeExtensions](https://dotnet.github.io/dotNext/api/DotNext.IO.Pipelines.PipeExtensions.html), [StreamExtensions](https://dotnet.github.io/dotNext/api/DotNext.IO.StreamExtensions.html), [SequenceBinaryReader](https://dotnet.github.io/dotNext/api/DotNext.IO.SequenceBinaryReader.html) types now containing methods for encoding/decoding primitive types, [DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime), [DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset), [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid) to/from string representation contained in underlying stream, pipe or [sequence](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1) in the binary form
* Fixed pooled memory leaks in [SequenceBinaryReader](https://dotnet.github.io/dotNext/api/DotNext.IO.SequenceBinaryReader.html)
* [TextWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.textwriter) over [IBufferWriter&lt;char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1) interface using extension method in [TextWriterSource](https://dotnet.github.io/dotNext/api/DotNext.IO.TextWriterSource.html) class

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.6.1">DotNext.Metaprogramming 2.6.1</a>
* Enabled consistent build which is recommended for SourceLink

<a href="https://www.nuget.org/packages/dotnext.reflection/2.6.1">DotNext.Reflection 2.6.1</a>
* Optimized construction of getter/setter for the reflected field
* Enabled consistent build which is recommended for SourceLink

<a href="https://www.nuget.org/packages/dotnext.threading/2.6.1">DotNext.Threading 2.6.1</a>
* Enabled consistent build which is recommended for SourceLink

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.6.1">DotNext.Unsafe 2.6.1</a>
* Enabled consistent build which is recommended for SourceLink

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.6.1">DotNext.Net.Cluster 2.6.1</a>
* Enabled consistent build which is recommended for SourceLink

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.6.1">DotNext.AspNetCore.Cluster 2.6.1</a>
* Reduced memory allocation caused by replication of log entries
* Enabled consistent build which is recommended for SourceLink

# 06-14-2020
<a href="https://www.nuget.org/packages/dotnext/2.6.0">DotNext 2.6.0</a>
* More ways to create `MemoryOwner<T>`
* Removed copying of synchronization context when creating continuation for `Future` object
* Introduced APM helper methods in `AsyncDelegate` class

<a href="https://www.nuget.org/packages/dotnext.io/2.6.0">DotNext.IO 2.6.0</a>
* Improved performance of `FileBufferingWriter`
* `FileBufferingWriter` now contains correctly implemented `BeginWrite` and `EndWrite` methods
* `FileBufferingWriter` ables to return written content as [ReadOnlySequence&lt;byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1)
* Introduced `BufferWriter` class with extension methods for [IBufferWriter&lt;byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1) aimed to encoding strings, primitive and blittable types
* Support of `ulong`, `uint` and `ushort` data types available for encoding/decoding in `SequenceBinaryReader` and `PipeExtensions` classes
* Ability to access memory-mapped file content via [ReadOnlySequence&lt;byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.readonlysequence-1)

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.6.0">DotNext.Metaprogramming 2.6.0</a>
* Introduced null-coalescing assignment expression
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.6.0">DotNext.Reflection 2.6.0</a>
* Introduced null-coalescing assignment expression
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/2.6.0">DotNext.Threading 2.6.0</a>
* Fixed race-condition caused by `AsyncTrigger.Signal` method
* `AsyncLock` now implements [IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable) interface
* `AsyncExclusiveLock`, `AsyncReaderWriterLock` and `AsyncSharedLock` now have support of graceful shutdown implemented via [IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable) interface

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.6.0">DotNext.Unsafe 2.6.0</a>
* Optimized performance of methods in `MemoryMappedFileExtensions` class
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.6.0">DotNext.Net.Cluster 2.6.0</a>
* Fixed behavior of `PersistentState.DisposeAsync` so it suppress finalization correctly

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.6.0">DotNext.AspNetCore.Cluster 2.6.0</a>
* Respect shutdown timeout inherited from parent host in Hosted Mode
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/2.1.0">DotNext.Augmentation.Fody 2.1.0</a>
* Removed usage of obsolete methods from `Fody`
* Updated `Fody` version

# 06-01-2020
<a href="https://www.nuget.org/packages/dotnext/2.5.0">DotNext 2.5.0</a>
* Improved performance of `PooledBufferWriter`
* `MemoryAllocator<T>` now allows to allocate at least requested number of elements

<a href="https://www.nuget.org/packages/dotnext.io/2.5.0">DotNext.IO 2.5.0</a>
* Ability to represent stream as [IBufferWriter&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1)
* `FileBufferingWriter` class is one more growable buffer backed by file in case of very large buffer size

# 05-29-2020
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.4.1">DotNext.Metaprogramming 2.4.1</a>
* Fixed dynamic construction of tuples using `ValueTupleBuilder` class (PR [#8](https://github.com/dotnet/dotNext/pull/8))

# 05-20-2020
<a href="https://www.nuget.org/packages/dotnext/2.4.2">DotNext 2.4.2</a>
* Reduced memory allocation caused by continuations in `Future` class
* Improved performance of some methods in `MemoryRental<T>` and `DelegateHelpers` classes
* Reduced amount of memory re-allocations in `PooledBufferWriter<T>` and `PooledArrayBufferWriter<T>` classes

# 05-18-2020
<a href="https://www.nuget.org/packages/dotnext/2.4.1">DotNext 2.4.1</a>
* `ArrayRental<T>` can automatically determine array cleanup policy
* `MemoryRental<T>` is improved for stackalloc/pooling pattern
* Fixed bug in `Clear` method of `PooledBufferWriter` class

# 05-17-2020
This release is mostly aimed to improving code quality of all .NEXT libraries with help of _StyleCop_ analyzer.

<a href="https://www.nuget.org/packages/dotnext/2.4.0">DotNext 2.4.0</a>
* `DotNext.IO.StreamSource` class allows to convert `ReadOnlyMemory<byte>` or `ReadOnlySequence<byte>` to stream
* `DotNext.IO.StreamSource` class allows to obtain writable stream for `IBufferWriter<byte>`

<a href="https://www.nuget.org/packages/dotnext.io/2.4.0">DotNext.IO 2.4.0</a>
* Support of `BeginRead` and `EndRead` methods in `StreamSegment` class
* Update to the latest `System.IO.Pipelines` library

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.4.0">DotNext.Metaprogramming 2.4.0</a>
* Fixed several compiler warnings

<a href="https://www.nuget.org/packages/dotnext.reflection/2.4.0">DotNext.Reflection 2.4.0</a>
* Fixed several compiler warnings

<a href="https://www.nuget.org/packages/dotnext.threading/2.4.0">DotNext.Threading 2.4.0</a>
* Fixed several compiler warnings
* Update to the latest `System.Threading.Channels` library

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.4.0">DotNext.Unsafe 2.4.0</a>
* Ability to convert `Pointer<T>` to `IMemoryOwner<T>`

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.4.0">DotNext.Net.Cluster 2.4.0</a>
* Added calls to `ConfigureAwait` in multiple places

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.4.0">DotNext.AspNetCore.Cluster 2.4.0</a>
* Added calls to `ConfigureAwait` in multiple places
* Fixed node status tracking when TCP or UDP transport in use

# 05-11-2020
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.3.2">DotNext.AspNetCore.Cluster 2.3.2</a>
* Section with local node configuration can be defined explicitly

# 05-09-2020
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.3.1">DotNext.AspNetCore.Cluster 2.3.1</a>
* Alternative methods for configuring local node

# 04-23-2020
<a href="https://www.nuget.org/packages/dotnext/2.3.0">DotNext 2.3.0</a>
* Performance improvements of `BitwiseComparer` and `Intrinsics` classes  
* Introduced new [MemoryOwner&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.MemoryOwner-1.html) value type that unifies working with memory and array pools
* Path MTU [discovery](https://dotnet.github.io/dotNext/api/DotNext.Net.NetworkInformation.MtuDiscovery.html)
* Pooled buffer writes: [PooledBufferWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.PooledBufferWriter-1.html) and [PooledArrayBufferWriter&lt;T&gt;](https://dotnet.github.io/dotNext/api/DotNext.Buffers.PooledArrayBufferWriter-1.html)
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.3.0">DotNext.Metaprogramming 2.3.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.3.0">DotNext.Unsafe 2.3.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.io/2.3.0">DotNext.IO 2.3.0</a>
* Fixed bugs that lead to unexpected EndOfStreamException in some methods of `StreamExtensions` class
* Introduced new methods in `StreamExtensions` class for reading data of exact size

<a href="https://www.nuget.org/packages/dotnext.threading/2.3.0">DotNext.Threading 2.3.0</a>
* Improved performance of existing asynchronous locks
* Added [AsyncTrigger](https://dotnet.github.io/dotNext/api/DotNext.Threading.AsyncTrigger.html) synchronization primitive

<a href="https://www.nuget.org/packages/dotnext.reflection/2.3.0">DotNext.Reflection 2.3.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.3.0">DotNext.Net.Cluster 2.3.0</a>
* TCP transport for Raft
* UDP transport for Raft
* Fixed bug in [PersistentState](https://dotnet.github.io/dotNext/api/DotNext.Net.Cluster.Consensus.Raft.PersistentState.html) class that leads to incorrect usage of rented memory and unexpected result during replication between nodes
* Methods for handling Raft messages inside of [RaftCluster&lt;TMember&gt;](https://dotnet.github.io/dotNext/api/DotNext.Net.Cluster.Consensus.Raft.RaftCluster-1.html) class now support cancellation via token

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.3.0">DotNext.AspNetCore.Cluster 2.3.0</a>
* Updated dependencies
* Fixed cancellation of asynchronous operations

# 03-08-2020
<a href="https://www.nuget.org/packages/dotnext/2.2.0">DotNext 2.2.0</a>
* Ability to slice lists using range syntax and new `ListSegment` data type
* Various extension methods for broader adoption of range/index feature from C# 8

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/2.2.0">DotNext.Metaprogramming 2.2.0</a>
* Support of range and index expressions from C# 8

<a href="https://www.nuget.org/packages/dotnext.unsafe/2.2.0">DotNext.Unsafe 2.2.0</a>
* Access to memory-mapped file via `System.Memory<T>` data type

<a href="https://www.nuget.org/packages/dotnext.io/2.2.0">DotNext.IO 2.2.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/2.2.0">DotNext.Threading 2.2.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/2.2.0">DotNext.Reflection 2.2.0</a>
* Lighweight API for fast reflection is added. See overloaded `Unreflect` methods in `Reflector` class.

<a href="https://www.nuget.org/packages/dotnext.net.cluster/2.2.0">DotNext.Net.Cluster 2.2.0</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/2.2.0">DotNext.AspNetCore.Cluster 2.2.0</a>
* Upgrade to latest ASP.NET Core

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/2.0.1">DotNext.Augmentation.Fody 2.0.1</a>
* Removed obsolete calls

# 02-23-2020
<a href="https://www.nuget.org/packages/dotnext/2.1.0">DotNext 2.1.0</a>
* Reduced memory footprint of `DotNext.Span` static constructor
* `DotNext.UserDataStorage` behavior is now customizable via `UserDataStorage.IContainer` interface
* Introduced `Intrinsics.GetReadonlyRef` method allows to reinterpret managed pointer to array element
* `DelegateHelpers.Bind` now supports both closed and open delegates

# 01-31-2020
Major release of version 2.0 is completely finished and contains polished existing and new API. All libraries in .NEXT family are upgraded. Migration guide for 1.x users is [here](https://dotnet.github.io/dotNext/migration/1.html). Please consider that this version is not fully backward compatible with 1.x.

Major version is here for the following reasons:
1. .NET Core 3.1 LTS is finally released
1. .NET Standard 2.1 contains a lot of new API required for optimizations. The most expected API is asynchronous methods in [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream) class. These enhancements are necessary for evolution of .NEXT library. For instance, new [DotNext.IO](https://www.nuget.org/packages/DotNext.IO/) library could not be released without new .NET API.
1. ASP.NET Core 2.2 is no longer supported by Microsoft. Therefore, [DotNext.AspNetCore.Cluster](https://www.nuget.org/packages/DotNext.AspNetCore.Cluster/) library of version 1.x relies on unmaintainable platform. Now it is based on ASP.NET Core 3.1 which has long-term support.

What is done in this release:
1. Quality-focused changes
    1. Removed trivial "one-liners" in **DotNext** library
    1. Reduced and unified API to work with unmanaged memory in **DotNext.Unsafe** library
    1. **DotNext.AspNetCore.Cluster** migrated to ASP.NET Core 3.1 LTS
    1. Increased test coverage and fixed bugs
    1. Additional optimizations of performance in [Write-Ahead Log](https://dotnet.github.io/dotNext/api/DotNext.Net.Cluster.Consensus.Raft.PersistentState.html)
    1. Fixed issue [#4](https://github.com/dotnet/dotNext/issues/4)
    1. Introduced API for client interaction support described in Chapter 6 of [Raft dissertation](https://github.com/ongardie/dissertation/blob/master/book.pdf)
    1. Migration to C# 8 and nullable reference types
1. New features
    1. Introduced [DotNext.IO](https://www.nuget.org/packages/DotNext.IO/) library with unified asynchronous API surface for .NET streams and I/O [pipelines](https://docs.microsoft.com/en-us/dotnet/api/system.io.pipelines). This API provides high-level methods for encoding and decoding of data such as strings and blittable types. In other words, if you want to have [BinaryReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader) or [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.binarywriter) for pipelines then welcome!
    1. Ability to obtain result of [task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) asynchronously when its result type is not known at compile-time
    1. Fast hexadecimal string conversion to `Span<byte>` and vice versa

Raft users are strongly advised to migrate to this new version.

# 01-12-2020
<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.11">DotNext.Net.Cluster 1.2.11</a>
* Ability to reconstruct internal state using `PersistentState.ReplayAsync` method

# 01-11-2020
<a href="https://www.nuget.org/packages/dotnext/1.2.10">DotNext 1.2.10</a>
* Fixed invalid behavior of `StreamSegment.Position` property

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.10">DotNext.Net.Cluster 1.2.10</a>
* Removed redundant validation of log entry index in `PersistentState`

# 12-06-2019
<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.10">DotNext.Unsafe 1.2.10</a>
* Fixed invalid usage of `GC.RemoveMemoryPressure` in `Reallocate` methods

# 12-04-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.9">DotNext 1.2.9</a>
* `UserDataStorage` no longer stores **null** values in its internal dictionary
* Updated dependencies
* Migration to SourceLink 1.0.0

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.2.9">DotNext.Metaprogramming 1.2.9</a>
* Updated dependencies
* Migration to SourceLink 1.0.0

<a href="https://www.nuget.org/packages/dotnext.reflection/1.2.9">DotNext.Reflection 1.2.9</a>
* Updated dependencies
* Migration to SourceLink 1.0.0

<a href="https://www.nuget.org/packages/dotnext.threading/1.3.3">DotNext.Threading 1.3.3</a>
* Updated dependencies
* Migration to SourceLink 1.0.0

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.9">DotNext.Unsafe 1.2.9</a>
* Updated dependencies
* Fixed invalid calculation of byte length in `Pointer.Clear` method

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.9">DotNext.Net.Cluster 1.2.9</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.9">DotNext.AspNetCore.Cluster 1.2.9</a>
* Updated dependencies

# 11-27-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.8">DotNext.AspNetCore.Cluster 1.2.8</a>
* Improved performance of one-way no-ack messages that can be passed using `ISubscriber.SendSignalAsync` method

# 11-25-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.7">DotNext 1.2.7</a>
* `BitwiseComparer` now available as singleton instance

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.7">DotNext.Net.Cluster 1.2.7</a>
* Improved performance of copying log entry content when `PersistentState` is used as persistent audit trail

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.7">DotNext.AspNetCore.Cluster 1.2.7</a>
* Improved performance of message exchange between cluster members

# 11-24-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.6">DotNext 1.2.6</a>
* Fixed typos in XML documentation
* Updated *InlineIL.Fody* dependency

<a href="https://www.nuget.org/packages/dotnext.threading/1.3.2">DotNext.Threading 1.3.2</a>
* Fixed `MissingManifestResourceException` caused by `AsyncLock` value type on .NET Core 3.x

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.6">DotNext.Unsafe 1.2.6</a>
* Updated *InlineIL.Fody* dependency

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.6">DotNext.Net.Cluster 1.2.6</a>
* Fixed NRE when `RaftCluster.StopAsync` called multiple times

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.6">DotNext.AspNetCore.Cluster 1.2.6</a>
* Migration to patched `RaftCluster` class

# 11-20-2019
<a href="https://www.nuget.org/packages/dotnext.threading/1.3.1">DotNext.Threading 1.3.1</a>
* Fixed NRE when `Dispose` method of [PersistentChannel](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.Channels.PersistentChannel-2.html) class called multiple times

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.5">DotNext.AspNetCore.Cluster 1.2.5</a>
* Fixed bug when log entry may have invalid content when retrieved from persistent audit trail. Usually this problem can be observed in case of concurrent read/write and caused by invalid synchronization of multiple file streams.

# 11-18-2019
<a href="https://www.nuget.org/packages/dotnext.threading/1.3.0">DotNext.Threading 1.3.0</a>
* [PersistentChannel](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.Channels.PersistentChannel-2.html) is added as an extension of **channel** concept from [System.Threading.Channels](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.threading.channels). It allows to use disk memory instead of RAM for storing messages passed from producer to consumer. Read more [here](https://dotnet.github.io/dotNext/features/threading/channel.html)
* [AsyncCounter](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncCounter.html) allows to simplify asynchronous coordination in producer/consumer scenario

# 11-15-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.4">DotNext 1.2.4</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.2.4">DotNext.Metaprogramming 1.2.4</a>
* Fixed NRE

<a href="https://www.nuget.org/packages/dotnext.reflection/1.2.4">DotNext.Reflection 1.2.4</a>
* Internal cache is optimized to avoid storage of null values

<a href="https://www.nuget.org/packages/dotnext.threading/1.2.4">DotNext.Threading 1.2.4</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.4">DotNext.Unsafe 1.2.4</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.4">DotNext.Net.Cluster 1.2.4</a>
* Fixed unnecessary boxing of generic log entry value

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.4">DotNext.AspNetCore.Cluster 1.2.4</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/1.2.4">DotNext.Augmentation.Fody 1.2.4</a>
* Updated dependencies

# 11-11-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.3">DotNext 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.2.3">DotNext.Metaprogramming 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.reflection/1.2.3">DotNext.Reflection 1.2.3</a>
* Fixed potential NRE
* Fixed reflection of value type constructors
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.threading/1.2.3">DotNext.Threading 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.3">DotNext.Unsafe 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.3">DotNext.Net.Cluster 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.3">DotNext.AspNetCore.Cluster 1.2.3</a>
* Updated dependencies

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/1.2.3">DotNext.Augmentation.Fody 1.2.3</a>
* Updated dependencies

# 11-05-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.2">DotNext 1.2.2</a>
* Fixed bitwise equality
* Fixed `Intrinsics.IsDefault` method

# 11-02-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.1">DotNext 1.2.1</a>
* Fixed type modifier of `Current` property declared in [CopyOnWriteList&lt;T&gt;.Enumerator](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Collections.Concurrent.CopyOnWriteList-1.Enumerator.html)

# 10-31-2019
<a href="https://www.nuget.org/packages/dotnext/1.2.0">DotNext 1.2.0</a>
* Fixed memory leaks caused by methods in [StreamExtensions](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.IO.StreamExtensions.html) class
* [MemoryRental](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Buffers.MemoryRental-1.html) type is introduced to replace memory allocation with memory rental in some scenarios
* [ArrayRental](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Buffers.ArrayRental-1.html) type is extended
* Value Delegates now are protected from _dangling pointer_ issue caused by dynamic assembly loading 
* Reduced amount of memory utilized by random string generation methods
* Strict package versioning rules are added to avoid accidental upgrade to major version
* Improved performance of [AtomicEnum](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AtomicEnum.html) methods
* Improved performance of [Atomic&lt;T&gt;](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.Atomic-1.html) using optimistic read locks
* Fixed unnecessary boxing in atomic operations
* `Intrinsics.HasFlag` static generic method is added as boxing-free and fast alternative to [Enum.HasFlag](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.enum.hasflag?view=netcore-2.2#System_Enum_HasFlag_System_Enum_) method

<a href="https://www.nuget.org/packages/dotnext.reflection/1.2.0">DotNext.Reflection 1.2.0</a>
* Updated version of `DotNext` dependency to fix potential memory leaks
* Strict package versioning rules are added to avoid accidental upgrade to major version

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.2.0">DotNext.Metaprogramming 1.2.0</a>
* Updated version of `DotNext` dependency to fix potential memory leaks
* Strict package versioning rules are added to avoid accidental upgrade to major version

<a href="https://www.nuget.org/packages/dotnext.threading/1.2.0">DotNext.Threading 1.2.0</a>
* Updated version of `DotNext` dependency to fix potential memory leaks
* Strict package versioning rules are added to avoid accidental upgrade to major version
* [AsyncReaderWriterLock](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncReaderWriterLock.html) now supports optimistic reads

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.2.0">DotNext.Unsafe 1.2.0</a>
* [UnmanagedMemoryPool](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Buffers.UnmanagedMemoryPool-1.html) is added
* Strict package versioning rules are added to avoid accidental upgrade to major version

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.2.0">DotNext.Net.Cluster 1.2.0</a>
* Updated version of `DotNext` dependency to fix potential memory leaks
* Strict package versioning rules are added to avoid accidental upgrade to major version
* Fixed incorrect computation of partition in `PersistentState.DropAsync` method

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.2.0">DotNext.AspNetCore.Cluster 1.2.0</a>
* HTTP/2 support
* Performance optimizations caused by changes in `ArrayRental` type
* Strict package versioning rules are added to avoid accidental upgrade to major version

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/1.2.0">DotNext.Augmentation.Fody 1.2.0</a>
* Improved support of `ValueRefAction` and `ValueRefFunc` value delegates

# 10-12-2019
<a href="https://www.nuget.org/packages/dotnext/1.1.0">DotNext 1.1.0</a>
* Reduced number of inline IL code
* Updated version of FxCop analyzer
* [ReaderWriterSpinLock](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.ReaderWriterSpinLock.html) type is introduced
* Improved performance of [UserDataStorage](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.UserDataStorage.html)

<a href="https://www.nuget.org/packages/dotnext.reflection/1.1.0">DotNext.Reflection 1.1.0</a>
* Updated version of FxCop analyzer
* Improved performance of internal caches

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.1.0">DotNext.Metaprogramming 1.1.0</a>
* Updated version of FxCop analyzer
* `RefAnyValExpression` is added

<a href="https://www.nuget.org/packages/dotnext.threading/1.1.0">DotNext.Threading 1.1.0</a>
* Updated version of FxCop analyzer

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.1.0">DotNext.Unsafe 1.1.0</a>
* Updated version of FxCop analyzer

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.1.0">DotNext.Net.Cluster 1.1.0</a>
* Minor performance optimizations of persistent WAL
* Updated version of FxCop analyzer

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.1.0">DotNext.AspNetCore.Cluster 1.1.0</a>
* Updated version of FxCop analyzer

# 10-02-2019
<a href="https://www.nuget.org/packages/dotnext/1.0.1">DotNext 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.reflection/1.0.1">DotNext.Reflection 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.0.1">DotNext.Metaprogramming 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.threading/1.0.1">DotNext.Threading 1.0.1</a>
* Introduced [AsyncSharedLock](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncSharedLock.html) as combination of reader/write lock and semaphore
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.0.1">DotNext.Unsafe 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.0.1">DotNext.Net.Cluster 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.0.1">DotNext.AspNetCore.Cluster 1.0.1</a>
* Minor performance optimizations

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/1.0.1">DotNext.Augmentation.Fody 1.0.1</a>
* Code refactoring

# 10-02-2019
This is the major release of all parts of .NEXT library. Now the version is 1.0.0 and backward compatibility is guaranteed across all 1.x releases. The main motivation of this release is to produce stable API because .NEXT library active using in production code, especially Raft implementation.

.NEXT 1.x is based on .NET Standard 2.0 to keep compatibility with .NET Framework.

<a href="https://www.nuget.org/packages/dotnext/1.0.0">DotNext 1.0.0</a>
* Optimized methods of [Memory](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.InteropServices.Memory.html) class
* Extension methods for I/O are introduced. Now you don't need to instantiate [BinaryReader](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.io.binaryreader) or [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.io.binarywriter) for high-level parsing of stream content. Encoding and decoding of strings are fully supported. Moreover, these methods are asynchronous in contrast to methods of `BinaryReader` and `BinaryWriter`.

<a href="https://www.nuget.org/packages/dotnext.reflection/1.0.0">DotNext.Reflection 1.0.0</a>
* API is stabilized

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/1.0.0">DotNext.Metaprogramming 1.0.0</a>
* API is stabilized

<a href="https://www.nuget.org/packages/dotnext.threading/1.0.0">DotNext.Threading 1.0.0</a>
* [AsyncManualResetEvent](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncManualResetEvent.html) has auto-reset optional behavior which allows to repeatedly unblock many waiters

<a href="https://www.nuget.org/packages/dotnext.unsafe/1.0.0">DotNext.Unsafe 1.0.0</a>
* [MemoryMappedFileExtensions](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.IO.MemoryMappedFiles.MemoryMappedFileExtensions.html) allows to work with virtual memory associated with memory-mapped file using unsafe pointer or [Span&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.span-1) to achieve the best performance.

<a href="https://www.nuget.org/packages/dotnext.net.cluster/1.0.0">DotNext.Net.Cluster 1.0.0</a>
* Audit trail programming model is redesigned
* Persistent and high-performance Write Ahead Log (WAL) is introduced. Read more [here](https://dotnet.github.io/dotNext/features/cluster/aspnetcore.html#replication)
* Log compaction is supported

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/1.0.0">DotNext.AspNetCore.Cluster 1.0.0</a>
* Redirection to leader now uses `307 Temporary Redirect` instead of `302 Moved Temporarily` by default
* Compatibility with persistent WAL is provided

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/1.0.0">DotNext.Augmentation.Fody 1.0.0</a>
* Behavior of augmented compilation is stabilized

# 09-03-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.5.7">DotNext.AspNetCore.Cluster 0.5.7</a>
* Custom redirection logic can be asynchronous
* Fixed compatibility of redirection to leader with MVC

# 09-02-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.5.5">DotNext.AspNetCore.Cluster 0.5.5</a>
* Automatic redirection to leader now works correctly with reverse proxies
* Custom redirection logic is introduced

# 08-31-2019
<a href="https://www.nuget.org/packages/dotnext/0.14.0">DotNext 0.14.0</a>
* [Timestamp](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Diagnostics.Timestamp.html) type is introduced as allocation-free alternative to [Stopwatch](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.diagnostics.stopwatch)
* [Memory](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.InteropServices.Memory.html) class now have methods for reading and writing null-terminated UTF-16 string from/to unmanaged or pinned managed memory
* Updated InlineIL dependency to 1.3.1

<a href="https://www.nuget.org/packages/dotnext.threading/0.14.0">DotNext.Threading 0.14.0</a>
* [AsyncTimer](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncTimer.html) is completely rewritten in backward-incompatible way. Wait handle are no longer used.

<a href="https://www.nuget.org/packages/dotnext.unsafe/0.14.0">DotNext.Unsafe 0.14.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.reflection/0.14.0">DotNext.Reflection 0.14.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/0.14.0">DotNext.Metaprogramming 0.14.0</a>
* Small code fixes
* Updated `DotNext` dependency to 0.14.0
* Updated `Fody` dependency to 6.0.0
* Updated augmented compilation to 0.14.0

<a href="https://www.nuget.org/packages/dotnext.net.cluster/0.5.0">DotNext.Net.Cluster 0.5.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.5.0">DotNext.AspNetCore.Cluster 0.5.0</a>
* Measurement of runtime metrics are introduced and exposed through [MetricsCollector](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Net.Cluster.Consensus.Raft.MetricsCollector.html) and [HttpMetricsCollector](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Net.Cluster.Consensus.Raft.Http.HttpMetricsCollector.html) classes

<a href="https://www.nuget.org/packages/dotnext.augmentation.fody/0.14.0">DotNext.Augmentation.Fody 0.14.0</a>
* Updated `Fody` dependency to 6.0.0

# 08-28-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.4.0">DotNext.AspNetCore.Cluster 0.4.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.net.cluster/0.4.0">DotNext.Net.Cluster 0.4.0</a>
* Heartbeat timeout can be tuned through configuration
* Optimized Raft state machine

# 08-27-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.3.5">DotNext.AspNetCore.Cluster 0.3.5</a><br/>
* Docker support

# 08-22-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.3.3">DotNext.AspNetCore.Cluster 0.3.3</a><br/>
<a href="https://www.nuget.org/packages/dotnext.net.cluster/0.3.3">DotNext.Net.Cluster 0.3.3</a>
* Reduced number of logs produced by cluster node

# 08-21-2019
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.3.2">DotNext.AspNetCore.Cluster 0.3.2</a>
* Fixed endpoint redirection to leader node

<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.3.1">DotNext.AspNetCore.Cluster 0.3.1</a>
* Fixed detection of local IP address
* Improved IPv6 support

# 08-20-2019
<a href="https://www.nuget.org/packages/dotnext/0.13.0">DotNext 0.13.0</a>
* Fixed bug with equality comparison of **null** arrays inside of [EqualityComparerBuilder](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.EqualityComparerBuilder-1.html)
* Improved debugging experience:
	* SourceLink is enabled
	* Debug symbols now embedded into assembly file
	* NuGet Symbols Package is no longer used

<a href="https://www.nuget.org/packages/dotnext.threading/0.13.0">DotNext.Threading 0.13.0</a>
* Internals of several classes now based on Value Delegates to reduce memory allocations
* Improved debugging experience:
	* SourceLink is enabled
	* Debug symbols now embedded into assembly file
	* NuGet Symbols Package is no longer used

<a href="https://www.nuget.org/packages/dotnext.unsafe/0.13.0">DotNext.Unsafe 0.13.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.reflection/0.13.0">DotNext.Reflection 0.13.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.metaprogramming/0.13.0">DotNext.Metaprogramming 0.13.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.net.cluster/0.3.0">DotNext.Net.Cluster 0.3.0</a><br/>
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.3.0">DotNext.AspNetCore.Cluster 0.3.0</a>
* Improved debugging experience:
	* SourceLink is enabled
	* Debug symbols now embedded into assembly file
	* NuGet Symbols Package is no longer used

# 08-18-2019
<a href="https://www.nuget.org/packages/dotnext/0.12.0">DotNext 0.12.0</a>
* Value (struct) Delegates are introduced as allocation-free alternative to classic delegates
* Atomic&lt;T&gt; is added to provide atomic memory access operations for arbitrary value types
* Arithmetic, bitwise and comparison operations for [IntPtr](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.intptr) and [UIntPtr](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.uintptr)
* Improved performance of methods declared in [EnumConverter](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.EnumConverter.html)
* Improved performance of atomic operations
* Improved performance of bitwise equality and bitwise comparison methods for value types
* Improved performance of [IsDefault](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.Intrinsics.html#DotNext_Runtime_Intrinsics_IsDefault__1_) method which allows to check whether the arbitrary value of type `T` is `default(T)`
* GetUnderlyingType() method is added to obtain underlying type of Result&lt;T&gt;
* [TypedReference](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.typedreference) can be converted into managed pointer (type T&amp;, or ref T) using [Memory](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.InteropServices.Memory.html) class

This release introduces a new feature called Value Delegates which are allocation-free alternative to regular .NET delegates. Value Delegate is a value type which holds a pointer to the managed method and can be invoked using `Invoke` method in the same way as regular .NET delegate. Read more [here](https://dotnet.github.io/dotNext/features/core/valued.html).

`ValueType<T>` is no longer exist and most of its methods moved into [BitwiseComparer](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.BitwiseComparer-1.html) class.

<a href="https://www.nuget.org/packages/dotnext.reflection/0.12.0">DotNext.Reflection 0.12.0</a>
* Ability to obtain managed pointer (type T&amp;, or `ref T`) to static or instance field from [FieldInfo](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.reflection.fieldinfo) using [Reflector](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Reflection.Reflector.html) class

<a href="https://www.nuget.org/packages/dotnext.threading/0.12.0">DotNext.Threading 0.12.0</a>
* [AsyncLazy&lt;T&gt;](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Threading.AsyncLazy-1.html) is introduced as asynchronous alternative to [Lazy&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/versions/1.x/api/system.lazy-1) class

<a href="https://www.nuget.org/packages/dotnext.metaprogramming/0.12.0">DotNext.Metaprogramming 0.12.0</a>
* [Null-safe navigation expression](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Linq.Expressions.NullSafetyExpression.html) is introduced

<a href="https://www.nuget.org/packages/dotnext.unsafe/0.12.0">DotNext.Unsafe 0.12.0</a>
* [UnmanagedFunction](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.InteropServices.UnmanagedFunction.html) and [UnmanagedFunction&lt;R&gt;](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Runtime.InteropServices.UnmanagedFunction-1.html) classes are introduced to call unmanaged functions by pointer

<a href="https://www.nuget.org/packages/dotnext.net.cluster/0.2.0">DotNext.Net.Cluster 0.2.0</a>
<a href="https://www.nuget.org/packages/dotnext.aspnetcore.cluster/0.2.0">DotNext.AspNetCore.Cluster 0.2.0</a>
* Raft client is now capable to ensure that changes are committed by leader node using [WriteConcern](https://dotnet.github.io/dotNext/versions/1.x/api/DotNext.Net.Cluster.Replication.WriteConcern.html)