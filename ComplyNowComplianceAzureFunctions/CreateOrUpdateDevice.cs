using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ComplyNowDocumentDbModelManager.DocumentDbManager;
using ComplyNowDocumentDbModelManager.Models;
using Santhos.DocumentDb.Repository;

namespace ComplyNowComplianceAzureFunctions
{
    public static class CreateOrUpdateDevice
    {

        [FunctionName("CreateOrUpdateDevice")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // the person objects are free-form in structure
            List<dynamic> results = new List<dynamic>();

            List<DeviceDto> listDevices = new List<DeviceDto>();
            DeviceDto deviceDto;

            var uniqueId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "UniqueId", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            log.Info("C# HTTP trigger function received UniqueId={0}.", uniqueId);

            deviceDto = await DeviceManager.CreateOrUpdateDevice(uniqueId);
            results.Add(deviceDto);

            log.Info("C# HTTP trigger function returned device UniqueId {0}.", deviceDto.UniqueId);

            return req.CreateResponse<List<dynamic>>(deviceDto.UniqueId.Length == 10 ? HttpStatusCode.OK : HttpStatusCode.NoContent, results);

            // return the list with an OK response or NoContent if cannot be created
        }

    }
}
