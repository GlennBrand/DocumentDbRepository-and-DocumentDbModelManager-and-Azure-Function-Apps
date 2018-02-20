using System;
using Microsoft.Azure.Documents.Client;

namespace ComplyNowDocumentDbModelManager.Helpers
{
    internal static class DocumentClientFactory
    {
        public static DocumentClient Create()
        {
            return new DocumentClient(new Uri(DocumentDbConfig.DocDbEndpoint), DocumentDbConfig.DocDbAuth);
        }
    }
}
