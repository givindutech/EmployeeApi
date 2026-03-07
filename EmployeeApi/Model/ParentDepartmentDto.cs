namespace EmployeeApi.Model
{
    public class ParentDepartmentDto
    {
        public int DepartmentId { get; set; }  // ignore for insert
        public string DepartmentName { get; set; } = null!;
        public string? DepartmentLogo { get; set; }
    }
}
