namespace Restless.Models
{
    public class ApiInput : IdObject
    {
        public ApiInputType InputType { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }         
    }
}