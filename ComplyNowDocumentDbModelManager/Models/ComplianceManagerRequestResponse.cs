using System.Net;

namespace ComplyNowDocumentDbModelManager.Models
{
    public class ComplianceManagerRequestResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public bool IsAlreadySavedStatus { get; set; }

        public string ResponseMessage { get; set; }

        public int RecordsSaved { get; set; }
    }
}