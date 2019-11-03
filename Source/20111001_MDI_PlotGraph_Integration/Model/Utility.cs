using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDI_PlotGraph_Integration
{
    static class Utility
    {
        #region Coversion to fraction
        // coverts a decimal to a fraction and returns it as a string
        // Code was obtained from http://social.msdn.microsoft.com/Forums/pl-PL/csharplanguage/thread/e4df16cf-4207-4b76-8116-e02f689135ec
        // and modified
        public static string Convert(decimal value)
        {
            string mStr = "";
            if (value == 0)
                mStr = "0";
            else if (value < 0)
            {
                mStr = "-";
            }

            value = Math.Abs(value);

            // get the whole value of the fraction
            uint mWhole = (uint)Math.Truncate(value);

            // get the fractional value
            decimal mFraction = value - mWhole;

            // initialize a numerator and denomintar
            uint mNumerator = 0;
            uint mDenomenator = 1;

            // ensure that there is actual a fraction
            if (mFraction > 0m)
            {
                // convert the value to a string so that you can count the number of decimal places there are
                string strFraction = mFraction.ToString().Remove(0, 2);

                // store teh number of decimal places
                uint intFractLength = (uint)strFraction.Length;

                // set the numerator to have the proper amount of zeros
                mNumerator = (uint)Math.Pow(10, intFractLength);

                // parse the fraction value to an integer that equals [fraction value] * 10^[number of decimal places]
                uint.TryParse(strFraction, out mDenomenator);

                // get the greatest common divisor for both numbers
                uint gcd = GreatestCommonDivisor(mDenomenator, mNumerator);

                // divide the numerator and the denominator by the gratest common divisor
                mNumerator = mNumerator / gcd;
                mDenomenator = mDenomenator / gcd;

                mDenomenator += (uint)(mWhole * mNumerator);
                mWhole = 0;
            }

            // create a string builder
            StringBuilder mBuilder = new StringBuilder();

            // add the whole number if it's greater than 0
            if (mWhole > 0m)
            {
                mBuilder.Append(mWhole);
            }

            // add the fraction if it's greater than 0m
            if (mFraction > 0m)
            {
                if (mBuilder.Length > 0)
                {
                    mBuilder.Append(" ");
                }

                mBuilder.Append(mDenomenator);
                mBuilder.Append("/");
                mBuilder.Append(mNumerator);
            }

            mStr +=mBuilder.ToString();
            return mStr;
        }

        private static uint GreatestCommonDivisor(uint valA, uint valB)
        {
            // return 0 if both values are 0 (no GSD)
            if (valA == 0 &&
              valB == 0)
            {
                return 0;
            }
            // return value b if only a == 0
            else if (valA == 0 &&
                  valB != 0)
            {
                return valB;
            }
            // return value a if only b == 0
            else if (valA != 0 && valB == 0)
            {
                return valA;
            }
            // actually find the GSD
            else
            {
                uint first = valA;
                uint second = valB;

                while (first != second)
                {
                    if (first > second)
                    {
                        first = first - second;
                    }
                    else
                    {
                        second = second - first;
                    }
                }

                return first;
            }
        }
        #endregion
        
        public static float Truncate(double incNum, int incDecimalPlaces)
        {
            int num = (int)Math.Pow(10, (double)incDecimalPlaces);
            return (float)Math.Truncate(incNum * num) / num;
        }
    }
}
