using System.Collections.Generic;

namespace exporterImage.Models
{
    public class Template
    {
        public string Titulo { get; set; }
        public List<TemplateHeader> Headers { get; set; }
        public List<TemplateData> Data { get; set; }
    }

    public class TemplateHeader
    {
        public string Name { get; set; }
    }

    public class TemplateData
    {
        public List<TemplateDataProperties> Properties { get; set; }
    }

    public class TemplateDataProperties
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}