namespace Restless.Models
{
    public class ApiHeader : IdObject, IHeader
    {
        public string Name { get; set; } 
        public string Value { get; set; }        
    }
}