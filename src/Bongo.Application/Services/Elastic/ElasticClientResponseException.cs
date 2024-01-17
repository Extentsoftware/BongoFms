namespace Bongo.Application.Services.Elastic
{
    public class ElasticClientResponseException : Exception
    {
        public ElasticClientResponseException()
        {
        }

        public ElasticClientResponseException(string message) : base(message)
        {
        }

        public ElasticClientResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
