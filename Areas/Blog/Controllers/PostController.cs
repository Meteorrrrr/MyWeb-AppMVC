using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_1.Models;
namespace MVC_1.Areas_Blog_Controllers
{
    
    [Area("Blog")]
    [Route("/blog/post/{action}/{id?}")]
 
    public class PostController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        private readonly IAuthorizationService _authorService;
        

    

        public PostController(SignInManager<AppUser> signinManager,AppDbContext context,UserManager<AppUser> userManager,IAuthorizationService authorizationService)
        
        {
            _userManager=userManager;
            _context = context;
            _signinManager=signinManager;
            _authorService=authorizationService;
        }
        [TempData]
        public string? StatusMessage {set;get;}
        public async Task<IActionResult> Index()
        {
            var qr= from c in _context.Posts 
                    orderby c.DateUpdated descending
                    select c;
            var post=await qr.ToListAsync();
           
            return View(post);
        }
        public IActionResult Create()
        {
            var categories=_context.Categories.ToList();
    
            var list=new SelectList(categories,"Id","Title");
            ViewData["cateid"]=list;

            return View();
        }
        [HttpPost,ActionName("Create")]
        public async Task<IActionResult> CreateCofirm([Bind] CreatePost createPost)
        {
            //Cách 2 để kiểm tra người dùng đã đăng nhập hay chưa
            // if(_signinManager.IsSignedIn(User)==false)
            // {
            //     return RedirectToAction("Login","Account",new{area="Identity"});

            // }
            

            //Kiểm tra người dùng đã đăng nhập hay chưa
            //Nếu chưa thì chuyển về trang đăng nhập

            // if(User.Identity.IsAuthenticated==false)
            // {
            //     return RedirectToAction("Login","Account",new{area="Identity"});
            // }
         

            createPost.DateCreated=createPost.DateUpdated=DateTime.Now;
            var user=await _userManager.GetUserAsync(this.User);
            createPost.AuthorId=user.Id;
          
            if(ModelState.IsValid)
            {
                _context.Add(createPost);
                await _context.SaveChangesAsync();
                if(createPost.CategoryID!=null)
                {
                    foreach(var cateid in createPost.CategoryID)
                    {
                        var postcategory=new PostCategory();
                        postcategory.CategoryID=cateid;
                        postcategory.PostID=createPost.PostId;
                        _context.Add(postcategory);

                    }
                }
                StatusMessage="Bạn đã tạo bài viết thành công!";
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            var categories=_context.Categories.ToList();
            var list=new SelectList(categories,"Id","Title");
            ViewData["cateid"]=list;
            return View(createPost);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var post= await _context.Posts.FindAsync(id);

            return View(post);
        }
        [HttpPost,ActionName("Delete")]
        [Authorize(Policy ="duocxoa")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var result =await _authorService.AuthorizeAsync(this.User,"duocxoa");
            if(result.Succeeded)
            {
            var post= await _context.Posts.FindAsync(id);
            if(post==null)
            {
                return NotFound();
            }
            _context.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage="Bạn đã xóa bài viết thành công!";
            }
            // else 
            // {
            //     return RedirectToAction("Account","AccessDenied",new{area="Identity"});
            // }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var post=await _context.Posts.FindAsync(id);
            var pc=await _context.PostCategories.ToListAsync();

            var cateid=from c in pc where c.PostID==id select c.CategoryID;



            var createPost= new CreatePost();
            createPost.PostId=post.PostId;
            createPost.Title=post.Title;
            createPost.Content=post.Content;
            createPost.Description=post.Description;
            createPost.Slug=post.Slug;
            createPost.CategoryID=cateid.ToArray();

           var categories=_context.Categories.ToList();
    
            var list=new MultiSelectList(categories,"Id","Title");
            ViewData["cateid"]=list;
            return View(createPost);
        }
        [HttpPost,ActionName("Edit")]
        public async Task<IActionResult> EditConfirm(int id,[Bind("PostId,Title,Description,Slug,Content,CategoryID")] CreatePost post )
        {
           
            var p= await _context.Posts.Include(p=>p.PostCategories).FirstOrDefaultAsync(p=>p.PostId==id);

             
            var result=await _authorService.AuthorizeAsync(this.User,p,"InAge");

            if(!result.Succeeded)
            {
                return Content("Ban khong co quyen chinh sua");

            }
          
            if(id!=post.PostId)
            {
                return NotFound();
            }
        
            p.Title=post.Title;
            p.Content=post.Content;
            p.Description=post.Description;
            p.Slug=post.Slug;
            p.DateUpdated=DateTime.Now;


            // var oldpostcate=_context.PostCategories.Where(pc=>pc.PostID==id).ToArray();
            //     _context.PostCategories.RemoveRange(oldpostcate);

            

        if(ModelState.IsValid)
        {
            // Cách này dễ hiểu hơn nhưng có thể gây lỗi(cũng chưa biết nữa)
            // var oldpostcate=_context.PostCategories.Where(pc=>pc.PostID==id).ToArray();
            //     _context.PostCategories.RemoveRange(oldpostcate);
                if(post.CategoryID==null)
                   {
                       post.CategoryID=new int[]{};
                   }
                   var oldcateid=p.PostCategories.Select(pc=>pc.CategoryID).ToArray();
                   var removecateid=from x in p.PostCategories 
                                         where post.CategoryID.Contains(x.CategoryID)==false 
                                         select x;
                    _context.PostCategories.RemoveRange(removecateid.ToArray()); 
  
                    var addcateid=from y in post.CategoryID
                               where oldcateid.Contains(y)==false
                               select y;                            

                    foreach(var cateupdate in addcateid)
                    {
                    var postcategory=new PostCategory();
                    postcategory.CategoryID=cateupdate;
                    postcategory.PostID=id;
                    _context.Add(postcategory);
                     }       
            
            try{

                //có 2 cách để thực hiện update,thêm xóa sửa vào cơ sở dữ liệu
                _context.Attach(p).State=EntityState.Modified;
                //_context.Update(p);
                await _context.SaveChangesAsync();
                StatusMessage="Bạn đã cập nhập bài viết thành công!";
                return RedirectToAction(nameof(Index));

            }
            catch(DbUpdateConcurrencyException)
            {
                if(!_context.Posts.Any(p=>p.PostId==id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("khong the cap nhap du lieu!");
                }
            }
        }

        var categories= await _context.Categories.ToListAsync();
        var list=new SelectList(categories,"Id","Title");
            ViewData["cateid"]=list;

    
            return View(post);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var post= await _context.Posts.Include(p=>p.Author).FirstOrDefaultAsync(p=>p.PostId==id);
            return View(post);
        }
    }   
}
    
