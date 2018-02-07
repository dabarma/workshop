using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;

namespace WorkShop
{
    public class WorkShop
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(WorkShop));

        private int maxWorkers;
        public Queue queue;
        public WorkGroup workGroup;
        public Manager manager;
        protected string Name;


        /// <summary>
        /// Constructor del WorkShop
        /// </summary>
        /// <param name="Name">Nombre</param>
        /// <param name="maxWorkers">Máximo número de trabajadores</param>
        public WorkShop(string Name, int maxWorkers)
        {
            this.Name = Name;
            this.maxWorkers = maxWorkers;
            workGroup = new WorkGroup(this);
            queue = new Queue(this);
            manager = new Manager(this);

            logger.Debug(Name + ".maxWorkers: " + maxWorkers.ToString());

        }

        /// <summary>
        /// Encolar tareas
        /// </summary>
        /// <param name="t">Tarea a ejecutar</param>
        /// <returns>True o false según el resultado de encolar la tarea</returns>
        public bool Run(Task t)
        {
            return queue.Put(t);
        }

        /// <summary>
        /// Finalizar la ejecución de tareas
        /// </summary>
        public void Stop()
        {
            this.manager.Stop();
        }
               
        /// <summary>
        /// Recuperar nombre
        /// </summary>
        /// <returns>Nombre</returns>
        public string getName()
        {
            return this.Name;
        }

        /// <summary>
        /// Recuperar máximo número de hilos trabajadores
        /// </summary>
        /// <returns>Número máximo de trabajadores</returns>
        public int getMaxWorkers()
        {
            return maxWorkers;
        }

    }
}
