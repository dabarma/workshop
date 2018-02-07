using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;

namespace WorkShop
{
    public class Manager
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(Manager));

        private WorkShop workshop;
        public EventWaitHandle Wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        public Thread thread;
        private bool terminated;


        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="w">WorkShop</param>
        public Manager(WorkShop w)
        {
            this.workshop = w;
            this.terminated = false;
            thread = new Thread(Run);

        }

        /// <summary>
        /// Paramos Manager
        /// </summary>
        public void Stop()
        {
            this.terminated = true;
            this.Wait.Set();
        }

        /// <summary>
        /// Gestión de tareas
        /// </summary>
        public void Run()
        {
            logger.Debug("-> Entra método Run()");

            try
            {
                // Iteramos hasta que nos lo manden
                while (!terminated)
                {
                    // Si hay alguna tarea encolada ...
                    if (workshop.queue.GetWaitingTaskCount() > 0)
                    {
                        // Obtenemos un trabajador disponible
                        Worker worker = workshop.workGroup.LockWorker();

                        // Si hay algún trabajador disponible ...
                        if (worker != null)
                        {
                            // Obtenemos la siguiente tarea a ejecutar
                            Task task = workshop.queue.Get();

                            // Ejecutamos la tarea
                            logger.Debug("Assigning task to worker");
                            worker.Run(task);

                            // Comprobamos si hay más trabajo y se puede repartir
                            continue;
                        }
                        else
                        {
                            logger.Debug("No workers available");
                        }
                    }
                    else
                    {
                        logger.Debug("No tasks available");
                    }

                    // Esperamos a que un trabajador termine su tarea o que haya nuevas tareas a realizar
                    logger.Debug("Waiting ...");
                    Wait.WaitOne();
                    logger.Debug("Woke up !");
                }

                // Parar a todos los workers
                workshop.workGroup.DestroyWorkers();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);           
            }
            finally
            {
                logger.Debug("<- Sale método Run()");
            }
         
        }
    }
}
