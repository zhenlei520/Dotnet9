﻿namespace Dotnet9.WebAPI.ViewModels.Albums;

public record AlbumDto(Guid Id, Guid[] CategoryIds, int SequenceNumber, string Name, string Slug, string Cover,
    string? Description = null, bool Visible = false);

public static class AlbumExtension
{
    public static AlbumDto? ConvertToAlbumDto(this Album? album)
    {
        if (album == null)
        {
            return null;
        }

        var categoryIds = album.Categories!.Select(category => category.CategoryId).ToArray();
        return new AlbumDto(album.Id, categoryIds, album.SequenceNumber, album.Name, album.Slug, album.Cover,
            album.Description, album.Visible);
    }

    public static AlbumDto[]? ConvertToAlbumDtoArray(this Album[]? albums)
    {
        if (albums == null || !albums.Any())
        {
            return null;
        }

        return albums.Select(album => album.ConvertToAlbumDto()!).ToArray();
    }
}