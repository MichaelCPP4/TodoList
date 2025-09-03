using System.ComponentModel;
using System.Globalization;

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
        public Priority PriorityStatus{get;set;}
    }

    interface IEditTask
    {
        public void Edit();
    }

    interface INotifiable
    {
        public void Notify();
    }

    public abstract class TodoItem : IPrioritizable, IEditTask
    {

        ~TodoItem(){}

        public event Action OnTaskCompleted;

        //private int id;
        public int Id{get;set;}

        //private string title;
        public string Title{get;set;}

        //private string description;
        public string Description{get;set;}

        public Priority PriorityStatus{get;set;}

        //private bool isCompleted = false;
        public bool IsCompleted{get;set;}

        public abstract void Display();

        public virtual void MarkAsDone()
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke();
        }

        public virtual void Edit(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }

    class SimpleTask : TodoItem
    {
        public SimpleTask(){Title = "Дельце";}

        public SimpleTask(int id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public override void Display()
        {
            Console.WriteLine($"Обычная задача. {Title}\nОписание:\n{Description}");
        }
        
    }

    class RecurringTask : SimpleTask, INotifiable
    {
        public override void Display()
        {
            base.Display();
            Console.WriteLine($"Задача повторяется каждый {day}");
        }

        public void Reloadtask()
        {
            if(day >= DateTime.Now.DayOfWeek)
                IsCompleted = false;
        }

        public void Notify()
        {
            Console.WriteLine($"Напоминание: Задача повторяется каждые {day} дней!");
        }

        private DayOfWeek day;
        internal DayOfWeek Day{get{return day;}set{day = value;}}

        public virtual void Edit(string title, string description, DayOfWeek day)
        {
            Title = title;
            Description = description;
            this.day = day;
        }
    }


    class TimedTask : TodoItem, INotifiable
    {
        public TimedTask(DateTime dateTime){ Title = "Срочное"; this.dateTime = dateTime;}

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
            Console.WriteLine($"Задача с дедлайном! {Title}\nОписание:\n{Description}");
        }

        public DateTime DeadLine
        {
            get =>dateTime;
            set
            {
                if(value.Year >= 0) dateTime = value;
                else Console.WriteLine("Ошибка: дата не назначена!");
            }
        }

        public override void MarkAsDone()
        {
            base.MarkAsDone();
            if(DateTime.Now < dateTime)
                Console.WriteLine("Задача выполнена вовремя, молодец!");
            else
                Console.WriteLine("Просрал дедлайн, бро..");
        }

        public virtual void Edit(string title, string description, DateTime dateTime)
        {
            Title = title;
            Description = description;
            this.dateTime = dateTime;
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

        public void EditSimpleTask()

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
                if(task.Id == freeId) freeId++;
                else break;
            }

            return freeId;
        }
    }


}




//ДОДЕЛАТЬ МЕТОД EDIT()