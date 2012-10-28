using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TODOListDemo.DAL;
using TODOListDemo.Models;
using System.Data;
namespace TODOListDemo.Controllers
{
    [Authorize]
    public class TodoItemController : Controller
    {
        public int SectionSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SectionSize"]);
        private IGenericRepository<TodoItem> _repositoryTodoItem;

        public TodoItemController(IGenericRepository<TodoItem> todoItemRepository)
        {
            _repositoryTodoItem = todoItemRepository;
        }
        //
        // GET: /TodoItem/

        public ActionResult Index()
        {
            return View();
        }

        #region CRUD
        public ActionResult Create(DateTime date)
        {
            TodoItem todoItem = new TodoItem();
            todoItem.StartTime = date;
            todoItem.UserId = User.Identity.Name;
            todoItem.Done = false;
            todoItem.SortNum = 0;
            var priorities = Enum.GetValues(typeof(Priority)).Cast<Priority>().Select(e => new { Value = (int)e, Text = e.ToString() });
            ViewData["PriorityId"] = new SelectList(priorities, "Value", "Text");
            return PartialView("_TodoItemForm", todoItem);
        }

        public ActionResult Edit(int id)
        {
            TodoItem todoItem = _repositoryTodoItem.Query.FirstOrDefault(c => c.Id == id);

            if (todoItem == null)
            {
                todoItem = new TodoItem();
                todoItem.StartTime = DateTime.Now;
                todoItem.UserId = User.Identity.Name;
                todoItem.Done = false;
                todoItem.SortNum = 0;
            }
            var priorities = Enum.GetValues(typeof(Priority)).Cast<Priority>().Select(e => new { Value = (int)e, Text = e.ToString() });
            ViewData["PriorityId"] = new SelectList(priorities, "Value", "Text", todoItem.PriorityId);
            return PartialView("_TodoItemForm", todoItem);
        }

        public ActionResult Finish(int id)
        {
            string Message = "";
            try
            {
                TodoItem todoItem = _repositoryTodoItem.Query.FirstOrDefault(c => c.Id == id);
                if (todoItem == null)
                {
                    return Json(new { Success = false, Msg = "You selected a non existent item." });
                }
                todoItem.Done = true;
                todoItem.TimeFinished = DateTime.Now;
                _repositoryTodoItem.Edit(todoItem);
                _repositoryTodoItem.SaveChanges();
                return Json(new { Success = true, Msg = "Task was succesfully noted as done." });
            }
            catch (DataException)
            {
                Message = "An error occured while saving changes.";
            }
            return Json(new { Success = true, Msg = Message });

        }

        public ActionResult Details(int id)
        {
            TodoItem todoItem = _repositoryTodoItem.Query.FirstOrDefault(c => c.Id == id);

            if (todoItem == null)
            {
                todoItem = new TodoItem();
                todoItem.StartTime = DateTime.Now;
                todoItem.UserId = User.Identity.Name;
                todoItem.Done = false;
                todoItem.SortNum = 0;
                return PartialView("_TodoItemForm", todoItem);
            }

            return PartialView("_TodoItemDetails", todoItem);
        }

        [HttpPost]
        public JsonResult Save(TodoItem todoItem)
        {
            string Message = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (todoItem.Id == 0)
                    {
                        Message = "Task was successfully created!";
                        _repositoryTodoItem.Add(todoItem);
                    }
                    else if (todoItem.Id > 0)
                    {
                        if (todoItem.UserId != User.Identity.Name)
                        {
                            Message = "You have no rights in changing this task. you have not created it!";
                            return Json(new { Success = false, Msg = Message });
                        }
                        Message = "Task was successfully updated.";
                        _repositoryTodoItem.Edit(todoItem);
                    }
                    _repositoryTodoItem.SaveChanges();

                    return Json(new { Success = todoItem.Id >= 0, Msg = Message });
                }
                else
                {
                    Message = "You have not entered all data or have entered data in invalid form. Please try again! If problem persists contact Administrator!";
                }
            }
            catch (DataException)
            {
                Message = "An error occured while saving changes.";
            }

            return Json(new { Success = false, Msg = Message });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            TodoItem todoItem = _repositoryTodoItem.Query.FirstOrDefault(c => c.Id == id);
            if (todoItem == null)
            {
                return Json(new { Success = false, Msg = "You selected a non existent item." });
            }

            string Message = "";

            if (todoItem.UserId != User.Identity.Name)
            {
                Message = "You have no rights in changing this task. you have not created it!";
                return Json(new { Success = false, Msg = Message });
            }
            try
            {
                _repositoryTodoItem.Delete(todoItem);
                _repositoryTodoItem.SaveChanges();
                Message = "Task was successfully deleted.";
                return Json(new { Success = true, Msg = Message });
            }
            catch (DataException)
            {
                Message = "An error occured while saving changes.";
            }
            return Json(new { Success = false, Msg = Message });
        }
        #endregion

        #region FetchingAndSortingData
        public ActionResult GetCalendarEvents()
        {
            string userId = User.Identity.Name;
            var events = _repositoryTodoItem.Query.Where(c => (c.UserId == userId));
            var calendar = new List<object>();

            foreach (TodoItem item in events)
            {
                calendar.Add(new { id = item.Id, title = item.Title, start = item.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), end = item.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), className = item.Priority, description = item.Description });
            }
            return new JsonResult { Data = calendar.ToArray(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SortTODOs(List<int> items)
        {
            int order = 1;
            string Message = "";
            try
            {
                foreach (int id in items)
                {
                    TodoItem todoItem = _repositoryTodoItem.Query.FirstOrDefault(c => c.Id == id);
                    todoItem.SortNum = order;
                    ++order;
                    _repositoryTodoItem.Edit(todoItem);
                }
                _repositoryTodoItem.SaveChanges();
                return Json(new { Success = true, Msg = "List was successfully sorted!" });
            }
            catch (DataException)
            {
                Message = "An error occured while saving changes.";
            }
            return Json(new { Success = false, Msg = Message });
        }

        public ActionResult GetTODOs(int section = 1)
        {
            string userId = User.Identity.Name;
            var events = _repositoryTodoItem.Query.Where(c => c.UserId == userId && c.Done == false).OrderBy(c => c.StartTime > DateTime.Now).ThenBy(c => c.SortNum).ThenBy(c => c.PriorityId).ThenBy(c => c.StartTime).Take(section * SectionSize);
            var viewModel = new TODOlistViewModel
            {
                TodoItems = events,
                SectionInfo = new SectionInfo
                {
                    CurrentSection = section,
                    ItemsLoaded = events.Count(),
                    TotalItems = _repositoryTodoItem.Query.Where(c => c.UserId == userId && c.Done == false).Count()
                }
            };
            ViewBag.Archive = false;
            return PartialView("_ListOfTodoItems", viewModel);
        }

        public ActionResult GetArchive(int section = 1)
        {
            string userId = User.Identity.Name;
            var events = _repositoryTodoItem.Query.Where(c => c.UserId == userId && c.Done == true).OrderByDescending(c => c.TimeFinished).Take(section * SectionSize);
            var viewModel = new TODOlistViewModel
            {
                TodoItems = events,
                SectionInfo = new SectionInfo
                {
                    CurrentSection = section,
                    ItemsLoaded = events.Count(),
                    TotalItems = _repositoryTodoItem.Query.Where(c => c.UserId == userId && c.Done == true).Count()
                }
            };
            ViewBag.Archive = true;
            return PartialView("_ListOfTodoItems", viewModel);
        }
        #endregion
    }
}
