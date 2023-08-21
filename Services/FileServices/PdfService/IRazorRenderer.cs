namespace Services
{
    public interface IRazorRenderer
    {
        string RenderPartialToString<TModel>(string partialName, TModel model);
    }
}
