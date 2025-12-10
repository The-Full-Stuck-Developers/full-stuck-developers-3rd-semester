namespace api.Etc;

public interface ISeeder
{
    public Task Seed();
    public Task Seed(string defaultPassword);
}