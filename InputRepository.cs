using TodoRepository;

namespace InputRepository
{ 
    public static class DisplayOperation
    {
        public static void Run(TodoList todoList)
        {
            while (true)
            {
                Console.Clear();
                Dysplay(todoList.todoItems);
                InputTask(todoList);
            }
        }

        public static void Dysplay(List<TodoItem> todoItemResult)
        {
            Console.WriteLine("----------\nЗаметки :D\n----------");
            Console.WriteLine("Задачи:");
            foreach (var item in todoItemResult)
            {
                item.Display();
            }
            Console.WriteLine("------------------------");
        }

        private static void InputTask(TodoList todoList)
        {
            //if(!isCreate && !isRemove && !isEdit)
            //{
                Console.WriteLine($"Редактировать задачу(Id) | Добавить задачу(a) | Удалить задачу (r) | Выйти из программы: Ctrl+C");
                string input = FunctionInput.AskString();

                if (int.TryParse(input, out int id))
                {
                    var task = todoList.todoItems.FindLast(x => x.Id == id);
                    if(task != null)
                    {
                        todoList.todoItems.FindLast(x => x.Id == id).Edit();
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
                    case "r":
                        Console.WriteLine("Выберите номер задачи для удаления: ");
                        int maxIndex = todoList.todoItems.OrderByDescending(x => x.Id).First().Id;
                        int idTask = FunctionInput.AskNumber("Попробуйте ещё раз!", maxIndex);
                        todoList.RemoveTask(todoList.todoItems.FindLast(x => x.Id == idTask));
                        return;
                    default: Console.WriteLine("Неизвестная команда"); break;
                }
}
            //}
        }


        private static bool isEdit = false;
        private static bool isCreate = false;
        private static bool isRemove = false;
    }

        
}





