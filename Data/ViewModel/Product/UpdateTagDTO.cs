using Data.Models;

namespace Data.ViewModel.Tag
{

    public class UpdateTagDTO
    {
        public string? TagName { get; set; }

        //public virtual ICollection<CreateTagValueDTO> TagValues { get; set; } = new List<CreateTagValueDTO>();
        public List<string>? TagValues { get; set; }
    }
}
