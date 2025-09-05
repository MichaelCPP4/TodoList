using System.Data.Common;
using System.Reflection;
using TodoRepository;
using InputRepository;

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

        TodoList todoList = new TodoList();

        todoList.todoItems.Add(new SimpleTask(1,"Нарисовать обложку", "Ч/б череп + молния", Priority.Light));
        todoList.todoItems.Add(new TimedTask(2,"Сдать демку", "Отправить трек на лейбл", Priority.Middle, new DateTime(2025, 12, 31)));
        todoList.todoItems.Add(new RecurringTask(3,"Репетиция", "Гоняем сет до 6 часов утра!", Priority.VeryHight, DayOfWeek.Wednesday));
        todoList.todoItems.Add(new SimpleTask(4,"Купить струны", "Комплект 10–46", Priority.Light));
        todoList.todoItems.Add(new TimedTask(5,"Оплатить студию", "Аренда за ноябрь", Priority.High, DateTime.Now.AddDays(2)));
        todoList.todoItems.Add(new RecurringTask(6,"Уборка репбазы", "Смахнуть пыль с комбиков", Priority.Light, DayOfWeek.Monday));

        DisplayOperation.Run(todoList);
    }
}