using System;
using System.ComponentModel;
using System.Reflection;

public enum Interval
{
    [Description("1s")]
    One_Second,
    [Description("1m")]
    One_Minute,
    [Description("3m")]
    Three_Minute,
    [Description("5m")]
    Five_Minute,
    [Description("15m")]
    Fifteen_Minute,
    [Description("30m")]
    Thirty_Minute,
    [Description("1h")]
    One_Hour,
    [Description("2h")]
    Two_Hour,
    [Description("4h")]
    Four_Hour,
    [Description("6h")]
    Six_Hour,
    [Description("8h")]
    Eight_Hour,
    [Description("12h")]
    Twelve_Hour,
    [Description("1d")]
    One_Day,
    [Description("3d")]
    Three_Day,
    [Description("1w")]
    One_Week,
    [Description("1M")]
    One_Month,
    [Description("1Y")]
    One_Year
}

public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        DescriptionAttribute attribute =
            Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
            as DescriptionAttribute;

        return attribute == null ? value.ToString() : attribute.Description;
    }
}
