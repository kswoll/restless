namespace Restless.Utils
{
    public static class HttpHeaders
    {
        public static bool IsContentHeader(string name)
        {
            return name == ContentTypes.ContentType;
        } 
    }
}