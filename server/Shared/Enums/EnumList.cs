using System.Reflection.Emit;

namespace Shared.Enums
{

    public enum TestEnumList { Low, Medium, High, Critical }

    public enum UserRole { Requestor, Manager, FinanceAdmin, SystemAdmin }

    public enum RequestStatus { Draft, PendingManagerApproval, PendingFinanceReview, Approved, Rejected, Cancelled, NeedClarification }

    public enum SponsorshipType { Event, Conference, Research, Community, Other }

}
