namespace EmployeeApi.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }
        public int DeptId { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
