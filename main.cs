// TASK 1: COMPLEX

using System;

public class Complex
{
    // Статические члены
    public static readonly Complex Zero = new Complex(0, 0);
    public static readonly Complex One = new Complex(1, 0);
    public static readonly Complex ImaginaryOne = new Complex(0, 1);

    // Поля
    private double real;
    private double imaginary;

    // Конструкторы
    public Complex(double real, double imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }

    public Complex(double real) : this(real, 0)
    {
    }

    // Методы Re и Im
    public double Re() => real;
    public double Im() => imaginary;

    // Свойства для Re и Im
    public double Re
    {
        get { return real; }
        set { real = value; }
    }

    public double Im
    {
        get { return imaginary; }
        set { imaginary = value; }
    }

    // Свойство "Длина"
    public double Length => Math.Sqrt(real * real + imaginary * imaginary);

    // Операторы
    public static Complex operator +(Complex a, Complex b) => new Complex(a.real + b.real, a.imaginary + b.imaginary);
    public static Complex operator -(Complex a, Complex b) => new Complex(a.real - b.real, a.imaginary - b.imaginary);
    public static Complex operator *(Complex a, Complex b) => new Complex(a.real * b.real - a.imaginary * b.imaginary, a.real * b.imaginary + a.imaginary * b.real);
    public static Complex operator /(Complex a, Complex b)
    {
        double denominator = b.real * b.real + b.imaginary * b.imaginary;
        if (denominator == 0)
            throw new DivideByZeroException("Division by zero.");
        return new Complex((a.real * b.real + a.imaginary * b.imaginary) / denominator,
                           (a.imaginary * b.real - a.real * b.imaginary) / denominator);
    }
    public static Complex operator +(Complex a) => a;
    public static Complex operator -(Complex a) => new Complex(-a.real, -a.imaginary);

    // Переопределение ToString
    public override string ToString()
    {
        if (imaginary >= 0)
            return $"{real} + {imaginary}i";
        else
            return $"{real} - {-imaginary}i";
    }

    // Переопределение Equals и GetHashCode
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Complex))
            return false;
        Complex other = (Complex)obj;
        return this.real == other.real && this.imaginary == other.imaginary;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(real, imaginary).GetHashCode();
    }
}

// TASK 2: EQUATION
// (Не поняла стоит ли скидывать вам файлы на почту поэтому просто перенесу код с каждого сюда)

//InterfaceIPolynomialEquation.cs

public interface IPolynomialEquation
{
    int Dimension { get; }
    double[] Coefficients { get; }
    Complex[] FindRoots();
}

//PolynomialEquation.cs

using System;
using System.Linq;

public class PolynomialEquation : IPolynomialEquation
{
    private double[] coefficients;
    private readonly IRootFindingStrategy rootFindingStrategy;

    public PolynomialEquation(double[] coefficients)
    {
        this.coefficients = coefficients;
        this.coefficients = Equations.RemoveExtraCoefficients(this.coefficients);
        this.rootFindingStrategy = Equations.SelectStrategy(this.coefficients);
    }

    public int Dimension => coefficients.Length;

    public double[] Coefficients => coefficients.ToArray();

    public Complex[] FindRoots()
    {
        if (rootFindingStrategy == null)
            throw new InvalidOperationException("Unknown equation type");

        return rootFindingStrategy.FindRoots(coefficients);
    }
}

//Equations.cs

using System;
using System.Linq;

public static class Equations
{
    public static double[] RemoveExtraCoefficients(double[] coefficients)
    {
        int lastIndex = Array.FindLastIndex(coefficients, c => c != 0);
        return coefficients.Take(lastIndex + 1).ToArray();
    }

    public static IRootFindingStrategy SelectStrategy(double[] coefficients)
    {
        int dimension = coefficients.Length;

        if (dimension == 0)
            throw new InvalidOperationException("Cannot determine equation type with no coefficients");

        if (dimension == 1)
            return new LinearEquationStrategy();
        else if (dimension == 2)
            return new QuadraticEquationStrategy();
        else
            throw new InvalidOperationException("Equations with dimension greater than 2 are not supported");
    }
}

