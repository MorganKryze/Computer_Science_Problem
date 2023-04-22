using Xunit;
using Instances;

namespace Tests
{
    /// <summary> This is the test class for the project. </summary>
    public class XUnitTests
    {
        #region Complex
        /// <summary> This test checks if two complex numbers are different within different values. </summary>
        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 4)]
        public void Identity(double re, double im)
        {
            var c1 = new Complex(re, im);
            var c2 = new Complex(5, 6);
            Assert.True(c1.Equals(c2) == false);
        }
        /// <summary> This test checks if the addition of two complex numbers is correct. </summary>
        [Theory]
        [InlineData(1, 2, 3, 4)]
        [InlineData(5, 6, 7, 8)]
        public void Addition(double re1, double im1, double re2, double im2)
        {
            var c1 = new Complex(re1, im1);
            var c2 = new Complex(re2, im2);
            var c3 = c1 + c2;
            Assert.True(c3.Equals(new Complex(re1 + re2, im1 + im2)));
        }
        /// <summary> This test checks if the multiplication of two complex numbers is correct. </summary>
        [Theory]
        [InlineData(1, 2, 3, 4)]
        [InlineData(5, 6, 7, 8)]
        public void Multiplication(double re1, double im1, double re2, double im2)
        {
            var c1 = new Complex(re1, im1);
            var c2 = new Complex(re2, im2);
            var c3 = c1 * c2;
            Assert.True(c3.Equals(new Complex(re1 * re2 - im1 * im2, re1 * im2 + im1 * re2)));
        }
        /// <summary> This test checks if the modulus of a complex number is correct. </summary>
        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 4)]
        public void Modulus(double re, double im)
        {
            var c1 = new Complex(re, im);
            Assert.True(c1.Modulus() == (double)Math.Sqrt(re * re + im * im));
        }
        /// <summary> This test checks if the complex number is correctly converted to a string. </summary>
        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 4)]
        public void ComplexToString(double re, double im)
        {
            var c1 = new Complex(re, im);
            Assert.True(c1.ToString() == $"{re} + {im}i");
        }
        #endregion

        #region Bitmap
        /// <summary> This test checks if the bitmap is correctly created. </summary>
        [Theory]
        [InlineData(200, 200)]
        [InlineData(450, 140)]
        public void BitmapCreation(int width, int height)
        {
            var bitmap = new PictureBitMap(width, height);
            Assert.True(bitmap.GetLength(0) == width && bitmap.GetLength(1) == height);
        }
        /// <summary> This test checks if the bitmap is correctly filled with pixels. </summary>
        [Fact]
        public void BitmapFill()
        {
            var bitmap = new PictureBitMap(200, 200);
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                    bitmap[i, j] = new Pixel(255, 255, 255);
            Assert.True(bitmap[0, 0].Equals(new Pixel(255, 255, 255)));
        }
        /// <summary> This test checks if the bitmap is correctly put in a grey scale. </summary>
        [Fact]
        public void BitmapGrey()
        {
            var bitmap = new PictureBitMap(200, 200);
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                    bitmap[i, j] = new Pixel(255, 255, 255);
            bitmap.AlterColors(PictureBitMap.Transformation.Grey);
            Assert.True(bitmap[0, 0].Equals(new Pixel(255, 255, 255)));
        }
        #endregion
    }
}