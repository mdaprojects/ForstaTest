using System.Collections.Generic;

namespace QuizService.Model.ResponseModels
{
    public class ListResponse<T> : BaseResponse
    {
        public IEnumerable<T> Data { get; set; }
    }
}
