using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CAB301_Assingment3
{
    public class Graph
    {
        private Dictionary<string, List<string>> adjacencyList;
        public Graph()
        {
            adjacencyList = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Adds a task to the graph by updating the adjacency list with the
        /// task title as the key and an empty list of dependencies as the value.
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Task task)
        {
            if (!adjacencyList.ContainsKey(task.Title))
            {
                adjacencyList[task.Title] = new List<string>();
            }
        }

        /// <summary>
        /// Adds a dependency to a task in the graph by updating the adjacency list.
        /// If the task is not already present in the adjacency list, it adds
        /// the task using the AddTask method and then adds the dependency to
        /// the task's list of dependencies.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="dependency"></param>
        public void AddDependency(Task task, string dependency)
        {
            if (!adjacencyList.ContainsKey(task.Title))
            {
                AddTask(task);
            }
            adjacencyList[task.Title].Add(dependency);

        }

        /// <summary>
        /// Checks if the graph has any cycles by performing a depth-first search
        /// (DFS) traversal on each task in the graph. It uses the HasCycleUtil
        /// method to recursively traverse the graph and detect cycles.
        /// </summary>
        /// <returns></returns>
        private bool HasCycle()
        {
            HashSet<string> visited = new HashSet<string>();
            HashSet<string> recursionStack = new HashSet<string>();

            foreach (string taskTitle in adjacencyList.Keys)
            {
                if (HasCycleUtil(taskTitle, visited, recursionStack))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A helper method used by HasCycle() to recursively traverse the
        /// graph and detect cycles. It keeps track of visited tasks and a
        /// recursion stack to detect if a cycle exists.
        /// </summary>
        /// <param name="taskTitle"></param>
        /// <param name="visited"></param>
        /// <param name="recursionStack"></param>
        /// <returns></returns>
        private bool HasCycleUtil(string taskTitle, HashSet<string> visited, HashSet<string> recursionStack)
        {
            if (taskTitle == null || !adjacencyList.TryGetValue(taskTitle, out List<string> dependencies))
            {
                return false;
            }
            visited.Add(taskTitle);
            recursionStack.Add(taskTitle);
            foreach (string dependency in dependencies)
            {
                if (!visited.Contains(dependency) && HasCycleUtil(dependency, visited, recursionStack))
                {
                    return true;
                }
                else if (recursionStack.Contains(dependency))
                {
                    return true;
                }
            }
            recursionStack.Remove(taskTitle);
            return false;
        }

        /// <summary>
        ///  A utility method used by TopologicalSort() to perform a depth-first
        ///  search (DFS) traversal on the graph and populate a stack with the
        ///  task titles in topological order.
        /// </summary>
        /// <param name="taskTitle"></param>
        /// <param name="visited"></param>
        /// <param name="stack"></param>
        private void TopologicalSortUtil(string taskTitle, HashSet<string> visited, Stack<string> stack)
        {
            if (taskTitle == null || !adjacencyList.TryGetValue(taskTitle, out List<string> dependencies))
            {
                return;
            }
            visited.Add(taskTitle);
            dependencies.Sort();
            foreach (string dependency in dependencies)
            {
                if (!visited.Contains(dependency))
                {
                    TopologicalSortUtil(dependency, visited, stack);
                }
            }
            stack.Push(taskTitle);
        }

        /// <summary>
        /// Performs a topological sort on the graph to get a list of task titles
        /// in a valid order. It checks for cycles using HasCycle() and then calls
        /// TopologicalSortUtil() to perform the DFS traversal and generate the sorted list.
        /// </summary>
        /// <returns></returns>
        public List<string> TopologicalSort()
        {
            if (HasCycle())
            {
                return null;
            }

            HashSet<string> visited = new HashSet<string>();
            Stack<string> stack = new Stack<string>();

            foreach (string taskTitle in adjacencyList.Keys)
            {
                if (!visited.Contains(taskTitle))
                {
                    TopologicalSortUtil(taskTitle, visited, stack);
                }
            }

            List<string> result = stack.ToList();
            result.Reverse();

            return result;
        }

        /// <summary>
        /// Checks if all jobs added to the graph have a way of finishing.
        /// </summary>
        /// <returns></returns>
        public bool AllJobsHaveFinish(ListOfTasks listOfTasks)
        {
            var tasks = listOfTasks.GetTasks(); // Retrieve the tasks from the ListOfTasks object
            foreach (var task in tasks)
            {
                if (task.DependentTask != null)
                {
                    foreach (var dependency in task.DependentTask)
                    {
                        var dependentTask = tasks.FirstOrDefault(t => t.Title == dependency);
                        if (dependentTask == null)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// The UpdateTaskName method updates the name of a task by replacing all
        /// occurrences of the old name with the new name in the adjacency list.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void UpdateTaskName(string oldName, string newName)
        {
            if (adjacencyList.ContainsKey(oldName))
            {
                List<string> dependencies = adjacencyList[oldName];
                adjacencyList.Remove(oldName);
                adjacencyList[newName] = dependencies;
            }
            foreach (var dependencies in adjacencyList.Values)
            {
                for (int i = 0; i < dependencies.Count; i++)
                {
                    if (dependencies[i] == oldName)
                    {
                        dependencies[i] = newName;
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the commencement time for each task in the graph based on
        /// the earliest completion times of its dependencies. It iterates over
        /// each task in the adjacency list, calculates the maximum dependency
        /// end time, and updates the task's EarliestTime property. Finally, it
        /// creates a ListOfTasks object and populates it with the task titles
        /// and their commencement times, which is returned as the result.
        /// </summary>
        /// <param name="taskDic"></param>
        /// <returns></returns>
        public ListOfTasks FindCommencementTime(Dictionary<string, Task> taskDic)
        {
            foreach (var taskEntry in adjacencyList)
            {
                string taskTitle = taskEntry.Key;
                Task taskToEdit = taskDic[taskTitle];

                int maxDependencyEndTime = 0;
                foreach (string dependency in taskEntry.Value)
                {
                    if (dependency != null)
                    {
                        int dependencyEndTime = taskDic[dependency].EarliestTime + taskDic[dependency].Time;
                        if (dependencyEndTime > maxDependencyEndTime)
                        {
                            maxDependencyEndTime = dependencyEndTime;
                        }
                    }
                    else
                    {
                        maxDependencyEndTime = 0;
                    }
                }

                taskToEdit.EarliestTime = maxDependencyEndTime;
            }
            ListOfTasks returnList = new ListOfTasks();
            foreach (var task in taskDic.Values)
            {
                string[] taskToAdd = { task.Title, task.EarliestTime.ToString() };
                returnList.AddTask(taskToAdd);
            }
            return returnList;
        }
    }
}
