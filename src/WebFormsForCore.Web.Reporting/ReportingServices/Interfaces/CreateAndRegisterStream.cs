﻿
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.ReportingServices.Interfaces
{
  internal delegate Stream CreateAndRegisterStream(
    string name,
    string extension,
    Encoding encoding,
    string mimeType,
    bool willSeek,
    StreamOper operation);
}
