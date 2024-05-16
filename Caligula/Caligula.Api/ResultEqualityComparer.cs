using Caligula.Model.SC2Pulse;

namespace Caligula.Api
{
    public class ResultEqualityComparer : IEqualityComparer<Result>
    {
        public bool Equals(Result x, Result y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            // Check whether the Result's properties are equal.
            return x.match.Duration == y.match.Duration && x.match.Date == y.match.Date;
        }

        public int GetHashCode(Result obj)
        {
            // Check whether the object is null
            if (obj is null)
                return 0;

            int hash = 17;
            // Calculate the hash code for the Result.
            hash = hash * 23 + obj.match.Duration.GetHashCode();
            hash = hash * 23 + obj.match.Date.GetHashCode();
            return hash;
        }
    }

}