namespace Vectarr.Palooza.Classes
{
    public class BaseModel : IBaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsHidden { get; set; }
        protected BaseModel()
        {
            LastModified = DateTime.UtcNow;
            CreatedDate = DateTime.UtcNow;
        }
    }
}
