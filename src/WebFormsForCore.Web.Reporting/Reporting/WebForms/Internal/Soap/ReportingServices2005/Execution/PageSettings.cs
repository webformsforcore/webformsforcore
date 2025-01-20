﻿
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution
{
  [DebuggerStepThrough]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
  [GeneratedCode("wsdl", "2.0.50727.42")]
  [DesignerCategory("code")]
  [Serializable]
  public class PageSettings
  {
    private ReportPaperSize paperSizeField;
    private ReportMargins marginsField;

    public ReportPaperSize PaperSize
    {
      get => this.paperSizeField;
      set => this.paperSizeField = value;
    }

    public ReportMargins Margins
    {
      get => this.marginsField;
      set => this.marginsField = value;
    }
  }
}
