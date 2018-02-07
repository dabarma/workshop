using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WorkShop
{
    public abstract class Task
    {
        static int taskId = 0;
        private string Name;
        private TimeSpan timeStamp;

        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="object">data</param>
        public Task()
        {
            taskId++;

            this.Name = "Tarea: " + taskId.ToString();
        }

        /// <summary>
        /// Obtener nombre
        /// </summary>
        /// <returns>Nombre</returns>
        public string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Obtenemos TimeStamp
        /// </summary>
        /// <returns>TimeStamp</returns>
        public TimeSpan GetTimeStamp()
        {
            return this.timeStamp;
        }

        /// <summary>
        /// Establecemos TimeStamp
        /// </summary>
        /// <param name="ts">TimeStamp</param>
        public void SetTimeStamp(TimeSpan ts)
        {
            this.timeStamp = ts;
        }

        /// <summary>
        /// Ejecutamos tarea con trabajador
        /// </summary>
        public abstract void Run();

    }
}
