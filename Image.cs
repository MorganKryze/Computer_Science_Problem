using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Computer_Science_Problem
{
    /// <summary> Représente une image de profondeur 24-bits (composantes R, G et B). </summary>
    public class Image : IEquatable<Image>
    {
        #region Constants
        /// <summary> Represents the file type (bmp in this case). </summary>
        public const int offType = 0x00;            
        /// <summary> Represents the total file size (in bytes). </summary>
        public const int offFileSize = 0x02;        
        /// <summary> Position of the first pixel in the file (in bytes). </summary>
        public const int offStartOffset = 0x0a; 
        /// <summary> Represents the size of the second part of the header </summary>    
        public const int offInfoHeadsize = 0x0e;  
        /// <summary> Represents the image width (in pixels). </summary>
        public const int offWidth = 0x12;           
        /// <summary> Represents the image height (in pixels). </summary>
        public const int offHeight = 0x16;          
        /// <summary> Represents the number of color planes (always equals to 1 for a bmp). </summary>
        public const int offColorplanes = 0x1a;     
        /// <summary> Represents the number of bits per pixel (often 24 because 3 bytes * 8 bits = 24). </summary>
        public const int offColorDepth = 0x1c;      
        #endregion

        #region Fields
        ///<summary> Creation of the array of bytes of the header.</summary>
        public byte[] Header;   
        ///<summary> Creation of the array of bytes of the image.</summary>                            
        public byte[] Pixels;                               
        #endregion
        
        #region Properties
        /// <summary> Encoding.ASCII.GetString is a method which turn a bytes array into a string </summary>
        public string Type => Encoding.ASCII.GetString(Header.ExtractBytes(2, offType));
        /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the file size contained in a bytes array into an unsigned integer </summary>
        public uint FileSize => Utils.LittleEndianToUInt(Header, offFileSize);
        /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the start offset contained in a bytes array into an unsigned integer </summary>
        public uint StartOffset => Utils.LittleEndianToUInt(Header, offStartOffset);
        /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the info header size contained in a bytes array into an unsigned integer </summary>
        public uint InfoHeaderSize => Utils.LittleEndianToUInt(Header, offInfoHeadsize);
        /// <summary> Utils.LittleEndianToInt is a method which turn the information of the width contained in a bytes array into an integer </summary>
        public int Width => Utils.LittleEndianToInt(Header, offWidth);
        /// <summary> Utils.LittleEndianToInt is a method which turn the information of the height contained in a bytes array into an integer </summary>
        public int Height => Utils.LittleEndianToInt(Header, offHeight);
        /// <summary> Utils.LittleEndianToUShort is a method which turn the information of the color planes contained in a bytes array into an unsigned short </summary>
        public ushort ColorPlanes => Utils.LittleEndianToUShort(Header, offColorplanes);
        /// <summary> Utils.LittleEndianToUShort is a method which turn the information of the color depth contained in a bytes array into an unsigned short </summary>
        public ushort ColorDepth => Utils.LittleEndianToUShort(Header, offColorDepth);
        public int Stride => (Width * ColorDepth / 8 + 3) / 4 * 4; // by dividing then multiplying, we floor to the nearest smallest integer, nombre d'octets pour décrire une ligne de pixels
        #endregion
        
        #region Constructors
        /// <summary> Creates an<see cref="Image"/>instance  from a referenced file by <paramref name="filename"/>. </summary>
        /// <param name="filename"> Image path to open </param>
        public Image(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                Header = stream.ReadBytes(54);
                if (Type is not "BM") throw new FormatException("Invalid file type. We can only load BMP files.");
                if (ColorDepth is not 24) throw new FormatException("We cannot load images with a depth of 24 bits.");
                Pixels = stream.ReadBytes((int)(FileSize - StartOffset));
            }
            if(Header is null) throw new ArgumentNullException(nameof(Header));
            if(Pixels is null) throw new ArgumentNullException(nameof(Pixels));
        }

        /// <summary> Creates an<see cref="Image"/>instance from an height <paramref name="height"/> and a width <paramref name="width"/>. <br/>The image is automatically filled in black (components set to 0).</summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public Image(int width, int height)
        {
            Header = new byte[54];

            Header.InsertBytes(Encoding.ASCII.GetBytes("BM"), offType);      // Encoding.ASCII.GetBytes est une méthode permettant de transformer un string en tableau octets
            Header.InsertBytes(Utils.UIntToLittleEndian((uint)Header.Length), offStartOffset);

            Header.InsertBytes(Utils.UShortToLittleEndian(0x28), offInfoHeadsize); // hexadécimal car adresse
            Header.InsertBytes(Utils.IntToLittleEndian(width), offWidth);
            Header.InsertBytes(Utils.IntToLittleEndian(height), offHeight);
            Header.InsertBytes(Utils.UShortToLittleEndian(1), offColorplanes);
            Header.InsertBytes(Utils.UShortToLittleEndian(24), offColorDepth);

            Pixels = new byte[height * Stride];

            Header.InsertBytes(Utils.UIntToLittleEndian((uint)(Header.Length + Pixels.Length)), offFileSize);
        }
        /// <summary> Creates a copy of an <see cref="Image"/> instance . </summary>
        /// <param name="original"><see cref="Image"/>to copy.</param>
        public Image(Image original)
        {
            Header = new byte[original.Header.Length];
            Array.Copy(original.Header, Header, Header.Length);        // Array.Copy acts as a for loop to copy the array

            Pixels = new byte[original.Pixels.Length];
            Array.Copy(original.Pixels, Pixels, Pixels.Length);

        }
        #endregion

        #region Transformations
        /// <summary>Transform this <see cref="Image"/> to shades of grey (this method generates a copy).</summary>
        /// <returns> A greyscale copy of this <see cref="Image"/>.</returns>
        public Image Greyscale()
        {
            Image result = this.Copy();

            for (int x = 0; x < 800; x++)
            {
                for (int y = 0; y < 600; y++)
                {
                    result[x, y] = this[x, y].Greyscale();
                }
            }
            return result;
        }
        /// <summary> Transform this <see cref="Image"/> to black and white (this method generate a copy). </summary>
        /// <returns> A black and white copy of this <see cref="Image"/>.</returns>
        public Image BlackAndWhite()
        {
            Image result = this.Copy();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    result[x, y] = this[x, y].Greyscale().R > 127 ? new Pixel(255) : new Pixel(0);
                }
            }

            return result;
        }


        /// <summary> Rotates the <see cref="Image"/> instance at an <paramref name="angle"/>.</summary>
        /// <param name="angle">Angle of the rotation (in degrees)</param>
        /// <returns>A copy of this <see cref="Image"/> rotated by <paramref name="angle"/> degrees.</returns>
        public Image Rotate(int angle)
        {
            double rad = angle * (double)Math.PI / 180;
            double cos = (double)Math.Cos(rad);
            double sin = (double)Math.Sin(rad);

            int newWidth = (int)(Width * Math.Abs(cos) + Height * Math.Abs(sin));
            int newHeight = (int)(Width * Math.Abs(sin) + Height * Math.Abs(cos));

            Image result = new Image(newWidth, newHeight);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    double newX = (x - newWidth / 2) * cos - (y - newHeight / 2) * sin + Width / 2;
                    double newY = (x - newWidth / 2) * sin + (y - newHeight / 2) * cos + Height / 2;

                    if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
                    {
                        result[x, y] = this[(int)newX, (int)newY];
                    }
                }
            }

            return result;
        }

        public Image Scale(float scale, bool reduceAntiAliasing = true)
        {
            if (scale == 0)
                throw new ArgumentOutOfRangeException("scale", "scale must not be 0");

            if (scale < 0)
                throw new ArgumentOutOfRangeException("scale", "scale must be a positive number");

            Image source = this.Copy();

            if (scale == 1)
                return source;

            int newWidth = (int)(Width * scale);
            int newHeight = (int)(Height * scale);

            if (newWidth == 0)
                newWidth = 1;

            if (newHeight == 0)
                newHeight = 1;

            if(scale < 1 && reduceAntiAliasing)
            {
                

                int convolSize = (int)Math.Ceiling(1 / scale);
                float[,] kernel = new float[convolSize, convolSize];
                float coef = 1f / kernel.Length;    

                for(int y = 0; y < convolSize; y++)
                {
                    for(int x = 0; x < convolSize; x++)
                    {
                        kernel[y, x] = coef;
                    }
                }

                source = source.ApplyKernel(kernel, Convolution.KernelOrigin.TopLeft, Convolution.EdgeProcessing.Extend);
            }

            Image result = new Image(newWidth, newHeight);

            for (int x = 0; x < newWidth; x++)
            {
                for(int y = 0; y < newHeight; y++)
                {
                    result[x, y] = new Pixel(source[(int)(x / scale), (int)(y / scale)]);     
                }
            }

            return result;
        }
        #endregion
        
        #region Operators
        /// <summary>
        /// Vérifie l'égalité entre 2 <see cref="Image"/> (taille et <see cref="Pixel"/>s identiques).
        /// </summary>
        /// <seealso cref="Equals(object)"/>
        public static bool operator ==(Image a, Image b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            if (a.Width != b.Width || a.Height != b.Height)
                return false;

            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    if (a[x, y] != b[x, y])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Vérifie l'inégalité entre 2 <see cref="Image"/> (taille ou <see cref="Pixel"/>s différents).
        /// </summary>
        /// <seealso cref="operator ==(Image, Image)"/>
        public static bool operator !=(Image a, Image b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Vérifie l'égalité de cette <see cref="Image"/> avec <paramref name="other"/>.
        /// </summary>
        /// <seealso cref="operator ==(Image, Image)"/>
        public override bool Equals(object? other) => other is Image && this == (Image)other;
        public bool Equals(Image? other) => other is not null && this == other;
        public override int GetHashCode()
        {
            // méthode pour enlever le warning à cause des opérateurs == et !=.
            return base.GetHashCode();
        }
        #endregion
    
        #region Methods

        public Image Copy()
        {
            return new Image(this);
        }

        private int _position(int x, int y) => x * 3 + (Height - y - 1) * Stride;           

       
        public Pixel this[int x, int y] 
        { 
            get
            {
                int position = _position(x, y);
                return new Pixel(Pixels[position + 2], Pixels[position + 1], Pixels[position + 0]);
            }

            set
            {
                int position = _position(x, y);
                Pixels[position + 2] = value.R;
                Pixels[position + 1] = value.G;
                Pixels[position + 0] = value.B;
            }
        }

        public void Save(string filename)
        {
            using (FileStream stream = File.OpenWrite(filename))
            {
                stream.Write(Header, 0, Header.Length);
                stream.Write(Pixels, 0, Pixels.Length);
            }
        }
        #endregion
    }
}
