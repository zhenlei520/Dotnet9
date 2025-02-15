﻿using MediatR;

namespace Dotnet9.WebAPI.Controllers;

[Route("api/blogposts")]
[ApiController]
public class BlogPostController : ControllerBase
{
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly Dotnet9DbContext _dbContext;
    private readonly BlogPostManager _manager;
    private readonly IMediator _mediator;
    private readonly IBlogPostRepository _repository;

    public BlogPostController(Dotnet9DbContext dbContext, IBlogPostRepository repository,
        BlogPostManager manager, IDistributedCacheHelper cacheHelper, IMediator mediator)
    {
        _dbContext = dbContext;
        _repository = repository;
        _manager = manager;
        _cacheHelper = cacheHelper;
        _mediator = mediator;
    }

    [HttpGet("topAndFeatured")]
    public async Task<GetTopAndFeaturedBlogPostResponse> TopAndFeatured()
    {
        (BlogPost[]? BlogPosts, long Count) result = await _repository.GetListAsync(new GetBlogPostListRequest());
        var top = result.BlogPosts![0].ConvertToBlogPostDetailDto(this._dbContext);
        var featured = result.BlogPosts!.Skip(1).Take(2)
            .Select(blogPost => blogPost.ConvertToBlogPostDetailDto(this._dbContext)).ToArray();
        return new GetTopAndFeaturedBlogPostResponse(top!, featured!);
    }

    [HttpGet]
    public async Task<GetBlogPostListResponse> List([FromQuery] GetBlogPostListRequest request)
    {
        (BlogPost[]? BlogPosts, long Count) result = await _repository.GetListAsync(request);
        return new GetBlogPostListResponse(result.BlogPosts.ConvertToBlogPostDtoArray(_dbContext), result.Count);
    }

    [HttpGet]
    [Route("/api/search/")]
    public async Task<GetBlogPostListResponse> Search([FromQuery] GetBlogPostListRequest request)
    {
        string cacheKey = $"BlogPostController_Search_{request.Keywords}_{request.Current}_{request.PageSize}";

        async Task<(BlogPost[]? BlogPosts, long Count)?> GetFromDb()
        {
            return await _repository.GetListAsync(request);
        }

        var blogPosts = await _cacheHelper.GetOrCreateAsync(cacheKey,
            async e => await GetFromDb());

        return new GetBlogPostListResponse(blogPosts!.Value.BlogPosts.ConvertToBlogPostDtoArray(_dbContext),
            blogPosts.Value.Count);
    }


    [HttpGet]
    [Route("/api/Album/{albumId}/BlogPost")]
    public async Task<GetBlogPostsByAlbumResponse> GetBlogPostsByAlbum(Guid albumId,
        [FromQuery] GetBlogPostsByAlbumRequest request)
    {
        (BlogPost[]? BlogPosts, long Count) result =
            await _repository.GetListByAlbumIdAsync(albumId, request.PageIndex, request.PageSize);
        return new GetBlogPostsByAlbumResponse(result.BlogPosts.ConvertToBlogPostDtoArray(_dbContext), result.Count);
    }


    [HttpGet]
    [Route("/api/Category/{categoryId}/BlogPost")]
    public async Task<GetBlogPostsByCategoryResponse> GetBlogPostsByCategory(Guid categoryId,
        [FromQuery] GetBlogPostsByCategoryRequest request)
    {
        (BlogPost[]? BlogPosts, long Count) result =
            await _repository.GetListByCategoryIdAsync(categoryId, request.Current, request.PageSize);
        return new GetBlogPostsByCategoryResponse(result.BlogPosts.ConvertToBlogPostDtoArray(_dbContext), result.Count);
    }


    [HttpGet]
    [Route("/api/Tag/{tagId}/BlogPost")]
    public async Task<GetBlogPostsByTagResponse> GetBlogPostsByTag(Guid tagId,
        [FromQuery] GetBlogPostsByTagRequest request)
    {
        (BlogPost[]? BlogPosts, long Count) result =
            await _repository.GetListByTagIdAsync(tagId, request.PageIndex, request.PageSize);
        return new GetBlogPostsByTagResponse(result.BlogPosts.ConvertToBlogPostDtoArray(_dbContext), result.Count);
    }

    [HttpGet]
    [Route("/api/blogPosts/getById/{id}")]
    public async Task<BlogPostDetailDto?> GetById(Guid id)
    {
        BlogPost? blogPost = await _repository.FindByIdAsync(id);
        return blogPost?.ConvertToBlogPostDetailDto(_dbContext);
    }

    [HttpGet]
    [Route("/api/blogPosts/{slug}")]
    public async Task<BlogPostDetailDto?> GetBySlug(string slug) {
        BlogPost? blogPost = await _repository.FindBySlugAsync(slug);
        return blogPost?.ConvertToBlogPostDetailDto(_dbContext);
    }

    [HttpPut]
    [Authorize(Roles = UserRoleConst.Admin)]
    public async Task<int> UpdateDeleteStatus([FromBody] DeleteBlogPostRequest request)
    {
        return await _repository.UpdateDeleteStatusAsync(request.Ids);
    }

    [HttpDelete]
    [Authorize(Roles = UserRoleConst.Admin)]
    public async Task<int> Delete([FromBody] DeleteBlogPostRequest request)
    {
        return await _repository.DeleteAsync(request.Ids);
    }

    [HttpPost]
    [Authorize(Roles = UserRoleConst.Admin)]
    public async Task<BlogPostDto?> Add([FromBody] AddBlogPostRequest request)
    {
        BlogPost data = await _manager.CreateAsync(null, request.Title, request.Slug, request.Description,
            request.Cover,
            request.Content, request.CopyRightType, request.Original, request.OriginalAvatar, request.OriginalTitle,
            request.OriginalLink, request.Banner, request.Visible, request.AlbumIds, request.CategoryIds,
            request.TagIds);
        EntityEntry<BlogPost> dataFromDb = await _dbContext.AddAsync(data);
        await _dbContext.SaveChangesAsync();

        return dataFromDb.Entity.ConvertToBlogPostDto(_dbContext);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles = UserRoleConst.Admin)]
    public async Task<BlogPostDto?> Update(Guid id, [FromBody] UpdateBlogPostRequest request)
    {
        BlogPost data = await _manager.CreateAsync(id, request.Title, request.Slug, request.Description, request.Cover,
            request.Content, request.CopyRightType, request.Original, request.OriginalAvatar, request.OriginalTitle,
            request.OriginalLink, request.Banner, request.Visible, request.AlbumIds, request.CategoryIds,
            request.TagIds);
        EntityEntry<BlogPost> dataFromDb = _dbContext.Update(data);
        await _dbContext.SaveChangesAsync();

        return dataFromDb.Entity.ConvertToBlogPostDto(_dbContext);
    }

    [HttpPut]
    [Route("/api/blogposts/{id}/changeVisible")]
    [Authorize(Roles = UserRoleConst.Admin)]
    public async Task<ResponseResult<BlogPostDetailDto?>> UpdateVisible(Guid id,
        [FromBody] UpdateAlbumVisibleRequest request)
    {
        BlogPost data = await _manager.ChangeVisible(id, request.Visible);

        await _dbContext.SaveChangesAsync();
        return data.Adapt<BlogPostDetailDto>();
    }

    [HttpPost]
    [Route("/api/blogposts/like/{slug}")]
    public async Task<int> Like(string slug)
    {
        int likeCount = await _repository.IncreaseLikeCountAsync(slug);
        _mediator?.Publish(new LikeBlogPostEvent(slug, likeCount));
        return likeCount;
    }
}