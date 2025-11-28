using BackendProFinAPi.Models;

namespace BackendProFinAPi.Models.DAO
{
    public abstract class UpdateDAO
    {
        public string Role { get; set; }
        public PersonModel Model { get; set; }
        public UserModels UserModel { get; set; }
    }
}