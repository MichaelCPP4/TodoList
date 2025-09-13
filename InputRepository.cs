using TodoRepository;

namespace InputRepository
{ 
    public static class DisplayOperation
    {
        public static async Task Run(TodoList todoList)
        {
            while (true)
            {
                Console.Clear();
                await Dysplay(todoList.todoItems);
                await InputTask(todoList);
            }
        }

        public static async Task Dysplay(List<TodoItem> todoItemResult)
        {
            Console.WriteLine("----------\nЗаметки :D\n----------");
            Console.WriteLine("Задачи:");
            foreach (var item in todoItemResult)
            {
                await item.Display();
            }
            Console.WriteLine("------------------------");
        }

        private static async Task InputTask(TodoList todoList)
        {
            //if(!isCreate && !isRemove && !isEdit)
            //{
                Console.WriteLine($"Редактировать задачу(Id) | Добавить задачу(a) | Удалить задачу (d) | Сортировать по приоритету (t) | Сохранить лист (s) | Загрузить лист (r) | Выйти из программы: Ctrl+C");
                string input = await FunctionInput.AskString();

                if (int.TryParse(input, out int id))
                {
                    var task = todoList.todoItems.FindLast(x => x.Id == id);
                    if(task != null)
                    {
                        var taskEdit = todoList.todoItems.FindLast(x => x.Id == id);
                        await taskEdit.Edit();
                    }
                    else Console.WriteLine("Ошибка: Задачи с таким номером не существует!");
                }
                else
                {
                // Обработка команд типа 'a', 'r'
                switch (input)
                {
                    case "a":
                        Console.WriteLine("Тип задачи: ");
                        Console.WriteLine("1) Простая задача\n2) Повторяющееся задача\n3) Задача с дедлайном");
                        Console.WriteLine("Ваш выбор (1-3):");
                        switch (FunctionInput.AskNumber("Повторите попытку!", 3))
                        {
                            case 1:
                                todoList.CreateSimpleTask();
                                return;
                            case 2:
                                todoList.CreateRecurringTask();
                                return;
                            case 3:
                                todoList.CreateTimedTask();
                                return;
                        }
                    break;
                    case "d":
                        Console.WriteLine("Выберите номер задачи для удаления: ");
                        int maxIndex = todoList.todoItems.OrderByDescending(x => x.Id).First().Id;
                        int idTask = FunctionInput.AskNumber("Попробуйте ещё раз!", maxIndex);
                        todoList.RemoveTask(todoList.todoItems.FindLast(x => x.Id == idTask));
                        return;
                    case "t":
                        todoList.todoItems = todoList.Filter(x => x.PriorityStatus == Priority.Middle);
                        return;
                    case "s":
                        Console.WriteLine("Введите название для файла: ");
                        if(await todoList.SaveTodoList(await FunctionInput.AskString()))
                            Console.WriteLine("Файл сохранён!");
                    return;
                    case "r":
                        Console.WriteLine("Введите название файла: ");
                        if(await todoList.LoadTodoList(await FunctionInput.AskString()))
                        {
                            Console.WriteLine("Файл открыт успешно!");
                            await Task.Delay(1000);
                        }
                        else
                        {
                            Console.WriteLine("Не удалось открыть файл!");
                        }
                    return;
                    default: Console.WriteLine("Неизвестная команда"); break;
                }
}
            //}
        }
    }

        
}





