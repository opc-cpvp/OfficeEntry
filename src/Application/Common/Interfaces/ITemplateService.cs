using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ITemplateService
    {
        public string GetTemplate(string name);
        public string GetEmailTemplate(string name, EmailTemplate data);
    }
}
