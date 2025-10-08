using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RAG.Infrastructure.Data
{
    public class VectorSerializer
    {
        public static byte[] ToBytes(float[] vector)
        {
            if (vector == null || vector.Length == 0) return Array.Empty<byte>();
            var bytes = new byte[vector.Length * sizeof(float)];
            MemoryMarshal.Cast<byte, float>(bytes.AsSpan()).Clear();
            MemoryMarshal.Cast<float, byte>(vector.AsSpan()).CopyTo(bytes);
            return bytes;
        }

        public static float[] FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<float>();
            var span = MemoryMarshal.Cast<byte, float>(bytes.AsSpan());
            return span.ToArray();
        }
    }
}
