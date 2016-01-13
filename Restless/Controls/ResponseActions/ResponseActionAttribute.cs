using System;

namespace Restless.Controls.ResponseActions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResponseActionAttribute : Attribute
    {
        public string[] ContentTypes { get; }

        public ResponseActionAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }
    }
}