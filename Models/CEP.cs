namespace CEPAggregator.Models
{
    public class CEP
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public long? CustomId { get; set; }
        public Address Address { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
