using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class ConditionEnumWrapper : BaseEnumClass<ConditionEnum>
    {
        public override string GetDisplayText(ConditionEnum enumValue)
        {
            switch (enumValue)
            {
                case ConditionEnum.Excellent:
                    return "Excellent";
                case ConditionEnum.Good:
                    return "Good";
                case ConditionEnum.Damaged:
                    return "Damaged";
                default:
                    return "";
            }
        }
    }
}
