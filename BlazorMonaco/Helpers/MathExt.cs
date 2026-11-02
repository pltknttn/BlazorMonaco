namespace BlazorMonaco.Helpers
{
    /// <summary>
    /// Small compatibility helpers for target frameworks that predate <c>Math.Clamp</c>.
    /// </summary>
    internal static class MathExt
    {
        public static int Clamp(int value, int min, int max)
            => value < min ? min : value > max ? max : value;

        public static double Clamp(double value, double min, double max)
            => value < min ? min : value > max ? max : value;
    }
}
