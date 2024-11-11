using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public record MessageDto(
    [Required] string Content,
    DateTime Timestamp,
    string SenderUserName);