using System;
using System.Collections.Generic;

namespace exporterImage.Models
{
    public class Template
    {
        public string Titulo { get; set; }
        public string UrlLogo { get; set; }
        public string TituloCabecalho { get; set; }
        public DateTime? DataGeracao { get; set; }
        public string DataGeracaoTexto => DataGeracao?.ToString("dd/MM/yyyy HH:mm:ss");
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