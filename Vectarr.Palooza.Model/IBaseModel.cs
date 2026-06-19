namespace Vectarr.Palooza.Model
{
    public interface IBaseModel
    {
        int Id { get; set; }
        DateTime LastModified { get; set; }
        DateTime CreatedDate { get; set; }
        bool IsRemoved { get; set; }
        bool IsHidden { get; set; }
    }
}