using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.VisualBasic;
using Newtonsoft.Json;


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
        public Task Edit();
    }

    interface INotifiable
    {
        public void Notify();
    }
    
    public static class FunctionInput
    {   

        public static async Task SaveTasksAsync()
        {
            await Task.Delay(1000);
        }

        public static async Task LoadTaskAsync()
        {
            await Task.Delay(500);
        }

        public static DateTime AskDate()
        {       
        DateTime date;
        while (true)
        {
            //Console.Write(message);
            string input = Console.ReadLine();

            if (DateTime.TryParse(input, out date))
            {
                return date; // всё ок, возвращаем дату
            }
            else
            {
                Console.WriteLine("Неверный формат даты! Попробуй снова (например: 01.11.2025).");
            }
        }
}

        public static async Task<string> AskString()
        {
        string input;

        do
        {
            //Console.Write(message);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                 Console.WriteLine("Пусто, давай ещё раз!");

        } while (string.IsNullOrWhiteSpace(input));

        return input;
        }

        public static int AskNumber(string message, int maxNumber)
        {

            while(true)
            {
                if(int.TryParse(Console.ReadLine(), out int id) && id > 0 && id <= maxNumber)
                    return id;
                else
                    Console.WriteLine(message);
            }
        }

        public static Priority PrioritySwitcher(int index)
        {
            switch (index)
            {
                case 1:
                return Priority.Light;
                case 2:
                return Priority.Middle;
                case 3:
                return Priority.High;
                case 4:
                return Priority.VeryHight;
                default:
                return Priority.Middle;
            }
        }

        public static DayOfWeek DaySwitcher(int index)
        {
            switch (index)
            {
                case 1:
                return DayOfWeek.Sunday;
                case 2:
                return DayOfWeek.Monday;
                case 3:
                return DayOfWeek.Tuesday;
                case 4:
                return DayOfWeek.Wednesday;
                case 5:
                return DayOfWeek.Thursday;
                case 6:
                return DayOfWeek.Friday;
                case 7:
                return DayOfWeek.Saturday;
                default:
                return DayOfWeek.Monday;
            }
        }
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

        public abstract Task Display();

        public virtual void MarkAsDone()
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke();
        }

        public virtual async Task Edit()
        {
            Console.WriteLine("Введите название: ");
            Title = await FunctionInput.AskString();
            Console.WriteLine("Выберите приоритет (1-4): ");
            Console.WriteLine("1) Низкий\n2) Обычный\n3) Высокий\n4) Очень высокий");
            PriorityStatus = FunctionInput.PrioritySwitcher(FunctionInput.AskNumber("Попробуйте ещё раз!", 4));
            Console.WriteLine("Введите описание: ");
            Description = await FunctionInput.AskString();
            await FunctionInput.SaveTasksAsync();
        }
    }

    class SimpleTask : TodoItem
    {
        public SimpleTask() { Title = "Дельце"; }

        [JsonConstructor]
        public SimpleTask(int id, string title, string description, Priority priority)
        {
            Id = id;
            Title = title;
            Description = description;
            PriorityStatus = priority;
        }

        public override async Task Display()
        {
            await FunctionInput.LoadTaskAsync();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Обычная задача");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nОписание:\n{Description}");
        }
    }

    class RecurringTask : SimpleTask, INotifiable
    {
        public override async Task Display()
        {
            await FunctionInput.LoadTaskAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Регулярная задача каждый {day}");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nПриоритет: {PriorityStatus}\nОписание:\n{Description}");
        }

        [JsonConstructor]
        public RecurringTask(int id, string title, string description, Priority priority, DayOfWeek day)
        {
            Id = id;
            Title = title;
            Description = description;
            PriorityStatus = priority;
            Day = day;
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

        public override async Task Edit()
        {
            await FunctionInput.SaveTasksAsync();
            await base.Edit();
            Console.WriteLine("Выберите день недели (1-7): ");
            Console.WriteLine("1) Понедельник\n2) Вторник\n3) Среда\n4) Четверг\n5) Пятница\n6) Суббота\n7) Воскресенье");
            Day = FunctionInput.DaySwitcher(FunctionInput.AskNumber("Попробуй ещё раз!", 7));
        }
    }


    class TimedTask : TodoItem, INotifiable
    {
        public TimedTask(DateTime dateTime) { Title = "Срочное"; this.dateTime = dateTime; }

        [JsonConstructor]
        public TimedTask(int id, string title, string description, Priority priority, DateTime dateTime)
        {
            Id = id;
            Title = title;
            Description = description;
            PriorityStatus = priority;
            this.dateTime = dateTime;
        }

        public void Notify()
        {
            Console.WriteLine($"Напоминание: задача {Title}\nСрок до {dateTime}.");
        }

        public override async Task Display()
        {
            await FunctionInput.LoadTaskAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Дедлайн - {DeadLine}");
            Console.ResetColor();
            Console.WriteLine($"№{Id}\nНазвание: {Title}\nПриоритет: {PriorityStatus}\nОписание:\n{Description}");
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

        public override async Task Edit()
        {
            await FunctionInput.SaveTasksAsync();
            base.Edit();
            Console.WriteLine("Введите дату дедлайна: ");
            dateTime = FunctionInput.AskDate();
        }


        private DateTime dateTime;
    }

    public class TodoList
    {
        public async void CreateSimpleTask()
        {
            int id = GenerateIdTask();
            Console.WriteLine("Введите название: ");
            string title = await FunctionInput.AskString();
            Console.WriteLine("Введите описание:");
            string description = await FunctionInput.AskString();
            Console.WriteLine("Выберите приоритет: ");
            Console.WriteLine("1) Низкий\n2) Обычный\n3) Высокий\n4) Очень высокий");
            Priority priority = FunctionInput.PrioritySwitcher(FunctionInput.AskNumber("Попробуй ещё раз!", 4));
            todoItems.Add(new SimpleTask(id, title, description, priority));
        }

        public async void CreateTimedTask()
        {
            int id = GenerateIdTask();
            Console.WriteLine("Введите название: ");
            string title = await FunctionInput.AskString();
            Console.WriteLine("Введите описание:");
            string description = await FunctionInput.AskString();
            Console.WriteLine("Выберите приоритет: ");
            Console.WriteLine("1) Низкий\n2) Обычный\n3) Высокий\n4) Очень высокий");
            Priority priority = FunctionInput.PrioritySwitcher(FunctionInput.AskNumber("Попробуй ещё раз!", 4));
            Console.WriteLine("Введите дату дедлайна: ");
            DateTime dateTime = FunctionInput.AskDate();
            todoItems.Add(new TimedTask(id, title, description, priority, dateTime));
        }

        public async void CreateRecurringTask()
        {
            int id = GenerateIdTask();
            Console.WriteLine("Введите название: ");
            string title = await FunctionInput.AskString();
            Console.WriteLine("Введите описание:");
            string description = await FunctionInput.AskString();
            Console.WriteLine("Выберите приоритет: ");
            Console.WriteLine("1) Низкий\n2) Обычный\n3) Высокий\n4) Очень высокий");
            Priority priority = FunctionInput.PrioritySwitcher(FunctionInput.AskNumber("Попробуй ещё раз!", 4));
            Console.WriteLine("Введите день недели для повторения задачи: ");
            Console.WriteLine("1) Понедельник\n2) Вторник\n3) Среда\n4) Четверг\n5) Пятница\n6) Суббота\n7) Воскресенье");
            DayOfWeek day = FunctionInput.DaySwitcher(FunctionInput.AskNumber("Попробуй ещё раз!", 7));
            todoItems.Add(new RecurringTask(id, title, description, priority, day));
        }

        public void RemoveTask(TodoItem task)
        {
            foreach (var item in todoItems)
            {
                if (task.Id == item.Id)
                {
                    todoItems.Remove(item);
                    Console.WriteLine("Задача удалена!");
                    return;
                }
            }
            Console.WriteLine("Ошибка: Задачи с таким номером не существует!");
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





        public List<TodoItem> Filter(Func<TodoItem, bool> condition)
        {
            return todoItems.Where(condition).ToList();
        }
        
        public async Task<bool> SaveTodoList(string fileName)
        {
            //var memotyStream = new MemoryStream();
            string json = JsonConvert.SerializeObject(todoItems, Formatting.Indented, 
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            await File.WriteAllTextAsync(fileName + ".json", json);
            return true;
        }

        public async Task<bool> LoadTodoList(string fileName)
        {
            //if (!File.Exists(fileName)) 
                //return false;

            string json = await File.ReadAllTextAsync(fileName + ".json");
            //todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);

            todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });

            return true;
        }














        private int GenerateIdTask()
        {
            int freeId = 1;

            foreach (var task in todoItems)
            {
                if (task.Id == freeId) freeId++;
                else break;
            }

            return freeId;
        }

        public List<TodoItem> todoItems;

        public TodoList()
        {
            todoItems = new List<TodoItem>();
        }
    }
}




//ДОДЕЛАТЬ МЕТОД EDIT()