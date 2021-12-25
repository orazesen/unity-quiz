using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using Assets.Scripts;
using System.Linq;

public class SQLiteController : MonoBehaviour
{
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    string DatabaseName = "QuizDB.s3db";
    public List<Category> categories { get; set; }

    public int challengeQuestionCount = 10;
    private CategorySelector selector;
    public List<string> options { get; set; }

    void Start()
    {
        selector = GetComponent<CategorySelector>();
        string filepath = "";
#if UNITY_ANDROID
        filepath = Application.persistentDataPath + DatabaseName;
#elif UNITY_EDITOR
        filepath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
#elif UNITY_IOS
        filepath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
#endif
        //open db connection
        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        categories = _getCategories();
        selector.SetCategoryBtns(categories);
    }

    //Getting challenge questions by category
    public List<Question> GetChallengeQuestions(QuestionCategory category)
    {
        List<Question> qs = new List<Question>();
        List<Question> sample = new List<Question>();

        qs = _getQuestions(category);

        for (int i = 0; i < challengeQuestionCount; i++)
        {
            int ran = UnityEngine.Random.Range(0, qs.Count);
            sample.Add(qs[ran]);
            qs.RemoveAt(ran);
        }

        return sample;
    }

    //Gets questions randomly from all questions
    public List<Question> GetRandomQuestions()
    {
        List<int> ids = new List<int>();
        int count = 0;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT COUNT(*) FROM " + "Questions";// table name
            dbcmd.CommandText = sqlQuery;
            count = Convert.ToInt32(dbcmd.ExecuteScalar());
            dbconn.Close();
        }
        Debug.Log(count);

        for (int i = 0; i < challengeQuestionCount; i++)
        {
            int id = UnityEngine.Random.Range(0, count);
            ids.Add(id);
            Debug.Log(id);
        }

        return _getQuestions(ids);
    }

    //Insert To Database
    private void _insertQuestion(string Question, string AnswerA, string AnswerB, string AnswerC, string AnswerD, int RightAnswer, QuestionCategory category)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("insert into Questions (Question, AnswerA, AnswerB, AnswerC, AnswerD, RightAnswer, CategoryId) " +
                "values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", 
                Question, AnswerA, AnswerB, AnswerC, AnswerD, RightAnswer, (int)(category + 1));// table name
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }
        Debug.Log("Insert Done  ");
    }

    //Reads all questions by given category
    private List<Question> _getQuestions(QuestionCategory categoryId)
    {
        List<Question> questions = new List<Question>();
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT  Id, Question, Answer1, Answer2, Answer3, Answer4, RightAnswer, CategoryId FROM Questions WHERE CategoryId=" + ((int)categoryId + 1);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Question question = new Question();
                question.Id = reader.GetInt32(0);
                question.Q = reader.GetString(1);
                question.A = reader.GetString(2);
                question.B = reader.GetString(3);
                question.C = reader.GetString(4);
                question.D = reader.GetString(5);
                question.TrueAnswer = reader.GetInt32(6);
                question.Type = (QuestionCategory)reader.GetInt32(7);
                questions.Add(question);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            return questions;
        }
    }

    //Reads all questions by given id
    private List<Question> _getQuestions(List<int> ids)
    {
        List<Question> questions = new List<Question>();
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.            

            Debug.Log("ids count: " + ids.Count);
            IDbCommand dbcmd = dbconn.CreateCommand();
            var list = string.Join(",", ids.Select(x => x.ToString()).ToArray());
            string sqlQuery = string.Format("SELECT * FROM Questions WHERE Id in ({0})", list);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Question question = new Question();
                question.Id = reader.GetInt32(0);
                question.Q = reader.GetString(1);
                question.A = reader.GetString(2);
                question.B = reader.GetString(3);
                question.C = reader.GetString(4);
                question.D = reader.GetString(5);
                question.TrueAnswer = reader.GetInt32(6);
                question.Type = (QuestionCategory)reader.GetInt32(7);
                Debug.Log(question.Id + " " + question.Q);
                questions.Add(question);
            }    
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            return questions;
        }
    }

    //Getting all categories
    private List<Category> _getCategories()
    {
        List<Category> categories = new List<Category>();
        options = new List<string>();
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT  Id, Name, SpriteName FROM Categories";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Category category = new Category();
                category.categoryId = (QuestionCategory)reader.GetInt32(0);
                category.name = reader.GetString(1);
                category.sprite = reader.GetString(2);
                categories.Add(category);
                if (category.categoryId != QuestionCategory.AddQuestion)
                {
                    options.Add(category.name);
                }
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
        return categories;
    }

    //Update on  Database 
    private void _updateQuestion(int questionId, string updateQuestion, string updateAnswerA, string updateAnswerB, string updateAnswerC, string updateAnswerD,
        int RightAnswer, QuestionCategory category)
    {
        int cat = (int)category + 1;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("UPDATE Questions set Question = @question, Answer1 = @answera," +
                "Answer2 = @answerb, Answer3 = @answerc, Answer4 = @answerd, RightAnswer = @rightanswer, CategoryId = @category where Id = @id ");

            SqliteParameter update_question = new SqliteParameter("@question", updateQuestion);
            SqliteParameter update_answera = new SqliteParameter("@answera", updateAnswerA);
            SqliteParameter update_answerb = new SqliteParameter("@answerb", updateAnswerB);
            SqliteParameter update_answerc = new SqliteParameter("@answerc", updateAnswerC);
            SqliteParameter update_answerd = new SqliteParameter("@answerd", updateAnswerD);
            SqliteParameter update_rightanswer = new SqliteParameter("@rightanswer", RightAnswer);
            SqliteParameter update_category = new SqliteParameter("@category", cat);
            SqliteParameter update_id = new SqliteParameter("@id", questionId);

            dbcmd.Parameters.Add(update_question);
            dbcmd.Parameters.Add(update_answera);
            dbcmd.Parameters.Add(update_answerb);
            dbcmd.Parameters.Add(update_answerc);
            dbcmd.Parameters.Add(update_answerd);
            dbcmd.Parameters.Add(update_rightanswer);
            dbcmd.Parameters.Add(update_category);
            dbcmd.Parameters.Add(update_id);

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }
    }

    //Delete
    private void Delete_function(int questionId)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "DELETE FROM Questions where id =" + questionId;// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();


            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            //data_staff.text = Delete_by_id + " Delete  Done ";
            Debug.Log(questionId + " Delete  Done ");
        }
    }
}