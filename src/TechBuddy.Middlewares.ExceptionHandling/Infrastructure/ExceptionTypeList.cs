namespace TechBuddy.Middlewares.ExceptionHandling
{
    public class ExceptionTypeList : List<Type>
    {
        public void Add<T>() where T : Exception
        {
            Add(typeof(T));
        }
    }
}
