namespace Utilitary;

/// <summary> This class contains methods to convert from and to little endian.</summary>
public static class ConvertTo
{
    #region FromLittleEndian
    /// <summary> This method converts a bytes array to a <see cref="uint"/>. </summary>
    /// <param name="array"> The bytes array. </param>
    /// <param name="offset"> The starting position. </param>
    /// <returns> An <see cref="uint"/>. </returns>
    public static uint UInt(byte[] array, int offset = 0)
    {
        uint result = 0;
        for (int i = 0; i < 4; i++) 
            result += array[offset + i] * (uint)Math.Pow(256, i);
        return result;
    }
    /// <summary> This method converts a bytes array to an <see cref="int"/>. </summary>
    /// <param name="array"> The bytes array. </param>
    /// <param name="offset"> The starting position. </param>
    /// <returns> An <see cref="int"/>. </returns>
    public static int Int(byte[] array, int offset = 0)
    {
        int result = 0;
        for (int i = 0; i < 4; i++) 
            result += array[offset + i] * (int)Math.Pow(256, i);
        return result;
    }
    /// <summary> This method converts a bytes array to an <see cref="ushort"/>. </summary>
    /// <param name="array"> The bytes array. </param>
    /// <param name="offset"> The starting position. </param>
    /// <returns> An <see cref="ushort"/>. </returns>
    public static ushort UShort(byte[] array, int offset = 0)
    {
        ushort result = 0;
        for (int i = 0; i < 2; i++) 
            result += (ushort)(array[offset + i] * Math.Pow(256, i));
        return result;
    }
    #endregion

    #region ToLittleEndian
    /// <summary> This method converts an <see cref="int"/> to a <see cref="byte"/> array. </summary>
    /// <param name="value"> The value to convert. </param>
    /// <returns> A <see cref="byte"/>array. </returns>
    public static byte[] LittleEndian(int value)
    {
        byte[] result = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = (byte)(value % 256);
            value /= 256;
        }
        return result;
    }
    /// <summary> This method converts an <see cref="uint"/> to a <see cref="byte"/> array. </summary>
    /// <param name="value"> The uint to convert. </param>
    /// <returns> A <see cref="byte"/> array. </returns>
    public static byte[] LittleEndian(uint value)
    {
        byte[] result = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = (byte)(value % 256);
            value >>= 8; 
        }
        return result;
    }
    /// <summary> This method converts an <see cref="ushort"/> to a <see cref="byte"/> array. </summary>
    /// <param name="value"> The ushort to convert. </param>
    /// <returns> A <see cref="byte"/> array. </returns>
    public static byte[] LittleEndian(ushort value)
    {
        byte[] result = new byte[2];
        for (int i = 0; i < 2; i++)
        {
            result[i] = (byte)(value % 256);
            value /= 256;
        }
        return result;
    }
    #endregion
}