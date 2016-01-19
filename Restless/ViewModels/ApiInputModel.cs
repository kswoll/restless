using Restless.Models;

namespace Restless.ViewModels
{
    public class ApiInputModel : BaseModel, IIdObject
    {
        public int Id { get; set; } 
        public ApiInputType InputType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
    }
}