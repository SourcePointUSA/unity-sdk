using System;
using String = System.String;

public static class GUID
{
    private static string value;
    public static string Value
    {
        get
        {
            if (String.IsNullOrEmpty(value))
                value = Guid.NewGuid().ToString();
            return value;
        }
    }
}