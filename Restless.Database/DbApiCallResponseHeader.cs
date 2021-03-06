﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiCallResponseHeader")]
    public class DbApiCallResponseHeader : DbObject
    {
        public int ApiCallId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public DbApiItem Api { get; set; }
    }
}