using RedCrossChat.Entities;
using RedCrossChat.Objects;
using System.Collections.Generic;

namespace RedCrossChat
{
    public class ReportVm
    {
        public string Name;

        public string Description;

        public int Value;

        public string Percentage;

        public string Icon;

        public string Background;

        public List<DBFeeling> feelings;

        public List<DBCounty> countys;



        public ReportVm() { 
        
        }

        public ReportVm(string name, string description, int value)
        {
            this.Name = name;
            this.Description = description;
            this.Value = value;
        }

        public ReportVm(string name, string description, int value, string percentage, string icon, string background) : this(name, description, value)
        {
            Percentage = percentage;
            Icon = icon;
            Background = background;
        }
    }
}
