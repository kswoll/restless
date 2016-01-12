using System;

namespace Restless.Controls.ResponseVisualizers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResponseVisualizerAttribute : Attribute
    {
        public string[] ContentTypes { get; }

        public ResponseVisualizerAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }
    }
}