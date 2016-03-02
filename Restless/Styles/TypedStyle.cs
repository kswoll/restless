using System.Windows;

namespace Restless.Styles
{
    public class TypedStyle<T> : Style
    {
        public TypedStyle() : base(typeof(T))
        {
        }
    }
}