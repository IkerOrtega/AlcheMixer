using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class ANGLES
    {
        public static readonly float MAX_ANGLE = 360f;
        public static readonly float ANGLE_CORRECTION = 90f;
    }
    
    public static class LINES
    {
        public const int FLOOR = 0;
        public const int LEFTWALL = 1;
        public const int CEILING = 2;
        public const int RIGHTWALL = 3;
    }

    public static class POINTS
    {
        public const int BOTTOMLEFT = 0;
        public const int TOPLEFT = 1;
        public const int TOPRIGHT = 2;
        public const int BOTTOMRIGHT = 3;
    }

}
