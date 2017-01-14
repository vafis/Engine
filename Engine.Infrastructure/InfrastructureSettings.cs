using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Engine.Infrastructure.Messaging;

namespace Engine.Infrastructure
{
    /// <summary>
    /// Settings class used for Azure Service Bus configuration 
    /// </summary>
    [XmlRoot("InfrastructureSettings", Namespace = XmlNamespace)]
    public class InfrastructureSettings
    {
        public const string XmlNamespace = @"urn:engine";
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(InfrastructureSettings));
        private static readonly XmlReaderSettings _readerSettings;

        static InfrastructureSettings()
        {
            var schema = XmlSchema.Read(typeof(InfrastructureSettings).Assembly.GetManifestResourceStream("Engine.Infrastructure.Settings.xsd"), null);
            _readerSettings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            _readerSettings.Schemas.Add(schema);
        }

        /// <summary>
        /// Reads settings from the specified file
        /// </summary>
        public static InfrastructureSettings Read(string file)
        {
            using (var reader = XmlReader.Create(file, _readerSettings))
            {
                return (InfrastructureSettings) _serializer.Deserialize(reader);
            }
        }

        public ServiceBusSettings ServiceBus { get; set; }

       
    }
}
