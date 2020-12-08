using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseBlog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using HouseBlog.Service;
using System.IO;
using Microsoft.EntityFrameworkCore;
using HouseBlog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace HouseBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly BlogContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ILogger<PostsController> _logger;
        private readonly ImageService _imageService;
        List<Topic> _topics;
        public PostsController(UserManager<User> userManager, BlogContext context, IWebHostEnvironment appEnvironment, ILogger<PostsController> logger, ImageService imageService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _appEnvironment = appEnvironment;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index(int? TopicId)
        {
            this._topics = await _context.Topics.ToListAsync();
            PostIndexViewModel postivm;

            if (TopicId != null)
            {
                int id = (int)TopicId;
                List<Post> posts;
                try
                {
                    posts = _context.Posts.Include(s => s.Topic).Where(s => s.TopicId == TopicId).ToList();
                }
                catch (Exception)
                {
                    _logger.LogError("Doesn't exist id. Controller:Posts. Action:Index");
                    return RedirectPermanent("~/Error/Index?statusCode=404");
                }
                postivm = new PostIndexViewModel { Posts = posts, Topics = _topics };
                return View(postivm);
            }
            postivm = new PostIndexViewModel { Posts = await _context.Posts.ToListAsync(), Topics = _topics };
            return View(postivm);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogError("Doesn't exist id. Controller:Posts. Action:Details");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                _logger.LogError("Doesn't exist post. Controller:Posts. Action:Details");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }

            var user = await _context.Users.FindAsync(post.UserId);
            if (user == null)
            {
                _logger.LogError("Doesn't exist user. Controller:Posts. Action:Details");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }
            ViewData["UserName"] = user.UserName;
            List<Comment> comments = new List<Comment>();
            foreach (var item in _context.Comments.Include(s => s.Post).Where(s => s.PostId == post.PostId).ToList())
            {
                item.User = await _context.Users.FindAsync(item.UserId);
                comments.Add(item);
            }

            ViewData["Comment"] = comments;

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!user.isBlocked)
            {
                SelectList topics = new SelectList(_context.Topics, "TopicId", "TopicName");
                ViewBag.Topics = topics;
                ViewData["topics"] = _context.Topics.ToList();
                return View();
            }
            else
            {
                ViewData["Info"] = "Ваша страница заблокирована администратором";
                return View("~/Views/Shared/Info.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Text,TopicId")] Post post, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadedFile != null && uploadedFile != null && uploadedFile.ContentType.ToLower().Contains("image"))
                {
                    post.ImageUrl = await _imageService.SaveImageAsync(uploadedFile, 1);
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "Некорректный формат");
                    ViewData["topics"] = await _context.Topics.ToListAsync();
                    return View(post);
                }
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                post.User = user;
                post.CreatedAt = DateTime.Now;
                post.Topic = await _context.Topics.FindAsync(post.TopicId);
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectPermanent("~/Posts/Index");
            }
            return View(post);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                _logger.LogError("Doesn't exist post. Controller:Posts. Action:Edit. post = null");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Text,ImageUrl")] Post post, IFormFile uploadedFile)
        {
            if (id != post.PostId)
            {
                _logger.LogError("Doesn't exist id. Controller:Article. Action:Edit");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }
            Post post1 = await _context.Posts.FirstOrDefaultAsync(m => m.PostId == post.PostId);
            if (ModelState.IsValid)
            {
                try
                {
                    if (post.Text != post1.Text && post1 != null)
                        post1.Text = post.Text;
                    if (post.Title != post1.Title && post1 != null)
                        post1.Title = post.Title;



                    if (post.ImageUrl != post1.ImageUrl && post1.ImageUrl != null && uploadedFile != null && uploadedFile.ContentType.ToLower().Contains("image"))
                    {
                        post1.ImageUrl = await _imageService.SaveImageAsync(uploadedFile, 1);
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "Некорректный формат");
                        return View(post1);
                    }
                    _context.Update(post1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        _logger.LogError("Doesn't exist db. Controller:Post. Action:Edit");
                        return RedirectPermanent("~/Error/Index?statusCode=404");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectPermanent("~/");
            }
            return View(post);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogError("Doesn't exist id. Controller:Post. Action:Delete");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                _logger.LogError("Doesn't exist areticle. Controller:Post. Action:Delete");
                return RedirectPermanent("~/Error/Index?statusCode=404");
            }

            return View(post);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectPermanent("~/Home/Index");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}

