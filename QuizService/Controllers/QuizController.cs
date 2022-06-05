using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Linq;
using QuizService.Services;
using System.Threading.Tasks;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : Controller
{
    /*
     * TODO I would use a repository for dealing with the database and a service for the business logic everywhere, 
     * because that way the code is much more readable and maintainable when the project becomes huge
    */

    private readonly IDbConnection _connection;
    private readonly IQuizzesService _service;

    public QuizController(IDbConnection connection, IQuizzesService service)
    {
        _connection = connection;
        _service = service;
    }

    // GET api/quizzes
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetAllQuizzes();

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return NotFound();
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetQuizById(id);

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return NotFound();
    }

    // POST api/quizzes
    [HttpPost]
    public IActionResult Post([FromBody] QuizCreateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains neded data and a proper message
         * for the client if process is done successfully or not
         */
        var sql = $"INSERT INTO Quiz (Title) VALUES('{value.Title}'); SELECT LAST_INSERT_ROWID();";
        var id = _connection.ExecuteScalar(sql);
        return Created($"/api/quizzes/{id}", null);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] QuizUpdateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains data if needed and a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
        int rowsUpdated = _connection.Execute(sql, new { Id = id, Title = value.Title });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        /*
         * TODO 1. I would return a specific DTO which contains a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "DELETE FROM Quiz WHERE Id = @Id";
        int rowsDeleted = _connection.Execute(sql, new { Id = id });
        if (rowsDeleted == 0)
            return NotFound();
        return NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public IActionResult PostQuestion(int id, [FromBody] QuestionCreateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains data if needed and a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";
        var questionId = _connection.ExecuteScalar(sql, new { Text = value.Text, QuizId = id });
        return Created($"/api/quizzes/{id}/questions/{questionId}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public IActionResult PutQuestion(int id, int qid, [FromBody] QuestionUpdateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains data if needed and proper message
         * for the client if process is done successfully or not
         */
        const string sql = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
        int rowsUpdated = _connection.Execute(sql, new { QuestionId = qid, Text = value.Text, CorrectAnswerId = value.CorrectAnswerId });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public IActionResult DeleteQuestion(int id, int qid)
    {
        /*
         * TODO 1. I would return a specific DTO which contains a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
        _connection.ExecuteScalar(sql, new { QuestionId = qid });
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public IActionResult PostAnswer(int id, int qid, [FromBody] AnswerCreateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains data if needed and a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
        var answerId = _connection.ExecuteScalar(sql, new { Text = value.Text, QuestionId = qid });
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateModel value)
    {
        /*
         * TODO 1. I would add data validations part and implement exceptions handling
         * TODO 2. I would return a specific DTO which contains data if needed and proper message
         * for the client if process is done successfully or not
         */
        const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
        int rowsUpdated = _connection.Execute(sql, new { AnswerId = qid, Text = value.Text });
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult DeleteAnswer(int id, int qid, int aid)
    {
        /*
         * TODO 1. I would return a specific DTO which contains a proper message
         * for the client if process is done successfully or not
         */
        const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
        _connection.ExecuteScalar(sql, new { AnswerId = aid });
        return NoContent();
    }
}