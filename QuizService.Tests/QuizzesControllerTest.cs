using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
        
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task ScoreBasedOnCorrectAnswersIsCorrect()
    {
        var quiz = new QuizCreateModel("Test Quiz");
        var correctAnswers = new Dictionary<int, int>();
        var score = 0;

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, QuizApiEndPoint), content);
            var quizLocation = response.Headers.Location;

            for (int i = 0; i < 4; i++)
            {
                var question = new QuestionCreateModel($"Question {i + 1} - Test Quiz");
                var contentQuestion = new StringContent(JsonConvert.SerializeObject(question));
                contentQuestion.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseQuestion = await client.PostAsync(new Uri(testHost.BaseAddress, $"{quizLocation}/questions"), contentQuestion);
                var questionLocation = responseQuestion.Headers.Location;

                for (int y = 0; y < 4; y++)
                {
                    var answer = new AnswerCreateModel($"Answer {y + 1} for Question {i + 1} - Test Quiz");
                    var contentAnswer = new StringContent(JsonConvert.SerializeObject(answer));
                    contentAnswer.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var responseAnswer = await client.PostAsync(new Uri(testHost.BaseAddress, $"{questionLocation}/answers"), contentAnswer);
                }

                var quizDetailsResponse = await client.GetAsync(new Uri(testHost.BaseAddress, quizLocation));
                var quizDetails = JsonConvert.DeserializeObject<QuizResponseModel>(await quizDetailsResponse.Content.ReadAsStringAsync());

                var correctAnswer = quizDetails.Questions.ToList()[i].Answers.ToList()[i].Id;
                correctAnswers.Add(quizDetails.Questions.ToList()[i].Id, correctAnswer);

                var questionUpdate = new QuestionUpdateModel
                {
                    Text = $"Question {i + 1} - {quiz}",
                    CorrectAnswerId = correctAnswer
                };

                var contentQuestionUpdate = new StringContent(JsonConvert.SerializeObject(questionUpdate));
                contentQuestionUpdate.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PutAsync(new Uri(testHost.BaseAddress, questionLocation), contentQuestionUpdate);
            }

            var quizResponse = await client.GetAsync(new Uri(testHost.BaseAddress, quizLocation));
            var quizDet = JsonConvert.DeserializeObject<QuizResponseModel>(await quizResponse.Content.ReadAsStringAsync());

            foreach (var question in quizDet.Questions)
            {
                if (correctAnswers[question.Id] == question.CorrectAnswerId)
                {
                    score += 1;
                }
            }

            Assert.Equal(score, correctAnswers.Count());
        }
    }
}