using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace CAB301_Assingment3
{
    public class ListOfTasks
    {
        public string filepath;
        private List<Task> tasks;
        public ListOfTasks()
        {
            tasks = new List<Task>();
            filepath = null;
        }
        public List<Task> GetTasks()
        {
            return tasks;
        }

        /// <summary>
        /// Creates tasks from a text file, allowing the user to clear existing tasks if present.
        /// </summary>
        /// <param name="textfile"></param>
        public void createTasks(string textfile)
        {
            if (!File.Exists(textfile))
            {
                Console.WriteLine("\n Error: File Not Found\n");
                return;
            }
            if (tasks.Count > 0)
            {
                Console.WriteLine("\nWarning: There are existing tasks in the project.");
                Console.WriteLine("Clearing the current tasks will remove all existing task data.");
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Continue and clear current tasks");
                Console.WriteLine("2. Cancel and keep current tasks\n");

                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        tasks.Clear();
                        LoadTasksFromFile(textfile);
                        break;
                    case "2":
                        Console.WriteLine("Operation cancelled. Current tasks preserved.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Operation cancelled. Current tasks preserved.");
                        break;
                }
            }
            else
            {
                LoadTasksFromFile(textfile);
            }
        }

        /// <summary>
        ///  Loads tasks from a text file into the program's task list, validating the format and handling dependent tasks.
        /// </summary>
        /// <param name="textfile"></param>
        private void LoadTasksFromFile(string textfile)
        {
            filepath = textfile;
            StreamReader sr = new StreamReader(textfile);
            string currentLine = null;
            currentLine = sr.ReadLine();
            while (currentLine != null)
            {
                string[] filecontents = currentLine.Split(',');
                if (IsValidLine(filecontents))
                {
                    if (filecontents.Length > 2)
                    {
                        List<string> Dependents = new List<string>();
                        foreach (var dependency in filecontents.Skip(2))
                        {
                            Dependents.Add(dependency.Trim());
                        }
                        tasks.Add(new Task(filecontents[0], int.Parse(filecontents[1]), Dependents));
                    }
                    else
                    {
                        tasks.Add(new Task(filecontents[0], int.Parse(filecontents[1])));
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid line format: {currentLine}");
                }
                currentLine = sr.ReadLine();
            }
            sr.Close();
        }

        /// <summary>
        /// Checks that given tasks are in the correct format.
        /// </summary>
        /// <param name="filecontents"></param>
        /// <returns></returns>
        private bool IsValidLine(string[] filecontents)
        {
            if (filecontents.Length >= 2)
            {
                if (!string.IsNullOrEmpty(filecontents[0].Trim()) && int.TryParse(filecontents[1].Trim(), out _))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new task to the list of tasks based on the provided task information.
        /// It handles both dependent and independent tasks.
        /// </summary>
        /// <param name="TaskInfo"></param>
        public void AddTask(string[] TaskInfo)
        {
            if (IsValidLine(TaskInfo))
            {
                if (TaskInfo.Length > 2)
                {
                    List<string> Dependents = new List<string>();
                    foreach (var dependency in TaskInfo.Skip(2))
                    {
                        Dependents.Add(dependency);
                    }
                    tasks.Add(new Task(TaskInfo[0], int.Parse(TaskInfo[1]), Dependents));

                }
                else
                {
                    tasks.Add(new Task(TaskInfo[0], int.Parse(TaskInfo[1])));
                }
            }
            else
            {
                Console.WriteLine($"Invalid task format: {string.Join(",", TaskInfo)}");
            }
        }

        /// <summary>
        /// Prints the list of tasks with their titles, time, and dependencies.
        /// </summary>
        public void PrintTasks()
        {
            int counter = 1;
            Console.WriteLine("\t Task Title \t Task Time \t Task Dependencies \t");
            foreach (var task in tasks)
            {
                Console.WriteLine(counter + "\t" + task.Title + "\t\t" + task.Time + "\t\t" + task.ToStringTasks());
                counter++;
            }
        }

        /// <summary>
        /// Allows the user to select and edit a task's properties such as task name, task time, and task dependencies.
        /// </summary>
        public void EditTask(Graph graph)
        {
            Console.WriteLine("\n____________________________________________________________________");
            PrintTasks();
            Console.WriteLine("\n____________________________________________________________________");
            if (tasks.Count == 0)
            {
                Console.WriteLine("\nNo tasks available for edit...");
                return;
            }
            else
            {
                Console.WriteLine("\nPlease select a task to edit");
            }
            int taskSelection;
            while (!int.TryParse(Console.ReadLine(), out taskSelection) || taskSelection < 1 || taskSelection > tasks.Count)
            {
                Console.WriteLine("\nInvalid input. Please enter a valid task number.");
            }
            taskSelection--;
            Console.WriteLine("\nWhich property would you like to edit?\n 1. Task name\n 2. Task time\n 3. Task dependency\n 4. Cancel\n");
            int taskProperty;
            while (!int.TryParse(Console.ReadLine(), out taskProperty) || taskProperty < 1 || taskProperty > 4)
            {
                Console.WriteLine("\nInvalid input. Please enter a valid property number.");
            }
            taskProperty--;
            switch (taskProperty)
            {
                case 0:
                    Console.WriteLine("\nPlease enter a new name");
                    string newTaskName = Console.ReadLine();
                    string oldTaskName = tasks[taskSelection].Title;
                    tasks[taskSelection].Title = newTaskName;
                    foreach (var task in tasks)
                    {
                        if (task.DependentTask != null && task.DependentTask.Contains(oldTaskName))
                        {
                            int index = task.DependentTask.IndexOf(oldTaskName);
                            task.DependentTask[index] = newTaskName;
                        }
                    }
                    graph.UpdateTaskName(oldTaskName, newTaskName);
                    break;
                case 1:
                    Console.WriteLine("\nPlease enter a new task time");
                    int taskTime;
                    while (!int.TryParse(Console.ReadLine(), out taskTime) || taskTime < 0)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid task time (a non-negative integer).");
                    }
                    tasks[taskSelection].Time = taskTime;
                    break;
                case 2:
                    Console.WriteLine("\nDo you want to:\n 1. Add a dependency\n 2. Remove a dependency");

                    int dependencyOption;
                    while (!int.TryParse(Console.ReadLine(), out dependencyOption) || dependencyOption < 1 || dependencyOption > 2)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid option number.");
                    }
                    dependencyOption--;
                    switch (dependencyOption)
                    {
                        case 0:
                            Console.WriteLine("\nPlease provide the name of the dependent task to add");
                            string dependencyToAdd = Console.ReadLine();
                            Console.WriteLine($"Adding dependency: {dependencyToAdd}");
                            Task modifyTask = tasks[taskSelection];
                            if (modifyTask.DependentTask == null)
                            {
                                modifyTask.DependentTask = new List<string>();
                            }
                            modifyTask.DependentTask.Add(dependencyToAdd);

                            tasks.RemoveAt(taskSelection);
                            tasks.Insert(taskSelection, modifyTask);
                            break;
                        case 1:
                            Console.WriteLine("\nPlease provide the name of the dependent task you wish to remove");
                            string dependencyToRemove = Console.ReadLine();
                            tasks[taskSelection].DependentTask.Remove(dependencyToRemove);
                            Task removedTask = tasks[taskSelection];
                            tasks.RemoveAt(taskSelection);
                            Task modifiedTask = new Task(removedTask.Title, removedTask.Time);
                            foreach (var dependency in removedTask.DependentTask)
                            {
                                if (dependency != null && dependencyToRemove != null && dependency != dependencyToRemove)
                                {
                                    if (modifiedTask.DependentTask == null)
                                    {
                                        modifiedTask.DependentTask = new List<string>();
                                    }
                                    modifiedTask.DependentTask.Add(dependency);
                                }
                            }
                            tasks.Insert(taskSelection, modifiedTask);
                            break;
                    }
                    break;
                case 3:
                    Console.WriteLine("\nTask editing canceled.");
                    break;
            }
        }

        /// <summary>
        /// This method removes a specific task from the list of tasks.
        /// It takes a task name as input and searches for a matching task in
        /// the list. If a match is found, the task is removed from the list.
        /// If no match is found, an appropriate message is displayed to
        /// indicate that the task was not found in the list.
        /// </summary>
        public void RemoveTask()
        {
            Console.WriteLine("\n____________________________________________________________________");
            PrintTasks();
            Console.WriteLine("\n____________________________________________________________________");
            if (tasks.Count == 0)
            {
                Console.WriteLine("\nNo tasks available for removal...");
                return;
            }
            else
            {
                Console.WriteLine("\nPlease select a task to remove");
            }
            int taskSelection;
            while (!int.TryParse(Console.ReadLine(), out taskSelection) || taskSelection < 1 || taskSelection > tasks.Count)
            {
                Console.WriteLine("\nInvalid input. Please enter a valid task number.");
            }
            taskSelection--;
            Task removedTask = tasks[taskSelection];
            foreach (var task in tasks)
            {
                if (task.DependentTask != null)
                {
                    task.DependentTask.Remove(removedTask.Title);
                }
            }
            tasks.RemoveAt(taskSelection);
            Console.WriteLine($"\nTask '{removedTask.Title}' has been removed.");
        }

        /// <summary>
        /// Checks if a list of tasks contains any duplicate entries.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <returns></returns>
        public static bool HasDuplicates(ListOfTasks Tasks)
        {
            HashSet<string> taskIds = new HashSet<string>();
            foreach (var task in Tasks.tasks)
            {
                if (taskIds.Contains(task.Title))
                {
                    return true;
                }
                taskIds.Add(task.Title);
            }
            return false;
        }


        /// <summary>
        /// Saves the current list of tasks to a file.
        /// </summary>
        public void SaveFile()
        {
            if (string.IsNullOrEmpty(filepath))
            {
                Console.WriteLine("File path is not specified. Please enter a file path to save the file:");
                filepath = Console.ReadLine();
            }
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                foreach (var task in tasks)
                {
                    writer.WriteLine(task.Title + "," + task.Time + task.ToStringTasksComma());
                }
            }
            string fullPath = Path.GetFullPath(filepath);
            Console.WriteLine("Tasks saved successfully.");
            Console.WriteLine("FilePath: " + fullPath);
        }


        /// <summary>
        /// Saves the list of tasks in a specific order (specified by the Order parameter) to a file.
        /// It only saves the task titles.
        /// </summary>
        /// <param name="Order"></param>
        public void SaveSequence(List<string> Order)
        {
            string sequenceFilePath = "Sequence.txt"; // Create the new file path
            using (StreamWriter writer = new StreamWriter(sequenceFilePath))
            {
                string taskTitles = string.Join(",", Order.Select(taskTitle => tasks.FirstOrDefault(t => t.Title == taskTitle)?.Title));
                writer.WriteLine(taskTitles);
            }
            Console.WriteLine("\nSequence of tasks saved in Sequence.txt");
            string fullPath = Path.GetFullPath(sequenceFilePath);
            Console.WriteLine("FilePath: " + fullPath);
        }



        /// <summary>
        /// Saves the earliest times of the tasks in the specified order (specified by the Order parameter) to a file.
        /// </summary>
        /// <param name="Order"></param>
        public void SaveEarliestTimes(List<string> Order)
        {
            string earliestTimesFilePath = "EarliestTimes.txt"; // Create the new file path
            using (StreamWriter writer = new StreamWriter(earliestTimesFilePath))
            {
                foreach (var taskTitle in Order)
                {
                    var task = tasks.FirstOrDefault(t => t.Title == taskTitle);
                    if (task != null)
                    {
                        writer.WriteLine($"{task.Title},{task.EarliestTime}");
                    }
                }
            }
            Console.WriteLine("\nEarliest times saved in EarliestTimes.txt");
            string fullPath = Path.GetFullPath(earliestTimesFilePath);
            Console.WriteLine("FilePath: " + fullPath);
        }

        /// <summary>
        /// Sorts the tasks based on their dependencies and returns a list of task titles in the sorted order.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public List<string> sortDependentTasks()
        {
            Graph graph = new Graph();
            foreach (var task in tasks)
            {
                if (task.DependentTask != null)
                {
                    foreach (var taskDependency in task.DependentTask)
                    {
                        graph.AddDependency(task, taskDependency);
                    }
                }
                else
                {
                    graph.AddDependency(task, null);
                }
            }
            List<string> sortedTasks = graph.TopologicalSort();
            return sortedTasks;
        }

        /// <summary>
        /// Calculates the earliest times for each task based on their dependencies
        /// and returns a new ListOfTasks object with the updated task list.
        /// </summary>
        /// <returns></returns>
        public ListOfTasks findCommencementTime()
        {
            Graph graph = new Graph();
            foreach (var task in tasks)
            {
                if (task.DependentTask != null)
                {
                    foreach (var taskDependency in task.DependentTask)
                    {
                        graph.AddDependency(task, taskDependency);
                    }
                }
                else
                {
                    graph.AddDependency(task, null);
                }
            }
            List<string> sortedTasks = graph.TopologicalSort();
            Dictionary<string, int> earliestTimes = new Dictionary<string, int>();
            foreach (string taskTitle in sortedTasks)
            {
                Task task = tasks.FirstOrDefault(t => t.Title == taskTitle);
                if (task != null)
                {
                    int maxDependencyTime = 0;
                    if (task.DependentTask != null)
                    {
                        foreach (string dependency in task.DependentTask)
                        {
                            if (earliestTimes.ContainsKey(dependency))
                            {
                                int dependencyTime = earliestTimes[dependency] + tasks.First(t => t.Title == dependency).Time;
                                maxDependencyTime = Math.Max(maxDependencyTime, dependencyTime);
                            }
                        }
                    }
                    earliestTimes[task.Title] = maxDependencyTime;
                    task.EarliestTime = maxDependencyTime;
                }
            }
            ListOfTasks taskList = new ListOfTasks();
            taskList.tasks = tasks;
            return taskList;
        }
    }
}