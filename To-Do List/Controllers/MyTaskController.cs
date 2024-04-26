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

namespace To_Do_List.Controllers
{
    public class MyTaskController : Controller
    {
        private readonly TasksContext _context;

        public MyTaskController(TasksContext context)
        {
            _context = context;
        }

        // GET: MyTask
        public async Task<IActionResult> Index(TaskSort order = PriorytyAsc)
        {
            ViewBag.NameSort = order == NameAsc ? NameDesc : NameAsc;
            ViewBag.CreatedSort = order == CreatedAsc ? CreatedDesc : CreatedAsc;
            ViewBag.PrioritySort = order == PriorytyAsc ? PriorytyDesc : PriorytyAsc;
            ViewBag.StatusSort = order == StatusAsc ? StatusDesc : StatusAsc;

            var tasks = await _context.MyTasks.ToListAsync();

            tasks = GetSortOrder(tasks, order);

            return View(tasks);
        }

        // GET: MyTask/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(myTask);
        }

        // GET: MyTask/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Executor,Priority,Status,Description,Created,ClosedDate,OpenedDate")] MyTask myTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(myTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myTask);
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
