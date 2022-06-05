using Dapper;
using QuizService.Model.Domain;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace QuizService.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly IDbConnection _connection;

        public QuizRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Answer> GetAnswersByQuizId(int quizId)
        {
            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
            return _connection.Query<Answer>(answersSql, new { QuizId = quizId });
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
            return await _connection.QueryAsync<Question>(questionsSql, new { QuizId = quizId });
        }

        public async Task<Quiz> GetQuizByIdAsync(int id)
        {
            const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
            return await _connection.QuerySingleAsync<Quiz>(quizSql, new { Id = id });
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesAsync()
        {
            const string sql = "SELECT * FROM Quiz;";
            return await _connection.QueryAsync<Quiz>(sql);
        }
    }
}
