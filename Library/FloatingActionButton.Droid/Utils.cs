using System;
using Android.Content.Res;
using Android.Util;
using Android.Views;

namespace FAB.Droid
{
    internal static class Utils
    {
        public static double GetDensityIndependentPixels(double dp)
        {
            double pixels = (double)(dp * Resources.System.DisplayMetrics.Density);

            return pixels;
        }

        public static void SetMargins(this View v, int l, int t, int r, int b)
        {
            if (v.LayoutParameters is ViewGroup.MarginLayoutParams)
            {
                ViewGroup.MarginLayoutParams p = (ViewGroup.MarginLayoutParams)v.LayoutParameters;
                p.SetMargins(l, t, r, b);
                v.RequestLayout();
            }
        }
    }
}