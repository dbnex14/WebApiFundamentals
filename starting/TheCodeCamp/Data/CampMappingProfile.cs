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
        //public CampMappingProfile()
        //{
        //    CreateMap<Camp, CampModel>()
        //        .ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName))
        //        .ReverseMap();
        //}

        public CampMappingProfile()
        {
            CreateMap <Camp, CampModel>()
              .ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName))
              .ReverseMap();

            // When mapping Camps with Talks, AutoMapper would automatically find TalkModel
            // and do the mapping.  However, that is not the case anymore and we need to do below.
            CreateMap <Talk, TalkModel>()
              .ReverseMap();

            CreateMap <Speaker, SpeakerModel>()
              .ReverseMap();

        }
    }
}