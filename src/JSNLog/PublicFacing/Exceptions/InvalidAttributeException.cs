namespace JSNLog.Exceptions
{
    public class InvalidAttributeException : JSNLogException
    {
        public InvalidAttributeException(string invalidValue) : 
            base(string.Format("Invalid value {0}", invalidValue))
        {
        }
    }
}
