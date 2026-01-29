public interface INadeoQuery<TModel>
{
    Task<TModel> ExecuteQuery();
}