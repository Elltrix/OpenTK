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

        public void Update(double time)
        {
            Value += Rate * (float)time;
            if (Value > Limit)
            {
                Value = Limit;
            }
        }
    }

}
