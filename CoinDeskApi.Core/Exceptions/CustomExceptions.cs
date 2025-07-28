namespace CoinDeskApi.Core.Exceptions
{
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException(string currencyId) 
            : base($"Currency with ID '{currencyId}' was not found.")
        {
        }
    }

    public class CurrencyAlreadyExistsException : Exception
    {
        public CurrencyAlreadyExistsException(string currencyId) 
            : base($"Currency with ID '{currencyId}' already exists.")
        {
        }
    }

    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message) : base(message)
        {
        }

        public ExternalApiException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
