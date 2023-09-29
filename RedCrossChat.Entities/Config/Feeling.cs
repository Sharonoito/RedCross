using Microsoft.AspNetCore.Identity;
using RedCrossChat.Entities._base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class Feeling : DefaultEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Synonyms { get; set; }

    }
}





