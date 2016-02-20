using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("ApiItem")]
    public class DbApiItem : DbObject
    {
        public int? CollectionId { get; set; }
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public ApiMethod Method { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }

        public ApiItemType Type { get; set; }

        public DbApiItem Collection { get; set; }
        public List<DbApiInput> Inputs { get; set; }
        public List<DbApiOutput> Outputs { get; set; }
        public List<DbApiHeader> Headers { get; set; }
        public List<DbApiResponseComplication> ResponseComplications { get; set; }
        public List<DbApiItem> Items { get; set; }
    }
}