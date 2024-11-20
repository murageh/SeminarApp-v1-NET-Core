namespace SeminarIntegration.DTOs
{
    public class Seminar
    {
    }

    public class SeminarRegistrationDto
    {
        public string SemNo { get; set; }
        public string CompanyNo { get; set; }
        public string ParticipantContactNo { get; set; }
        public bool Confirmed { get; set; }
    }

    public class SeminarUpdateDto
    {
        public string SemNo { get; set; }
        public int LineNo { get; set; }
        public bool Confirmed { get; set; }
    }

    public class SemRegistrationODataItem
    {
        public string Document_No { get; set; }
        public int Line_No { get; set; }
        public string Bill_to_Customer_No { get; set; }
        public string Participant_Contact_No { get; set; }
        public string Participant_Name { get; set; }
        public bool To_Invoice { get; set; }
        public DateTime Registration_Date { get; set; }
        public decimal Amount { get; set; }
        public string Confirmation_Status { get; set; }
        public decimal Discount_Amount { get; set; }
    }

    public class SeminarRegistrationRespItem
    {
        public int LineNo { get; set; }
        public string SeminarNo { get; set; }
        public string CompanyNo { get; set; }
        public string ParticipantContactNo { get; set; }
        public string ParticipantName { get; set; }
        public bool ToInvoice { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal Amount { get; set; }
        public string ConfirmationStatus { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
