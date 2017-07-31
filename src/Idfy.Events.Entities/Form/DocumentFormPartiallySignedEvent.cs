using System;
using System.Collections.Generic;

namespace Idfy.Events.Entities.Form
{
    public class DocumentFormPartiallySignedEvent
    {
        public Guid DocumentId { get; set; }

        public Guid SigneeRefId { get; set; }

        public Guid SchemaId { get; set; }

        public string Schema { get; set; }

        public string SignName { get; set; }

        public DateTime SignedTime { get; set; }

        public string DateOfBirth { get; set; }

        public string IdentityProviderId { get; set; }

        public string IdentityProvider { get; set; }

        public string SocialSecurityNumber { get; set; }

        public Dictionary<string,string> FormFields { get; set; }

        public string ExternalDocumentId { get; set; }

        public string ExternalSigneeId { get; set; }
    }
}