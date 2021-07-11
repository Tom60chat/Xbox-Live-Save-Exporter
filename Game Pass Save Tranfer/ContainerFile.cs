using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Pass_Save_Tranfer
{
    /// <summary>
    /// Aka sub container
    /// </summary>
    class ContainerFile
    {
        public string Name { get; private set; }
        public Guid Guid { get; private set; }
        public string Path { get; private set; }

        public ContainerFile(string name, Guid guid, string path)
        {
            Name = name;
            Guid = guid;
            Path = path;
        }

        public static IList<ContainerFile> TryParse(ContainerFolder folder)
        {
            List<ContainerFile> files = new List<ContainerFile>();

            try
            {
                // Open a filestream with the user selected file
                FileStream file = new FileStream(System.IO.Path.Combine(folder.Path, "container." + folder.Id), FileMode.Open);

                // Create a binary reader that will be used to read the file
                BinaryReader reader = new BinaryReader(file);

                // Grab the type
                int type = reader.ReadInt32();

                int numFiles = reader.ReadInt32();

                for (int y = 0; y < numFiles; y++)
                {
                    string fileName = BinaryReaderHelper.ReadUnicodeString(reader);

                    // Ignore all the white space of the block
                    while (reader.ReadByte() == 0x0) { }

                    // Go back to a position after reading a non-empty byte
                    reader.BaseStream.Position--;

                    // The guid folder that the files reside in
                    byte[] guid1 = reader.ReadBytes(4);
                    Array.Reverse(guid1);
                    byte[] guid2 = reader.ReadBytes(2);
                    Array.Reverse(guid2);
                    byte[] guid3 = reader.ReadBytes(2);
                    Array.Reverse(guid3);
                    byte[] guid4 = reader.ReadBytes(2);
                    byte[] guid5 = reader.ReadBytes(6);

                    // The second guid folder that the files reside in
                    byte[] guid6 = reader.ReadBytes(4);
                    Array.Reverse(guid6);
                    byte[] guid7 = reader.ReadBytes(2);
                    Array.Reverse(guid7);
                    byte[] guid8 = reader.ReadBytes(2);
                    Array.Reverse(guid8);
                    byte[] guid9 = reader.ReadBytes(2);
                    byte[] guid10 = reader.ReadBytes(6);

                    Guid guid = new Guid(BitConverter.ToString(guid1).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid2).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid3).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid4).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid5).Replace("-", string.Empty));
                    // The second guid is the same
                    string subSecondGuid = BitConverter.ToString(guid6).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid7).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid8).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid9).Replace("-", string.Empty) + "-" + BitConverter.ToString(guid10).Replace("-", string.Empty);

                    string path = System.IO.Path.Combine(folder.Path, guid.ToString("N").ToUpper());

                    files.Add(new ContainerFile(fileName, guid, path));
                }

                reader.Dispose();
                file.Dispose();

                return files;
            }
            catch
            {
                return null;
            }
        }
    }
}
