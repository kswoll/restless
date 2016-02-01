namespace Restless.WpfExtensions.CodeTriggers
{
    public interface ICodeTriggerSetter
    {
        object Key { get; }
        void Set();
        void Unset();
    }
}