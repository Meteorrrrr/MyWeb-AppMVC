using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_1.Models;
namespace MVC_1.Areas_Blog_Controllers
{
    [Route("/viewpost/{categoryslug?}")]
    [Area("Blog")]
    public class ViewPostController:Controller
    {

        private readonly AppDbContext _context;

        public ViewPostController(AppDbContext context)
        {
            _context = context;
        }
        


        private void GetChildCategoryId(Category category,List<int> cateId )
        {
            cateId.Add(category.Id);
            if(category.CategoryChildren?.Count>0)
            {
                foreach(var cate in category.CategoryChildren)
                {
                    GetChildCategoryId(cate,cateId);
                }
            }


        }
        private List<Category> GetParentCategory(Category category)
        {
            List<Category> list=new List<Category>();
            while(category.ParentCategory!=null)
            {
                list.Add(category.ParentCategory);
                category=category.ParentCategory;
            }
            list.Reverse();
            return list;

        }

        public IActionResult Index(string categoryslug)
        {
            var list=_context.Categories.Include(c=>c.CategoryChildren)
                                         .AsEnumerable()
                                         .Where(c=>c.ParentId==null)
                                         .ToList();
            ViewBag.categories=list; 
            ViewBag.categoryslug=categoryslug;

            

            Category? category=null;
            
            if(categoryslug!=null)
            {
                 category=_context.Categories.Where(c=>c.Slug==categoryslug)
                                            .Include(c=>c.ParentCategory).FirstOrDefault();
                 if(category==null)
                 {
                    return NotFound();
                 }   
               
            }

            //lấy danh sách cha của chuyên mục đang được chọn để hiển thị trên trang(breadcrumbs)
           if(category!=null)
           {
            List<Category> lisparent=GetParentCategory(category);
              ViewBag.listparent=lisparent;
           }


            ViewBag.category=category;
            var posts=_context.Posts
                              .Include(p => p.Author)
                              .Include(p => p.PostCategories)
                              .ThenInclude(p => p.Category)
                              .AsQueryable();
                              
            posts=posts.OrderByDescending(p=>p.DateUpdated);                 
                             
            if(category!=null)
            {
                List<int> listId=new List<int>();
                
                GetChildCategoryId(category,listId);
    
               posts= posts.Where(p=>p.PostCategories.Where(pc=>listId.Contains(pc.CategoryID)).Any());
                
            }                  
           
            return View(posts.ToList());
        }
        [Route("/viewpost/{postslug}.html")]
         public IActionResult Detail(string postslug)
         {
            var list=_context.Categories.Include(c=>c.CategoryChildren)
                                         .AsEnumerable()
                                         .Where(c=>c.ParentId==null)
                                         .ToList();
            ViewBag.categories=list; 


            
            var post=_context.Posts.Where(p => p.Slug == postslug)
                                   .Include(p => p.Author)
                                   .Include(p => p.PostCategories)
                                   .ThenInclude(p => p.Category)
                                   .FirstOrDefault();
                                   
            if(post==null)
            {
                return NotFound("Khong tim thay bai viet!");
            }  

            var listpostcategory=post.PostCategories;

            List<int> listId=new List<int>();
            foreach(var postCategory in listpostcategory)
            {
                
               listId.Add(postCategory.CategoryID);

            }
       

            var otherpost=_context.Posts.Where(p=>p.PostCategories.Any(pc=>listId.Contains(pc.CategoryID)))
                                        .Where(p=>p.PostId!=post.PostId)
                                        .OrderByDescending(p=>p.DateUpdated)
                                        .Take(5);
            ViewBag.otherpost=otherpost;

                     
            return View(post);
         }

    }



}