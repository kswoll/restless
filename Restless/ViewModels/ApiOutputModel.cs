using System;
using Restless.Models;

namespace Restless.ViewModels
{
    public class ApiOutputModel : BaseModel, IIdObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public ApiOutputType Type { get; set; }
        public object Value { get; set; }
        public string ValueAsString => Value?.ToString().Replace(Environment.NewLine, "\\n") ?? "";
    }
}