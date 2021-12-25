using System.Collections.Generic;

namespace Assets.Scripts
{
    //Question Model
    [System.Serializable]
    public class Question
    {
        public int Id;
        public string Q;
        public string A;
        public string B;
        public string C;
        public string D;
        public int TrueAnswer;
        public QuestionCategory Type;
    }

    //Category Model
    [System.Serializable]
    public class Category
    {
        public string name;
        public string sprite;
        // Category type CategoryId in DB.
        public QuestionCategory categoryId;
    }

    //Categories
    //Just +1 added in DB
    [System.Serializable]
    public enum QuestionCategory
    {
        //0
        History,
        //1
        Science,
        //2
        English,
        //3
        Geography,
        //4
        Sport,
        //5
        Art,
        //6
        Others,
        //7
        AddQuestion,
        //8
        None
    }
}
