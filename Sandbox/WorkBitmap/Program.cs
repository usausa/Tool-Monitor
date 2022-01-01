namespace WorkBitmap;

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Program
{
    public static void Main()
    {
        {
            using var map = new ValueBitMap(stackalloc byte[4]);

            map.Set(1);
            map.Set(3);
            map.Set(6);
            map.Set(10);
            map.Set(15);
            map.Set(21);
            map.Set(28);

            for (var i = 0; i < 32; i++)
            {
                Debug.WriteLine($"{i} : {map.Get(i)}");
            }
        }

    }
}

public ref struct ValueBitMap
{
    private readonly byte[]? arrayReturnToPool;

    private readonly Span<byte> buffer;

    public ValueBitMap(Span<byte> buffer)
    {
        arrayReturnToPool = null;
        this.buffer = buffer;
    }

    public ValueBitMap(int capacity)
    {
        arrayReturnToPool = ArrayPool<byte>.Shared.Rent(capacity);
        buffer = arrayReturnToPool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = arrayReturnToPool;
        this = default;
        if (toReturn != null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index)
    {
        var i = index / 8;
        buffer[i] = (byte)(buffer[i] | (1 << (index % 8)));
    }

    public bool Get(int index)
    {
        var i = index / 8;
        return (buffer[i] & (1 << (index % 8))) > 0;
    }
}
