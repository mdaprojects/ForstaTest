using QuizService.Model;
using QuizService.Model.ResponseModels;
using System.Threading.Tasks;

namespace QuizService.Services
{
    public interface IQuizzesService
    {
        Task<ListResponse<QuizResponseModelShort>> GetAllQuizzes();
        Task<ItemResponse<QuizResponseModel>> GetQuizById(int id);
    }
}
