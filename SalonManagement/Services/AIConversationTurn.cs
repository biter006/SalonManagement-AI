namespace SalonManagement.Services;

/// <summary>One non-persistent chat turn kept in the current staff session only.</summary>
public sealed record AIConversationTurn(string Role, string Text, bool UsedFallback = false);
