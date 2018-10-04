using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp.Helpers
{
    public static class Numeric
    {
        public static decimal CalculateDiscount(decimal? originalPrice, decimal currentPrice)
        {
            if (!originalPrice.HasValue || originalPrice <= 0 || currentPrice <= 0)
            {
                return 0;
            }

            var discountCalc = (originalPrice - currentPrice) / originalPrice;
            return Math.Min(Convert.ToInt32(discountCalc * 100), 100);
        }

        public static decimal? CalculateOriginalPrice(decimal? discount, decimal currentPrice)
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
