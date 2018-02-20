using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ComplyNowDocumentDbModelManager.Helpers;
using ComplyNowDocumentDbModelManager.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Santhos.DocumentDb.Repository;
using static ComplyNowDocumentDbModelManager.DocumentDbManager.AutoMapperConfiguration;

namespace ComplyNowDocumentDbModelManager.DocumentDbManager
{
    public class ComplianceManager
    {
        public static Repository<ComplianceResponseSet> ComplyNowDoumentDbComplianceResponseRepository;
        public static DocumentClient ComplyNowDoumentDbDocumentClient { get; set; }

        /// <summary>
        ///  Initialize repository and client document
        ///  Must be in this order
        /// </summary>
        static ComplianceManager()
        {
            ComplyNowDoumentDbDocumentClient = DocumentClientFactory.Create();
            ComplyNowDoumentDbComplianceResponseRepository = new Repository<ComplianceResponseSet>(ComplyNowDoumentDbDocumentClient, DocumentDbConfig.DocDbDatabase);
        }

        public static async Task<ComplianceManagerRequestResponse> CreateOrUpdateCompliance(string uniqueId, List<ComplianceResponseDto> listComplianceResponseDtos)
        {
            // open the client's connection
            using (var client = new DocumentClient(new Uri(DocumentDbConfig.DocDbEndpoint),
                DocumentDbConfig.DocDbAuth,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                }))
            {
                // get a reference to the database the console app created
                Database database = await client.CreateDatabaseIfNotExistsAsync(
                    new Database
                    {
                        Id = DocumentDbConfig.DocDbDatabase
                    });

                // get an instance of the database's graph
                DocumentCollection deviceCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(DocumentDbConfig.DocDbDatabase),
                    new DocumentCollection {Id = DocumentDbConfig.DeviceCollection},
                    new RequestOptions {OfferThroughput = 1000});
            }

            if (ComplyNowDoumentDbComplianceResponseRepository == null)
            {
                ComplyNowDoumentDbComplianceResponseRepository =
                    new Repository<ComplianceResponseSet>(ComplyNowDoumentDbDocumentClient,
                        DocumentDbConfig.DocDbDatabase);
            }

            Configure();
            var mapper = Config.CreateMapper();

            var complianceManagerRequestResponse = new ComplianceManagerRequestResponse();
            var firstComplianceResponseDto = listComplianceResponseDtos.FirstOrDefault();
            if (firstComplianceResponseDto == null)
            {
                complianceManagerRequestResponse.IsAlreadySavedStatus = false;
                complianceManagerRequestResponse.ResponseMessage = "No Compliance Data records received from CN Unit with Unique ID : " + uniqueId;
                complianceManagerRequestResponse.HttpStatusCode = HttpStatusCode.NoContent;
                return complianceManagerRequestResponse;
            }

            var savedComplianceResponseSet = (await ComplyNowDoumentDbComplianceResponseRepository.GetWhere(d =>
                    d.Device.UniqueId.Equals(firstComplianceResponseDto.Device.UniqueId) &&
                    d.StartDateTime == firstComplianceResponseDto.StartDateTime))
                .ToList()
                .OrderBy(d => d.StartDateTime)
                .ToList();

            var complianceResponseSet = savedComplianceResponseSet.FirstOrDefault();
            if (complianceResponseSet != null)
            {
                complianceManagerRequestResponse.IsAlreadySavedStatus = true;
                complianceManagerRequestResponse.ResponseMessage = @"Compliance Data already received for CN Unit with Unique ID : " + uniqueId;
                complianceManagerRequestResponse.HttpStatusCode = HttpStatusCode.Found;
                return complianceManagerRequestResponse;
            }

            var listComplianceResponses = new List<ComplianceResponse>();
            foreach (var complianceResponseDto in listComplianceResponseDtos)
            {
                var complianceResponseMapped =
                    mapper.Map<ComplianceResponseDto, ComplianceResponse>(complianceResponseDto);
                listComplianceResponses.Add(complianceResponseMapped);
            }

            var firstComplianceResponse = listComplianceResponses.FirstOrDefault();
            if (firstComplianceResponse != null)
            {
                complianceResponseSet = new ComplianceResponseSet
                {
                    ComplianceResponses = listComplianceResponses,
                    StartDateTime = firstComplianceResponse.StartDateTime,
                    Device = firstComplianceResponse.Device
                };
                complianceManagerRequestResponse = await CreateComplianceDocument(complianceResponseSet);
            }
            else
            {
                complianceManagerRequestResponse.ResponseMessage = "No Compliance Data records received from CN Unit with Unique ID : " + uniqueId;
            }
            return complianceManagerRequestResponse;
        }

        public static async Task<ComplianceManagerRequestResponse> CreateComplianceDocument(ComplianceResponseSet complianceResponseSet)
        {
            var complianceManagerRequestResponse = new ComplianceManagerRequestResponse { IsAlreadySavedStatus = false };

            var resultComplianceResponse = await ComplyNowDoumentDbComplianceResponseRepository.Create(complianceResponseSet);

            if (resultComplianceResponse != null)
            {
                complianceManagerRequestResponse.RecordsSaved = complianceResponseSet.ComplianceResponses.Count;
            }

            if (complianceManagerRequestResponse.RecordsSaved > 0)
            {
                complianceManagerRequestResponse.ResponseMessage =
                    @"Saved " + complianceManagerRequestResponse.RecordsSaved + " ComplianceResponse records.";
                complianceManagerRequestResponse.HttpStatusCode = HttpStatusCode.OK;
            }
            else
            {
                complianceManagerRequestResponse.ResponseMessage =
                    @"Saved " + complianceManagerRequestResponse.RecordsSaved + " ComplianceResponse records.";
                complianceManagerRequestResponse.HttpStatusCode = HttpStatusCode.SeeOther;
            }

            return complianceManagerRequestResponse;
        }

    }
}
