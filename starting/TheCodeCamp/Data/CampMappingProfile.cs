using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap <Camp, CampModel>()
              .ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName))
              .ReverseMap();

            // AutoMapper will try to infer other reffered types when possible.  But that will 
            // not always work and eventually you will need to create them and revert them as well.
            CreateMap <Talk, TalkModel>()
              .ReverseMap()
              // dont override Speaker and Camp when going from TalkModel to Talk
              .ForMember(t => t.Speaker, opt => opt.Ignore())
              .ForMember(t => t.Camp, opt => opt.Ignore());

            CreateMap <Speaker, SpeakerModel>()
              .ReverseMap();

        }
    }
}