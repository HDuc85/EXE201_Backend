using Data.Models;

namespace Data.ViewModel.Tag
{

    public class CreateTagDTO
    {
        public string? TagName { get; set; }

        //public virtual ICollection<CreateTagValueDTO> TagValues { get; set; } = new List<CreateTagValueDTO>();
        public List<string>? TagValues { get; set; }
    }
    public class CreateTagValueDTO
    {
        public string? Value { get; set; }
    }
}
