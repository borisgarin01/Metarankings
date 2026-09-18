namespace Domain.Auth;

public sealed record VkidLinkState(string UserId, string CodeVerifier);
