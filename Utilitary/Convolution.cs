using Computer_Science_Problem;

namespace Utilitary;

/// <summary> This class represents a convolution. </summary>
public static class Convolution
{
    #region Kernels
    /// <summary> Enumerates the different kernels. </summary>
    public enum Kernel
    {
        /// <summary> Detects the edges. </summary>
        EdgeDetection,
        /// <summary> Pushes the edges. </summary>
        EdgePushing,
        /// <summary> Sharpen the image. </summary>
        Sharpen,
        /// <summary> Better emboss the image. </summary>
        GaussianBlur, 
        /// <summary> Contrasts the image. </summary>
        Contrast,
    }

    #endregion
    
    #region Dictionary
    private static Dictionary<Kernel, float[,]> kernels = new Dictionary<Kernel, float[,]>
    {
        {
            Kernel.EdgeDetection, new float[,]
            {
                {  0,  1,  0 },
                {  1, -4,  1 },
                {  0,  1,  0 }
            }
        },
        {
            Kernel.EdgePushing, new float[,]
            {
                { -2, -1,  0 },
                { -1,  1,  1 },
                {  0,  1,  2 }
            }
        },
        {
            Kernel.Sharpen, new float[,]
            {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            }
        },
        {
            Kernel.GaussianBlur, new float[,]
            {
                { 1/16f, 2/16f, 1/16f },
                { 2/16f, 4/16f, 2/16f },
                { 1/16f, 2/16f, 1/16f }
            }
        },
        {
            Kernel.Contrast, new float[,]
            {
                {  0,  -1,  0 },
                { -1,   5, -1 },
                {  0,  -1,  0 }
            }
        }
    };
    #endregion

    #region Extension methods for Image
    ///<summary> This method applies a <see cref="Kernel"/> to an <see cref="Picture"/>. </summary>
    /// <param name="image"> The <see cref="Picture"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Kernel"/> to apply. </param>
    /// <returns> An <see cref ="Picture"/> where a kernel has been applied. </returns>
    public static Picture ApplyKernelByName(this Picture image, Kernel kernel)
    {
        return ApplyKernel(image, kernels[kernel]);
    }
    ///<summary> This method applies a <see cref="Kernel"/> to an <see cref="Picture"/>. </summary>
    /// <param name="image"> The <see cref="Picture"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Kernel"/> to apply. </param>
    /// <returns> An <see cref ="Picture"/> where a kernel has been applied. </returns>
    public static Picture ApplyKernel(this Picture image, float[,] kernel)
    {
        Picture newImage = new (image.Width, image.Height);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
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

                        if (position_dy < 0 || position_dy >= image.Height)
                            continue;
                        if (position_dx < 0 || position_dx >= image.Width)
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