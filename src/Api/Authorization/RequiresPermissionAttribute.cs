namespace Azoxia.Core.Api.Authorization
{
    using System;

    /// <summary>
    /// Declares a required permission for an MVC endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RequiresPermissionAttribute(string permission) :
        Attribute
    {
        /// <summary>
        /// Gets the required permission name in <c>resource.action</c> format.
        /// </summary>
        public string Permission { get; } = permission;
    }
}

