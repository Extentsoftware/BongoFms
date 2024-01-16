namespace BongoDomain.Api
{
    public class GetSprintsRequest
    {
        public User User { get; set; } = default!;
        public DateTime CreatedFrom { get; set; } = default!;
        public bool IncludeClosed { get; set; } = default!;
    }

}
