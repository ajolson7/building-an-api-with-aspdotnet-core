using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1,100)]
        public int Length { get; set; } = 1;

        // prefixing the variables with the name of the object you're getting
        // the data of the variables from allows AutoMapper to automatically
        // bind/map the variables to the corresponding variables in the
        // object you're getting the data from
        // public string LocationVenueName { get; set; }
        // public string LocationAddress1 { get; set; }
        // public string LocationAddress2 { get; set; }
        // public string LocationAddress3 { get; set; }
        // public string LocationCityTown { get; set; }
        // public string LocationStateProvince { get; set; }
        // public string LocationPostalCode { get; set; }
        // public string LocationCountry { get; set; }

        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}
