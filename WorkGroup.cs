using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace WorkShop
{
    public class WorkGroup : Pool
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(WorkGroup));

        private WorkShop workshop;

        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="W">WorkShop</param>
        public WorkGroup(WorkShop W)
            : base(W.getName())
        {
            this.workshop = W;
            
            SetMaxSize(this.workshop.getMaxWorkers());
        }

        /// <summary>
        /// Crear Objeto Worker
        /// </summary>
        /// <returns>Worker</returns>
        protected override object createObject()
        {
            logger.Debug("Worker creado ...");
            return new Worker(workshop);
        }

        /// <summary>
        /// Destruir objeto
        /// </summary>
        /// <param name="o">Objeto a liberar</param>
        protected override void destroyObject(object o)
        {
            //Worker w = (Worker)o;
            logger.Debug("Worker destruido ...");
        }


        /// <summary>
        /// Destruye todos los workers
        /// </summary>
        public void DestroyWorkers()
        {
            foreach (Worker w in this.lockedObjects)
            {
                w.Stop();
            }
            foreach (Worker w in this.unlockedObjects)
            {
                w.Stop();
            }
        }

        /// <summary>
        /// Bloquea y obtiene un worker
        /// </summary>
        /// <returns>Worker</returns>
        public Worker LockWorker()
        {
            Worker w = null;

            try
            {
                lock (this)
                {
                    w = (Worker)this.GetObject();
                }
            }
            catch
            {

            }

            return w;
        }

        /// <summary>
        /// Elimina worker
        /// </summary>
        /// <param name="w">Worker</param>
        /// <returns></returns>
        public bool UnlockWorker(Worker w)
        {
            lock (this)
            {
                ReleaseObject(w);
            }
            return true;
        }

        /// <summary>
        /// Establecer número de Workers máximo
        /// </summary>
        /// <param name="maxWorkers">Número de Workers máximo</param>
        public void SetMaxWorkers(int maxWorkers)
        {
            base.SetMaxSize(maxWorkers);
        }
    }
}
