using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using log4net;

namespace WorkShop
{
    public abstract class Pool
    {
        //Declaramos el logger
        private static readonly ILog logger = LogManager.GetLogger(typeof(Pool));

        private string name;
        private int minSize;
        private int maxSize;
        public ArrayList lockedObjects;
        public ArrayList unlockedObjects;


        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="Name">Nombre del pool</param>
        public Pool(string Name)
        {
            this.name = Name;
            lockedObjects = new ArrayList();
            unlockedObjects = new ArrayList();
            minSize = 0;
            maxSize = 1;
        }

        /// <summary>
        /// Constructor sobrecargado
        /// </summary>
        /// <param name="Name">Nombre del pool</param>
        /// <param name="MinSize">Tama�o m�nimo</param>
        /// <param name="MaxSize">Tama�o m�ximo</param>
        public Pool(string Name, int MinSize, int MaxSize)
        {
            this.name = Name;
            lockedObjects = new ArrayList();
            unlockedObjects = new ArrayList();
            this.minSize = MinSize;
            this.maxSize = MaxSize;
        }

        /// <summary>
        /// Recuperar nombre
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.name;
        }

        /// <summary>
        /// Recuperar objetos totales
        /// </summary>
        /// <returns>N�mero de objetos bloqueados y libres</returns>
        public int GetSize()
        {
            return lockedObjects.Count + unlockedObjects.Count;
        }

        /// <summary>
        /// Recuperar objetos en uso
        /// </summary>
        /// <returns>N�mero de objetos en uso</returns>
        public int GetUsed()
        {
            return lockedObjects.Count;
        }

        /// <summary>
        /// Recuperar objetos libres
        /// </summary>
        /// <returns>N�mero de objetos disponibles</returns>
        public int GetAvailable()
        {
            return unlockedObjects.Count;
        }

        /// <summary>
        /// Recuperar tama�o m�nimo
        /// </summary>
        /// <returns>Tama�o m�nimo</returns>
        public int GetMinSize()
        {
            return this.minSize;
        }

        /// <summary>
        /// Recuperar tama�o m�ximo
        /// </summary>
        /// <returns>Tama�o m�ximo</returns>
        public int GetMaxSize()
        {
            return this.maxSize;
        }

        /// <summary>
        /// Establecer tama�o m�nimo
        /// </summary>
        /// <param name="MinSize">Nuevo tama�o m�nimo</param>
        public void SetMinSize(int MinSize)
        {
            if (minSize > maxSize)
            {
                SetMaxSize(minSize);
            }
            this.minSize = MinSize;

            while (GetSize() < minSize)
            {
                object obj = createObject();
                if (obj == null)
                {
                    break;
                }
                else
                {
                    unlockedObjects.Add(obj);
                }
            }
        }

        /// <summary>
        /// Establecer tama�o m�ximo
        /// </summary>
        /// <param name="MaxSize">Nuevo tama�o m�ximo</param>
        public void SetMaxSize(int MaxSize)
        {
            if (MaxSize >= minSize)
            {
                this.maxSize = MaxSize;

                while ((unlockedObjects.Count > 0) && (GetSize() > maxSize))
                {
                    destroyObject(unlockedObjects[0]);
                    unlockedObjects.RemoveAt(0);
                }
            }

            logger.Debug(name + ".maxSize: " + this.maxSize);
        }
      
        /// <summary>
        /// M�todo abstracto de creaci�n de objeto para el pool
        /// </summary>
        /// <returns>Objeto creado</returns>
        protected abstract object createObject();

        /// <summary>
        /// M�todo abstracto para destrucci�n de objeto del pool
        /// </summary>
        /// <param name="o">Objeto destruido</param>
        protected abstract void destroyObject(object o);

        /// <summary>
        /// Obtenci�n de objeto
        /// </summary>
        /// <returns>Objeto libre</returns>
        public object GetObject()
        {
            object o = null;

            if (unlockedObjects.Count > 0)
            {
                o = unlockedObjects[0];
                unlockedObjects.RemoveAt(0);
            }
            if (o == null)
            {
                if ((maxSize <= 0) || (GetSize() < maxSize))
                {
                    o = createObject();
                }
            }
            if (o != null)
            {
                lockedObjects.Add(o);
                logger.Debug(name + "[Get]");
            }
            return o;
        }

        /// <summary>
        /// Liberar objeto 
        /// </summary>
        /// <param name="o">Objeto a liberar</param>
        /// <returns>True o false si se ha liberado el objeto</returns>
        public bool ReleaseObject(object o)
        {
            bool Removed = this.lockedObjects.Contains(o);
            this.lockedObjects.Remove(o);
            if (Removed)
            {
                if (GetSize() < maxSize)
                {
                    unlockedObjects.Add(o);
                }
                else
                {
                    destroyObject(o);
                }

                logger.Debug(name + "[Release]");
            }
            return Removed;
        }
    }
}