//IRootFindingStrategy.cs

public interface IRootFindingStrategy
{
    Complex[] FindRoots(double[] coefficients);
}

//LinearEquationStrategy.cs

using System;

public class LinearEquationStrategy : IRootFindingStrategy
{
    public Complex[] FindRoots(double[] coefficients)
    {
        double a = coefficients[0];

        if (a == 0)
            throw new InvalidOperationException("Infinite number of roots");

        return new Complex[] { new Complex(-coefficients[1] / a) };
    }
}

//QuadraticEquationStrategy.cs

using System;

public class QuadraticEquationStrategy : IRootFindingStrategy
{
    public Complex[] FindRoots(double[] coefficients)
    {
        double a = coefficients[0];
        double b = coefficients[1];
        double c = coefficients[2];

        double discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
            throw new InvalidOperationException("Complex roots are not supported");
        else if (discriminant == 0)
            return new Complex[] { new Complex(-b / (2 * a)) };
        else
        {
            double sqrtDiscriminant = Math.Sqrt(discriminant);
            return new Complex[]
            {
                new Complex((-b + sqrtDiscriminant) / (2 * a)),
                new Complex((-b - sqrtDiscriminant) / (2 * a))
            };
        }
    }
}

//Complex.cs

public class Complex
{
    public double Real { get; }
    public double Imaginary { get; }

    public Complex(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }
}

// TASK 3: STRATEGY

//Interface.cs

public interface IPolynomialEquationSolver
{
    double[] Solve(double[] coefficients);
}

//Strategies.cs

public static class Strategies
{
    public static readonly IPolynomialEquationSolver LinearSolver = new LinearEquationSolver();
    public static readonly IPolynomialEquationSolver QuadraticSolver = new QuadraticEquationSolver();

    private const double Epsilon = 1e-10;
}

//LinearEquationSolver.cs

public class LinearEquationSolver : IPolynomialEquationSolver
{
    public double[] Solve(double[] coefficients)
    {
        double a = coefficients[0];
        double b = coefficients[1];

        if (Math.Abs(a) < Strategies.Epsilon)
        {
            throw new InvalidOperationException("No solution: coefficient a is zero");
        }

        return new double[] { -b / a };
    }
}

//QuadraticEquationSolver.cs

using System;

public class QuadraticEquationSolver : IPolynomialEquationSolver
{
    public double[] Solve(double[] coefficients)
    {
        double a = coefficients[0];
        double b = coefficients[1];
        double c = coefficients[2];

        if (Math.Abs(a) < Strategies.Epsilon)
        {
            return Strategies.LinearSolver.Solve(new double[] { b, c });
        }

        double discriminant = b * b - 4 * a * c;

        if (Math.Abs(discriminant) < Strategies.Epsilon)
        {
            return new double[] { -b / (2 * a) };
        }
        else if (discriminant > 0)
        {
            double sqrtDiscriminant = Math.Sqrt(discriminant);
            return new double[]
            {
                (-b + sqrtDiscriminant) / (2 * a),
                (-b - sqrtDiscriminant) / (2 * a)
            };
        }
        else
        {
            return Array.Empty<double>();
        }
    }
}

// TASK 4: MAIN

using System;

class Program
{
    static void Main()
    {
        double[] coefficients = ReadCoefficients();

        IPolynomialEquationSolver solver;
        try
        {
            solver = Strategies.SelectSolver(coefficients);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        try
        {
            double[] roots = solver.Solve(coefficients);
            Console.WriteLine("Roots:");
            foreach (var root in roots)
            {
                Console.WriteLine(root);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static double[] ReadCoefficients()
    {
        double[] coefficients = new double[3];
        string[] prompts = { "Enter coefficient a: ", "Enter coefficient b: ", "Enter coefficient c: " };

        for (int i = 0; i < coefficients.Length; i++)
        {
            bool isValidInput = false;
            do
            {
                Console.Write(prompts[i]);
                string input = Console.ReadLine();

                if (double.TryParse(input, out coefficients[i]))
                {
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            } while (!isValidInput);
        }

        return coefficients;
    }
}

