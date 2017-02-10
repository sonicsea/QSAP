using QSAP.Helpers;
using QSAP.Models;
using QSAP.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSAP.Controllers
{
    public class YouthController : Controller
    {
        //
        // GET: /Youth/

        private const int QSAP_ID = 4;

        public ActionResult Index()
        {
            Session.Clear();

            return RedirectToAction("Questionnaire");
        }

        public ActionResult Questionnaire()
        {

            QuestionAndRatingViewModel qr = null;

            QSAPLogic logic = new QSAPLogic();

            if (Session["CurrentInfo"] == null)
            {

                CurrentInfo cInfo = new CurrentInfo();
                cInfo.currentSectionNum = 1;
                //cInfo.currentQuestionNum = 1;
                cInfo.currentCategoryId = 1;

                Session["CurrentInfo"] = cInfo;

                qr = logic.GetQuestionInfo(1, 1, QSAP_ID);
            }
            else
            {
                CurrentInfo cInfo = (CurrentInfo)Session["CurrentInfo"];
                qr = logic.GetQuestionInfo(cInfo.currentCategoryId, cInfo.currentSectionNum, QSAP_ID);

            }

            return View(qr);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Next")]
        public ActionResult Next()
        {

            if (Session["CurrentInfo"] == null)
            {
                ViewBag.timeoutWarning = System.Configuration.ConfigurationManager.AppSettings["sessionTimeoutMessage"];
                return View();
            }

            CurrentInfo cInfo = (CurrentInfo)Session["CurrentInfo"];



            QSAPLogic logic = new QSAPLogic();




            List<Question> currentQuestions = logic.GetQuestions(cInfo.currentSectionNum, cInfo.currentCategoryId, QSAP_ID);





            foreach (var question in currentQuestions)
            {
                //int questionID = GetQuestionID(question.Section., question.Number);

                Dictionary<string, string> inputs = null;

                if (logic.GetIndicators(question.Id).Count() > 0)
                {
                    List<Indicator> indicators = logic.GetIndicators(question.Id);
                    if (Session["UserInputI"] == null)
                        inputs = new Dictionary<string, string>();
                    else
                        inputs = (Dictionary<string, string>)Session["UserInputI"];

                    foreach (var indicator in indicators)
                    {
                        var ids = "I" + indicator.Id.ToString();
                        if (!inputs.ContainsKey(ids))
                            inputs.Add(ids, Request[ids]);
                        else
                            inputs[ids] = Request[ids];
                    }
                    Session["UserInputI"] = inputs;
                }
                else
                {
                    if (Session["UserInputQ"] == null)
                        inputs = new Dictionary<string, string>();
                    else
                        inputs = (Dictionary<string, string>)Session["UserInputQ"];

                    if (!inputs.ContainsKey("Q" + question.Id))
                        inputs.Add("Q" + question.Id, Request["Q" + question.Id]);
                    else
                        inputs["Q" + question.Id] = Request["Q" + question.Id];

                    Session["UserInputQ"] = inputs;
                }
            }






            if (cInfo.currentCategoryId == logic.GetCategoryCount())
            {
                cInfo.currentCategoryId = 1;
                cInfo.currentSectionNum++;



            }
            else
            {
                cInfo.currentCategoryId++;
            }


            cInfo = logic.NextQuestionSet(cInfo.currentCategoryId, cInfo.currentSectionNum, QSAP_ID);



            if (cInfo == null)
                return RedirectToAction("Summary");
            else
            {
                if (logic.isLastScreen(cInfo.currentCategoryId, cInfo.currentSectionNum, QSAP_ID))
                    TempData["IsLastScreen"] = true;
                else
                    TempData["IsLastScreen"] = false;
            }


            Session["CurrentInfo"] = cInfo;

            return RedirectToAction("Questionnaire");

        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Previous")]
        public ActionResult Previous()
        {
            if (Session["CurrentInfo"] == null)
            {
                ViewBag.timeoutWarning = System.Configuration.ConfigurationManager.AppSettings["sessionTimeoutMessage"];
                return View();
            }

            CurrentInfo cInfo = (CurrentInfo)Session["CurrentInfo"];




            QSAPLogic logic = new QSAPLogic();




            if (cInfo.currentCategoryId == 1)
            {
                cInfo.currentSectionNum--;
                cInfo.currentCategoryId = logic.GetSectionCount(QSAP_ID);




            }
            else
                cInfo.currentCategoryId--;


            cInfo = logic.PreviousQuestionSet(cInfo.currentCategoryId, cInfo.currentSectionNum, QSAP_ID);
            if (cInfo == null)
                return RedirectToAction("Questionnaire");


            Session["CurrentInfo"] = cInfo;


            return RedirectToAction("Questionnaire");
        }



        [HttpPost]

        public ActionResult SendEmail(string email)
        {
            QSAPLogic logic = new QSAPLogic();
            //return new ViewAsPdf("PDF", new { UserInputQ = (Dictionary<string, string>)Session["UserInputQ"], UserInputI = (Dictionary<string, string>)Session["UserInputI"] }) { FileName = "Partnership.pdf" };
            PDFViewModel pdfInfo = new PDFViewModel();

            pdfInfo.userInputI = (Dictionary<string, string>)Session["UserInputI"];
            pdfInfo.userInputQ = (Dictionary<string, string>)Session["UserInputQ"];
            pdfInfo.summary = logic.GetSummary(QSAP_ID);

            //string htmlString = QSAP.Helpers.ControllerExtensions.RenderView(this, "PDF", pdfInfo);

            var pdfResult = new ViewAsPdf("PDF", pdfInfo);



            var binary = pdfResult.BuildPdf(this.ControllerContext);

            System.IO.File.WriteAllBytes(Server.MapPath("~/Docs/YouthServices.pdf"), binary);


            Mailer.SendMail(email, Server.MapPath("~/Docs/YouthServices.pdf"));



            if (System.IO.File.Exists(Server.MapPath("~/Docs/YouthServices.pdf"))) System.IO.File.Delete(Server.MapPath("~/Docs/YouthServices.pdf"));

            return RedirectToAction("Summary");

        }


        public ActionResult Summary()
        {
            QSAPLogic logic = new QSAPLogic();

            //var context = new QSAPEntities();

            //var questions = from q in context.Questions
            //                where q.Section.QSAP_ID == QSAP_ID
            //                select q;

            //var sections = from s in context.Sections
            //               where s.QSAP_ID == QSAP_ID
            //               select s;

            //var ratings = from r in context.Ratings
            //              select r;

            //foreach (var q in questions)
            //{
            //    q.Indicators.Clear();
            //    q.Indicators = logic.GetIndicators(logic.GetQuestionID(Convert.ToInt32(q.Section.Number), Convert.ToInt32(q.Number), QSAP_ID));
            //}

            //SummaryViewModel summary = new SummaryViewModel();

            //summary.questions = questions.ToList();
            //summary.sections = sections.ToList();
            //summary.ratings = ratings.ToList();

            return View(logic.GetSummary(QSAP_ID));
        }


        public ActionResult PDF(PDFViewModel pdfInfo)
        {
            //PDFViewModel pdfInfo = new PDFViewModel();

            //pdfInfo.userInputI = UserInputI;
            //pdfInfo.userInputQ = UserInputQ;
            //pdfInfo.summary = GetSummary();

            return View(pdfInfo);
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Download")]
        public ActionResult DownloadPDF()
        {
            QSAPLogic logic = new QSAPLogic();
            //return new ViewAsPdf("PDF", new { UserInputQ = (Dictionary<string, string>)Session["UserInputQ"], UserInputI = (Dictionary<string, string>)Session["UserInputI"] }) { FileName = "Partnership.pdf" };
            PDFViewModel pdfInfo = new PDFViewModel();

            pdfInfo.userInputI = (Dictionary<string, string>)Session["UserInputI"];
            pdfInfo.userInputQ = (Dictionary<string, string>)Session["UserInputQ"];
            pdfInfo.summary = logic.GetSummary(QSAP_ID);

            return new ViewAsPdf("PDF", pdfInfo) { FileName = "YouthServices.pdf" };
        }

    }
}
