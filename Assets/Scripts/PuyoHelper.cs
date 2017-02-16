using System;

namespace Assets.Scripts
{
    public static class PuyoHelper
    {
        public static PuyoColor GetPuyoColorFromString(string color)
        {
            if (string.IsNullOrEmpty(color))
                throw new ArgumentNullException("color");

            switch (color)
            {
                case "Blue":
                    return PuyoColor.Blue;
                case "Green":
                    return PuyoColor.Green;
                case "Yellow":
                    return PuyoColor.Yellow;
                case "Red":
                    return PuyoColor.Red;
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }
    }
}
