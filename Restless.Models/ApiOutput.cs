namespace Restless.Models
{
    public class ApiOutput : IdObject
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public ApiOutputType Type { get; set; }
    }
}