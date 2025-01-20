﻿
using System;
using System.Globalization;
using System.Security;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class SecurityAssertionHandler
  {
    private static bool m_assumeFullTrust = true;

    [SecurityCritical]
    [SecurityTreatAsSafe]
    internal static void RunWithSecurityAssert(CodeAccessPermission permission, Action action)
    {
      if (SecurityAssertionHandler.m_assumeFullTrust)
      {
        try
        {
          permission.Assert();
        }
        catch (InvalidOperationException ex)
        {
          string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Security Assertions disabled due to exception: {0}", (object) ex);
          SecurityAssertionHandler.m_assumeFullTrust = false;
        }
      }
      action();
    }

    internal static TResult RunWithSecurityAssert<TResult>(
      CodeAccessPermission permission,
      Func<TResult> action)
    {
      TResult ret = default (TResult);
      SecurityAssertionHandler.RunWithSecurityAssert(permission, (Action) (() => ret = action()));
      return ret;
    }
  }
}
