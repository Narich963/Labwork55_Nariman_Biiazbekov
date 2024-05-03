using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;
using static To_Do_List.Services.TaskSort;
using To_Do_List.Services;
using To_Do_List.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace To_Do_List.Controllers
{
    public class MyTaskController : Controller
    {
        private readonly TasksContext _context;
        private readonly UserManager<User> _userManager;
        public MyTaskController(TasksContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MyTask
        public async Task<IActionResult> Index(string? fullName, DateTime? CreatedFrom, DateTime? CreatedTo, 
            string? wordContains, string? priority, string? status, 
            TaskSort order = PriorytyAsc, int page = 1)
        {
            ViewBag.NameSort = order == NameAsc ? NameDesc : NameAsc;
            ViewBag.CreatedSort = order == CreatedAsc ? CreatedDesc : CreatedAsc;
            ViewBag.PrioritySort = order == PriorytyAsc ? PriorytyDesc : PriorytyAsc;
            ViewBag.StatusSort = order == StatusAsc ? StatusDesc : StatusAsc;

            var tasks = await _context.MyTasks.Include(t => t.Creator).Include(t => t.Executor).ToListAsync();

            if (fullName != null)
            {
                tasks = tasks.Where(t => t.Name ==  fullName).ToList();
            }
            if (CreatedFrom.HasValue)
            {
                tasks = tasks.Where(t => t.Created > CreatedFrom).ToList();
            }
            if (CreatedTo.HasValue)
            {
                tasks = tasks.Where(t => t.Created < CreatedTo).ToList();
            }
            if (wordContains != null)
            {
                tasks = tasks.Where(t => t.Description.Contains(wordContains)).ToList();
            }
            if (priority != null)
            {
                tasks = tasks.Where(t => t.Priority == priority).ToList();
            }
            if (status != null)
            {
                tasks = tasks.Where(t => t.Status == status).ToList();
            }

            tasks = GetSortOrder(tasks, order);

            int pagesize = 10;
            var count = tasks.Count();
            var items = tasks.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pagesize);
            IndexViewModel viewModel = new()
            {
                PageViewModel = pageViewModel,
                Tasks = items
            };

            return View(viewModel);
        }

        // GET: MyTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks.Include(t => t.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            return View(myTask);
        }

        // GET: MyTask/Create
        public async Task<IActionResult> Create(string? email)
        {
            if (email != null)
            {
                User user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    ViewBag.UserId = user.Id;
                }
            }
            return View();
        }

        // POST: MyTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MyTask myTask, string? email)
        {
            if (ModelState.IsValid)
            {
                _context.Add(myTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (email != null)
            {
                User user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    ViewBag.UserId = user.Id;
                }
            }
            return View(myTask);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var task = await _context.MyTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                return View(task);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MyTask task)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyTaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }
        // GET: MyTask/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            if (myTask.Status == "Открыта")
            {
                return RedirectToAction("Index");
            }

            return View(myTask);
        }

        // POST: MyTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask != null)
            {
                _context.MyTasks.Remove(myTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyTaskExists(int id)
        {
            return _context.MyTasks.Any(e => e.Id == id);
        }
        public async Task<IActionResult> TakeTask(int? id, string? name)
        {
            User user = await _userManager.FindByNameAsync(name);
            if (user != null)
            {
                MyTask task = await _context.MyTasks.FirstOrDefaultAsync(e => e.Id == id);
                if (task != null)
                {
                    task.ExecutorId = user.Id;
                    task.Executor = user;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [ActionName("Open")]
        public async Task<IActionResult> Open(int? id)
        {
            if (id != null)
            {
                MyTask task = await _context.MyTasks.FirstOrDefaultAsync(t => t.Id == id);
                if (task != null)
                {
                    task.Status = "Открыта";
                    task.OpenedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [ActionName("Close")]
        public async Task<IActionResult> Close(int? id)
        {
            if (id != null)
            {
                MyTask task = await _context.MyTasks.FirstOrDefaultAsync(e => e.Id == id);
                if (task != null)
                {
                    task.Status = "Закрыта";
                    task.ClosedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [NonAction]
        public List<MyTask> GetSortOrder(List<MyTask> tasks, TaskSort order) =>
            order switch
            {
                PriorytyAsc => SortPrioriyAsc(tasks),
                PriorytyDesc => SortPrioriyDesc(tasks),
                NameAsc => tasks.OrderBy(t => t.Name).ToList(),
                NameDesc => tasks.OrderByDescending(t => t.Name).ToList(),
                CreatedAsc => tasks.OrderBy(t => t.Created).ToList(),
                CreatedDesc => tasks.OrderByDescending(t => t.Created).ToList(),
                StatusAsc => SortStatusAsc(tasks),
                StatusDesc => SortStatusDesc(tasks)
            };
        [NonAction]
        public List<MyTask> SortPrioriyAsc(List<MyTask> tasks) =>
            tasks.OrderBy(t => t.Priority switch
            {
                "Высокий" => 0,
                "Средний" => 1,
                "Низкий" => 2,
                _ => 3
            }).ToList();
        [NonAction]
        public List<MyTask> SortPrioriyDesc(List<MyTask> tasks) =>
            tasks.OrderBy(t => t.Priority switch
            {
                "Высокий" => 3,
                "Средний" => 2,
                "Низкий" => 1,
                _ => 0
            }).ToList();
        [NonAction]
        public List<MyTask> SortStatusAsc(List<MyTask> tasks) =>
            tasks.OrderBy(t => t.Status switch
            {
                "Новая" => 0,
                "Открыта" => 1,
                "Закрыта" => 2,
                _ => 3
            }).ToList();
        [NonAction]
        public List<MyTask> SortStatusDesc(List<MyTask> tasks) =>
            tasks.OrderBy(t => t.Status switch
            {
                "Новая" => 3,
                "Открыта" => 2,
                "Закрыта" => 1,
                _ => 0
            }).ToList();
    }
}
