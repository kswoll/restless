﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("ApiHeader")]
    public class DbApiHeader : DbObject
    {
        public int ApiId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public DbApiItem Api { get; set; }
    }
}