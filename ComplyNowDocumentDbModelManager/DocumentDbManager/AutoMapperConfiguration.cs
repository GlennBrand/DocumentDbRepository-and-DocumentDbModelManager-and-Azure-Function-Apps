// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperConfiguration.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AutoMapperConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ComplyNowDocumentDbModelManager.DocumentDbManager
{
    using AutoMapper;
    using ComplyNowDocumentDbModelManager.Models;

    /// <summary>
    /// The auto mapper configuration.
    /// </summary>
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Gets or sets the config.
        /// </summary>
        public static MapperConfiguration Config { get; set; }

        /// <summary>
        /// The configuration of Automapper once for the App Domain.
        /// </summary>
        public static void Configure()
        {
            if (Config == null)
            {
                Config = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<Device, DeviceDto>().ReverseMap();
                        cfg.CreateMap<ComplianceEntry, ComplianceEntryDto>().ReverseMap();
                        cfg.CreateMap<ComplianceResponse, ComplianceResponseDto>().ReverseMap();
                    });
            }
        }
    }
}