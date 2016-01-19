using Restless.Models;
using Restless.Utils;

namespace Restless.ViewModels
{
    public class ApiHeaderModel : BaseModel, IIdObject
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Value { get; set; }
    }
}