﻿namespace Dotnet9.WebAPI.ViewModel.Comments;

public record CommentDto(Guid Id, Guid? ParentId, string Url, string UserName,string Avatar, string Email, string Content,
    string CreationTime);