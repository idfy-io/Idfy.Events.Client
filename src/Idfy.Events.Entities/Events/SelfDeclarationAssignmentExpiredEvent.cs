using System;

namespace Idfy.Events.Entities
{
    public class SelfDeclarationAssignmentExpiredEvent : Event<SelfDeclarationAssignmentExpiredPayload>
    {
        public SelfDeclarationAssignmentExpiredEvent(SelfDeclarationAssignmentExpiredPayload payload, Guid accountId) 
            : base(EventType.SelfDeclarationAssignmentExpired, payload, accountId)
        {
        }
    }
}