namespace SG.LevelEditor
{
    public struct Range
    {
        public int max, min;

        public Range(int min, int max)
        {
            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            this.min = min;
            this.max = max;
        }

        public int Length()
        {
            return max - min + 1;
        }

        //public override bool Equals(object obj)
        //{
        //    return base.Equals(obj);
        //}

        public static Range operator +(Range a, Range b)
        {
            a.min += b.min;
            a.max += b.max;
            return a;
        }

        public static Range operator -(Range a, Range b)
        {
            a.min -= b.min;
            a.max -= b.max;
            return a;
        }

        public static bool operator ==(Range a, Range b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Range a, Range b)
        {
            return a.min != b.min || a.max != b.max;
        }
    }
}