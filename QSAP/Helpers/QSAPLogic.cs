using QSAP.Models;
using QSAP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSAP.Helpers
{
    public class QSAPLogic
    {
        public SummaryViewModel GetSummary(int QSAP_ID)
        {
            var context = new QSAPEntities();

            var questions = (from q in context.Questions
                            where q.Section.QSAP_ID == QSAP_ID
                             select q).OrderBy(n => n.Number);

            var sections = (from s in context.Sections
                           where s.QSAP_ID == QSAP_ID
                            select s).OrderBy(n => n.Number);

            var ratings = from r in context.Ratings
                          select r;

            foreach (var q in questions)
            {
                q.Indicators.Clear();
                q.Indicators = GetIndicators(GetQuestionID(Convert.ToInt32(q.Section.Number), Convert.ToInt32(q.Number), QSAP_ID));
            }

            SummaryViewModel summary = new SummaryViewModel();

            summary.questions = questions.ToList();
            summary.sections = sections.ToList();
            summary.ratings = ratings.ToList();

            return summary;
        }

        public CurrentInfo NextQuestionSet(int categoryId, int sectionNum, int QSAP_ID)
        {
            CurrentInfo cInfo = new CurrentInfo();
            cInfo.currentCategoryId = categoryId;
            cInfo.currentSectionNum = sectionNum;

            List<Question> currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);

            while (currentQuestions.Count() == 0)
            {
                cInfo.currentCategoryId++;

                if (cInfo.currentCategoryId > GetCategoryCount())
                {
                    cInfo.currentCategoryId = 1;
                    cInfo.currentSectionNum++;

                    if (cInfo.currentSectionNum > GetSectionCount(QSAP_ID))
                    {
                        return null;
                    }
                    else
                        currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);
                }
                else
                    currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);
            }


            return cInfo;
        }

        public CurrentInfo PreviousQuestionSet(int categoryId, int sectionNum, int QSAP_ID)
        {
            CurrentInfo cInfo = new CurrentInfo();
            cInfo.currentCategoryId = categoryId;
            cInfo.currentSectionNum = sectionNum;

            List<Question> currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);

            while (currentQuestions.Count() == 0)
            {
                cInfo.currentCategoryId--;

                if (cInfo.currentCategoryId <= 0)
                {
                    cInfo.currentCategoryId = GetCategoryCount();
                    cInfo.currentSectionNum--;

                    if (cInfo.currentSectionNum <= 0)
                    {
                        return null;
                    }
                    else
                        currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);
                }
                else
                    currentQuestions = GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);
            }


            return cInfo;
        }

        public QuestionAndRatingViewModel GetQuestionInfo(int categoryId, int sectionNum, int QSAP_ID)
        {
            var context = new QSAPEntities();

            var questions = GetQuestions(sectionNum, categoryId, QSAP_ID);

            /*if (questions.Count() == 0) categoryId++;
            if (categoryId > 3)
            {
                sectionNum++;
                categoryId = 1;

                questions = from q in context.Questions
                            where q.Section.Number == sectionNum && q.Section.QSAP.Id == QSAP_ID && q.Category_ID == categoryId
                            select q;
            }
            */




            var ratings = from r in context.Ratings
                          select r;

            var section = from s in context.Sections
                          where s.Number == sectionNum && s.QSAP_ID == QSAP_ID
                          select s;

            QuestionAndRatingViewModel qr = new QuestionAndRatingViewModel();
            qr.questions = questions.ToList();

            qr.ratings = ratings.ToList();
            qr.section = (Section)section.First();

            foreach (var q in questions)
            {
                q.Indicators.Clear();
                q.Indicators = GetIndicators(GetQuestionID(sectionNum, Convert.ToInt32(q.Number), QSAP_ID));
            }



            //context.Dispose();

            return qr;
        }

        public int GetQuestionCount(int sectionNum, int QSAP_ID)
        {
            var context = new QSAPEntities();

            var questions = from q in context.Questions
                            where q.Section.Number == sectionNum && q.Section.QSAP.Id == QSAP_ID
                            select q;

            return questions.Count();
        }

        public int GetSectionCount(int QSAP_ID)
        {
            var context = new QSAPEntities();

            var sections = from s in context.Sections
                           where s.QSAP_ID == QSAP_ID
                           select s;

            return sections.Count();
        }

        public int GetCategoryCount()
        {
            var context = new QSAPEntities();

            var categories = from c in context.Categories
                             select c;

            return categories.Count();
        }

        public List<Indicator> GetIndicators(int questionId)
        {
            var context = new QSAPEntities();

            var indicators = from i in context.Indicators
                             where i.Question_ID == questionId
                             select i;

            return indicators.ToList();
        }

        public int GetQuestionID(int sectionNum, int questionNum, int QSAP_ID)
        {
            var context = new QSAPEntities();

            var questions = from q in context.Questions
                            where q.Section.Number == sectionNum && q.Section.QSAP.Id == QSAP_ID && q.Number == questionNum
                            select q;

            return questions.First().Id;
        }

        public List<Question> GetQuestions(int sectionNum, int categoryId, int QSAP_ID)
        {
            var context = new QSAPEntities();

            var questions = (from q in context.Questions
                            where q.Section.Number == sectionNum && q.Section.QSAP.Id == QSAP_ID && q.Category_ID == categoryId
                            select q).OrderBy(n => n.Number);


            return questions.ToList();

        }

        public bool isLastScreen(int currentCateogryNum, int currentSectionNum, int QSAP_ID)
        {
            bool isLastScreen = false;

            var logic = new QSAPLogic();

            if (currentCateogryNum == logic.GetCategoryCount())
            {
                currentCateogryNum = 1;
                currentSectionNum++;



            }
            else
            {
                currentCateogryNum++;
            }


            var nextInfo = logic.NextQuestionSet(currentCateogryNum, currentSectionNum, QSAP_ID);

            if (nextInfo == null)
                isLastScreen = true;

            return isLastScreen;
        }
    }
}