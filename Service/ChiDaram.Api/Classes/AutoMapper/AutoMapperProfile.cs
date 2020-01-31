using System;
using AutoMapper;
using ChiDaram.Common.Helper;

namespace ChiDaram.Api.Classes.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<string, string>().ConvertUsing(s => string.IsNullOrWhiteSpace(s) ? "" : s.RemoveArabicChars());
            CreateMap<string, DateTime>().ConvertUsing(q => DateTime.Parse(q));
        }
    }
}

