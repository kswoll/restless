﻿using System.Collections.Generic;

namespace Restless.Utils
{
    public class ContentTypes
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string TextPlain = "text/plain";
        public const string TextHtml = "text/html";
        public const string TextXml = "text/xml";

        private static readonly HashSet<string> textContentTypes = new HashSet<string>();

        static ContentTypes()
        {
            textContentTypes.Add(ApplicationJson);
            textContentTypes.Add(ApplicationXml);
            textContentTypes.Add(TextPlain);
            textContentTypes.Add(TextHtml);
            textContentTypes.Add(TextXml);
        }

        public static bool IsText(string contentType)
        {
            return textContentTypes.Contains(contentType);
        }
    }
}