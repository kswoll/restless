﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiCall")]
    public class DbApiCall
    {
        public int Id { get; set; } 
        public int ApiId { get; set; }
        public byte[] Body { get; set; }

        public DbApi Api { get; set; }
        public List<DbApiCallRequestHeader> RequestHeaders { get; set; }
        public List<DbApiCallResponseHeader> ResponseHeaders { get; set; }
    }
}