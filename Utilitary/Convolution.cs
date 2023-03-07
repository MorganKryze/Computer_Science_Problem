using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Computer_Science_Problem;

/// <summary> This class represents a convolution. </summary>
public static class Convolution
{
    private static Dictionary<Kernel, float[,]> kernels = new Dictionary<Kernel, float[,]>
        {
            {
                Kernel.EdgeDetection1, new float[,]
                {
                    {  1,  0, -1 },
                    {  0,  0,  0 },
                    { -1, 0 ,  1 }
                }
            },
            {
                Kernel.EdgeDetection2, new float[,]
                {
                    {  0,  1,  0 },
                    {  1, -4,  1 },
                    {  0,  1,  0 }
                }
            },
            {
                Kernel.EdgeDetection3, new float[,]
                {
                     { -1, -1, -1 },
                     { -1,  8, -1 },
                     { -1, -1, -1 }
                }
            }

        };

    /// <summary>
    /// Applique une convolution à une instance d'<see cref="Image"/> donnée.
    /// </summary>
    /// <param name="image"><see cref="Image"/> sur laquelle va être appliquée le noyau.</param>
    /// <param name="kernel">Noyau à appliquer.</param>
    /// <param name="origin">Origine du noyau.</param>
    /// <param name="edgeProcessing">Gestion des bords de l'image.</param>
    /// <returns>An <see cref ="Image"/> where a kernel has been applied. </returns>
    public static Image ApplyKernel(this Image image, Kernel kernel, KernelOrigin origin = KernelOrigin.Center, EdgeProcessing edgeProcessing = EdgeProcessing.KernelCrop)
    {
        return ApplyKernel(image, kernels[kernel], origin, edgeProcessing);
    }

    /// <summary>
    /// Applique une convolution à une instance d'ImagePSI donnée.
    /// </summary>
    /// <param name="image"><see cref="Image"/> sur laquelle va être appliquée le noyau.</param>
    /// <param name="kernel">Noyau à appliquer.</param>
    /// <param name="origin">Origine du Kernel.</param>
    /// <param name="edgeProcessing">Gestion des bords de l'image.</param>
    /// <returns></returns>
    public static Image ApplyKernel(this Image image, float[,] kernel, KernelOrigin origin = KernelOrigin.Center, EdgeProcessing edgeProcessing = EdgeProcessing.KernelCrop)
    {
        if (kernel is null) throw new ArgumentException("kernel", "kernel cannot be null!");
            
        int kernelHeight = kernel.GetLength(0);
        int kernelWidth = kernel.GetLength(1);

        int imageHeight = image.Height;
        int imageWidth = image.Width;

        if(origin == KernelOrigin.Center && (kernelHeight % 2 != 1 || kernelWidth % 2 != 1)) origin = KernelOrigin.TopLeft;

        int halfKerH = kernelHeight / 2;
        int halfKerW = kernelWidth / 2;

        int resultH = imageHeight;
        int resultW = imageWidth;
        if (edgeProcessing == EdgeProcessing.Crop)
        {
            resultH -= kernelHeight - 1;
            resultW -= kernelWidth - 1;
        }

        if (resultH <= 0 || resultW <= 0) throw new ArithmeticException("result of negative size!");
            
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
                        if (origin == KernelOrigin.Center && edgeProcessing != EdgeProcessing.Crop)
                        {
                            pos_dy -= halfKerH;
                            pos_dx -= halfKerW;
                        }
                        if (pos_dy < 0 || pos_dy >= imageHeight)
                        {
                            switch (edgeProcessing)
                            {
                                case EdgeProcessing.Extend:
                                    if (pos_dy < 0) pos_dy = 0;
                                    else pos_dy = imageHeight - 1;
                                    break;
                                case EdgeProcessing.KernelCrop:
                                    continue; 
                                case EdgeProcessing.Mirror:
                                    if (pos_dy < 0) pos_dy = -pos_dy;
                                    else pos_dy = imageHeight - (pos_dy % imageHeight);
                                    break;
                                case EdgeProcessing.Wrap:
                                    while (pos_dy < 0) pos_dy += imageHeight;
                                    pos_dy %= imageHeight;
                                    break;
                            }
                        }
                        if (pos_dx < 0 || pos_dx >= imageWidth)
                        {
                            switch (edgeProcessing)
                            {
                                case EdgeProcessing.Extend:
                                    if (pos_dx < 0) pos_dx = 0;
                                    else pos_dx = imageWidth - 1;
                                    break;
                                case EdgeProcessing.KernelCrop:
                                    continue; 
                                case EdgeProcessing.Mirror:
                                    if (pos_dx < 0) pos_dx = -pos_dx;
                                    else pos_dx = imageWidth - (pos_dx % imageWidth);
                                    break;
                                case EdgeProcessing.Wrap:
                                    while (pos_dx < 0) pos_dx += imageWidth;
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
        /// <summary> First edge detection kernel. </summary>
        [Description("Edge detection 1")]
        EdgeDetection1,
        /// <summary> Second edge detection kernel. </summary>
        [Description("Edge detection 2")]
        EdgeDetection2,
        /// <summary> Third edge detection kernel. </summary>
        [Description("Edge detection 3")]
        EdgeDetection3
    }
}

