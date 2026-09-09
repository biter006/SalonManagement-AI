namespace SalonManagement.Services;

public class OperationResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public int? EntityId { get; init; }
    public static OperationResult Success(int? id = null) => new() { Succeeded = true, EntityId = id };
    public static OperationResult Failure(string error) => new() { Succeeded = false, Error = error };
}
