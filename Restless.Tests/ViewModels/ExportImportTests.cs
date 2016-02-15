using Newtonsoft.Json;
using NUnit.Framework;
using SexyReact;

namespace Restless.Tests.ViewModels
{
    [TestFixture]
    public class ExportImportTests
    {
        [Test]
        public void Foo()
        {
            var bar = new Bar { List = new RxList<string>() };
            bar.List.Add("foobar");
            var json = JsonConvert.SerializeObject(bar);
            var deserializedBar = JsonConvert.DeserializeObject<Bar>(json);
        }

        public class Bar
        {
            public RxList<string> List { get; set; }
        }
    }
}