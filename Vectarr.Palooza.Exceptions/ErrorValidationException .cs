namespace Vectarr.Palooza.Exceptions
{
    public class ErrorValidationException : Exception
    {
        public ErrorValidationException(string message) : base(message)
        {

        }

        public ErrorValidationException(string message, params object[] parameters)
            : base(string.Format(message, parameters))
        {

        }
    }
}