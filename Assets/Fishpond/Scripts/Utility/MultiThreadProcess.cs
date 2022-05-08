using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace Hsinpa.Utility
{
    public class MultiThreadProcess : MonoBehaviour
    {
        public delegate void ThreadTask();

        public List<ThreadTask> threadTasks = new List<ThreadTask>();

        private bool NextProcessFlag = true;

        public void Enqueue(ThreadTask task)
        {
            threadTasks.Add(task);
        }

        private void Process()
        {
            ThreadPool.QueueUserWorkItem(ThreadProcess);
        }

        private void ThreadProcess(System.Object stateInfo) {
            int queueCount = threadTasks.Count;
            for (int i = 0; i < queueCount; i++)
            {
                if (threadTasks[i] != null)
                    threadTasks[i]();
            }

            threadTasks.Clear();
        }

        private void Update()
        {
            Process();
        }
    }
}

