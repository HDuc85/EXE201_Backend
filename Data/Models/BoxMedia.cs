namespace Data.Models
{
    public class BoxMedia
    {
        public int Id { get; set; }

        public int? BoxId { get; set; }

        public int? MediaId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Box? Box { get; set; }

        public virtual Media? Media { get; set; }
    }
}
