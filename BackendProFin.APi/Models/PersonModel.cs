namespace BackendProFinAPi.Models
{
    public abstract class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }


    }
}
