namespace Restless.Models
{
    public enum ApiMethod
    {
        None = 0,
        Get = 1,
        Post = 2,
        Put = 3,
        Delete = 4
    }

    public static class ApiMethods
    {
        public static bool IsBodyAllowed(this ApiMethod method)
        {
            switch (method)
            {
                case ApiMethod.Get:
                    return false;
                default:
                    return true;
            }
        }
    }
}