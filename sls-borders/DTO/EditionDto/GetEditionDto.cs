namespace sls_borders.DTO.EditionDto
{
    public class GetEditionDto
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
