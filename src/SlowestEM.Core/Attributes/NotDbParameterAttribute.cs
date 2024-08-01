namespace SlowestEM.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotDbParameterAttribute : Attribute
    {
    }
}