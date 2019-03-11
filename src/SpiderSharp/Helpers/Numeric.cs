using System;

namespace SpiderSharp.Helpers
{
    public static class Numeric
    {
        public static float? CalculateDiscount(double? originalPrice, double currentPrice)
        {
            if (!originalPrice.HasValue || originalPrice <= 0 || currentPrice <= 0)
            {
                return null;
            }

            var discountCalc = (originalPrice - currentPrice) / originalPrice;
            return Math.Max(Math.Min(Convert.ToInt32(discountCalc * 100), 100), 0);
        }

        public static double? CalculateOriginalPrice(double? discount, double currentPrice)
        {
            if (!discount.HasValue || discount <= 0 || currentPrice <= 0)
            {
                return null;
            }

            var originalPrice = currentPrice / (1 - (discount.Value / 100));
            return Math.Round(originalPrice, 2);
        }
    }
}