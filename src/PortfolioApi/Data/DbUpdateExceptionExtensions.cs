using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PortfolioApi.Data;

public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException ex)
        => ex.InnerException is SqlException { Number: 2601 or 2627 };  
}
