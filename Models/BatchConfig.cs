using System.Xml.Serialization;

namespace BatchRunner.Models
{
    public class UIConfig
    {
        public string Title { get; set; } = "Batch Runner";
        public string UIDescription { get; set; } = "Run batch operations easily with this simple web interface.";
    }

    public class BatchConfig
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string BatchFilePath { get; set; }
    }

    [XmlRoot("BatchConfigurations")]
    public class BatchConfigurations
    {
        [XmlElement("UIConfig")]
        public UIConfig UIConfig { get; set; } = new();

        [XmlElement("BatchConfig")]
        public List<BatchConfig> Configs { get; set; } = new();
    }
}