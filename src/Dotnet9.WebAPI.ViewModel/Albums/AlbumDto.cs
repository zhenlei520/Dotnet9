﻿namespace Dotnet9.WebAPI.ViewModel.Albums;

public record AlbumDto(Guid Id, string[]? CategoryNames, int SequenceNumber, string Name, string Slug, string Cover,
    string? Description = null, bool Visible = false);