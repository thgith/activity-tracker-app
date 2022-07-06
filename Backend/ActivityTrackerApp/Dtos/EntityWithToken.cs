namespace ActivityTrackerApp.Dtos;

public class EntityWithToken<T>
{
    public T Entity { get; set; }

    public string Token { get; set; }
}