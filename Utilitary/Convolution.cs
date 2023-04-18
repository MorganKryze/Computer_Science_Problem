using Instances;

using static Instances.PictureBitMap;

namespace Utilitary;

/// <summary> This class represents a convolution. </summary>
public static class Convolution
{
    
    #region Dictionary
    private static Dictionary<Transformation, float[,]> kernels = new Dictionary<Transformation, float[,]>
    {
        {
            Transformation.EdgeDetection, new float[,]
            {
                {  0,  1,  0 },
                {  1, -4,  1 },
                {  0,  1,  0 }
            }
        },
        {
            Transformation.EdgePushing, new float[,]
            {
                { -2, -1,  0 },
                { -1,  1,  1 },
                {  0,  1,  2 }
            }
        },
        {
            Transformation.Sharpen, new float[,]
            {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            }
        },
        {
            Transformation.GaussianBlur, new float[,]
            {
                { 1/16f, 2/16f, 1/16f },
                { 2/16f, 4/16f, 2/16f },
                { 1/16f, 2/16f, 1/16f }
            }
        },
        {
            Transformation.Contrast, new float[,]
            {
                {  0,  -1,  0 },
                { -1,   5, -1 },
                {  0,  -1,  0 }
            }
        }
    };
    #endregion

    #region Extension methods for Image
    ///<summary> This method applies a <see cref="Transformation"/> to an <see cref="PictureBitMap"/>. </summary>
    /// <param name="image"> The <see cref="PictureBitMap"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Transformation"/> to apply. </param>
    /// <returns> An <see cref ="PictureBitMap"/> where a kernel has been applied. </returns>
    public static PictureBitMap ApplyKernelByName(this PictureBitMap image, Transformation kernel)
    {
        return ApplyKernel(image, kernels[kernel]);
    }
    ///<summary> This method applies a <see cref="Transformation"/> to an <see cref="PictureBitMap"/>. </summary>
    /// <param name="image"> The <see cref="PictureBitMap"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Transformation"/> to apply. </param>
    /// <returns> An <see cref ="PictureBitMap"/> where a kernel has been applied. </returns>
    public static PictureBitMap ApplyKernel(this PictureBitMap image, float[,] kernel)
    {
        PictureBitMap newImage = new (image.GetLength(0), image.GetLength(1));

        for (int y = 0; y < image.GetLength(1); y++)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                int redFinal = 0;
                int greenFinal = 0;
                int blueFinal = 0;

                for (int dy = 0; dy < kernel.GetLength(0); dy++)
                {
                    for (int dx = 0; dx < kernel.GetLength(1); dx++)
                    {
                        int position_dy = y + dy;
                        int position_dx = x + dx;

                        position_dy -= kernel.GetLength(0) / 2;
                        position_dx -= kernel.GetLength(1) / 2;

                        if (position_dy < 0 || position_dy >= image.GetLength(1))
                            continue;
                        if (position_dx < 0 || position_dx >= image.GetLength(0))
                            continue;

                        float factor = kernel[dy, dx];
                        Pixel pixel = image[position_dx, position_dy];

                        redFinal += (int)(factor * pixel.Red);
                        greenFinal += (int)(factor * pixel.Green);
                        blueFinal += (int)(factor * pixel.Blue);
                    }
                }
                redFinal = Math.Min(255, Math.Max(0, redFinal));
                greenFinal = Math.Min(255, Math.Max(0, greenFinal));
                blueFinal = Math.Min(255, Math.Max(0, blueFinal));

                newImage[x, y] = new ((byte)redFinal, (byte)greenFinal, (byte)blueFinal);
            }
        }
        return newImage;
    }
    #endregion
}