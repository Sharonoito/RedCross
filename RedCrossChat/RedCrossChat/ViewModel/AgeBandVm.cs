using System;

namespace RedCrossChat
{
    public class AgeBandVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Kiswahili { get; set; }

        public string Synonyms { get; set; } = "";

        public string Lowest { get; set; } = "";

        public string Highest { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}
