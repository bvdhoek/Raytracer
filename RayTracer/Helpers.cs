using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public static class Helpers
    {
        // Clamp integer to minimum 0 and max 255
        public static int ClampColor(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }

        public static float ClampToMinimumDesktopCoordinate(float f)
        {
            if (f < -107374032)
            {
                return -107374032;
            }
            if (f > 107374032)
            {
                return 107374032;
            }
            return f;
        }
    }
}
