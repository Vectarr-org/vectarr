using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class QualityCheckTypeEnumWrapper : BaseEnumClass<QualityCheckTypeEnum>
    {
        public override string GetDisplayText(QualityCheckTypeEnum enumValue)
        {
            return enumValue switch
            {
                QualityCheckTypeEnum.DimensionalInspection => "Dimensional Inspection",
                QualityCheckTypeEnum.SurfaceFinish => "Surface Finish",
                QualityCheckTypeEnum.VisualInspection => "Visual Inspection",
                QualityCheckTypeEnum.MaterialVerification => "Material Verification",
                QualityCheckTypeEnum.FunctionalTest => "Functional Test",
                QualityCheckTypeEnum.DocumentationReview => "Documentation Review",
                _ => throw new ArgumentOutOfRangeException(nameof(enumValue))
            };
        }

        public string GetDisplayDescription(QualityCheckTypeEnum enumValue)
        {
            return enumValue switch
            {
                QualityCheckTypeEnum.DimensionalInspection => "Verify dimensions match specifications",
                QualityCheckTypeEnum.SurfaceFinish => "Check surface quality and finish",
                QualityCheckTypeEnum.VisualInspection => "General visual quality check",
                QualityCheckTypeEnum.MaterialVerification => "Confirm correct material used",
                QualityCheckTypeEnum.FunctionalTest => "Test part functionality",
                QualityCheckTypeEnum.DocumentationReview => "Review certifications and documents",
                _ => throw new ArgumentOutOfRangeException(nameof(enumValue))
            };
        }
    }
}
