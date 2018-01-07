using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HostBServerClient
{
    class Transition
    {
        public string Tran { get; set; }
        public byte[] HashBytes { get; set; }
        public Transition(string tran)
        {
            Tran = tran;
            HashBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Tran));

        }

        public static string HashToString(byte[] hash)
        {
            string TranHash = BitConverter.ToString(hash);
            TranHash = TranHash.Replace("-", "");
            return TranHash;
        }

        public override string ToString()
        {
            return "Name: " + Tran + " " + "Hash: " + HashToString(HashBytes);
        }
    }
}
