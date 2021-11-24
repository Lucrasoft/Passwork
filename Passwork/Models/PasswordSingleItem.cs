namespace Passwork.Models
{
    class PasswordSingleItem : PasswordListItem
    {
        public PasswordCustomItem[] custom { get; set; }
        public string cryptedPassword { get; set; }

    }
}
