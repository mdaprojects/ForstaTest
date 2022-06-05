using AutoMapper;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Collections.Generic;
using static QuizService.Model.QuizResponseModel;

namespace QuizService.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Quiz, QuizResponseModelShort>();
            CreateMap<Quiz, QuizResponseModel>()
                .ForMember(dest => dest.Links, src => src
                    .MapFrom(
                        x => new Dictionary<string, string>
                        {
                            {"self", $"/api/quizzes/{x.Id}"},
                            {"questions", $"/api/quizzes/{x.Id}/questions"}
                        }));

            CreateMap<Question, QuestionItem>();
            CreateMap<Answer, AnswerItem>();
        }
    }
}
