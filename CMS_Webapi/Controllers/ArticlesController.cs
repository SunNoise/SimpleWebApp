using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    }
}