using System;

namespace OpenGL
{
    public class Accumulator
    {
        /// <summary>
        /// Value change per second
        /// </summary>
        public float Rate { get; set; }
        public float Value { get; set; } = 0;
        public float Limit { get; set; }
        public bool Enabled { get; set; } = false;

        public void Update(double time)
        {
            if (Enabled)
            {
                Value += Rate * (float)time;
                if (Value > Limit)
                {
                    Value = Limit;
                }
            }
        }
    }

}
