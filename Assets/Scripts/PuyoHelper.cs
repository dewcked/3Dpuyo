using System;

namespace Assets.Scripts
{
    public static class PuyoHelper
    {
        public static PuyoColor GetPuyoColorFromString(string str)
        {
            switch (str)
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
