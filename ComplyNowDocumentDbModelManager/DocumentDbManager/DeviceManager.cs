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
using Device = ComplyNowDocumentDbModelManager.Models.Device;

namespace ComplyNowDocumentDbModelManager.DocumentDbManager
{
    public class DeviceManager
    {
        public static Repository<Device> ComplyNowDoumentDbDeviceRepository;
        public static DocumentClient ComplyNowDoumentDbDocumentClient { get; set; }

        /// <summary>
        ///  Initialize repository and client document
        ///  Must be in this order
        /// </summary>
        static DeviceManager()
        {
            ComplyNowDoumentDbDocumentClient = DocumentClientFactory.Create();
            ComplyNowDoumentDbDeviceRepository = new Repository<Device>(ComplyNowDoumentDbDocumentClient, DocumentDbConfig.DocDbDatabase);
        }

        public static async Task<DeviceDto> CreateOrUpdateDevice(string uniqueId)
        {
            // Automapper
            // open the client's connection
            using (DocumentClient client = new DocumentClient(new Uri(DocumentDbConfig.DocDbEndpoint), DocumentDbConfig.DocDbAuth,
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
                    new DocumentCollection { Id = DocumentDbConfig.DeviceCollection },
                    new RequestOptions { OfferThroughput = 1000 });

                // build a gremlin query based on the existence of a name parameter
                //string uniqueId = req.GetQueryNameValuePairs()
                //    .FirstOrDefault(q => string.Compare(q.Key, "UniqueId", true) == 0)
                //    .Value;

            }

            if (ComplyNowDoumentDbDeviceRepository == null)
            {
                ComplyNowDoumentDbDeviceRepository = new Repository<Device>(ComplyNowDoumentDbDocumentClient, DocumentDbConfig.DocDbDatabase);
            }

            Configure();
            var mapper = Config.CreateMapper();

            DeviceDto deviceDto;
            Device device;

            // need to lookup by 10 digit string
            var convertedUniqueId = Convert.ToInt32(uniqueId);
            
            if (convertedUniqueId != 0)
            {
                var listDevices = (await ComplyNowDoumentDbDeviceRepository.GetWhere(d => d.UniqueId == convertedUniqueId.ToString("D10")))
                    .ToList()
                    .OrderBy(d => d.UniqueId)
                    .ToList();
                device = listDevices.FirstOrDefault();
                deviceDto = mapper.Map<Device, DeviceDto>(device);
                if (device != null) return deviceDto;
            }

            device = await CreateNewDevice();
            deviceDto = mapper.Map<Device, DeviceDto>(device);

            return deviceDto;
        }

        public static async Task<Device> CreateNewDevice()
        {
            var uniqueId = 1;
            var lastDocument = await GetLastDevice();
            if (lastDocument != null)
            {
                uniqueId = Convert.ToInt32(lastDocument.UniqueId);
                uniqueId++;
            }

            var device = new Device()
            {
                UniqueId = uniqueId.ToString("D10"),
                HardwareVersion = "1.0",
                FirmwareVersion = "2.0",
                PatientId = 0,
                DeviceType = "DeviceController",
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now,
                ComplianceResponses = new List<ComplianceResponse>()
            };

            var resultDevice = await ComplyNowDoumentDbDeviceRepository.Create(device);

            return resultDevice;
        }

        private static async Task<Device> GetLastDevice()
        {
            if (ComplyNowDoumentDbDeviceRepository == null)
            {
                ComplyNowDoumentDbDeviceRepository = new Repository<Device>(ComplyNowDoumentDbDocumentClient, DocumentDbConfig.DocDbDatabase);
            }
            var lastDocument = await ComplyNowDoumentDbDeviceRepository.GetLastDocument();
            return lastDocument;
        }

    }
}
