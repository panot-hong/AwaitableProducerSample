using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace AwaitableProducerSample
{
    public abstract class BlockBuilderBase
    {
        public string HashOutput { get; protected set; }
        public int NumberOfBlocks { get; protected set; }
        public string Status => $"Current block is #{NumberOfBlocks} with Hash [{HashOutput}]";


        public abstract void Start();
        public abstract void Stop();
        public abstract void Write(string input);

        protected string HashBytes(byte[] input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(input);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        protected byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            BinaryFormatter bf = new();
            MemoryStream ms = new();
            // It is safe to use just for demo sake
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            bf.Serialize(ms, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete

            return ms.ToArray();
        }

    }

    [Serializable]
    public class Block
    {
        public string? PreviousHash { get; set; }
        public int Nouce { get; set; } = Random.Shared.Next(1, 1000000);
        public byte[] Data { get; set; }
    }
}
