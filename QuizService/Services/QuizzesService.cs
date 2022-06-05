using AutoMapper;
using QuizService.Model;
using QuizService.Model.Domain;
using QuizService.Model.ResponseModels;
using QuizService.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static QuizService.Model.QuizResponseModel;

namespace QuizService.Services
{
    public class QuizzesService : IQuizzesService
    {
        private readonly IQuizRepository _repo;
        private readonly IMapper _mapper;

        public QuizzesService(IQuizRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ItemResponse<QuizResponseModel>> GetQuizById(int id)
        {
            var quiz = await _repo.GetQuizByIdAsync(id);

            if (quiz == null)
                return new ItemResponse<QuizResponseModel>
                {
                    Message = "There is no such quiz",
                    Success = false
                };

            var questions = await _repo.GetQuestionsByQuizIdAsync(id);
            var answers = _repo.GetAnswersByQuizId(id);

            var quizToReturn = _mapper.Map<QuizResponseModel>(quiz);
            var questionsToReturn = _mapper.Map<IEnumerable<QuestionItem>>(questions);

            foreach (var question in questionsToReturn)
            {
                question.Answers = _mapper.Map<IEnumerable<AnswerItem>>(answers.Where(x => x.QuestionId == question.Id));
            }

            quizToReturn.Questions = questionsToReturn;

            return new ItemResponse<QuizResponseModel>
            {
                Success = true,
                Data = quizToReturn
            };
        }

        public async Task<ListResponse<QuizResponseModelShort>> GetAllQuizzes()
        {
            var quizzes = await _repo.GetQuizzesAsync();

            if (quizzes.Count() == 0)
            {
                return new ListResponse<QuizResponseModelShort>
                {
                    Message = "There are no quizzes in the Database",
                    Success = false
                };
            }

            var quizzesToReturn = _mapper.Map<IEnumerable<QuizResponseModelShort>>(quizzes);
            return new ListResponse<QuizResponseModelShort>
            {
                Success = true,
                Data = quizzesToReturn
            };
        }
    }
}
