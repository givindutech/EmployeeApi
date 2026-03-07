namespace EmployeeApi.Model
{
    public class ParentDepartment
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string? DepartmentLogo { get; set; } = null!; // storing logo path/base64
        public List<ChildDepartment> ChildDepartments { get; set; } = new();
    }
}
