using Newtonsoft.Json.Linq;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class email
    {
        public static JObject MapFrom(Email email)
        {
            if (email is null)
                return null;

            var jObject = new JObject
            {
                { "subject", email.Subject },
                { "description", email.Description },
            };

            var parties = new JArray();
            var from = new JObject
            {
                { "partyid_systemuser@odata.bind", $"/systemusers({email.From.Id})" },
                { "participationtypemask", 1 }
            };
            parties.Add(from);

            foreach (var recipient in email.To)
            {
                var to = new JObject
                {
                    { "partyid_contact@odata.bind", $"/contacts({recipient.Id})" },
                    { "participationtypemask", 2 }
                };
                parties.Add(to);
            }

            foreach (var recipient in email.Cc)
            {
                var cc = new JObject
                {
                    { "addressused", recipient },
                    { "participationtypemask", 3 }
                };
                parties.Add(cc);
            }

            jObject["email_activity_parties"] = parties;

            return jObject;
        }
    }
}
