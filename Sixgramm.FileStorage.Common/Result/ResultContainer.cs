using Sixgramm.FileStorage.Common.Error;

namespace Sixgramm.FileStorage.Common.Result
{
    public class ResultContainer<T>
    {
        public T Data { get; set; }
        public ErrorType? ErrorType { get; set; }
    }
}