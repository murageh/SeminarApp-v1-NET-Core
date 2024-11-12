namespace SeminarIntegration.DTOs
{
    public class Employee
    {
        public string No { get; set; }
        public string FullName { get; set; }
        public string Resource_No { get; set; }
    }

    public class EmployeesResponse
    {
        public List<Employee> value { get; set; }
    }
}
