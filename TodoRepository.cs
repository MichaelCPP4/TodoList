using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace TodoRepository
{

    public enum Priority
    {
        Light,
        Middle,
        High,
        VeryHight
    }


    interface IPrioritizable
    {
        public Priority PriorityStatus { get; set; }
    }

    interface IEditTask
    {
        public void Edit(string title, string description, DayOfWeek? day = null, DateTime? dateTime = null);
    }

    interface INotifiable
    {
        public void Notify();
    }

    public abstract class TodoItem : IPrioritizable, IEditTask
    {

        ~TodoItem() { }

        public event Action OnTaskCompleted;

        //private int id;
        public int Id { get; set; }

        //private string title;
        public string Title { get; set; }

        //private string description;
        public string Description { get; set; }

        public Priority PriorityStatus { get; set; }= Priority.Middle;

        //private bool isCompleted = false;
        public bool IsCompleted { get; set; }

        public abstract void Display();

        public virtual void MarkAsDone()
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke();
        }

        public abstract void Edit(string title, string description, DayOfWeek? day = null, DateTime? dateTime = null);
    }

    class SimpleTask : TodoItem
    {
        public SimpleTask() { Title = "Дельце"; }

        public SimpleTask(int id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public override void Display()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Обычная задача");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nОписание:\n{Description}");
        }

        public override void Edit(string title, string description, DayOfWeek? day = null, DateTime? dateTime = null)
        {
            Title = title;
            Description = description;
        }

    }

    class RecurringTask : SimpleTask, INotifiable
    {
        public override void Display()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Регулярная задача каждый {day}");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nПриоритет: {PriorityStatus}\nОписание:\n{Description}");

            //Console.WriteLine($"Задача повторяется каждый {day}");
        }

        public void Reloadtask()
        {
            if (day >= DateTime.Now.DayOfWeek)
                IsCompleted = false;
        }

        public void Notify()
        {
            Console.WriteLine($"Напоминание: Задача повторяется каждые {day} дней!");
        }

        private DayOfWeek day;
        internal DayOfWeek Day { get { return day; } set { day = value; } }

        public override void Edit(string title, string description, DayOfWeek? day = null, DateTime? dateTime = null)
        {
            Title = title;
            Description = description;
            if (day.HasValue)
                this.day = (DayOfWeek)day;
            else Console.WriteLine("Ошибка: Некорректная дата!");
        }
    }


    class TimedTask : TodoItem, INotifiable
    {
        public TimedTask(DateTime dateTime) { Title = "Срочное"; this.dateTime = dateTime; }

        public TimedTask(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public void Notify()
        {
            Console.WriteLine($"Напоминание: задача {Title}\nСрок до {dateTime}.");
        }

        public override void Display()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Дедлайн - {DeadLine}");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nПриоритет: {PriorityStatus}\nОписание:\n{Description}");

            //Console.WriteLine($"Задача с дедлайном! {Title}\nОписание:\n{Description}");
        }

        public DateTime DeadLine
        {
            get => dateTime;
            set
            {
                if (value.Year >= 0) dateTime = value;
                else Console.WriteLine("Ошибка: дата не назначена!");
            }
        }

        public override void MarkAsDone()
        {
            base.MarkAsDone();
            if (DateTime.Now < dateTime)
                Console.WriteLine("Задача выполнена вовремя, молодец!");
            else
                Console.WriteLine("Просрал дедлайн, бро..");
        }

        public override void Edit(string title, string description, DayOfWeek? day = null, DateTime? dateTime = null)
        {
            Title = title;
            Description = description;
            if (dateTime.HasValue)
                this.dateTime = (DateTime)dateTime;
        }


        private DateTime dateTime;
    }

    public class TodoList
    {
        private List<TodoItem> todoItems;

        TodoList()
        {
            todoItems = new List<TodoItem>();
        }

        public void CreateSimpleTask()
        {
            int id = GenerateIdTask();
            string title = AskString("Введите название: ");
            string description = AskString("Введите описание:");
            todoItems.Add(new SimpleTask(id, title, description));
        }

        public void RemoveTask(TodoItem task)
        {
            foreach (var item in todoItems)
            {
                if (task.Id == item.Id)
                {
                    todoItems.Remove(item);
                }
            }
        }

        public void RemoveTask(int idTask)
        {
            foreach (var item in todoItems)
            {
                if (idTask == item.Id)
                {
                    todoItems.Remove(item);
                }
            }
        }

        public List<TodoItem> FindTaskByName(string name)
        {
            return todoItems.FindAll(x => x.Title.Contains(name)).ToList();
        }

        public List<TodoItem> FilterByPriority(Priority priority)
        {
            return todoItems.FindAll(x => x.PriorityStatus == priority).ToList();
        }

        public List<TodoItem> OrderByPriority()
        {
            return todoItems.OrderBy(x => x.PriorityStatus).ToList();
        }

        public List<TodoItem> OrderByDescendingPriority()
        {
            return todoItems.OrderByDescending(x => x.PriorityStatus).ToList();
        }

        public void Dysplay(List<TodoItem> todoItemResult)
        {
            Console.WriteLine("----------\nЗаметки :D\n----------");
            Console.WriteLine("Задачи:");
            foreach (var item in todoItemResult)
            {
                item.Display();
            }
            Console.WriteLine("------------------------");
            // if(isCreate && isRemove && isEdit)
            // {
            //     Console.WriteLine($"Выбрать задачу(Id) | Добавить задачу(a) | Удалить задачу (r) | Выйти из программы: Ctrl+C");
            //     string input = AskString(Console.ReadLine());
            //     InputTask(input);
            // }
            
            // if()
            // {
                
            // }

        }

        private void InputTask()
        {
            if(isCreate && isRemove && isEdit)
            {
                Console.WriteLine($"Редактировать задачу(Id) | Добавить задачу(a) | Удалить задачу (r) | Выйти из программы: Ctrl+C");
                string input = AskString(Console.ReadLine());

                if (int.TryParse(input, out int id))
                {
                    todoItems.FindLast(x => x.Id == id).Edit();
                }
                else
                {
                // Обработка команд типа 'a', 'r'
                switch (input)
                {
                    case "a": AddTask(); break;
                    case "r": RemoveTask(); break;
                    default: Console.WriteLine("Неизвестная команда"); break;
                }
}
            }
            
            if()
            {
                
            }
            return
        }

        private string AskString(string message)
        {
            string input;

            do
            {
                Console.Write(message);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Пусто, давай ещё раз!");

            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        private int GenerateIdTask()
        {
            int freeId = 0;

            foreach (var task in todoItems)
            {
                if (task.Id == freeId) freeId++;
                else break;
            }

            return freeId;
        }

        private bool isEdit = false;
        private bool isCreate = false;
        private bool isRemove = false;
    }


}




//ДОДЕЛАТЬ МЕТОД EDIT()