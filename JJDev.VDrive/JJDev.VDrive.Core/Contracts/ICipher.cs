using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Contracts
{
  public interface ICipher
  {
    byte[] Encode(string content);
    string Decode(string encodedContent);
  }
}
