using Api.Dto;
using Domain;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService(
    AppDbContext context,
    FileRepository files,
    UserManager<User> userManager)
{
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        var usersList = await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
        
        return usersList;
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Avatar)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        
        return user;
    }
    
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Avatar)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserName == username, cancellationToken);
        
        return user;
    }

    [HttpPut]
    public async Task UpdateAsync([FromForm] UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(dto.Id.ToString());
        if(user is null) throw new NotFoundException(nameof(User), dto.Id);
        
        if (dto.File is not null)
        {
            var avatar = await EnsureCreatedUserAvatarAsync(user.Id, dto.File.FileName, cancellationToken);
            await files
                .PutFileAsync(dto.File, avatar, BucketConstants.Avatar, cancellationToken);
            user.Avatar = avatar;
        }
        
        dto.Adapt(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<UserAvatar> EnsureCreatedUserAvatarAsync(Guid userId, string filename, CancellationToken cancellationToken)
    {
        var avatar = await context.UserAvatars.FirstOrDefaultAsync(avatar => avatar.UserId == userId, cancellationToken);
        if (avatar is not null) return avatar;
        
        var fileId = Guid.NewGuid();
        var fileExtension = Path.GetExtension(filename);
        avatar = new UserAvatar
        {
            Filename = $"{fileId}{fileExtension}",
            UserId = userId,
            Bucket = BucketConstants.Avatar,
        };
        await context.Files.AddAsync(avatar, cancellationToken);

        return avatar;
    }
}