namespace DockerDemo.Docker.Model
{
    public class Store
    {
        public int Id { get; set; }   // Primary Key by convention
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
