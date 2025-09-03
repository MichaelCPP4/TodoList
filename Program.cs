using System.Data.Common;
using System.Reflection;
using TodoRepository;

namespace Multipotok;

class Program
{
    public delegate bool TaskFilter(TodoItem task);

    List<TodoItem> todoItems = new List<TodoItem>();

    public List<TodoItem>FilterTasks(TaskFilter filter)
    {
        List<TodoItem> listTask = new List<TodoItem>();

        foreach (var task in todoItems)
        {
            if(filter(task))
            {
                listTask.Add(task);
            }
        }


        return listTask;
    }


    public static void Main()
    {   



        var program = new Program();

        var todoItems = new List<TodoItem>{
    new SimpleTask("Нарисовать обложку", "Ч/б череп + молния")
    {
        Id = 1,
        PriorityStatus = Priority.High,
        IsCompleted = false
    },

    new TimedTask("Сдать демку", "Отправить трек на лейбл")
    {
        Id = 2,
        PriorityStatus = Priority.VeryHight, // использую твоё название enum
        DeadLine = new DateTime(2025, 12, 31),
        IsCompleted = false
    },

    new RecurringTask
    {
        Id = 3,
        Title = "Репетиция",
        Description = "Гоняем сет до упора",
        PriorityStatus = Priority.Middle,
        Day = DayOfWeek.Friday,
        IsCompleted = false
    },

    new SimpleTask("Купить струны", "Комплект 10–46")
    {
        Id = 4,
        PriorityStatus = Priority.Light,
        IsCompleted = true
    },

    new TimedTask("Оплатить студию", "Аренда за ноябрь")
    {
        Id = 5,
        PriorityStatus = Priority.High,
        DeadLine = DateTime.Now.AddDays(2),
        IsCompleted = false
    },

    new RecurringTask
    {
        Id = 6,
        Title = "Уборка репбазы",
        Description = "Смахнуть пыль с комбиков",
        PriorityStatus = Priority.Light,
        Day = DayOfWeek.Monday,
        IsCompleted = false
    }

    };



        //  Тут мой мозжевельник был перегружен и я поник, сижу в унынии и пью сок
        todoItems.Add(new SimpleTask());
        todoItems.Add(new TimedTask(new DateTime(2027,11,1)));

        foreach (var item in todoItems)
        {
            item.Display();
        }

        List<TodoItem> completedTasks = program.FilterTasks(task => task.IsCompleted);

        foreach (var task in completedTasks)
        {
            task.Display();
        }
    }
}