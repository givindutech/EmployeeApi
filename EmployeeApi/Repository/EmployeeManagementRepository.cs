using EmployeeApi.Helper;

namespace EmployeeApi.Repository
{
    public class EmployeeManagementRepository
    {
        public readonly DataBaseContext _dataBaseContext;

        public EmployeeManagementRepository(DataBaseContext dataBaseContext) 
        {
            _dataBaseContext = dataBaseContext;
        }

    }
}
