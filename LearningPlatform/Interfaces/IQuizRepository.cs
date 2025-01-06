using LearningPlatform.Models;
using System.Collections.Generic;

public interface IQuizRepository
{
    IEnumerable<Quiz> GetAllQuizzes();
    Quiz GetQuizById(int id);
    void AddQuiz(Quiz quiz);
    void UpdateQuiz(Quiz quiz);
    void DeleteQuiz(int id);
}
