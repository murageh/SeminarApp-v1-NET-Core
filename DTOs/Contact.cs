namespace SeminarIntegration.DTOs
{
    public class Contact
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string Company_Name { get; set; }
        public string E_Mail { get; set; }
    }

    public class ContactsResponse
    {
        public List<Contact> value { get; set; }
    }
}