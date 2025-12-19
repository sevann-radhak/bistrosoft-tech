namespace Bistrosoft.API.DTOs;

public record LoginResponseDto(
    string Token,
    string TokenType,
    int ExpiresIn
);

