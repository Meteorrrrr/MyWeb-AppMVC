using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_1.Models;
namespace MVC_1.Areas_Blog_Controllers
{
    [Area("Blog")]
    [Route("/blog/{action}")]
    public class CategoryController:Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [TempData]
        public string StatusMessage {set;get;}
        public async Task<IActionResult> Index()
        {
            var qr=(from c in _context.Categories select c)
            .Include(c=>c.CategoryChildren)
            .Include(c=>c.ParentCategory);
            var categories=(await qr.ToListAsync()).Where(c=>c.ParentCategory==null).ToList();
            return View(categories);
        }
      
        public async Task<IActionResult> Create() {
           // ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Slug");
            // var categories= await _context.Categories
            // .Include(c=>c.CategoryChildren)
            // .Include(c=>c.ParentCategory)
            // .Where(c=>c.ParentCategory==null).ToListAsync();
            var qr=(from c in _context.Categories select c)
            .Include(c=>c.CategoryChildren)
            .Include(c=>c.ParentCategory);
            var categories=(await qr.ToListAsync()).Where(c=>c.ParentCategory==null).ToList();
            
             categories.Insert(0,new Category(){
                Id=-1,
                Title="Không có lớp cha"

            });
            var categorieslist=new List<Category>();
            GetSelectListCatergory(categories,categorieslist,0);

            var selectlist=new SelectList(categorieslist,"Id","Title",-1);
            ViewData["ParentId"]=selectlist;
            
            return View ();
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirm([Bind("Id,CategoryChildren,ParentId,Title,Content,Slug")] Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.ParentId==-1)
                {
                    category.ParentId=null;
                }
                StatusMessage="Ban da tao danh muc thanh cong";
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // var categories=_context.Categories
            // .Include(c=>c.CategoryChildren)
            // .Include(c=>c.ParentCategory)
            // .Where(c=>c.ParentCategory==null).ToList();
            var qr=(from c in _context.Categories select c)
            .Include(c=>c.CategoryChildren)
            .Include(c=>c.ParentCategory);
            var categories=(await qr.ToListAsync()).Where(c=>c.ParentCategory==null).ToList();
            categories.Insert(0,new Category(){
                Id=-1,
                Title="Không có lớp cha"

            });
            var categorieslist=new List<Category>();
            GetSelectListCatergory(categories,categorieslist,0);
            ViewData["ParentId"]=new SelectList(categorieslist,"Id","Title",-1);

            return View();
        }
        private void GetSelectListCatergory(List<Category> sourse,List<Category> des,int level)
        {
            var prefix=String.Concat(Enumerable.Repeat("---",level));
            foreach(var c in sourse)
            {
                des.Add(new Category(){
                    Id=c.Id,
                    Title=prefix+c.Title

                });
            
                if(c.CategoryChildren?.Count>0)
                {
                    
                     GetSelectListCatergory(c.CategoryChildren.ToList(),des,level+1);
                    
                }
                
            }

        }
        public async Task<IActionResult> Delete(int id)
        {
            var categorie= await _context.Categories.FindAsync(id);
            if(categorie==null)
            {
                return NotFound();
            }

            return View(categorie);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConFirm(int id)
        {
            var categorie= await _context.Categories
            .Include(c=>c.CategoryChildren)
            .FirstOrDefaultAsync(c=>c.Id==id);
            if(categorie==null)
            {
                return NotFound();
            }
            if(categorie.CategoryChildren?.Count>0)
            {
                foreach(var c in categorie.CategoryChildren)
                {
                    c.ParentId=categorie.ParentId;
                }
            }
            
            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            StatusMessage="Ban da xoa thanh cong !";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
             var cate= await _context.Categories.FindAsync(id);
            if(cate==null)
            {
                return NotFound();
            }


            // var categorie= await _context.Categories
            // .Include(c=>c.CategoryChildren)
            // .Include(c=>c.ParentCategory)
            // .Where(c=>c.ParentCategory==null).ToListAsync();
            var qr=(from c in _context.Categories select c)
            .Include(c=>c.CategoryChildren)
            .Include(c=>c.ParentCategory);
            var categorie=(await qr.ToListAsync()).Where(c=>c.ParentCategory==null).ToList();
              categorie.Insert(0,new Category(){
                Id=-1,
                Title="Không có lớp cha"

            });

            var categorieslist=new List<Category>();
            GetSelectListCatergory(categorie,categorieslist,0);
            ViewData["ParentId"]=new SelectList(categorieslist,"Id","Title",cate.ParentId);
            return View(cate);
             
        }


        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]

         public async Task<IActionResult> EditConFirm(int id,[Bind("Id,Title,ParentId,Content,Slug")] Category category)
         {
            
            if(id!=category.Id)
            {
                return NotFound();
            }
            if(category.Id==category.ParentId)
             {
               ModelState.AddModelError(string.Empty,"Không được chọn lớp cha là chính mình");
                                    
            }
         
            if(ModelState.IsValid&&category.Id!=category.ParentId)
            {
                try
                {
         

                    if(category.ParentId==-1)
                     {
                       category.ParentId=null;
                     }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    StatusMessage="Ban da cap nhap du lieu thanh cong";
                    return RedirectToAction(nameof(Index));

                }
                catch(DbUpdateConcurrencyException)
                {
                  
                    if( _context.Categories.Any(c=>c.Id==id) ==false)
                    {
                        return NotFound();
                    }
                    else
                    {
                        StatusMessage="Cap nhap du lieu that bai";
                        throw new Exception("Khong the cap nhap du lieu");
                    }

                }
               
            }
             var qr=(from c in _context.Categories select c)
            .Include(c=>c.CategoryChildren)
            .Include(c=>c.ParentCategory);
            var categorie=(await qr.ToListAsync()).Where(c=>c.ParentCategory==null).ToList();
              categorie.Insert(0,new Category(){
                Id=-1,
                Title="Không có lớp cha"

            });

            var categorieslist=new List<Category>();
            GetSelectListCatergory(categorie,categorieslist,0);
            ViewData["ParentId"]=new SelectList(categorieslist,"Id","Title",category.ParentId);
                 
           return View(category);
         
         }







        

        // async Task<IEnumerable<Category>> GetItemsSelectCategorie() {

        //     var items = await _context.Categories
        //                         .Include(c => c.CategoryChildren)
        //                         .Where(c => c.ParentCategory == null)
        //                         .ToListAsync();



        //     List<Category> resultitems = new List<Category>() {
        //         new Category() {
        //             Id = -1,
        //             Title = "Không có danh mục cha"
        //         }
        //     };
        //     Action<List<Category>, int> _ChangeTitleCategory = null;
        //     Action<List<Category>, int> ChangeTitleCategory =  (items, level) => {
        //         string prefix = String.Concat(Enumerable.Repeat("—", level));
        //         foreach (var item in items) {
        //             item.Title = prefix + " " + item.Title; 
        //             resultitems.Add(item);
        //             if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0)) {
        //                 _ChangeTitleCategory(item.CategoryChildren.ToList(), level + 1);
        //             }
                        
        //         }
                
        //     };

        //     _ChangeTitleCategory = ChangeTitleCategory;
        //     ChangeTitleCategory?.Invoke(items, 0);

        //     return resultitems;
        // }

       
    
   
 }
}
    
