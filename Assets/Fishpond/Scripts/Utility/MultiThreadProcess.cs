using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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

        private async void Process()
        {
            if (!NextProcessFlag) return;

            NextProcessFlag = false;

            await Task.Run(() =>
            {
                int queueCount = threadTasks.Count;
                for (int i = 0; i < queueCount; i++)
                {
                    if (threadTasks[i] != null)
                        threadTasks[i]();
                }

                threadTasks.Clear();
            });

            NextProcessFlag = true;
        }

        private void Update()
        {
            Process();
        }
    }
}

