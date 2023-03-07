using System;

namespace Computer_Science_Problem;

/// <summary> Represents a complex number composed by a Real part and an Imaginary part with the associated mathematical operators.</summary>
public class Complex
{
    #region Fields
    /// <summary> The real part of the complex number. </summary>
    public double Re;
    /// <summary> The imaginary part of the complex number. </summary>
    public double Im;
    #endregion

    #region Constructor
    /// <summary> The natural constructor of the <see cref="Complex"/> class. </summary>
    /// <param name="real"> The real part of the complex number. </param>
    /// <param name="imaginary"> The imaginary part of the complex number. </param>
    public Complex(double real, double imaginary)
    {
        Re = real;
        Im = imaginary;
    }
    #endregion

    #region Operators
    /// <summary> The sum operator of the <see cref="Complex"/> class. </summary>
    /// <returns> A <see cref="Complex"/> number, the sum of 2 complex numbers. </returns>
    public static Complex operator +(Complex a, Complex b) => new Complex(a.Re + b.Re, a.Im + b.Im);
    /// <summary> The difference operator of the <see cref="Complex"/> class. </summary>
    /// <returns> A <see cref="Complex"/> number, the difference between 2 complex numbers.</returns>
    public static Complex operator -(Complex a, Complex b) => new Complex(a.Re - b.Re, a.Im - b.Im);
    /// <summary> Multiplication operator of an <i>a</i> complex number times by a <i>b</i> complex number.</summary>
    /// <returns> A <see cref="Complex"/> number, the result of the multiplication. </returns>
    public static Complex operator *(Complex a, Complex b) => new Complex(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
    /// <summary> Division operator between 2 complex numbers. </summary>
    /// <returns> A <see cref="Complex"/> number, the result of the division. </returns>
    public static Complex operator /(Complex a, Complex b) => new Complex((a.Re * b.Re + a.Im * b.Im) / (b.Re * b.Re + b.Im * b.Im), (a.Im * b.Re - a.Re * b.Im) / (b.Re * b.Re + b.Im * b.Im));
    /// <summary> The sum operator of a complex number plus a real number. </summary>
    /// <returns> Un nombre complexe, la somme du réel et du complexe. </returns>
    public static Complex operator +(Complex a, double b) => new Complex(a.Re + b, a.Im);
    /// <summary> Subtraction operator for a real number minus a complex number. </summary>
    /// <returns> A <see cref="Complex"/> number, the difference between the real and complex. </returns>
    public static Complex operator -(Complex a, double b) => new Complex(a.Re - b, a.Im);
    /// <summary> Multiplication operator of a complex number times a real number. </summary>
    /// <returns> A <see cref="Complex"/> number, the result of the multiplication. </returns>
    public static Complex operator *(Complex a, double b) => new Complex(a.Re * b, a.Im * b);
    /// <summary> Division operator of a complex number divided by a reel number. </summary>
    /// <returns> A <see cref="Complex"/> number, the result of the division. </returns>
    public static Complex operator /(Complex a, double b) => new Complex(a.Re / b, a.Im / b);
    /// <summary> Addition operator of a complexe number plus a real number. </summary>
    /// <returns> A <see cref="Complex"/> number which is the sum of the real number and the complex one. </returns>
    public static Complex operator +(double a, Complex b) => new Complex(a + b.Re, b.Im);
    /// <summary> Substraction operator of a real number minus a complexe number. </summary>
    /// <returns> A <see cref="Complex"/> number, the difference beetween the real and complex. </returns>
    public static Complex operator -(double a, Complex b) => new Complex(a - b.Re, a - b.Im);
    #endregion

    #region Methods
    /// <summary> Gets the absolute value of this <see cref="Complex"/> number. </summary>
    /// <returns> The absolute value of this <see cref="Complex"/> number. </returns>
    public double Abs() => (double)Math.Sqrt(Re * Re + Im * Im);
    /// <summary> Textual representation of the value of this complex number. </summary>
    /// <returns> A <see cref="string"/> representaion of the <see cref="Complex"/> number. </returns>
    public override string ToString() => Re + " + " + "i" + Im;
    #endregion
}