namespace WebApiPractice.Api.ResponseStructure
{
    /// <summary>
    /// Response's meta-data for response pagination
    /// </summary>
    public record ResponseMetadata(string NextCursor, bool HasNext);
}
