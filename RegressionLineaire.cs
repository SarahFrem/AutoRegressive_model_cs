using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simple_AR_from_scratch
{
    /// <summary>
    /// This class create a linear regression and computes the predicted coefficients from the regression through MCO
    /// Also computes the errors and t stats 
    /// </summary>
    public class LinearRegression
    {
        public Matrix variableExpliquee;
        public Matrix explanatoryMatrix;
        public int sizeData;

        public LinearRegression(Matrix Y, Matrix X)
        {
            sizeData = Math.Min(Y.row, X.row);
            responseVariable  = Y.Truncate(1, sizeData, 1, 1);
            explanatoryMatrix = X.Truncate(1, sizeData, 1, X.col);
        }

        /// <summary>
        /// Add a vector of ones to estimate the constant of the model
        /// </summary>
        public Matrix RegressionMatrix()
        {
            Matrix vectConst = new Matrix(sizeData, 1);

            for (int i = 0; i < sizeData; i++)
            {
                vectConst.data[i, 0] = 1;
            }
            return vectConst.ConcatenationColumns(this.explanatoryMatrix);
        }

        /// <summary>
        /// Compute the regressors coefficients from MCO 
        /// beta = inv(X'*X)*X'*Y
        /// </summary>
        /// <returns>beta</returns>
        public Matrix RegressorsMCO()
        {
            Matrix R = RegressionMatrix();
            Matrix beta = ((R.Transpose() * R).Inverse()) * R.Transpose() * responseVariable;
            return beta;
        }

        public Matrix Predictions()
        {
            return RegressionMatrix() * RegressorsMCO();
        }

        public Matrix Errors()
        {
            return responseVariable - Predictions();
        }

        public double ResidualVariance()
        {
            Matrix E = Errors();
            Matrix var = E.Transpose() * E;
            double size = (double)E.row;
            return var.data[0, 0] / size;
        }

        public Matrix VarianceCovarianceBeta()
        {
            Matrix R = RegressionMatrix();
            double var = ResidualVariance();
            Matrix res = (R.Transpose() * R).Inverse();

            for (int i = 0; i < res.row; i++)
            {
                for (int j = 0; j < res.col; j++)
                {
                    res.data[i, j] *= var;
                }
            }
            return res;
        }

        public Matrix Tstats()
        {
            Matrix beta = RegressorsMCO();
            Matrix varCovBeta = VarianceCovarianceBeta();
            Matrix tstats = new Matrix(beta.row, 1);

            for (int i = 0; i < tstats.row; i++)
            {
                tstats.data[i, 0] = beta.data[i, 0] / Math.Sqrt(varCovBeta.data[i, i]);
            }
            return tstats;
        }
    }

    /// <summary>
    /// This class create a Auto regressive model
    /// This is therefore a linear regression with a lag to set up 
    /// </summary>

    public class ARmodel : LinearRegression
    {
        public int lag;

        public ARmodel(int lag, Matrix Y) : base(Y, new Matrix(Y.row, Y.col))
        {
            this.lag = lag;
            base.sizeData = Y.row - lag;
            base.explanatoryMatrix = CreationRegressionMatrixAR();
            base.responseVariable = Y.Truncate(lag + 1, Y.row, 1, 1);
        }

        public Matrix CreationRegressionMatrixAR()
        {
            int t = sizeData + lag;
            Matrix init = responseVariable.Truncate(lag, t - 1, 1, 1);

            for (int i = 1; i < lag; i++)
            {
                Matrix vect = responseVariable.Truncate(lag - i, t - i - 1, 1, 1);
                init = init.ConcatenationColumns(vect);
            }
            return init;
        }

    }
}
