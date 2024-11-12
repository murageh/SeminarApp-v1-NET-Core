namespace SeminarIntegration.DTOs
{
    public class Customer
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string Post_Code { get; set; }
        public string Country_Region_Code { get; set; }
        public string Phone_No { get; set; }
        public string Contact { get; set; }
        public string Currency_Code { get; set; }
        public string Blocked { get; set; }
        public decimal Balance_LCY { get; set; }
        public decimal Balance_Due_LCY { get; set; }
        public decimal Sales_LCY { get; set; }
        public decimal Payments_LCY { get; set; }
    }
    
    public class CustomerResponse
    {
        public List<Customer> value { get; set; }
    }
}
