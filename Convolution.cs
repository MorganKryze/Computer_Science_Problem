using Utilitary;

namespace Computer_Science_Problem;

/// <summary> This class represents a convolution. </summary>
public static class Convolution
{
    #region Kernel Enumeration
    /// <summary> Enumerates the different kernels. </summary>
    public enum Kernel
    {
        /// <summary> Detects the edges. </summary>
        EdgeDetection,
        /// <summary> Sharpen the image. </summary>
        Sharpen,
        /// <summary> Better emboss the image. </summary>
        GaussianBlur3x3, 
        /// <summary> Contrasts the image. </summary>
        Contrast,
        /// <summary> Pushes the edges. </summary>
        EdgePushing
    }
    #endregion
    
    #region Dictionary Kernel to Matrix
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
            Kernel.Sharpen, new float[,]
            {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            }
        },
        {
            Kernel.GaussianBlur3x3, new float[,]
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
        },
        {
            Kernel.EdgePushing, new float[,]
            {
                { -2, -1,  0 },
                { -1,  1,  1 },
                {  0,  1,  2 }
            }
        }
    };
    #endregion

    #region Methods
    ///<summary> This method applies a <see cref="Kernel"/> to an <see cref="Image"/>. </summary>
    /// <param name="image"> The <see cref="Image"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Kernel"/> to apply. </param>
    /// <returns> An <see cref ="Image"/> where a kernel has been applied. </returns>
    public static Image ApplyKernelByName(this Image image, Kernel kernel)
    {
        return ApplyKernelByMatrix(image, kernels[kernel]);
    }
    ///<summary> This method applies a convolution matrix to an <see cref="Image"/>. </summary>
    /// <param name="image"> The <see cref="Image"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Kernel"/> to apply. </param>
    /// <returns> An <see cref ="Image"/> where a kernel has been applied. </returns>
    public static Image ApplyKernelByMatrix(this Image image, float[,] kernel)
    {
        if (kernel is null) 
            throw new ArgumentException("kernel", "The kernel cannot be null.");
            
        int kernelHeight = kernel.GetLength(0);
        int kernelWidth = kernel.GetLength(1);

        int imageHeight = image.Height;
        int imageWidth = image.Width;

        int halfKerH = kernelHeight / 2;
        int halfKerW = kernelWidth / 2;

        int resultH = imageHeight;
        int resultW = imageWidth;

        if (resultH <= 0 || resultW <= 0) 
            throw new ArithmeticException("The size cannot be negative.");
            
        Image newImage = new Image(resultW, resultH);

        for (int y = 0; y < resultH; y++)
        {
            for (int x = 0; x < resultW; x++)
            {
                int redFinal = 0;
                int greenFinal = 0;
                int blueFinal = 0;

                for (int dy = 0; dy < kernelHeight; dy++)
                {
                    for (int dx = 0; dx < kernelWidth; dx++)
                    {
                        int position_dy = y + dy;
                        int position_dx = x + dx;

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

                newImage[x, y] = new Pixel((byte)redFinal, (byte)greenFinal, (byte)blueFinal);
            }
        }
        return newImage;
    }
    #endregion
}

