using System;

namespace WhiteWillow
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class CategoryAttribute : Attribute
    {
        public string[] Category;
        public CategoryAttribute(params string[] category) => Category = category;
    }
}
