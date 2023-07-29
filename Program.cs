using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace CAB301_Assingment3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ListOfTasks tasks = new ListOfTasks();
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nPlease choose an option... " +
                    "\n 1. Import Tasks " +
                    "\n 2. Add New Task " +
                    "\n 3. Remove a Task " +
                    "\n 4. Edit a Task " +
                    "\n 5. Save Task File " +
                    "\n 6. Find a Sequence of Tasks " +
                    "\n 7. Find Commencement Times " +
                    "\n 8. Exit " +
                    "\n");

                switch (Console.ReadLine())
                {
                    case "1":
                        importFiles(tasks);
                        break;
                    case "2":
                        addTask(tasks);
                        break;
                    case "3":
                        removeTask(tasks);
                        break;
                    case "4":
                        editTask(tasks);
                        break;
                    case "5":
                        saveFile(tasks);
                        break;
                    case "6":
                        findSequence(tasks);
                        break;
                    case "7":
                        findTime(tasks);
                        break;
                    case "8":
                        running = false;
                        break;
                    default:
                        Console.Write("\nInvalid Input. Try Again.\n");
                        break;
                }
            }
        }

        /// <summary>
        /// Prompts the user to provide a valid path to a file containing tasks,
        /// then calls the createTasks method of the ListOfTasks class to create
        /// the tasks based on the provided file.
        /// </summary>
        /// <param name="Tasks"></param>
        public static void importFiles(ListOfTasks Tasks)
        {
            Console.WriteLine("\n Please provide a valid path to the file containing the tasks \n");
            Tasks.createTasks(Console.ReadLine());
        }

        /// <summary>
        ///  The addTask method adds a task to a task list based on user
        ///  input and task information provided through the console.
        /// </summary>
        /// <param name="Tasks"></param>
        public static void addTask(ListOfTasks Tasks)
        {
            Console.WriteLine("\n Please input the task's name, its time cost and any dependencies (leave blank if none) seperated by commas as such: \n T5, 70, T2, T4");
            string TaskInfo = Console.ReadLine();
            string[] TaskInfoArray = TaskInfo.Split(',');
            Tasks.AddTask(TaskInfoArray);
        }

        /// <summary>
        /// Calls the RemoveTask method of the ListOfTasks class
        /// to remove a specific task from the list of tasks.
        /// </summary>
        /// <param name="Tasks"></param>
        public static void removeTask(ListOfTasks Tasks)
        {
            Tasks.RemoveTask();
        }

        /// <summary>
        /// The editTask method allows users to edit tasks by interacting with
        /// the console and making changes to the task list using a Graph object.
        /// </summary>
        /// <param name="tasks"></param>
        public static void editTask(ListOfTasks tasks)
        {
            Console.WriteLine("\n");
            Graph graph = new Graph();
            tasks.EditTask(graph);
        }

        /// <summary>
        /// Calls the SaveFile method of the ListOfTasks class to save the list of tasks to a file.
        /// </summary>
        /// <param name="Tasks"></param>
        public static void saveFile(ListOfTasks Tasks)
        {
            Tasks.SaveFile();
        }

        /// <summary>
        /// The findSequence method checks for duplicates, sorts tasks based on dependencies,
        /// checks for circular dependencies, checks for valid task configuration, and saves the sorted sequence of tasks.
        /// </summary>
        /// <param name="Tasks"></param>
        public static void findSequence(ListOfTasks Tasks)
        {
            if (ListOfTasks.HasDuplicates(Tasks))
            {
                Console.WriteLine("The input file contains duplicate tasks. Please remove the duplicates and try again.");
                return;
            }
            Graph graph = new Graph();         
            List<string> sortedTasks = Tasks.sortDependentTasks();         
            if (sortedTasks == null)
            {
                Console.WriteLine("\nWarning: Circular dependency detected. The tasks cannot be sorted.");
                Console.WriteLine("Please fix tasks and try again.");
                return;
            }
            if (!graph.AllJobsHaveFinish(Tasks))
            {
                Console.WriteLine("\nWarning: Some jobs do not have a way of finishing. The tasks cannot be sorted.");
                Console.WriteLine("Please fix tasks and try again.");
                return;
            }
            Tasks.SaveSequence(sortedTasks);
        }

        /// <summary>
        /// The findTime method checks for duplicates, sorts tasks based on
        /// dependencies, calculates earliest times, checks for valid task configuration, and saves the results.
        /// </summary>Way 
        /// <param name="Tasks"></param>
        public static void findTime(ListOfTasks Tasks)
        {
            if (ListOfTasks.HasDuplicates(Tasks))
            {
                Console.WriteLine("\nThe input file contains duplicate tasks. Please remove the duplicates and try again.");
                return;
            }
            Graph graph = new Graph();
            List<string> order = Tasks.sortDependentTasks();
            if (order == null)
            {
                Console.WriteLine("\nWarning: Circular dependency detected. The tasks cannot be sorted.");
                Console.WriteLine("Please fix tasks and try again.");
                return;
            }
            List<string> reorderedTasks = order.OrderBy(t => t).ToList();
            ListOfTasks earliestTimes = Tasks.findCommencementTime();
            if (!graph.AllJobsHaveFinish(Tasks))
            {
                Console.WriteLine("\nWarning: Some jobs do not have a way of finishing. The tasks cannot be sorted.");
                Console.WriteLine("Please fix tasks and try again.");
                return;
            }
            earliestTimes.SaveEarliestTimes(reorderedTasks);
        }
    }
}