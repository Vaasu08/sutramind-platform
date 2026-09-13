namespace SutraMind.Application.Contracts;

public sealed record LoginRequest(string Email, string Password, Guid? DeviceId = null);
public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAtUtc, UserSummary User);
public sealed record RefreshTokenRequest(string RefreshToken, Guid DeviceId);
