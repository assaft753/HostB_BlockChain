using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HostBServerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Client.HostAServicesClient client = new Client.HostAServicesClient())
            {
                Console.WriteLine("Connected to Host A");
                List<Transition> tranList = new List<Transition>();
                List<byte[]> rootHash = new List<byte[]>();
                int number;
                Random rnd = new Random();
                for (int i = 0; i < 12; i++)
                {
                    number = rnd.Next(1000, 10000);
                    var temp = new Transition(number.ToString());
                    tranList.Add(temp);
                    Console.Write("Sending transition " + "{" + temp + "}" + " to Host A");
                    var treehash = client.CreateTreeNode(number.ToString());
                    if (treehash != null)
                    {
                        Console.WriteLine(" and return with Tree Hash " + Transition.HashToString(treehash));
                        Console.WriteLine();
                        rootHash.Add(treehash);
                    }
                    else
                    {
                        Console.WriteLine(" and return without Tree Hash ");
                    }
                }
                Console.WriteLine();
                foreach (var t in tranList)
                {
                    Console.Write("Sending transition " + "{" + t + "}" + " to Audit Proof...");
                    var e = client.CheckAuditProof(t.Tran);
                    if (e == null)
                    {
                        Console.WriteLine(" No Such Transaction in the Blockchain");
                        break;
                    }
                    foreach (var root in rootHash)
                    {
                        var check = CheckAudit(root, t.HashBytes, e.ToList());
                        if (check == true)
                        {
                            Console.WriteLine(" Transaction Found in Hash Root: " + Transition.HashToString(root));
                            break;
                        }
                    }
                }
                Console.ReadLine();

            }
        }


        public static bool CheckAudit(byte[] root, byte[] source, List<Client.AuditProofDetails> trail)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] testHash = source;

            foreach (var auditHash in trail)
            {
                testHash = auditHash.Direction == "Left" ?
                    sha256.ComputeHash(testHash.Concat(auditHash.Hash).ToArray()) :
                    sha256.ComputeHash(auditHash.Hash.Concat(testHash).ToArray());
            }
            return root.SequenceEqual<byte>(testHash);
        }
    }
}
