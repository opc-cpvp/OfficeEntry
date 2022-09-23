using HandlebarsDotNet;
using Microsoft.Extensions.FileProviders;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services
{
    internal class TemplateService : ITemplateService
    {
        private const string TemplatesDirectory = "templates";
        private const string EmailLayoutTemplate = "EmailLayout";
        private const string EmailContentPlaceholder = "Content";

        private readonly IFileProvider _fileProvider;

        public TemplateService(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public string GetTemplate(string name)
        {
            var path = _fileProvider.GetFileInfo(Path.Combine(TemplatesDirectory, $"{name}.hbs"));
            using var reader = new StreamReader(path.CreateReadStream());
            return reader.ReadToEnd();
        }

        public string GetEmailTemplate(string name, EmailTemplate data)
        {
            var layoutTemplate = GetTemplate(EmailLayoutTemplate);
            var contentTemplate = GetTemplate(name);

            Handlebars.RegisterHelper("formatDate", (writer, context, parameters) =>
            {
                var date = parameters[0] as DateTime?;
                if (date.HasValue)
                    writer.WriteSafeString(date.Value.ToString("yyyy-MM-dd"));
            });

            Handlebars.RegisterHelper("formatTime", (writer, context, parameters) =>
            {
                var date = parameters[0] as DateTime?;
                if (date.HasValue)
                    writer.WriteSafeString(date.Value.ToString("HH:mm"));
            });

            Handlebars.RegisterTemplate(EmailContentPlaceholder, contentTemplate);
            var template = Handlebars.Compile(layoutTemplate);

            return template(data);
        }
    }
}
