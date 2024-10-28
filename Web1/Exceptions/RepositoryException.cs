namespace Web1.Exceptions
{
    // Xử lý lỗi từ tầng Repository để đưa lên tầng Controller thông báo cho Client
    public class RepositoryException : Exception
    {
        public RepositoryException() { }

        public RepositoryException(string message) : base(message) { }

        public RepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
