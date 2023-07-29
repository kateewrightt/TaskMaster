using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CAB301_Assingment3
{
    public class Task
    {

        public string Title;
        public int Time;
        public List<string> DependentTask = new List<string>();
        public int EarliestTime;
        public Task(string title, int time, List<string> dependentTask)
        {
            Title = title;
            Time = time;
            DependentTask = dependentTask;
        }

        public Task(string title, int time)
        {
            Title = title;
            Time = time;
            DependentTask = null;
        }

        /// <summary>
        /// The ToStringTasks method converts the list of dependent tasks into a
        /// string representation, where each task is separated by a space.
        /// </summary>
        /// <returns></returns>
        public string ToStringTasks()
        {
            string ToReturn = null;
            if (DependentTask != null)
            {
                foreach (var task in DependentTask)
                {
                    ToReturn += task+" ";           
                }
            }
            return ToReturn;
        }

        /// <summary>
        /// The ToStringTasksComma method converts the list of dependent tasks into
        /// a string representation, where each task is separated by a comma.
        /// </summary>
        /// <returns></returns>
        public string ToStringTasksComma()
        {
            string ToReturn = null;
            if (DependentTask != null)
            {
                foreach (var task in DependentTask)
                {

                    ToReturn += "," + task;

                }
            }
            return ToReturn;
        }

    }
}
