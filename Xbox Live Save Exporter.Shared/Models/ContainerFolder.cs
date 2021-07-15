using System;

namespace Xbox_Live_Save_Exporter
{
    class ContainerFolder
    {
        #region Constructors
        public ContainerFolder(string name, byte id, Guid guid, string path)
        {
            Name = name;
            Id = id;
            Guid = guid;
            Path = path;
        }
        #endregion

        #region Properties
        /// <summary> Folder name </summary>
        public string Name { get; private set; }
        /// <summary> Id of the container inside the folder </summary>
        public byte Id { get; private set; }
        /// <summary> Folder GUID </summary>
        public Guid Guid { get; private set; }
        /// <summary> Folder path </summary>
        public string Path { get; private set; }
        #endregion
    }
}
