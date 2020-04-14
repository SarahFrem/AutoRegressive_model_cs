using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simple_AR_from_scratch
{
    /// <summary>
    /// This class allows to create a Matrix instance and matrix computations 
    /// </summary>
    public class Matrix
    {
        public int row;
        public int col;
        public double[,] data;

        public Matrix(int rows, int columns)
        {
            this.row = rows;
            this.col = columns;
            data     = new double[row, col];
        }

        public Matrix(double[,] inputs)
        {
            this.data = inputs;
            row       = inputs.GetLength(0);
            col       = inputs.GetLength(1);
        }

        public void Description() 
        {
            for (int i = 0; i < row; i++)
            {
                string row = "";
                for (int j = 0; j < col; j++)
                {
                    row += data[i, j] + "\t \t";
                }
                Console.WriteLine(row + "\n");
            }
        }

        /// <summary>
        /// Compute the multiplication between two matrices
        /// </summary>
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.col != B.row)
            {
                return new Matrix(1, 1);
            }
            Matrix res = new Matrix(A.row, B.col);

            for (int i = 0; i < A.row; i++)
            {
                for (int j = 0; j < B.col; j++)
                {
                    double s = 0;

                    for (int m = 0; m < A.col; m++)
                    {
                        s += A.data[i, m] * B.data[m, j];
                    }
                    res.data[i, j] = s;
                }
            }
            return res;
        }

        /// <summary>
        /// Compute Matrix A - Matrix B
        /// </summary>
        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A.row != B.row || A.col != B.col)
            {
                return new Matrix(1, 1);
            }
            int l = Math.Min(A.row, B.row);
            int c = Math.Min(A.col, B.col);

            Matrix res = new Matrix(l, c);

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    res.data[i, j] = A.data[i, j] - B.data[i, j];
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the transpose of a matrix
        /// </summary>
        public Matrix Transpose()
        {
            Matrix res = new Matrix(col, row);

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    res.data[i, j] = data[j, i];
                }
            }
            return res;
        }

        /// <summary>
        /// Truncate a matrix
        /// </summary>
        public Matrix Truncate(int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            bool minimumIndex = rowStart == 0 || rowEnd == 0 || columnStart == 0 || columnEnd == 0;
            bool coherenceIndex = rowEnd < rowStart || columnEnd < columnStart;
            bool boundIndex = rowEnd > row || columnEnd > col;

            if (minimumIndex || coherenceIndex || boundIndex)
            {
                return new Matrix(1, 1);
            }

            Matrix result = new Matrix(rowEnd - rowStart + 1, columnEnd - columnStart + 1);

            for (int i = rowStart - 1; i < rowEnd; i++)
            {
                for (int j = columnStart - 1; j < columnEnd; j++)
                {
                    result.data[i - rowStart + 1, j - columnStart + 1] = data[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Concatenation of matrices by columns
        /// </summary>
        public Matrix ConcatenationColumns(Matrix B)
        {
            if (row != B.row)
            {
                return new Matrix(1, 1);
            }

            Matrix res = new Matrix(row, col + B.col);

            for (int k = 0; k < col; k++)
            {
                for (int i = 0; i < row; i++)
                {
                    res.data[i, k] = data[i, k];
                }
            }

            for (int k = col; k < col + B.col; k++)
            {
                for (int i = 0; i < row; i++)
                {
                    res.data[i, k] = B.data[i, k - col];
                }
            }
            return res;
        }

        /// <summary>
        /// Check if a matrix is squared
        /// </summary>
        public bool IsSquared()
        {
            return row == col;
        }

        /// <summary>
        /// Check if a matrix is lower
        /// </summary>
        public bool IsLowerMatrix()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = i + 1; j < row; j++)
                {
                    if (data[i, j] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check if a matrix is upper
        /// </summary>
        public bool IsUpperMatrix()
        {
            for (int i = 1; i < row; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (data[i, j] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Compute LU decomposition on a squared matrix
        /// </summary>
        /// <param name="L">lower triangular matrix</param>
        /// <param name="U">upper triangular matrix</param>
        public void DecompositionLU(out Matrice L, out Matrice U)
        {
            if (!this.IsSquared())
            {
                L = new Matrix(1, 1);
                U = new Matrix(1, 1);
            }
            else
            {
                L = new Matrix(row, col);
                U = new Matrix(row, col);

                for (int i = 0; i < row; i++)
                {
                    L.data[i, i] = 1;
                }

                for (int i = 0; i < row; i++)
                {
                    for (int j = i; j < row; j++)
                    {
                        double sum_U = 0;
                        for (int k = 0; k < i; k++)
                            sum_U += (L.data[i, k] * U.data[k, j]);

                        U.data[i, j] = data[i, j] - sum_U;
                    }

                    for (int j = i; j < row; j++)
                    {
                        double sum_L = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum_L = sum_L + (L.data[j, k] * U.data[k, i]);
                        }
                        L.data[j, i] = (data[j, i] - sum_L) / U.data[i, i];
                    }
                }
            }
        }

        /// <summary>
        /// Resolve the following linear equation LY = f 
        /// where L is a lower triangular matrix
        /// </summary>
        /// <param name="f">vector of shape (d,1)</param>
        /// <returns>Y vector y of shape (d,1)</returns>
        public Matrix ResolveLinearEquation_LowerTriangularMatrix(Matrix f)
        {
            bool violations = !this.IsLowerMatrix() || row != f.row || f.col > 1 || row < 2;
            if (violations)
            {
                return new Matrix(1, 1);
            }

            Matrix Y = new Matrix(row, 1);

            Y.data[0, 0] = f.data[0, 0] / this.data[0, 0];

            for (int m = 1; m < row; m++)
            {
                double sum = 0;
                for (int i = 0; i < m; i++)
                {
                    sum = sum + this.data[m, i] * Y.data[i, 0];
                }
                Y.data[m, 0] = (f.data[m, 0] - sum) / this.data[m, m];
            }
            return Y;
        }

        /// <summary>
        /// Resolve the following linear equation UX = y
        /// where U is a upper triangular matrix
        /// </summary>
        /// <param name="y">vector of shape (d,1)</param>
        /// <returns>X vector y of shape (d,1)</returns>
        public Matrix ResolveLinearEquation_UpperTriangularMatrix(Matrix y)
        {
            bool violations = !this.IsUpperMatrix() || row != y.row || y.col > 1;
            if (violations)
            {
                return new Matrix(1, 1);
            }

            Matrix X = new Matrix(row, 1);

            for (int i = row - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int k = i + 1; k < row; k++)
                {
                    sum = sum + this.data[i, k] * X.data[k, 0];
                }
                X.data[i, 0] = (y.data[i, 0] - sum) / this.data[i, i];
            }
            return X;
        }

        /// <summary>
        /// compute and returns the inverse of a lower matrix 
        /// </summary>  
        public Matrix InverseLowerMatrix()
        {
            if (!this.IsLowerMatrix())
            {
                return new Matrix(1, 1);
            }

            Matrix inv = new Matrix(row, 1);

            for (int i = 0; i < row; i++)
            {
                Matrix id = new Matrix(row, 1);
                id.data[i, 0] = 1;

                Matrix vect = this.ResolveLinearEquation_LowerTriangularMatrix(id);
                inv = inv.ConcatenationColumns(vect);

            }
            return inv.Truncate(1, row, 2, col + 1);
        }

        /// <summary>
        /// compute and returns the inverse of an upper matrix 
        /// </summary> 
        public Matrix InverseUpperMatrix()
        {
            if (!this.IsUpperMatrix())
            {
                return new Matrix(1, 1);
            }

            Matrix inv = new Matrix(row, 1);

            for (int i = 0; i < row; i++)
            {
                Matrix id = new Matrix(row, 1);
                id.data[i, 0] = 1;

                Matrix vect = this.ResolveLinearEquation_UpperTriangularMatrix(id);
                inv = inv.ConcatenationColumns(vect);

            }
            return inv.Truncate(1, row, 2, col + 1);
        }

        /// <summary>
        /// Trick: compute the inverse of a squared matrix using LU decomposition
        /// (LU)^-1 = U^-1 * L^-1
        /// </summary>
        public Matrix Inverse()
        {
            if (!this.IsSquared())
            {
                return new Matrix(1, 1);
            }

            Matrix L; Matrix U;

            this.DecompositionLU(out L, out U);

            Matrix L_inv = L.InverseLowerMatrix();
            Matrix U_inv = U.InverseUpperMatrix();

            return U_inv * L_inv;
        }
    }
}