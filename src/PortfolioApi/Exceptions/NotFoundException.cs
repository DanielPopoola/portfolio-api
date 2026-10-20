namespace PortfolioApi.Exceptions;

public class NotFoundException(string detail) : AppException(404, "Not found", detail);