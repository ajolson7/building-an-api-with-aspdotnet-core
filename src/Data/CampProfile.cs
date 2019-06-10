using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            // ForMember allows you to change the name of a variable that is
                // found from the mapping
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));

            this.CreateMap<CampModel, Camp>();

            this.CreateMap<Talk, TalkModel>();
            this.CreateMap<TalkModel, Talk>()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            this.CreateMap<Speaker, SpeakerModel>();
            this.CreateMap<SpeakerModel, Speaker>();
        }
    }
}
