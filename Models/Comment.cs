namespace CEPAggregator.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public CEP CEP { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
    }
}
