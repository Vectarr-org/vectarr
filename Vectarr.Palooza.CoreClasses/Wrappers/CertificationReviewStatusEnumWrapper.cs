using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class CertificationReviewStatusEnumWrapper : BaseEnumClass<CertificationReviewStatusEnum>
    {
        public override string GetDisplayText(CertificationReviewStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case CertificationReviewStatusEnum.Pending:
                    return "Pending";
                case CertificationReviewStatusEnum.Verified:
                    return "Verified";
                case CertificationReviewStatusEnum.Rejected:
                    return "Rejected";
                default:
                    return "";
            }
        }
    }
}
