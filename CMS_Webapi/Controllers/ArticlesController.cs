using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using CMS_Webapi.Models;

namespace CMS_Webapi.Controllers
{
    public class ArticlesController : ApiController
    {
        private Context db = new Context();

        // GET: api/Articles
        public IQueryable<Article> GetArticles(int? authorId = null, int? reviewerId = null, bool? onlyApproved = null)
        {
            IQueryable<Article> articles = db.Articles;
            if (onlyApproved != null)
            {
                if ((bool)onlyApproved)
                    articles = articles.Where(a => a.Approved);
            }
            if (authorId != null)
            {
                articles = articles.Where(a => a.AuthorId == authorId);
            }
            if (reviewerId != null)
            {
                articles = articles.Where(a => a.ReviewerId == reviewerId && !a.Reviewed);
            }

            return articles;
        }

        // GET: api/Articles/5
        [ResponseType(typeof(Article))]
        public IHttpActionResult GetArticle(int id)
        {
            var article = db.Articles.Find(id);
            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // PUT: api/Articles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArticle(int id, Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.Id)
            {
                return BadRequest();
            }

            var currentArticle = db.Articles.Find(article.Id);
            db.Entry(currentArticle).CurrentValues.SetValues(article);
            
            try
            {
                db.SaveChanges();
                SendMail(article.Approved ? 1 : 2, article);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Articles
        [ResponseType(typeof(Article))]
        public IHttpActionResult PostArticle(Article article)
        {
            article.Approved = false;
            article.Reviewed = false;
            article.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Articles.Add(article);
            db.SaveChanges();
            SendMail(0, article);

            return CreatedAtRoute("DefaultApi", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [ResponseType(typeof(Article))]
        public IHttpActionResult DeleteArticle(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return NotFound();
            }

            db.Articles.Remove(article);
            db.SaveChanges();

            return Ok(article);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArticleExists(int id)
        {
            return db.Articles.Count(e => e.Id == id) > 0;
        }

        public void SendMail(int msgType, Article article)
        {
            if (article.Author == null)
                article.Author = db.Users.First(u=>u.Id == article.AuthorId);
            if(article.Reviewer == null)
                article.Reviewer = db.Users.First(u => u.Id == article.ReviewerId);
            String typeSubject = "";
            String content = "";
            MailAddress toAddress = new MailAddress(article.Author.Email, article.Author.Username);
            switch (msgType)
            {
                //Reviewer has new Article to review
                case 0:
                    typeSubject = "------!NEW Article to review!------";
                    content = "A new article has been given to you to review";
                    toAddress = new MailAddress(article.Reviewer.Email, article.Reviewer.Username);
                    break;
                //Writer's Article is accpeted and published
                case 1:
                    typeSubject = "------Article APPROVED------";
                    content = "Your article has been aproved";
                    break;
                //Writer's Article is not approved
                case 2:
                    typeSubject = "------Article NOT APPROVED------";
                    content = "Your article has not been aproved";
                    break;
            }
            try
            {
                var fromAddress = new MailAddress("juegorupi@gmail.com", "NotificationBot");
                const string fromPassword = "rupirupi";
                const string subject = "NEW NOTIFICATION";
                StringBuilder sb = new StringBuilder();
                sb.Append(typeSubject);
                sb.Append(Environment.NewLine);
                sb.Append(content);
                sb.Append(Environment.NewLine);
                sb.Append("Article Name: " + article.Title);

                string body = sb.ToString();

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}