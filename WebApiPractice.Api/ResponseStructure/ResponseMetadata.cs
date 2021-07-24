namespace WebApiPractice.Api.ResponseStructure
{
    /// <summary>
    /// Response's meta-data for response pagination
    /// </summary>
    public class ResponseMetadata
    {
        public string NextCursor { get; set; } = string.Empty;
        public bool HasNext { get; set; }
    }
}
