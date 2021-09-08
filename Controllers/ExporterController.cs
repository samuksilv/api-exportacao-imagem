using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreHtmlToImage;
using exporterImage.Models;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace ExporterImage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExporterController : ControllerBase
    {
        private readonly ILogger<ExporterController> _logger;

        public ExporterController(ILogger<ExporterController> logger)
        {
            _logger = logger;
        }

        private string GenerateTemplateHtml()
        {
            Template data = GenerateDataTemplate();

            string templateStr = System.IO.File.ReadAllText("Templates/index.handlebars");
            string templateTabelaStr = System.IO.File.ReadAllText("Templates/tabela.handlebars");
            string templateStylesStr = System.IO.File.ReadAllText("Templates/styles.handlebars");

            var template = Handlebars.Compile(templateStr);

            Handlebars.RegisterTemplate("tabela", templateTabelaStr);
            Handlebars.RegisterTemplate("styles", templateStylesStr);

            string templateHtml = template(data);

            return templateHtml;
        }

        private Template GenerateDataTemplate()
        {
            string[] headers = new string[] {
                "Index",
                "Nome",
                "Idade",
                "Cpf",
                "Rg",
                "Cidade",
                "Uf",
                "Telefone",
                "Email",
                "Cep",
            };

            Template template = new Template
            {
                Titulo = "Algum Título Qualquer - apenas mais um teste",
                Data = new List<TemplateData>(),
                Headers = headers.Select(header => new TemplateHeader { Name = header }).ToList(),
            };

            for (int i = 0; i < 1800; i++)
            {
                var data = new TemplateData
                {
                    Properties = new List<TemplateDataProperties>
                    {
                        new TemplateDataProperties
                        {
                            Name = "Index",
                            Value = i,
                        },
                        new TemplateDataProperties
                        {
                            Name = "Nome",
                            Value = "Hagamenon Frederico Paulista Sanches",
                        },
                        new TemplateDataProperties{
                            Name = "Idade",
                            Value = "23",
                        },
                        new TemplateDataProperties{
                            Name = "Cpf",
                            Value = "468.897.687-98"
                        },
                        new TemplateDataProperties{
                            Name = "Rg",
                            Value = "35.433.687-9"
                        },
                        new TemplateDataProperties{
                            Name = "Cidade",
                            Value = "Ribeirão Preto"
                        },
                        new TemplateDataProperties{
                            Name = "Uf",
                            Value = "SP"
                        },
                        new TemplateDataProperties{
                            Name = "Telefone",
                            Value="(18)96658-8596",
                        },
                        new TemplateDataProperties{
                            Name = "Email",
                            Value="samuel@gmail.com",
                        },
                        new TemplateDataProperties{
                            Name = "Cep",
                            Value = "14085-070",
                        },
                    },
                };

                template.Data.Add(data);
            }

            return template;
        }


        [HttpGet("{tipoExportacao}")]
        public async Task<IActionResult> ExportJpgAsync(ScreenshotType tipoExportacao)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            string templateHtml = GenerateTemplateHtml();

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                DefaultViewport = new ViewPortOptions
                {
                    Height = 1080,
                    Width = 1440,
                    HasTouch = false,
                    IsLandscape = false,
                    IsMobile = false,
                }
            }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.SetContentAsync(templateHtml, new NavigationOptions
                    {
                        Timeout = 0,
                        WaitUntil = new WaitUntilNavigation[]
                        {
                            WaitUntilNavigation.DOMContentLoaded,
                            WaitUntilNavigation.Load,
                            WaitUntilNavigation.Networkidle2,
                            WaitUntilNavigation.Networkidle0,
                        },
                    });

                    string filePath = $"Output/{DateTime.Now.ToString("yyyyMMddhhmmss")}.{tipoExportacao}";
                    string filePathHtml = $"Output/{DateTime.Now.ToString("yyyyMMddhhmmss")}.html";

                    Task taskImg = page.ScreenshotAsync(filePath,
                        new ScreenshotOptions
                        {
                            Type = tipoExportacao,
                            Quality = tipoExportacao == ScreenshotType.Jpeg ? 100 : null,
                            FullPage = true,
                        });

                    Task taskHtml = System.IO.File.WriteAllTextAsync(filePathHtml, templateHtml);

                    await Task.WhenAll(taskImg, taskHtml);

                    return Ok(new
                    {
                        templateHtml = new FileInfo(filePathHtml).FullName,
                        imagem = new FileInfo(filePath).FullName,
                    });
                }
            }

        }


        [HttpGet("test")]
        public async Task<IActionResult> Teste2()
        {
            var converter = new HtmlConverter();
            string templateHtml = GenerateTemplateHtml();

            string filePath = $"Output/{DateTime.Now.ToString("yyyyMMddhhmmss")}.png";
            // string filePath = $"Output/{DateTime.Now.ToString("yyyyMMddhhmmss")}.jpg";
            string filePathHtml = $"Output/{DateTime.Now.ToString("yyyyMMddhhmmss")}.html";

            var bytes = converter.FromHtmlString(templateHtml, 1024, ImageFormat.Png, 100);
            // var bytes = converter.FromHtmlString(templateHtml);

            Task taskImg = System.IO.File.WriteAllBytesAsync(filePath, bytes);
            Task taskHtml = System.IO.File.WriteAllTextAsync(filePathHtml, templateHtml);

            await Task.WhenAll(taskImg, taskHtml);

            return Ok(new
            {
                templateHtml = new FileInfo(filePathHtml).FullName,
                imagem = new FileInfo(filePath).FullName,
            });
        }
    }
}
