using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Pass_Save_Tranfer
{
    class Container
    {
        public Container(string friendlyName, string packageFullName, string id, Guid guid, IList<ContainerFolder> folders, string path)
        {
            FriendlyName = friendlyName;
            PackageFullName = packageFullName;
            Id = id;
            Guid = guid;
            Folders = folders;
            Path = path;
        }

        public string FriendlyName { get; private set; }
        public string PackageFullName { get; private set; }
        public string Id { get; private set; }
        public Guid Guid { get; private set; }
        public IList<ContainerFolder> Folders { get; private set; }
        public string Path { get; private set; }

        public static Container TryParse(string path)
        {
            List<ContainerFolder> folders = new List<ContainerFolder>();

            try
            {
                // Open a filestream with the user selected file
                FileStream file = new FileStream(path, FileMode.Open);

                // Create a binary reader that will be used to read the file
                BinaryReader reader = new BinaryReader(file);

                // Grab the type
                int type = reader.ReadInt32();

                int numFolders = reader.ReadInt32();

                string friendlyName = BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32());

                string[] name = BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32()).Split('!');
                string packageFullName = name[0];
                string id = name[1];

                // Skip unknown value
                reader.ReadBytes(0xc);

                // Unknown guid
                Guid guid = new Guid(BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32()));

                if (type == 0xe)
                {
                    // 8 padding bytes
                    reader.ReadBytes(8);
                }

                // Loop through every folder in the index, and print info about it
                for (int i = 0; i < numFolders; i++)
                {
                    string folderName = BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32());

                    string secondName = BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32());

                    // Skip unknown value
                    string UnknownValue = BinaryReaderHelper.ReadUnicodeString(reader, reader.ReadInt32());

                    byte containerId = reader.ReadByte();

                    // Skip unknown data
                    reader.ReadBytes(4);

                    // The guid folder that the files reside in
                    byte[] guid1 = reader.ReadBytes(4);
                    Array.Reverse(guid1);
                    byte[] guid2 = reader.ReadBytes(2);
                    Array.Reverse(guid2);
                    byte[] guid3 = reader.ReadBytes(2);
                    Array.Reverse(guid3);
                    byte[] guid4 = reader.ReadBytes(2);
                    byte[] guid5 = reader.ReadBytes(6);

                    Guid folderGuid = new Guid(BitConverter.ToString(guid1).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid2).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid3).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid4).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid5).Replace("-", string.Empty));

                    // Skip unknown value
                    reader.ReadBytes(0x18);

                    string folderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), folderGuid.ToString("N").ToUpper());

                    folders.Add(new ContainerFolder(folderName, containerId, folderGuid, folderPath));
                }

                reader.Dispose();
                file.Dispose();

                return new Container(friendlyName, packageFullName, id, guid, folders, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
