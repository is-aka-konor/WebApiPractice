namespace WebApiPractice.Api.ResponseStructure
{
    /// <summary>
    /// This class describes error messages that will be exposed to API consumers
    /// </summary>
    public class ErrorMessage
    {
        public string Property { get; set; }
        public string Description { get; set; }
        public ErrorMessage(string property, string description)
        {
            this.Property = !string.IsNullOrEmpty(property) ? property : string.Empty;
            this.Description = description;
        }
    }
}
