using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using log4net;

namespace WorkShop
{
    public class Queue
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(Queue));

        private WorkShop workshop;
        private ArrayList waitingTasks;
        private ArrayList runningTasks;

        /// <summary>
        /// Sobrecarga del constructor
        /// </summary>
        /// <param name="w">WorkShop</param>
        public Queue(WorkShop w)
        {
            workshop = w;

            waitingTasks = new ArrayList();
            runningTasks = new ArrayList();

        }

        /// <summary>
        /// Recupera número de tareas esperando
        /// </summary>
        /// <returns>Número de tareas esperando</returns>
        public int GetWaitingTaskCount()
        {
            return waitingTasks.Count;
        }

        /// <summary>
        /// Recupera número de tareas en total en el WorkShop
        /// </summary>
        /// <returns>Número de tareas en total</returns>
        public int GetTotalTaskCount()
        {
            return (waitingTasks.Count + runningTasks.Count);
        }

        /// <summary>
        /// Recupera número de tareas ejecutando
        /// </summary>
        /// <returns></returns>
        public int GetRunningTaskCount()
        {
            return runningTasks.Count;
        }

        /// <summary>
        /// Encolar tareas
        /// </summary>
        /// <param name="t">Tarea</param>
        private void Enqueue(Task t)
        {
            waitingTasks.Add(t);

            t.SetTimeStamp(DateTime.Now.TimeOfDay);
        }

        /// <summary>
        /// Desencolar tarea
        /// </summary>
        /// <param name="index">Posición de la tarea</param>
        /// <returns>Tarea</returns>
        private Task Dequeue(int index)
        {
            Task t = (Task)waitingTasks[index];

            waitingTasks.RemoveAt(index);

            return t;
        }

        /// <summary>
        /// Coger tarea
        /// </summary>
        /// <returns>Tarea</returns>
        public Task Get()
        {
            Task t;

            lock (waitingTasks)
            {
                t = (Task)waitingTasks[0];
                waitingTasks.RemoveAt(0);

                logger.Debug(workshop.getName() + ": Tarea desencolada - Tareas esperando: " + this.GetWaitingTaskCount().ToString());
            }

            lock (runningTasks)
            {
                runningTasks.Add(t);
                logger.Debug(workshop.getName() + ": Iniciada - Tareas ejecutandose: " + this.GetRunningTaskCount().ToString());
            }

            return t;

        }

        /// <summary>
        /// Pone tarea en la cola
        /// </summary>
        /// <param name="t">Tarea</param>
        /// <returns>True o false según el éxito de la operación</returns>
        public bool Put(Task t)
        {
            lock (waitingTasks)
            {
                Enqueue(t);
                logger.Debug(workshop.getName() + ": Encolada tarea - Tareas encoladas: " + this.GetWaitingTaskCount().ToString());

                // El manager esta parado en primera iteración y dormido en las siguientes
                if (workshop.manager.thread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    workshop.manager.thread.Start();
                }
                else
                {
                    workshop.manager.Wait.Set();
                }

            }

            return true;
        }

        /// <summary>
        /// Borra tarea
        /// </summary>
        /// <param name="t">Tarea a borrar</param>
        public void Remove(Task t)
        {
            lock (runningTasks)
            {
                runningTasks.Remove(t);
                logger.Debug(workshop.getName() + ": terminada tarea - Tareas ejecutandose: " + this.GetRunningTaskCount().ToString());
            }
        }

    }
}
