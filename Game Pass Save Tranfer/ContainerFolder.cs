using System;

namespace Game_Pass_Save_Tranfer
{
    class ContainerFolder
    {
        public ContainerFolder(string name, byte id, Guid guid, string path)
        {
            Name = name;
            Id = id;
            Guid = guid;
            Path = path;
        }

        public string Name { get; private set; }
        public byte Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Path { get; private set; }
    }
}
