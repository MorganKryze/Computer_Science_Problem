using System;
using System.Collections.Generic;
using System.ComponentModel;

using Utilitary;

namespace Computer_Science_Problem;

/// <summary> This class represents a convolution. </summary>
public static class Convolution
{
    #region Enumerations
    /// <summary> Enumerates the different edge processing. </summary>
    public enum EdgeProcessing
    {
        /// <summary> Extends the edges. </summary>
        Extend, 
        /// <summary> Wraps the edges. </summary>
        Wrap, 
        /// <summary> Mirrors the edges. </summary>
        Mirror, 
        /// <summary> Crops the edges. </summary>
        Crop, 
        /// <summary> Crops the kernel. </summary>
        KernelCrop
    }
    /// <summary> Enumerates the different kernel origins. </summary>
    public enum KernelOrigin
    {
        /// <summary> The center. </summary>
        Center, 
        /// <summary> The top left. </summary>
        TopLeft
    }
    
    /// <summary> Enumerates the different kernels. </summary>
    public enum Kernel
    {
        /// <summary> Detects the edges. </summary>
        EdgeDetection,
        
        /// <summary> Sharpen the image. </summary>
        Sharpen,
        /// <summary> Emboss the image. </summary>
        BoxBlur,
        /// <summary> Better emboss the image. </summary>
        GaussianBlur3x3, 
        /// <summary> Contrasts the image. </summary>
        Contrast,
        /// <summary> Pushes the edges. </summary>
        EdgePushing
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
            Kernel.Sharpen, new float[,]
            {
                {  0, -1,  0 },
                { -1,  5, -1 },
                {  0, -1,  0 }
            }
        },
        {
            Kernel.BoxBlur, new float[,]
            {
                { 1/9f, 1/9f, 1/9f },
                { 1/9f, 1/9f, 1/9f },
                { 1/9f, 1/9f, 1/9f }
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
    /// <param name="origin"> The origin of the <see cref="Kernel"/>. </param>
    /// <param name="edgeProcessing"> The way the edges of the <see cref="Image"/> will be processed. </param>
    /// <returns> An <see cref ="Image"/> where a kernel has been applied. </returns>
    public static Image ApplyKernelgeneral(this Image image, Kernel kernel, KernelOrigin origin = KernelOrigin.Center, EdgeProcessing edgeProcessing = EdgeProcessing.KernelCrop)
    {
        return ApplyKernel(image, kernels[kernel], origin, edgeProcessing);
    }
    ///<summary> This method applies a <see cref="Kernel"/> to an <see cref="Image"/>. </summary>
    /// <param name="image"> The <see cref="Image"/> on which the kernel will be applied. </param>
    /// <param name="kernel"> The <see cref="Kernel"/> to apply. </param>
    /// <param name="origin"> The origin of the <see cref="Kernel"/>. </param>
    /// <param name="edgeProcessing"> The way the edges of the <see cref="Image"/> will be processed. </param>
    /// <returns> An <see cref ="Image"/> where a kernel has been applied. </returns>
    public static Image ApplyKernel(this Image image, float[,] kernel, KernelOrigin origin = KernelOrigin.Center, EdgeProcessing edgeProcessing = EdgeProcessing.KernelCrop)
    {
        if (kernel is null) 
            throw new ArgumentException("kernel", "The kernel cannot be null.");
            
        int kernelHeight = kernel.GetLength(0);
        int kernelWidth = kernel.GetLength(1);

        int imageHeight = image.Height;
        int imageWidth = image.Width;

        if(origin == KernelOrigin.Center && (kernelHeight % 2 != 1 || kernelWidth % 2 != 1)) 
            origin = KernelOrigin.TopLeft;

        int halfKerH = kernelHeight / 2;
        int halfKerW = kernelWidth / 2;

        int resultH = imageHeight;
        int resultW = imageWidth;

        if (edgeProcessing is EdgeProcessing.Crop)
        {
            resultH -= kernelHeight - 1;
            resultW -= kernelWidth - 1;
        }

        if (resultH <= 0 || resultW <= 0) 
            throw new ArithmeticException("The size cannot be negative.");
            
        Image result = new Image(resultW, resultH);

        for (int y = 0; y < resultH; y++)
        {
            for (int x = 0; x < resultW; x++)
            {
                int acc_r = 0;
                int acc_g = 0;
                int acc_b = 0;

                for (int dy = 0; dy < kernelHeight; dy++)
                {
                    for (int dx = 0; dx < kernelWidth; dx++)
                    {
                        int pos_dy = y + dy, pos_dx = x + dx;
                        if (origin is KernelOrigin.Center && edgeProcessing is not EdgeProcessing.Crop)
                        {
                            pos_dy -= halfKerH;
                            pos_dx -= halfKerW;
                        }
                        if (pos_dy < 0 || pos_dy >= imageHeight)
                        {
                            switch (edgeProcessing)
                            {
                                case EdgeProcessing.Extend:
                                    if (pos_dy < 0) 
                                        pos_dy = 0;
                                    else 
                                        pos_dy = imageHeight - 1;
                                    break;
                                case EdgeProcessing.KernelCrop:
                                    continue; 
                                case EdgeProcessing.Mirror:
                                    if (pos_dy < 0) 
                                        pos_dy = -pos_dy;
                                    else 
                                        pos_dy = imageHeight - (pos_dy % imageHeight);
                                    break;
                                case EdgeProcessing.Wrap:
                                    while (pos_dy < 0) 
                                        pos_dy += imageHeight;
                                    pos_dy %= imageHeight;
                                    break;
                            }
                        }
                        if (pos_dx < 0 || pos_dx >= imageWidth)
                        {
                            switch (edgeProcessing)
                            {
                                case EdgeProcessing.Extend:
                                    if (pos_dx < 0) 
                                        pos_dx = 0;
                                    else 
                                        pos_dx = imageWidth - 1;
                                    break;
                                case EdgeProcessing.KernelCrop:
                                    continue; 
                                case EdgeProcessing.Mirror:
                                    if (pos_dx < 0) 
                                        pos_dx = -pos_dx;
                                    else 
                                        pos_dx = imageWidth - (pos_dx % imageWidth);
                                    break;
                                case EdgeProcessing.Wrap:
                                    while (pos_dx < 0) 
                                        pos_dx += imageWidth;
                                    pos_dx %= imageWidth;
                                    break;
                            }
                        }
                        float factor = kernel[dy, dx];
                        Pixel pixel = image[pos_dx, pos_dy];

                        acc_r += (int)(factor * pixel.R);
                        acc_g += (int)(factor * pixel.G);
                        acc_b += (int)(factor * pixel.B);
                    }
                }
                acc_r = Math.Min(255, Math.Max(0, acc_r));
                acc_g = Math.Min(255, Math.Max(0, acc_g));
                acc_b = Math.Min(255, Math.Max(0, acc_b));

                result[x, y] = new Pixel((byte)acc_r, (byte)acc_g, (byte)acc_b);
            }
        }
        return result;
    }
    #endregion
}

