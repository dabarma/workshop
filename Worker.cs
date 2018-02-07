using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;

namespace WorkShop
{
    public class Worker
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(Worker));

        private WorkShop workShop;
        public EventWaitHandle Wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        private Task task;
        private Thread thread;
        private bool terminated;

        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="W">Workshop</param>
        public Worker(WorkShop W)
        {
            this.workShop = W;
            this.terminated = false;
            thread = new Thread(Run);
        }

        /// <summary>
        /// Método Ejecutar
        /// </summary>
        public void Run()
        {
            logger.Debug("-> Entra método Run()");

            try
            {
                while (! terminated)
                {
                    // Ejecutamos la tarea
                    logger.Debug("Ejecutando task ...");
                    try
                    {
                        task.Run();
                        logger.Debug("Hecho !!...");
                    }
                    catch (Exception e)
                    {
                        logger.Error("Fallo ejecutando la tarea: " + e.Message);
                    }                   

                    // La tarea ha terminado
                    workShop.queue.Remove(task);
                    task = null;

                    // El trabajador está disponible de nuevo.
                    workShop.workGroup.UnlockWorker(this);

                    lock (this)
                    {
                        // Avisamos al jefe de que un trabajador ha terminado su 
                        // tarea y ahora está disponible
                        lock (workShop.manager)
                        {
                            workShop.manager.Wait.Set();
                        }
                    }
                    
                    // Esperamos a que haya más faena
                    logger.Debug("Esperando ...");
                    Wait.WaitOne();
                    logger.Debug("Despertado ! ...");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                logger.Debug("<- Sale método Run()");
            }
        }

        /// <summary>
        /// Paramos el worker
        /// </summary>
        public void Stop()
        {
            this.terminated = true;
            Wait.Set();
        }


        /// <summary>
        /// Método ejecutar tarea
        /// </summary>
        /// <param name="task">Tarea</param>
        public void Run(Task task)
        {
            // Anotamos la tarea a ejecutar
            this.task = task;

            // Si el hilo está activo ...
            if (thread.IsAlive)
            {
                // Le indicamos que hay una nueva tarea por ejecutar
                lock (this)
                {
                    Wait.Set();
                }
            }
            else
            {
                // Arrancamos el hilo para ejecutar la tarea
                thread.Start();
            }
        }

    }
}
