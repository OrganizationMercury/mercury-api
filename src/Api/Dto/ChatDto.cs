using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Models;

namespace Api.Dto;

public record ChatDto(
    Guid Id,
    ChatType Type,
    [MaxLength(40)] string? Name);

public class ChatWithAvatarDto
{
    public Guid Id { get; set; }
    public ChatType Type { get; set; }
    [MaxLength(40)] public string? Name { get; set; }
    public string? Avatar { get; set; }
}