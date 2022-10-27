using AutoMapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace AzureFunction.UniqueCode.Entities.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<JObject, Event>()
                .ForMember(dest => dest.Code, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["code"].ToString().Trim().ToUpper()); })
                .ForMember(dest => dest.Date, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["date"].ToString().Trim()); })
                .ForMember(dest => dest.Area, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["area"].ToString().Trim()); })
                .ForMember(dest => dest.Obsv, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["obsv"].ToString().Trim()); });

            CreateMap<JObject, EventBRU>()
                .ForMember(dest => dest.Type, cfg => { cfg.MapFrom(jo => jo["data"]["type"].ToString().Trim().ToUpper()); })
                .ForMember(dest => dest.SerializationType, cfg => { cfg.MapFrom(jo => jo["data"]["serializationType"].ToString().Trim().ToUpper()); })
                .ForMember(dest => dest.Parameter1, cfg => { cfg.MapFrom(jo => jo["data"]["parameter1"].ToString().Trim()); })
                .ForMember(dest => dest.Parameter2, cfg => { cfg.MapFrom(jo => jo["data"]["parameter2"].ToString().Trim()); })
                .ForPath(dest => dest.Event.Code, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["code"].ToString().Trim()); })
                .ForPath(dest => dest.Event.Date, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["date"].ToString().Trim()); })
                .ForPath(dest => dest.Event.Area, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["area"].ToString().Trim()); })
                .ForPath(dest => dest.Event.Obsv, cfg => { cfg.MapFrom(jo => jo["data"]["event"]["obsv"].ToString().Trim()); });

            CreateMap<DataRow, Box>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["usda"].ToString().Trim()))
                .ForMember(dest => dest.Usda, opt => opt.MapFrom(src => src["usda"].ToString().Trim()))
                .ForMember(dest => dest.MasterGuideId, opt => { opt.PreCondition(src => !string.IsNullOrEmpty(src["masterGuideId"].ToString())); opt.MapFrom(src => Convert.ToInt32(src["masterGuideId"].ToString())); })
                .ForMember(dest => dest.AWBId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src["awbId"].ToString().Trim()) ? null : src["awbId"].ToString().Trim()))
                .ForMember(dest => dest.MasterGuide, opt => opt.MapFrom(src => src["masterGuide"].ToString().Trim()))
                .ForMember(dest => dest.Importer, opt => opt.MapFrom(src => src["importer"].ToString().Trim()))
                .ForMember(dest => dest.OrderRef, opt => opt.MapFrom(src => src["orderRef"].ToString().Trim()))
                .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src["item"].ToString().Trim()))
                .ForMember(dest => dest.Grower, opt => opt.MapFrom(src => src["grower"].ToString().Trim()))
                .ForMember(dest => dest.DispatchId, opt => opt.MapFrom(src => src["dispatchId"].ToString().Trim()))
                .AfterMap((src, dest) =>
                {
                    dest.Events = new List<Event>()
                    {
                        new Event(){
                            Code = src["code"].ToString().Trim(),
                            Date = src["date"].ToString().Trim(),
                            Area = src["area"].ToString().Trim(),
                            Obsv = src["obsv"].ToString().Trim()
                        }
                    };
                    dest.Cheksum = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Concat(dest.Importer, 
                        dest.OrderRef, 
                        dest.Item, 
                        dest.Grower, 
                        dest.Usda, 
                        Convert.ToDateTime(dest.Events[0].Date).ToString("yyyy-MM-dd"))
                    ));
                });
        }
    }
}
