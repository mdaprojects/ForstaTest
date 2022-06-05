using QuizService.Model.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizService.Repositories
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetQuizzesAsync();
        Task<Quiz> GetQuizByIdAsync(int id);
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        IEnumerable<Answer> GetAnswersByQuizId(int quizId);
    }
}
