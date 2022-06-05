namespace QuizService.Model.ResponseModels
{
    public class ItemResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
