﻿using AutoMapper;
using Dotnet9.Application.Contracts.Abouts;
using Dotnet9.Application.Contracts.Albums;
using Dotnet9.Application.Contracts.Blogs;
using Dotnet9.Application.Contracts.Categories;
using Dotnet9.Application.Contracts.Donations;
using Dotnet9.Application.Contracts.Privacies;
using Dotnet9.Application.Contracts.Tags;
using Dotnet9.Application.Contracts.Timelines;
using Dotnet9.Application.Contracts.UrlLinks;
using Dotnet9.Core;
using Dotnet9.Domain.Abouts;
using Dotnet9.Domain.Albums;
using Dotnet9.Domain.Blogs;
using Dotnet9.Domain.Categories;
using Dotnet9.Domain.Donations;
using Dotnet9.Domain.Privacies;
using Dotnet9.Domain.Tags;
using Dotnet9.Domain.Timelines;
using Dotnet9.Domain.UrlLinks;

namespace Dotnet9.Extensions.AutoMapper;

public class CustomProfile : Profile
{
    public CustomProfile()
    {
        CreateMap<AlbumCount, AlbumCountDto>();
        CreateMap<Album, AlbumDto>();

        CreateMap<AlbumBrief, AlbumBriefDto>();

        CreateMap<Category, CategoryCountDto>();
        CreateMap<CategoryCount, CategoryCountDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryBrief, CategoryBriefDto>();

        CreateMap<TagCount, TagCountDto>();

        CreateMap<BlogPostWithDetails, BlogPostWithDetailsDto>();
        CreateMap<BlogPost, BlogPostForSitemap>();
        CreateMap<BlogPostBrief, BlogPostBriefDto>();

        CreateMap<UrlLink, UrlLinkDto>()
            .ForMember(dest => dest.CreateDate,
                opt => opt.MapFrom(src => DateTimeOffsetHelper.DateTimeToString(src.CreateDate)));
        ;

        CreateMap<About, AboutDto>().ReverseMap();

        CreateMap<Donation, DonationDto>().ReverseMap();

        CreateMap<Timeline, TimelineDto>();

        CreateMap<Privacy, PrivacyDto>().ReverseMap();
    }
}