using RedCrossChat.Entities._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Entities
{
    public class DBFeeling : DefaultEntity
    {
        public string? Name { get; set; }//this will be the one with an emoji

        public string? Description { get; set; } // this is the official name

        public string Synonymns { get; set; } = "";

        public string Kiswahili { get; set; }
    }
}


