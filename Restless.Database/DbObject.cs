using Restless.Models;
using SexyReact;

namespace Restless.Database
{
    [Rx]
    public class DbObject : RxObject, IIdObject
    {
        public int Id { get; set; }
    }
}