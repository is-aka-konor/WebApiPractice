using System.Collections.Generic;

namespace WebApiPractice.Api.ResponseStructure
{
    /// <summary>
    /// Response message that will be showed to client
    /// </summary>
    public class ResponseMessage
    {
        public string Code { get; set; }
        public string Summary { get; set; }
        public List<ErrorMessage> Messages { get; set; }

        public ResponseMessage(string code, string summary)
        {
            this.Code = code;
            this.Summary = summary;
            this.Messages = new List<ErrorMessage>();
        }

        public ResponseMessage(string code, string summary, List<ErrorMessage> messages)
        {
            this.Code = code;
            this.Summary = summary;
            this.Messages = messages;
        }
    }
}
