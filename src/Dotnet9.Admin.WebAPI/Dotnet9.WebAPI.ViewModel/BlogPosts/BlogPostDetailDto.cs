﻿namespace Dotnet9.WebAPI.ViewModel.BlogPosts;

public record BlogPostDetailDto(Guid Id, string Title, string Slug, string Description, string Cover, string Content,
    string CopyRightType, string? Original, string? OriginalAvatar, string? OriginalTitle, string? OriginalLink,
    bool Visible, string[]? AlbumNames, string[]? CategoryNames, string[]? TagNames,
    BlogPostStatus Status = BlogPostStatus.Public, bool IsTop = true, bool IsFeatured = true,
    string? CreationTime = "2023-03-04 23:29:35", int Year = 2019, int Month = 11,
    BlogPostDetailDto? PreBlogPost = null,
    BlogPostDetailDto? NextBlogPost = null);

public enum BlogPostStatus
{
    Default,
    Public,
    Password
}