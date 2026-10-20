namespace PortfolioApi.Exceptions;

public class ConflictException(string title, string detail) : AppException(409, title, detail);