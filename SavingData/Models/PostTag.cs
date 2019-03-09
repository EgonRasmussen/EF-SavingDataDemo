namespace SavingData.Models
{
    public class PostTag
    {
        public int PostTagId { get; set; }

        public int PostId { get; set; }         // FK
        public string TagId { get; set; }       // FK
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}
