namespace Restless.Utils
{
    public interface ICompareTo<in T>
    {
        bool IsThisPrimary(T other);
        int CompareTo(T other);
    }
}