﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("ApiItem")]
    public class DbApiItem
    {
        public int Id { get; set; }

        public int? CollectionId { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public ApiMethod Method { get; set; }

        public string Url { get; set; }

        public string RequestBody { get; set; }

        public ApiItemType Type { get; set; }

        public DbApiItem Collection { get; set; }
        public List<DbApiInput> Inputs { get; set; }
        public List<DbApiOutput> Outputs { get; set; }
        public List<DbApiHeader> RequestHeaders { get; set; }
        public List<DbApiResponseComplication> ResponseComplications { get; set; }
        public List<DbApiItem> Items { get; set; }
    }
}