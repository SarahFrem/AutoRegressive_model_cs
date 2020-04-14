namespace simple_AR_from_scratch
{
    /// <summary>
    /// This class shows an example of using an instance of the AR model when predicting the spot price given the historic prices
    /// </summary>
    public class SpotPricePrediction
    {
        public static ARmodel reg;
        public Matrix vectorHistoricPrices;
        public int lag = 7; //to set up

        public SpotPricePrediction(double[,] prices)
        {
            vectorHistoricPrices = new Matrix(prices);
            reg = new ARmodel(lag, vectorHistoricPrices);
        }

        public Matrix PredictedPrices()
        {
            return reg.Predictions();
        }

        public double PredictedPriceDay(int indexDay)
        {
            bool boundaries = indexDay - lag < 0 || indexDay - lag >= PredictedPrices().row;
            if (boundaries)
            {
                return 0;
            }
            else
            {
                return PredictedPrices().data[indexDay - lag, 0];
            }
        }
    }
}