﻿using AutoMapper;
using Gml.Web.Skin.Service.Core.Cloudflare;
using Gml.Web.Skin.Service.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Gml.Web.Skin.Service.Core.Requests;

internal abstract class TextureRequests
{
    internal static async Task<IResult> LoadSkin(HttpRequest request, [FromForm] IFormFile file, IMapper mapper,
        string userName)
    {
        if (file.Length == 0)
            return Results.BadRequest();

        var tempFile = Path.Combine(SkinHelper.SkinTextureDirectory, $"{userName}.png");

        try
        {
            // Если файл существует, удаляем его
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            // Создаем новый файл и копируем в него данные
            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
            }

            var texture = SkinHelper.Create($"http://{request.Host.Value}", userName);
            return Results.Ok(mapper.Map<UserTextureReadDto>(texture));
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Error saving skin: {ex.Message}");
        }
    }

    internal static async Task<IResult> DeleteSkin(HttpRequest request, IMapper mapper,
        string userName)
    {

        var filePath = Path.Combine(SkinHelper.SkinTextureDirectory, $"{userName}.png");

        if (!File.Exists(filePath))
            return Results.NotFound();

        File.Delete(filePath);
        return Results.Ok();

    }

    internal static async Task<IResult> LoadCloak(HttpRequest request, [FromForm] IFormFile file, IMapper mapper,
        string userName)
    {
        if (file.Length == 0)
            return Results.BadRequest();

        var tempFile = Path.Combine(SkinHelper.CloakTextureDirectory, $"{userName}.png");

        try
        {
            // Если файл существует, удаляем его
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            // Создаем новый файл и копируем в него данные
            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
            }

            var texture = SkinHelper.Create($"http://{request.Host.Value}", userName);
            return Results.Ok(mapper.Map<UserTextureReadDto>(texture));
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Error saving cloak: {ex.Message}");
        }
    }

    internal static async Task<IResult> DeleteCloak(HttpRequest request, IMapper mapper,
        string userName)
    {
        var filePath = Path.Combine(SkinHelper.CloakTextureDirectory, $"{userName}.png");

        if (!File.Exists(filePath))
            return Results.NotFound();

        File.Delete(filePath);
        return Results.Ok();
    }

    internal static Task<IResult> GetSkin(HttpRequest request, string userName, string? uuid)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName.Substring(2, userName.Length - 2));

        return Task.FromResult(Results.File(user.SkinFullPath, "image/png"));
    }

    internal static Task<IResult> GetUserTexture(HttpRequest request, IMapper mapper, string userName)
    {
        var texture = SkinHelper.Create($"http://{request.Host.Value}", userName);

        return Task.FromResult(Results.Ok(mapper.Map<UserTextureReadDto>(texture)));
    }

    internal static async Task<IResult> RefreshCache(HttpRequest request, IMapper mapper, string userName)
    {
        var enpoints = new[]
        {
            $"/skin/{userName}",
            $"/skin/{userName}/head",
            $"/skin/{userName}/front",
            $"/skin/{userName}/back",
            $"/skin/{userName}/full-back",
            $"/cloak/{userName}"
        };

        await CloudflareUtil.ClearCacheAsync(enpoints);

        return Results.Ok();
    }

    internal static Task<IResult> GetCloak(HttpRequest request, string userName, int? size = 128)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName);

        if (!user.HasCloak) return Task.FromResult(Results.NotFound("Cloak not exists"));

        var image = SkinViewer.SkinViewer.GetCloak(user.CloakFullPath, user, size ?? 128);

        return Task.FromResult(Results.File(image, "image/png"));
    }

    internal static Task<IResult> GetCloakTexture(HttpRequest request, string userName)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName.Substring(2, userName.Length - 2));

        if (!user.HasCloak) return Task.FromResult(Results.NotFound("Cloak not exists"));

        return Task.FromResult(Results.File(user.CloakFullPath, "image/png"));
    }

    internal static Task<IResult> GetSkinHead(HttpRequest request, string userName, int size = 128)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName);

        var image = SkinViewer.SkinViewer.GetHead(user.SkinFullPath, user, size);

        return Task.FromResult(Results.File(image, "image/png"));
    }

    internal static Task<IResult> GetSkinFront(HttpRequest request, string userName, int size = 128)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName);

        var image = SkinViewer.SkinViewer.GetFront(user.SkinFullPath, user, size);

        return Task.FromResult(Results.File(image, "image/png"));
    }

    internal static Task<IResult> GetSkinBack(HttpRequest request, string userName, int size = 128)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName);

        var image = SkinViewer.SkinViewer.GetBack(user, size);

        return Task.FromResult(Results.File(image, "image/png"));
    }

    internal static Task<IResult> GetSkinAndCloakBack(HttpRequest request, string userName, int size = 128)
    {
        var user = SkinHelper.Create($"http://{request.Host.Value}", userName);

        var image = SkinViewer.SkinViewer.GetBack(user, size, true);

        return Task.FromResult(Results.File(image, "image/png"));
    }
}
