namespace Framework.Attribiutes.Dependency
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DependencyInjectionAttribute : Attribute
    {
        public DependencyEnum? DependencyType { get; private set; }
        public Type? InterfaceType { get; private set; }

        public DependencyInjectionAttribute(Type? interfaceType = null, DependencyEnum dependencyType = DependencyEnum.Scope)
        {
            DependencyType = dependencyType;
            InterfaceType = interfaceType is null ? null : CheckInterface(interfaceType);
        }
        private static Type CheckInterface(Type type)
        {
            if (!type.IsInterface) throw new Exception("Inncorect type of interface");
            return type;
        }
    }
}
