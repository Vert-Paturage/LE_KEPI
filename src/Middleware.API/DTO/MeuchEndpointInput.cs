using System.Text.Json;

namespace Middleware.API.DTO;

public sealed class MeuchEndpointInput
{
    public required string Key { get; init; }
    public required string Endpoint { get; init; }
    public required string Description { get; init; }
    public required string Type { get; init; }
    public string? RouteFormat { get; init; }
    public string[]? QueryParams { get; init; }
    public string? Body { get; init; }
}